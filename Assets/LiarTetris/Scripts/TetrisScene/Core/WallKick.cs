using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    /// <summary>
    /// rotation From a rotation state To another rotation state
    /// as 0: default, 1: right, 2: reverse, 3: left
    /// </summary>
    public enum FromToRotation
    {
        from0to1, from1to0, from1to2, from2to1,
        from2to3, from3to2, from3to0, from0to3
    }

    public static class WallKick
    {
        public const int maxRotationTestNum = 4;
        static Vector2Int[,] GeneralWallKickData = new Vector2Int[8, maxRotationTestNum + 1]
        {
            {Vector2Int.zero, new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -2), new Vector2Int(-1, -2)},
            {Vector2Int.zero, new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, 2), new Vector2Int(1, 2)},

            {Vector2Int.zero, new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, 2), new Vector2Int(1, 2)},
            {Vector2Int.zero, new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -2), new Vector2Int(-1, -2)},

            {Vector2Int.zero, new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, -2), new Vector2Int(1, -2)},
            {Vector2Int.zero, new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 2), new Vector2Int(-1, 2)},

            {Vector2Int.zero, new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 2), new Vector2Int(-1, 2)},
            {Vector2Int.zero, new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, -2), new Vector2Int(1, -2)},
        };

        static Vector2Int[,] ITetrominoWallKickData = new Vector2Int[8, maxRotationTestNum + 1]
        {
            {Vector2Int.zero, new Vector2Int(-2, 0), new Vector2Int(1, 0), new Vector2Int(-2, -1), new Vector2Int(1, 2)},
            {Vector2Int.zero, new Vector2Int(2, 0), new Vector2Int(1, 0), new Vector2Int(2, 1), new Vector2Int(-1, -2)},

            {Vector2Int.zero, new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-1, 2), new Vector2Int(2, -1)},
            {Vector2Int.zero, new Vector2Int(1, 0), new Vector2Int(-2, 0), new Vector2Int(1, -2), new Vector2Int(-2, 1)},

            {Vector2Int.zero, new Vector2Int(2, 0), new Vector2Int(1, 0), new Vector2Int(2, 1), new Vector2Int(-1, -2)},
            {Vector2Int.zero, new Vector2Int(-2, 0), new Vector2Int(1, 0), new Vector2Int(-2, -1), new Vector2Int(1, 2)},

            {Vector2Int.zero, new Vector2Int(1, 0), new Vector2Int(-2, 0), new Vector2Int(1, -2), new Vector2Int(-2, 1)},
            {Vector2Int.zero, new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-1, 2), new Vector2Int(2, -1)},
        };

        public static FromToRotation GetFromToRotationState(int from, int to)
        {
            if (from == 0)
            {
                if (to == 1)
                {
                    return FromToRotation.from0to1;
                }
                else if (to == 3)
                {
                    return FromToRotation.from0to3;
                }
                else
                {
                    throw new System.ArgumentException($"from:{from}, to:{to}");
                }
            }

            if (from == 1)
            {
                if (to == 0)
                {
                    return FromToRotation.from1to0;
                }
                else if (to == 2)
                {
                    return FromToRotation.from1to2;
                }
                else
                {
                    throw new System.ArgumentException($"from:{from}, to:{to}");
                }

            }

            if (from == 2)
            {
                if (to == 1)
                {
                    return FromToRotation.from2to1;
                }
                else if (to == 3)
                {
                    return FromToRotation.from2to3;
                }
                else
                {
                    throw new System.ArgumentException($"from:{from}, to:{to}");
                }

            }

            if (from == 3)
            {
                if (to == 2)
                {
                    return FromToRotation.from3to2;
                }
                else if (to == 0)
                {
                    return FromToRotation.from3to0;
                }
                else
                {
                    throw new System.ArgumentException($"from:{from}, to:{to}");
                }

            }

            throw new System.ArgumentException($"from:{from}, to:{to}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tetrominoData"></param>
        /// <param name="testNum"></param>
        /// <param name="fromTo"> </param>
        /// <returns></returns>
        public static Vector2Int GetData(TetrominoData tetrominoData, int testNum, FromToRotation fromTo)
        {
            testNum = Mathf.Max(testNum, 0);
            testNum = Mathf.Min(testNum, maxRotationTestNum);

            Vector2Int test;
            switch (tetrominoData.tetrominoType)
            {
                case TetrominoType.General:
                    test = GeneralWallKickData[(int)fromTo, testNum];
                    break;
                case TetrominoType.lowerLmino:
                    test = ITetrominoWallKickData[(int)fromTo, testNum];
                    break;
                case TetrominoType.Omino:
                    test = Vector2Int.zero;
                    break;
                default:
                    throw new System.InvalidOperationException($"fromTo:{System.Enum.GetName(typeof(FromToRotation), fromTo)} not supported");
            }

            return test;
        }
    }
}
