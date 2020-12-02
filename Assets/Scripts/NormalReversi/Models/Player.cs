using System;
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
		public IGridData Put(IGridData gridData, IGameManager gameManager)
		{
			var pieceGameObject = GetPieceFromResource();
			Object.Instantiate(pieceGameObject, gridData.Location, Quaternion.identity)
				.TryGetComponent(out IPiece piece);
			var nowTurn = gameManager.GetGameState();
			switch (nowTurn)
			{
				case GameState.BLACKTURN:
					gridData.ChangeGridState(GridState.BLACK);
					piece.InitColor(Color.black);
					break;	
				case GameState.WHITETURN:
					gridData.ChangeGridState(GridState.WHITE);
					piece.InitColor(Color.white);
					break;
				case GameState.GAMESET:
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