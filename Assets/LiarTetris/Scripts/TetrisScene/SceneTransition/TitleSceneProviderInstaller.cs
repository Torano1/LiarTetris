using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace LiarTetris
{
    public class TitleSceneProviderInstaller : MonoInstaller<TitleSceneProviderInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<INextSceneProvider>().To<TitleSceneProvider>().AsSingle();
        }
    }
}
