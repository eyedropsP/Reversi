using System;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Manager;
using NormalReversi.View;
using UniRx;
using UnityEngine;
using Zenject;

namespace NormalReversi.Presenter
{
	public class ReversiPresenter : MonoBehaviour
	{
		[SerializeField] private ReversiView reversiView;
		[SerializeField] private ReversiGUI reversiGUI;
		
		private IGameManager gameManager;
		private IGridManager gridManager;
		private IPlayer player;
		
		[Inject]
		public void Constructor(IGameManager gameManager, IGridManager gridManager, IPlayer player)
		{
			this.gameManager = gameManager;
			this.gridManager = gridManager;
			this.player = player;
		}

		private void Awake()
		{
			gridManager.RefreshGameManager(gameManager);
		}

		private void Start()
		{
			reversiView.OnGridClicked()
				.Where(_ => gridManager.CanPutGridCount.Value > 0)
				.TakeUntil(gameManager.NowGameState.Where(state => state == GameState.GAMESET))
				.Subscribe(gridData =>
				{
					try
					{
						var playerPutGridData = player.Put(gridData, gameManager);
						gridManager.ReceivePieceFromPlayer(playerPutGridData);
						gridManager.FlipPiece(playerPutGridData);
						gameManager.ChangeGameState();
						gridManager.RefreshGameManager(gameManager);
						gridManager.RefreshGrid();
					}
					catch (Exception e)
					{
						Debug.Log(e);
					}
				});
			
			var subject1 = gridManager.BlackPieceCount;
			var subject2 = gridManager.WhitePieceCount;
			subject1
				.CombineLatest(subject2, (item1, item2) => new Tuple<int, int>(item1, item2))
				.Subscribe(tuple => reversiGUI.SetPieceCount(tuple.Item1, tuple.Item2))
				.AddTo(this);

			subject1
				.CombineLatest(subject2, (item1, item2) => new Tuple<int, int>(item1, item2))
				.Where(tuple => tuple.Item1 + tuple.Item2 == GridManager.GridCount)
				.TakeUntil(gameManager.NowGameState.Where(state => state == GameState.GAMESET))
				.Subscribe(_ =>
				{
					gameManager.GameSet();
				});

			gridManager.CanPutGridCount
				.Where(value => value == 0)
				.Subscribe(value =>
				{
					gameManager.ChangeGameState();
					gridManager.RefreshGameManager(gameManager);
					gridManager.RefreshGrid();
					
					if (gridManager.CanPutGridCount.Value == 0)
					{
						gameManager.GameSet();
					}
				});

			gameManager.NowGameState
				.Subscribe(gameState =>
				{
					reversiGUI.ShowNowTurn(gameState);
				}).AddTo(this);

			gameManager.NowGameState
				.Where(state => state == GameState.GAMESET)
				.Subscribe(_ =>
				{
					var outcome = gridManager.JudgeWinner();
					reversiGUI.ShowWinner(outcome);
				}).AddTo(this);
		}
	}
}