using JetBrains.Annotations;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using UniRx;

namespace NormalReversi.Models.Manager
{
	[UsedImplicitly]
	public class GameManager : IGameManager
	{
		public GameManager()
		{
			_nowGameState.Value = GameState.BlackTurn;
		}

		private readonly ReactiveProperty<GameState> _nowGameState = new ReactiveProperty<GameState>();
		public IReadOnlyReactiveProperty<GameState> NowGameState => _nowGameState;

		public GameState GetGameState()
		{
			return _nowGameState.Value;
		}

		public void ChangeGameState()
		{
			_nowGameState.Value = _nowGameState.Value == GameState.BlackTurn ? GameState.WhiteTurn : GameState.BlackTurn;
		}

		public void GameSet()
		{
			_nowGameState.Value = GameState.GameSet;
		}
	}
}