using UnityEngine;
using Zenject;

namespace LiarTetris
{
    public class KeyboardInputInstaller : MonoInstaller<KeyboardInputInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<ILiarTetrisInput>().To<KeyboardInput>().AsSingle();
        }
    }
}