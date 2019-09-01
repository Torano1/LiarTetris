using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

namespace LiarTetris
{
    /// <summary>
    /// manages grids
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [Inject]
        TetrisSceneAudioManager audioManager;

        [Inject]
        MovementManager move;

        [Inject]
        LevelManager levelManger;

        [Inject]
        BlockInstanceManager blockInstanceManager;

        [Inject]
        MovingTetrominoManager spawnManager;

        [Inject]
        GameStateManager stateManager;

        [Inject]
        ScoreManager scoreManager;

        [SerializeField]
        int extraHeight = 2;

        [SerializeField]
        float dissolveTimeInSeconds = 1f;

        Block[,] grids;
        public int Width => grids == null ? 0 : grids.GetLength(0);
        public int Height => grids == null ? 0 : grids.GetLength(1);

        BoolReactiveProperty onClearingLines = new BoolReactiveProperty(false);
        public ReactiveProperty<bool> OnClearingLines => onClearingLines;

        // Start is called before the first frame update
        void Awake()
        {
            InitBlocks();
        }

        private void Start()
        {
            stateManager.CurrentState
                        .Select(state => state == GameState.LiarTetrisMode)
                        .Subscribe(isLiarMode => SetLiarTetrisMode(isLiarMode)).AddTo(gameObject);
        }

        void SetLiarTetrisMode(bool enabled)
        {
            if (grids != null)
            {
                for (var x = 0; x < grids.GetLength(0); x++)
                {
                    for (var y = 0; y < grids.GetLength(1); y++)
                    {
                        var b = grids[x, y];
                        if (b != null)
                        {
                            b.SetLiarTetrisMode(enabled);
                        }
                    }
                }
            }
        }

        void InitBlocks()
        {
            if (levelManger != null && levelManger.Width >= 2 && levelManger.Height >= 2)
            {
                var w = levelManger.Width;
                var h = levelManger.Height;
                grids = new Block[w, h + extraHeight];
            }
        }

        public bool InvalidMove(Tetromino tetromino)
        {
            var center = tetromino.Center;
            for (var i = 0; i < tetromino.BlockCount; i++)
            {
                var localPos = tetromino.GetLocalBlockPosition(i);
                var gridPos = center + localPos;

                if (gridPos.x < 0 || grids.GetLength(0) <= gridPos.x || gridPos.y < 0 || grids.GetLength(1) <= gridPos.y)
                {
                    return true;
                }

                if (grids[gridPos.x, gridPos.y] != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// add the current blocks to grids
        /// </summary>
        /// <returns> min height and max height of the added blocks </returns>
        public Vector2Int AddToGrid(Tetromino tetromino)
        {
            var center = tetromino.Center;
            var minHeight = grids.GetLength(1) - 1;
            var maxHeight = 0;
            for (var i = 0; i < tetromino.BlockCount; i++)
            {
                var localPos = tetromino.GetLocalBlockPosition(i);
                var blockPos = center + localPos;
                grids[blockPos.x, blockPos.y] = tetromino.GetBlock(i);
                minHeight = Mathf.Min(blockPos.y, minHeight);
                maxHeight = Mathf.Max(blockPos.y, maxHeight);
            }

            return new Vector2Int(minHeight, maxHeight);
        }

        public void CheckForLines(int from, int to, bool EnableLiarModeIfCleared = false)
        {
            onClearingLines.Value = true;

            if (from > to)
            {
                Debug.LogAssertion($"from:{from}, to:{to}");
                return;
            }

            var clearedLines = new List<int>();

            for (var height = from; height <= to; height++)
            {
                if (HasLine(height))
                {
                    clearedLines.Add(height);
                }
            }

            if (clearedLines.Count > 0)
            {
                if (EnableLiarModeIfCleared)
                {
                    audioManager.PlayLineClearAndEnableLiarModeSE();
                }
                else
                {
                    audioManager.PlayLineClearSE();
                }

                StartCoroutine(ClearLines(clearedLines, EnableLiarModeIfCleared));
            }
            else
            {
                onClearingLines.Value = false;
            }
        }

        bool HasLine(int height)
        {
            for (var width = 0; width < grids.GetLength(0); width++)
            {
                if (grids[width, height] == null)
                {
                    return false;
                }
            }

            return true;
        }


        IEnumerator ClearLines(List<int> heights, bool enableLiarMode)
        {
            var count = heights.Count;
            var elapsed = 0f;
            var waitTime = .05f;

            while (dissolveTimeInSeconds > elapsed)
            {
                yield return new WaitForSeconds(waitTime);
                elapsed += waitTime;
                DissolveBlocks(elapsed / dissolveTimeInSeconds, heights);
            }

            foreach (var height in heights)
            {
                ClearLine(height);
            }

            ArrangeGridsAfterClearing(heights);

            scoreManager.ClearLines(count);

            if (enableLiarMode)
            {
                stateManager.EnableLiarMode();
            }

            onClearingLines.Value = false;
        }

        void DissolveBlocks(float amount, List<int> heights)
        {
            foreach (var height in heights)
            {
                for (var width = 0; width < grids.GetLength(0); width++)
                {
                    var b = grids[width, height];
                    if (b != null)
                    {
                        grids[width, height].Dissolve(amount);
                    }
                }
            }
        }

        void ClearLine(int height)
        {
            for (var width = 0; width < grids.GetLength(0); width++)
            {
                var b = grids[width, height];
                if (b != null)
                {
                    grids[width, height] = null;
                    blockInstanceManager.EndUseBlock(b);
                }
            }
        }

        /// <summary>
        /// arrange tetrominos by moving lines down after clearing lines 
        /// </summary>
        /// <param name="clearedLines"> lines to be cleared. this list should be accending order </param>
        void ArrangeGridsAfterClearing(List<int> clearedLines)
        {
            // true if empty
            var emptyLines = new bool[grids.GetLength(1)];

            for (var i = 0; i < clearedLines.Count; i++)
            {
                var emptyLineId = clearedLines[i];
                emptyLines[emptyLineId] = true;
            }

            var lowestEmptyLineId = clearedLines[0];
            clearedLines.Clear();

            for (var height = 0; height < grids.GetLength(1); height++)
            {
                // go next if the current line is empty or the lowest empty line is above the current line
                if (emptyLines[height] || height <= lowestEmptyLineId)
                {
                    continue;
                }

                // move the block down
                var moved = false;
                for (var width = 0; width < grids.GetLength(0); width++)
                {
                    var b = grids[width, height];

                    if (b != null)
                    {
                        grids[width, lowestEmptyLineId] = b;
                        grids[width, height] = null;
                        var pos = spawnManager.GetBlockPosition(new Vector2Int(width, lowestEmptyLineId));
                        b.Move(pos);
                        moved = true;
                    }
                }

                // if moved, then update emptyLines and the lowest empty line index
                if (moved)
                {
                    emptyLines[lowestEmptyLineId] = false;
                    emptyLines[height] = true;

                    // find next empty line
                    for (var id = lowestEmptyLineId + 1; id < emptyLines.Length; id++)
                    {
                        if (emptyLines[id])
                        {
                            lowestEmptyLineId = id;
                            break;
                        }
                    }
                }
            }
        }
    }
}
