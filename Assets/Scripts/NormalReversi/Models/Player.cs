using JetBrains.Annotations;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NormalReversi.Models
{
	[UsedImplicitly]
	public class Player : IPlayer
	{
		public IGridData Put(IGridData gridData, IGameStateManager gameStateManager)
		{
			var pieceGameObject = GetPieceFromResource();
			Object.Instantiate(pieceGameObject, gridData.Location, Quaternion.identity)
				.TryGetComponent(out IPiece piece);
			var nowTurn = gameStateManager.NowGameState.Value;
			switch (nowTurn)
			{
				case GameState.BlackTurn:
					gridData.ChangeGridState(GridState.Black);
					piece.InitColor(Color.black);
					break;	
				case GameState.WhiteTurn:
					gridData.ChangeGridState(GridState.White);
					piece.InitColor(Color.white);
					break;
				case GameState.GameSet:
					break;
				default:
					Debug.Log("異常なゲームステートです。");
					break;
			}
			gridData.AcceptPiece(piece);
			return gridData;
		}

		private static GameObject GetPieceFromResource()
		{
			var pieceGameObject = Resources.Load<GameObject>("Prefabs/Piece");
			if (pieceGameObject == null)
			{
				Debug.Log("Cannot load piece object.");
			}

			return pieceGameObject;
		}
	}
}