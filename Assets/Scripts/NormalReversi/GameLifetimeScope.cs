using NormalReversi.Models;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Manager;
using NormalReversi.Presenter;
using NormalReversi.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace NormalReversi
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GridManager _gridManager;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ReversiPresenter>();
            builder.Register<IGameStateManager, GameStateManager>(Lifetime.Singleton);
            builder.Register<IPlayer, Player>(Lifetime.Singleton);
            builder.RegisterComponent(_gridManager)
                .AsImplementedInterfaces();
            builder.RegisterComponentInHierarchy<ReversiView>();
            builder.RegisterComponentInHierarchy<ReversiGUI>();
        }
    }
}