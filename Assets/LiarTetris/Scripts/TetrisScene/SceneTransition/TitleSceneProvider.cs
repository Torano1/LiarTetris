using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using UnityEngine.SceneManagement;

namespace LiarTetris
{
    public class TitleSceneProvider : INextSceneProvider
    {
        public string GetNextScene()
        {
            return "TitleScene";
        }
    }
}