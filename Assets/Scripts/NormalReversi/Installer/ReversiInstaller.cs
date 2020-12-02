using NormalReversi.Models;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Manager;
using Zenject;

namespace NormalReversi.Installer
{
    public class ReversiInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IGridManager>()
                .To<GridManager>()
                .FromComponentsInHierarchy()
                .AsSingle()
                .NonLazy();
            
            Container.Bind<IGameManager>()
                .To<GameManager>()
                .FromNew()
                .AsSingle()
                .NonLazy();

            Container.Bind<IPlayer>()
                .To<Player>()
                .FromNew()
                .AsCached()
                .NonLazy();
        }
    }
}