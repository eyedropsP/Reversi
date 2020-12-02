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
			nowGameState.Value = GameState.BLACKTURN;
		}

		private ReactiveProperty<GameState> nowGameState = new ReactiveProperty<GameState>();
		public IReadOnlyReactiveProperty<GameState> NowGameState => nowGameState;

		public GameState GetGameState()
		{
			return nowGameState.Value;
		}

		public void ChangeGameState()
		{
			nowGameState.Value = nowGameState.Value == GameState.BLACKTURN ? GameState.WHITETURN : GameState.BLACKTURN;
		}

		public void GameSet()
		{
			nowGameState.Value = GameState.GAMESET;
		}
	}
}