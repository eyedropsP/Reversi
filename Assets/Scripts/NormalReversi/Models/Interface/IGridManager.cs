using NormalReversi.Models.Enum;
using UniRx;
using UnityEngine;

namespace NormalReversi.Models.Interface
{
    public interface IGridManager
    {
        void Initialize();

        // ここ消せるように頑張る
        void SetGridPrefab(GameObject gridPrefab);
        void SetPiecePrefab(GameObject piecePrefab);
        void SetBackgroundPrefab(GameObject backgroundPrefab);
        void SetPiece(int x, int y, IGridData gridData);
        IGridData GetPiece(int x, int y);
        
        IReadOnlyReactiveProperty<int> BlackPieceCount { get; }
        IReadOnlyReactiveProperty<int> WhitePieceCount { get; }
        IReadOnlyReactiveProperty<int> CanPutGridCount { get; }
        void RefreshGameManager(IGameStateManager gameStateManager);
        void RefreshGrid();
        void ReceivePieceFromPlayer(IGridData gridData);
        void FlipPiece(IGridData gridData);
        Outcome JudgeWinner();
    }
}