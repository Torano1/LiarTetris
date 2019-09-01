using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    [CreateAssetMenu(fileName = "LiarTetrisLevelSetting", menuName = "LiarTetrisData/LiarTetrisLevelSetting", order = 10)]
    public class LiarTetrisLevelSetting : ScriptableObject
    {
        [SerializeField]
        Vector3 tetrominoScale = Vector3.zero;
        public Vector3 blockScale => tetrominoScale;

        [SerializeField]
        int width = 10, height = 20, heldTetrominoCount = 1, followingTetrominoCount = 5;
        public int Width => width;
        public int Height => height;
        public int HeldTetrominoCount => heldTetrominoCount;
        public int FollowingTetrominoCount => followingTetrominoCount;
    }
}