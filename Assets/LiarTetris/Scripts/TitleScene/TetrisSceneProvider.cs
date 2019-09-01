using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    public class TetrisSceneProvider : INextSceneProvider
    {
        public string GetNextScene()
        {
            return "TetrisScene";
        }
    }
}
