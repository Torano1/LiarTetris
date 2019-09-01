using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    public interface INextSceneProvider
    {
        string GetNextScene();
    }
}
