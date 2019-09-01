using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LiarTetris
{
    [RequireComponent(typeof(BlockInstanceManager))]
    public class TetrominoesGenerator : MonoBehaviour
    {
        [SerializeField]
        TetrominoData[] toGenerate;

        [SerializeField]
        Transform position;

        [SerializeField]
        Vector3 offset;

        BlockInstanceManager instanceManager;

        // Start is called before the first frame update
        void Start()
        {
            instanceManager = GetComponent<BlockInstanceManager>();

            if (toGenerate != null)
            {
                var origin = position == null ? Vector3.zero : position.position;
                for (var i = 0; i < toGenerate.Length; i++)
                {
                    var pos = origin + offset * i;
                    var tetromino = new Tetromino(instanceManager, toGenerate[i]);
                    tetromino.MoveTetromino(pos);
                }
            }
        }
    }
}
