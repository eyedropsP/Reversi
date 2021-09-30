using NormalReversi.Models.Enum;
using UniRx;

namespace NormalReversi.Models.Interface
{
	public interface IGameStateManager
	{
		IReadOnlyReactiveProperty<GameState> NowGameState { get; }
		
		void ChangeGameState();

		void GameSet();
	}
}