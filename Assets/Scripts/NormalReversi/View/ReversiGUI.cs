using System;
using NormalReversi.Models.Enum;
using TMPro;
using UnityEngine;

namespace NormalReversi.View
{
	public class ReversiGUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI blackPieceCountText;
		[SerializeField] private TextMeshProUGUI whitePieceCountText;
		[SerializeField] private TextMeshProUGUI gameStateText;
		[SerializeField] private TextMeshProUGUI nowTurnText;
		[SerializeField] private SpriteRenderer nowTurnPieceSpriteRenderer;
		
		public void SetPieceCount(int blackPieceCount, int whitePieceCount)
		{
			blackPieceCountText.text = $"BLACK : {blackPieceCount}";
			whitePieceCountText.text = $"WHITE : {whitePieceCount}";
		}

		public void ShowNowTurn(GameState gameState)
		{
			switch (gameState)
			{
				case GameState.BlackTurn:
					nowTurnPieceSpriteRenderer.color = Color.black;
					break;
				case GameState.WhiteTurn:
					nowTurnPieceSpriteRenderer.color = Color.white;
					break;
				case GameState.GameSet:
					nowTurnText.text = default;
					nowTurnPieceSpriteRenderer.color = new Color(0,0,0,0);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
			}
		}

		public void ShowWinner(Outcome outcome)
		{
			gameStateText.text =
				outcome != Outcome.Draw ?
					$"GAME SET!!\n{outcome} Win!!" :
					$"GAME SET!!\nDRAW...";
		}
	}
}