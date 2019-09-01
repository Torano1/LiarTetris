using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    public class Tetromino
    {
        // 0:default, 1:right, 2:reverse, 3:left
        int prevRotationState, curRotationState;
        public int PrevRotationState => prevRotationState;
        public int CurRotationState => curRotationState;

        TetrominoData data;
        public TetrominoData Data => data;

        Vector2Int center;
        public Vector2Int Center => center;

        Vector2[] currentPositions;
        Block[] blocks;

        BlockInstanceManager instanceManager;
        bool isLiarMode = false;

        public int BlockCount => currentPositions == null ? 0 : currentPositions.Length;
        public Vector2Int GetLocalBlockPosition(int blockIdx)
        {
            var offset = data.Offset;
            var pos = currentPositions[blockIdx];
            return new Vector2Int(Mathf.RoundToInt(pos.x + offset.x), Mathf.RoundToInt(pos.y + offset.y));
        }

        public Tetromino(BlockInstanceManager instanceManager, TetrominoData data)
        {
            Initialize(instanceManager, data);
        }

        public Tetromino(Block[] blocks, TetrominoData data)
        {
            Initialize(blocks, data);
        }

        public void Initialize(BlockInstanceManager instanceManager, TetrominoData data)
        {
            this.instanceManager = instanceManager;
            this.data = data;
            InitPositions();
            InitBlocks(instanceManager);
        }

        public void Initialize(Block[] blocks, TetrominoData data)
        {
            this.data = data;
            InitPositions();
            InitBlocks(blocks);
        }

        void InitPositions()
        {
            prevRotationState = 0;
            curRotationState = 0;

            if (currentPositions == null || currentPositions.Length != data.BlockCount)
            {
                currentPositions = new Vector2[data.BlockCount];
            }

            for (var i = 0; i < currentPositions.Length; i++)
            {
                var pos = data.GetPosition(i);
                currentPositions[i] = pos;
            }
        }

        void InitBlocks(BlockInstanceManager instanceManager)
        {
            if (blocks == null || blocks.Length != data.BlockCount)
            {
                blocks = new Block[data.BlockCount];
            }

            for (var i = 0; i < data.BlockCount; i++)
            {
                blocks[i] = instanceManager.StartUseBlock();
                blocks[i].ChangeColor(data.Color);
            }
        }

        void InitBlocks(Block[] blocks)
        {
            this.blocks = blocks;

            for (var i = 0; i < data.BlockCount; i++)
            {
                blocks[i].ChangeColor(data.Color);
            }
        }

        public void InitPose()
        {
            InitPositions();
        }

        public void UpdateCenterPosition(Vector2Int pos)
        {
            center = pos;
        }

        public void RotateRight()
        {
            if (currentPositions == null)
            {
                return;
            }

            prevRotationState = curRotationState;
            curRotationState = (curRotationState == 3) ? 0 : curRotationState + 1;


            for (var i = 0; i < currentPositions.Length; i++)
            {
                var pos = currentPositions[i];
                currentPositions[i] = new Vector2(currentPositions[i].y, -currentPositions[i].x);
            }
        }

        public void RotateLeft()
        {
            if (currentPositions == null)
            {
                return;
            }

            prevRotationState = curRotationState;
            curRotationState = (curRotationState == 0) ? 3 : curRotationState - 1;

            for (var i = 0; i < currentPositions.Length; i++)
            {
                var pos = currentPositions[i];
                currentPositions[i] = new Vector2(-currentPositions[i].y, currentPositions[i].x);
            }
        }

        public Block GetBlock(int blockIdx)
        {
            return blocks[blockIdx];
        }

        public void MoveBlock(int blockIdx, Vector3 pos)
        {
            blocks[blockIdx].Move(pos);
        }

        public void MoveTetromino(Vector3 center)
        {
            for (var i = 0; i < this.BlockCount; i++)
            {
                var localPos = this.GetLocalBlockPosition(i);
                var pos = center + new Vector3(localPos.x, localPos.y, 0);
                this.MoveBlock(i, pos);
            }
        }

        public void RemoveAllBlocks()
        {
            if (blocks != null)
            {
                for (var i = 0; i < blocks.Length; i++)
                {
                    if (instanceManager)
                    {
                        instanceManager.EndUseBlock(blocks[i]);
                    }
                    else
                    {
                        Debug.LogWarning("trying to remove all blocks, but instanceManager = null");
                        GameObject.Destroy(blocks[i]);
                    }
                }
                blocks = null;
            }
        }

        public void SetLiarTetrisMode(bool enabled)
        {
            isLiarMode = enabled;

            if (blocks != null)
            {
                for (var i = 0; i < blocks.Length; i++)
                {
                    var b = blocks[i];
                    b.SetLiarTetrisMode(enabled);
                }
            }
        }

        public void Dissolve(float amount)
        {
            if (blocks != null)
            {
                for (var i = 0; i < blocks.Length; i++)
                {
                    var b = blocks[i];
                    b.Dissolve(amount);
                }
            }
        }
    }
}