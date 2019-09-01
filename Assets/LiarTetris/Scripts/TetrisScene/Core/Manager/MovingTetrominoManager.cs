using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace LiarTetris
{

    /// <summary>
    /// move the current tetromino
    /// </summary>
    public class MovingTetrominoManager : MonoBehaviour
    {
        [Inject]
        TetrisSceneAudioManager audioManager;

        [Inject]
        MovementManager move;

        [Inject]
        LevelManager levelManager;

        [Inject]
        GridManager gridManager;

        [Inject]
        TetrominoSpawner spawner;

        [Inject]
        GameTimeManager timeManager;

        [Inject]
        GameStateManager stateManager;

        [Inject]
        BlockInstanceManager instanceManager;

        [Inject]
        InputManager inputManager;

        [SerializeField]
        float dropGraceTime;

        [SerializeField]
        Block previewBlockPrefab;

        [SerializeField]
        Transform bottomCenterObjectTransform;

        [SerializeField]
        Transform heldTetrominoPositionTransform;

        Vector3 bottomCenterPos => bottomCenterObjectTransform == null ? Vector3.zero : bottomCenterObjectTransform.position;

        // used to calculate z in GetWorldBlockPosition(Vector2Int)
        [SerializeField]
        float depth;

        bool holdable = true;
        public bool Holdable => holdable;

        List<Block> blocksForPreviewTetromino = new List<Block>();
        Tetromino currentTetromino, heldTetromino = null, previewTetromino;
        Vector2Int spawnPoint;

        Vector3 Grid00;
        Subject<Unit> onMove = new Subject<Unit>();

        bool waitingForCompeleteMove = false;
        float elapsedTime;

        bool clearingLines => (gridManager && gridManager.OnClearingLines.Value);

        // Start is called before the first frame update
        void Start()
        {
            move.OnMoveDownObservable()
                .Subscribe(_ => MoveDown());

            move.OnMoveRightObservable()
                .Subscribe(_ => MoveRight());

            move.OnMoveLeftObservable()
                .Subscribe(_ => MoveLeft());

            move.OnRotateRightObservable()
                .Subscribe(_ => RotateRight());

            move.OnRotateLeftObservable()
                .Subscribe(_ => RotateLeft());

            move.OnHardDropObservable()
                .Subscribe(_ => HardDrop());

            move.OnHoldObservable()
                .Subscribe(_ => Hold());


            onMove.Where(_ => waitingForCompeleteMove && !clearingLines)
                  .Where(_ => stateManager && (stateManager.CurrentState.Value == GameState.NormalTetrisMode || stateManager.CurrentState.Value == GameState.LiarTetrisMode))
                  .Subscribe(_ =>
                  {
                      MoveTetrominoDown();
                      if (gridManager.InvalidMove(currentTetromino))
                      {
                          MoveTetrominoUp();
                          DelayCompleteMove();
                      }
                      else
                      {
                          MoveTetrominoUp();
                          waitingForCompeleteMove = false;
                      }
                  });

            onMove.Where(_ => previewTetromino != null)
                  .Subscribe(_ => MovePreviewTetromino());

            this.UpdateAsObservable()
                .Where(_ => !clearingLines)
                .Where(_ => stateManager && (stateManager.CurrentState.Value == GameState.NormalTetrisMode || stateManager.CurrentState.Value == GameState.LiarTetrisMode))
                .Where(_ => waitingForCompeleteMove)
                .Do(_ => elapsedTime += Time.deltaTime)
                .Where(_ => elapsedTime > dropGraceTime)
                .Subscribe(_ =>
                {
                    MoveTetrominoDown();
                    if (gridManager.InvalidMove(currentTetromino))
                    {
                        MoveTetrominoUp();
                        CompleteMove();
                    }
                    else
                    {
                        MoveTetrominoUp();
                        waitingForCompeleteMove = false;
                    }
                });

            stateManager.CurrentState
                        .FirstOrDefault(state => state == stateManager.InitialTetrisMode)
                        .Subscribe(_ => SpawnTetromino()).AddTo(gameObject);

            stateManager.CurrentState
                        .Select(state => state == GameState.LiarTetrisMode)
                        .Subscribe(isLiarMode => SetLiarTetrisMode(isLiarMode)).AddTo(gameObject);

            gridManager.OnClearingLines
                       .SkipLatestValueOnSubscribe()
                       .Where(clearing => !clearing)
                       .Where(_ => stateManager && (stateManager.CurrentState.Value == GameState.LiarTetrisMode || stateManager.CurrentState.Value == GameState.NormalTetrisMode))
                       .Subscribe(_ =>
                       {
                           SpawnTetromino();
                       }).AddTo(gameObject);

            InitSpawnPoint();
            move.Movable = true;
        }

        void SetLiarTetrisMode(bool enabled)
        {
            if (currentTetromino != null)
            {
                currentTetromino.SetLiarTetrisMode(enabled);
            }

            if (heldTetromino != null)
            {
                heldTetromino.SetLiarTetrisMode(enabled);
            }
        }

        public Vector3 GetBlockPosition(Vector2Int gridPos)
        {
            var width = gridManager.Width;
            var height = gridManager.Height;
            var scale = levelManager.blockScale;
            return Grid00 + new Vector3(gridPos.x * scale.x, gridPos.y * scale.y, 0);
        }

        public void UpdateTetrominoPositionInGrids(Tetromino tetromino)
        {
            var center = tetromino.Center;
            for (var i = 0; i < tetromino.BlockCount; i++)
            {
                var gridPos = center + tetromino.GetLocalBlockPosition(i);
                var pos = GetBlockPosition(gridPos);
                tetromino.MoveBlock(i, pos);
            }
        }

        void InitSpawnPoint()
        {
            if (levelManager != null && levelManager.Width >= 2 && levelManager.Height >= 2)
            {
                var w = levelManager.Width;
                var h = levelManager.Height;
                spawnPoint = new Vector2Int(w / 2, h);

                var scale = levelManager.blockScale;
                Grid00 = bottomCenterPos + new Vector3(-w / 2 * scale.x, 0, depth);
            }
        }

        void InitPreviewTetromino(Tetromino current)
        {
            var blocks = new Block[current.BlockCount];

            while (blocks.Length > blocksForPreviewTetromino.Count)
            {
                Block b = Instantiate(previewBlockPrefab);
                b.transform.parent = instanceManager?.BlockParent;
                blocksForPreviewTetromino.Add(b);
            }

            var id = 0;
            for (var i = 0; i < blocksForPreviewTetromino.Count; i++)
            {
                var b = blocksForPreviewTetromino[i];
                if (blocks.Length <= i)
                {
                    b.gameObject.SetActive(false);
                    continue;
                }

                b.gameObject.SetActive(true);
                blocks[i] = b;
            }

            previewTetromino = new Tetromino(blocks, current.Data);
        }

        void MovePreviewTetromino()
        {
            while (currentTetromino.CurRotationState != previewTetromino.CurRotationState)
            {
                previewTetromino.RotateRight();
            }

            previewTetromino.UpdateCenterPosition(currentTetromino.Center);

            var pos = previewTetromino.Center;

            do
            {
                pos.y -= 1;
                previewTetromino.UpdateCenterPosition(pos);
            }
            while (!gridManager.InvalidMove(previewTetromino));

            pos.y += 1;
            previewTetromino.UpdateCenterPosition(pos);

            UpdateTetrominoPositionInGrids(previewTetromino);
        }

        void SpawnTetromino(bool holdable = true)
        {
            this.holdable = holdable;
            currentTetromino = spawner.Spawn();

            if (previewTetromino == null || previewTetromino.Data != currentTetromino.Data)
            {
                InitPreviewTetromino(currentTetromino);
            }

            currentTetromino.UpdateCenterPosition(spawnPoint);
            UpdateTetrominoPositionInGrids(currentTetromino);

            MovePreviewTetromino();

            if (gridManager.InvalidMove(currentTetromino))
            {
                stateManager.GameOver();
            }
        }

        /// <summary>
        /// move down
        /// </summary>
        /// <returns> true if the current tetoromino could move down </returns>
        bool MoveDown(bool isHardDrop = false)
        {
            if (waitingForCompeleteMove || clearingLines)
            {
                return false;
            }

            MoveTetrominoDown();

            if (gridManager.InvalidMove(currentTetromino))
            {
                MoveTetrominoUp();

                if (!isHardDrop)
                {
                    DelayCompleteMove();
                    return false;
                }

                CompleteMove();
                return false;
            }

            if (!isHardDrop && inputManager && inputManager.OnMoveDownButton.Value)
            {
                audioManager.PlaySoftDropSE();
            }

            UpdateTetrominoPositionInGrids(currentTetromino);
            MovePreviewTetromino();
            return true;
        }

        void DelayCompleteMove()
        {
            waitingForCompeleteMove = true;
            elapsedTime = 0f;
        }

        void CompleteMove()
        {
            var heights = gridManager.AddToGrid(currentTetromino);
            waitingForCompeleteMove = false;

            gridManager.CheckForLines(heights.x, heights.y, stateManager.CurrentState.Value == GameState.NormalTetrisMode && currentTetromino.Data.IsLiarTetris);
        }

        void MoveTetrominoDown()
        {
            var pos = currentTetromino.Center;
            pos.y -= 1;
            currentTetromino.UpdateCenterPosition(pos);
        }

        void MoveTetrominoUp()
        {
            var pos = currentTetromino.Center;
            pos.y += 1;
            currentTetromino.UpdateCenterPosition(pos);
        }

        void MoveRight()
        {
            MoveTetrominoRight();
            if (gridManager.InvalidMove(currentTetromino))
            {
                MoveTetrominoLeft();
                return;
            }

            audioManager.PlayMoveTetrominoSE();
            UpdateTetrominoPositionInGrids(currentTetromino);
            onMove.OnNext(Unit.Default);
        }

        void MoveLeft()
        {
            MoveTetrominoLeft();
            if (gridManager.InvalidMove(currentTetromino))
            {
                MoveTetrominoRight();
                return;
            }

            audioManager.PlayMoveTetrominoSE();
            UpdateTetrominoPositionInGrids(currentTetromino);
            onMove.OnNext(Unit.Default);
        }

        void MoveTetrominoRight()
        {
            var pos = currentTetromino.Center;
            pos.x += 1;
            currentTetromino.UpdateCenterPosition(pos);
        }

        void MoveTetrominoLeft()
        {
            var pos = currentTetromino.Center;
            pos.x -= 1;
            currentTetromino.UpdateCenterPosition(pos);
        }

        Vector2Int GetRotationTestData(int testNum)
        {
            var fromTo = WallKick.GetFromToRotationState(currentTetromino.PrevRotationState, currentTetromino.CurRotationState);
            var offset = WallKick.GetData(currentTetromino.Data, testNum, fromTo);
            return currentTetromino.Center + offset;
        }

        bool MoveTest(Vector2Int testData)
        {
            var initialCenter = currentTetromino.Center;
            currentTetromino.UpdateCenterPosition(testData);

            // test
            var succeeded = !gridManager.InvalidMove(currentTetromino);

            // back the initial value
            currentTetromino.UpdateCenterPosition(initialCenter);
            return succeeded;
        }

        void RotateRight()
        {
            RotateTetrominoRight();
            var initialCenter = currentTetromino.Center;

            // test until success
            var succeeded = false;
            for (var test = 0; test <= WallKick.maxRotationTestNum; test++)
            {
                var testData = GetRotationTestData(test);
                if (MoveTest(testData))
                {
                    succeeded = true;
                    currentTetromino.UpdateCenterPosition(testData);
                    break;
                }
            }

            if (succeeded)
            {
                audioManager.PlayRotateTetrominoSE();
                UpdateTetrominoPositionInGrids(currentTetromino);
                onMove.OnNext(Unit.Default);
            }
            else
            {
                // failed to rotate, so back the initial state
                currentTetromino.UpdateCenterPosition(initialCenter);
                RotateTetrominoLeft();
            }
        }

        void RotateLeft()
        {
            RotateTetrominoLeft();
            var initialCenter = currentTetromino.Center;

            // test until success
            var succeeded = false;
            for (var test = 0; test <= WallKick.maxRotationTestNum; test++)
            {
                var testData = GetRotationTestData(test);
                if (MoveTest(testData))
                {
                    succeeded = true;
                    currentTetromino.UpdateCenterPosition(testData);
                    break;
                }
            }

            if (succeeded)
            {
                audioManager.PlayRotateTetrominoSE();
                UpdateTetrominoPositionInGrids(currentTetromino);
                onMove.OnNext(Unit.Default);
            }
            else
            {
                // failed to rotate, so back the initial state
                currentTetromino.UpdateCenterPosition(initialCenter);
                RotateTetrominoRight();
            }
        }

        void RotateTetrominoRight()
        {
            currentTetromino.RotateRight();
        }

        void RotateTetrominoLeft()
        {
            currentTetromino.RotateLeft();
        }

        void HardDrop()
        {
            if (waitingForCompeleteMove || clearingLines)
            {
                waitingForCompeleteMove = false;
            }

            audioManager.PlayHardDropSE();
            while (MoveDown(true)) ;
        }

        void Hold()
        {
            if (waitingForCompeleteMove || clearingLines)
            {
                waitingForCompeleteMove = false;
            }

            if (!holdable)
            {
                return;
            }

            audioManager.PlayHoldSE();

            holdable = false;
            var temp = heldTetromino;
            heldTetromino = currentTetromino;
            currentTetromino = temp;

            // initialize the pose
            if (heldTetromino != null)
            {
                heldTetromino.InitPose();

                // add the held tetromino data to the candidates to spawn
                //spawner.AddCandidate(heldTetromino.Data);
            }

            if (currentTetromino == null)
            {
                currentTetromino = spawner.Spawn();
            }

            currentTetromino.UpdateCenterPosition(spawnPoint);

            UpdateTetrominoPositionInGrids(currentTetromino);

            heldTetromino.MoveTetromino(heldTetrominoPositionTransform.position);

            InitPreviewTetromino(currentTetromino);
            MovePreviewTetromino();
        }

        public void MoveTetromino(Vector3 center, Tetromino tetromino)
        {
            for (var i = 0; i < tetromino.BlockCount; i++)
            {
                var localPos = tetromino.GetLocalBlockPosition(i);
                var pos = center + new Vector3(localPos.x, localPos.y, 0);
                tetromino.MoveBlock(i, pos);
            }
        }
    }
}