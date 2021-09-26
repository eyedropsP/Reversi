using NormalReversi.Models.Enum;
using UniRx;

namespace NormalReversi.Models.Interface
{
	public interface IGridManager
	{
		void Initialize();
		IReadOnlyReactiveProperty<int> BlackPieceCount { get; }
		IReadOnlyReactiveProperty<int> WhitePieceCount { get; }
		IReadOnlyReactiveProperty<int> CanPutGridCount { get; }
		void RefreshGameManager(IGameManager gameManager);
		void RefreshGrid();
		void ReceivePieceFromPlayer(IGridData gridData);
		void FlipPiece(IGridData gridData);
		Outcome JudgeWinner();
	}
}