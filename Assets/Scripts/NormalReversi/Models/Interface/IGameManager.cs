using NormalReversi.Models.Enum;
using UniRx;

namespace NormalReversi.Models.Interface
{
	public interface IGameManager
	{
		IReadOnlyReactiveProperty<GameState> NowGameState { get; }
		
		GameState GetGameState();

		void ChangeGameState();

		void GameSet();
	}
}