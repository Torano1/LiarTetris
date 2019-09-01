using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using System.Linq;

namespace LiarTetris
{
    public class TetrominoSpawner : MonoBehaviour
    {
        [Inject]
        GameStateManager stateManager;

        [Inject]
        BlockInstanceManager blockInstanceManager;

        [Inject]
        LevelManager levelManager;

        [Inject]
        MovingTetrominoManager tetrominoManager;

        [SerializeField]
        TetrominoData[] normalTetrominoDataArray;

        [SerializeField]
        TetrominoData[] liarTetrominoDataArray;

        /// <summary>
        /// tetrominoes spawning at first. used to debug.
        /// </summary>
        [SerializeField]
        TetrominoData[] firstSpawningTetrominoes;

        [SerializeField]
        Transform nextTetrominoPositionTransform;

        [SerializeField]
        float spaceBtwFollowingTetrominos = 4f;

        List<TetrominoData> candidatesToSpawn = new List<TetrominoData>();
        List<Tetromino> followingTetrominos = new List<Tetromino>();
        int count = 0;

        TetrominoData[] currentDataArray
        {
            get
            {
                if (stateManager != null)
                {
                    switch (stateManager.CurrentState.Value)
                    {
                        case GameState.NormalTetrisMode:
                            return normalTetrominoDataArray;
                        case GameState.LiarTetrisMode:
                            return liarTetrominoDataArray;
                    }
                }

                return null;
            }
        }

        void Awake()
        {
            //InitSpawnCandidates();
            //InitFollowingTetrominos();
        }

        private void Start()
        {
            stateManager.CurrentState
                        .FirstOrDefault(state => state == GameState.NormalTetrisMode)
                        .Subscribe(_ =>
                        {
                            InitSpawnCandidates();
                            InitFollowingTetrominos();
                        });

            stateManager.CurrentState
                        .Where(state => state == GameState.LiarTetrisMode)
                        .Subscribe(_ =>
                        {
                            InitSpawnCandidates();
                            InitFollowingTetrominos();
                        });

            stateManager.CurrentState
                        .Where(state => state == GameState.NormalTetrisMode)
                        .Subscribe(_ =>
                        {
                            InitSpawnCandidates();

                            foreach (var t in followingTetrominos)
                            {
                                t.SetLiarTetrisMode(false);
                            }
                        });
        }

        void InitSpawnCandidates()
        {
            if (currentDataArray == null)
            {
                switch (stateManager.InitialTetrisMode)
                {
                    case GameState.NormalTetrisMode:
                        candidatesToSpawn = normalTetrominoDataArray.ToList();
                        break;
                    case GameState.LiarTetrisMode:
                        candidatesToSpawn = liarTetrominoDataArray.ToList();
                        break;
                    default:
                        Debug.LogAssertion($"stateManager.InitialTetrisMode:{stateManager.InitialTetrisMode}");
                        break;
                }
            }
            else
            {
                candidatesToSpawn = currentDataArray.ToList();
            }
        }

        void InitFollowingTetrominos()
        {
            var numFollowingTetrominos = Mathf.Max(1, levelManager.FollowingTetrominoCount);

            if (followingTetrominos.Count > 0)
            {
                for (var i = 0; i < followingTetrominos.Count; i++)
                {
                    var t = followingTetrominos[i];
                    t.RemoveAllBlocks();
                }
                followingTetrominos.Clear();
            }

            for (var i = 0; i < numFollowingTetrominos; i++)
            {
                TetrominoData data = GetNextTetrominoData();
                var t = new Tetromino(blockInstanceManager, data);
                t.SetLiarTetrisMode(stateManager != null && stateManager.CurrentState.Value == GameState.LiarTetrisMode);

                followingTetrominos.Add(t);
            }

            UpdateFollowingTetrominosPositions();
        }

        void UpdateFollowingTetrominosPositions()
        {
            var curPos = nextTetrominoPositionTransform.position;
            for (var i = 0; i < followingTetrominos.Count; i++)
            {
                var t = followingTetrominos[i];
                t.MoveTetromino(curPos);
                curPos.y -= spaceBtwFollowingTetrominos;
            }
        }

        TetrominoData GetNextTetrominoData()
        {
            TetrominoData data;
            if (firstSpawningTetrominoes != null && firstSpawningTetrominoes.Length > 0 && count < firstSpawningTetrominoes.Length)
            {
                data = firstSpawningTetrominoes[count++];
                return data;
            }

            if (candidatesToSpawn.Count > 0)
            {
                float max = 0f;
                foreach (var c in candidatesToSpawn)
                {
                    if (followingTetrominos != null && followingTetrominos.Count > 0 && followingTetrominos[followingTetrominos.Count - 1].Data == c)
                    {
                        continue;
                    }

                    max += c.SpawnRate;
                }

                var num = UnityEngine.Random.Range(0, max);

                data = candidatesToSpawn[0];
                for (var i = 0; i < candidatesToSpawn.Count; i++)
                {
                    var c = candidatesToSpawn[i];
                    if (i == candidatesToSpawn.Count - 1)
                    {
                        data = c;
                        break;
                    }

                    if (followingTetrominos != null && followingTetrominos.Count > 0 && followingTetrominos[followingTetrominos.Count - 1].Data == c)
                    {
                        continue;
                    }

                    if (c.SpawnRate < num)
                    {
                        num -= c.SpawnRate;
                        continue;
                    }
                    else
                    {
                        data = c;
                        break;
                    }
                }
            }
            else
            {
                Debug.LogWarning("candidates.Count = 0");
                data = currentDataArray[UnityEngine.Random.Range(0, normalTetrominoDataArray.Length)];
            }

            return data;
        }

        public Tetromino Spawn()
        {
            var data = GetNextTetrominoData();
            var t = new Tetromino(blockInstanceManager, data);
            followingTetrominos.Add(t);
            t.SetLiarTetrisMode(stateManager.CurrentState.Value == GameState.LiarTetrisMode);


            var tetromino = followingTetrominos[0];
            followingTetrominos.RemoveAt(0);
            UpdateFollowingTetrominosPositions();


            return tetromino;
        }
    }
}