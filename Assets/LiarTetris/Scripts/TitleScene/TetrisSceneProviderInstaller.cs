using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace LiarTetris
{
    public class TetrisSceneProviderInstaller : MonoInstaller<TetrisSceneProviderInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<INextSceneProvider>().To<TetrisSceneProvider>().AsSingle();
        }
    }
}
