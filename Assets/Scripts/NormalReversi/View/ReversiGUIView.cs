using System;
using NormalReversi.Models.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace NormalReversi.View
{
	public class ReversiGUIView : MonoBehaviour
	{
		[FormerlySerializedAs("blackPieceCountText")] [SerializeField] private TextMeshProUGUI _blackPieceCountText;
		[FormerlySerializedAs("whitePieceCountText")] [SerializeField] private TextMeshProUGUI _whitePieceCountText;
		[FormerlySerializedAs("gameStateText")] [SerializeField] private TextMeshProUGUI _gameStateText;
		[FormerlySerializedAs("nowTurnText")] [SerializeField] private TextMeshProUGUI _nowTurnText;
		[FormerlySerializedAs("nowTurnPieceSpriteRenderer")] [SerializeField] private SpriteRenderer _nowTurnPieceSpriteRenderer;
		
		public void SetPieceCount(int blackPieceCount, int whitePieceCount)
		{
			_blackPieceCountText.text = $"BLACK : {blackPieceCount}";
			_whitePieceCountText.text = $"WHITE : {whitePieceCount}";
		}

		public void ShowNowTurn(GameState gameState)
		{
			switch (gameState)
			{
				case GameState.BlackTurn:
					_nowTurnPieceSpriteRenderer.color = Color.black;
					break;
				case GameState.WhiteTurn:
					_nowTurnPieceSpriteRenderer.color = Color.white;
					break;
				case GameState.GameSet:
					_nowTurnText.text = default;
					_nowTurnPieceSpriteRenderer.color = new Color(0,0,0,0);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
			}
		}

		public void ShowWinner(Outcome outcome)
		{
			_gameStateText.text =
				outcome != Outcome.Draw ?
					$"GAME SET!!\n{outcome} Win!!" :
					$"GAME SET!!\nDRAW...";
		}
	}
}