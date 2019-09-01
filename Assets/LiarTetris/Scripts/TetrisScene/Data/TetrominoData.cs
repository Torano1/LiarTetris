using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    public enum TetrominoType
    {
        General, lowerLmino, Omino
    }

    [CreateAssetMenu(fileName = "TetrominoData", menuName = "LiarTetrisData/TetrominoData", order = 0)]
    public class TetrominoData : ScriptableObject
    {
        [SerializeField]
        bool isLiarTetris = false;
        public bool IsLiarTetris => isLiarTetris;

        [SerializeField]
        TetrominoType type = TetrominoType.General;
        public TetrominoType tetrominoType => type;

        [SerializeField]
        Color color = Color.white;
        public Color Color => color;

        [SerializeField]
        Vector2 offset = Vector2.zero;
        public Vector2 Offset => offset;

        [SerializeField]
        Vector2[] positions;

        [SerializeField]
        float spawnRate = 10f;
        public float SpawnRate => spawnRate;

        public int BlockCount => positions == null ? 0 : positions.Length;

        public Vector2 GetPosition(int index)
        {
            return positions[index];
        }

    }
}
