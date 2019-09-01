using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField]
        LiarTetrisLevelSetting levelSetting;

        public Vector3 blockScale => levelSetting == null ? Vector3.zero : levelSetting.blockScale;

        public int Width => levelSetting == null ? 0 : levelSetting.Width;
        public int Height => levelSetting == null ? 0 : levelSetting.Height;
        public int HeldTetrominoCount => levelSetting == null ? 0 : levelSetting.HeldTetrominoCount;
        public int FollowingTetrominoCount => levelSetting == null ? 0 : levelSetting.FollowingTetrominoCount;

        // todo
        public int GetMaxTetrominoBlockCount(int largestTetrominoBlockCount)
        {
            var canBeFilled = Width * Height + 10;
            var outsideLevel = (HeldTetrominoCount + FollowingTetrominoCount) * largestTetrominoBlockCount;
            return canBeFilled + outsideLevel;
        }
    }
}
