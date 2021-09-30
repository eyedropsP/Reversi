using JetBrains.Annotations;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using UniRx;

namespace NormalReversi.Models.Manager
{
    [UsedImplicitly]
    public class GameStateManager : IGameStateManager
    {
        private readonly ReactiveProperty<GameState> _nowGameState = new ReactiveProperty<GameState>();
        public IReadOnlyReactiveProperty<GameState> NowGameState => _nowGameState;

        public GameStateManager()
        {
            _nowGameState.Value = GameState.BlackTurn;
        }

        public void ChangeGameState()
        {
            _nowGameState.Value =
                _nowGameState.Value == GameState.BlackTurn ? GameState.WhiteTurn : GameState.BlackTurn;
        }

        public void GameSet()
        {
            _nowGameState.Value = GameState.GameSet;
        }
    }
}