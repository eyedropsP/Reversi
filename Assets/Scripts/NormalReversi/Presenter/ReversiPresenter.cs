using System;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Manager;
using NormalReversi.View;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace NormalReversi.Presenter
{
	public class ReversiPresenter : MonoBehaviour
	{
		[FormerlySerializedAs("reversiView")] [SerializeField] private ReversiView _reversiView;
		[FormerlySerializedAs("reversiGUI")] [SerializeField] private ReversiGUI _reversiGUI;
		
		private IGameManager _gameManager;
		private IGridManager _gridManager;
		private IPlayer _player;
		
		[Inject]
		public void Constructor(IGameManager gameManager, IGridManager gridManager, IPlayer player)
		{
			_gameManager = gameManager;
			_gridManager = gridManager;
			_player = player;
		}

		private void Awake()
		{
			_gridManager.RefreshGameManager(_gameManager);
		}

		private void Start()
		{
			_reversiView.OnGridClicked()
				.Where(_ => _gridManager.CanPutGridCount.Value > 0)
				.TakeUntil(_gameManager.NowGameState.Where(state => state == GameState.GameSet))
				.Subscribe(gridData =>
				{
					try
					{
						var playerPutGridData = _player.Put(gridData, _gameManager);
						_gridManager.ReceivePieceFromPlayer(playerPutGridData);
						_gridManager.FlipPiece(playerPutGridData);
						_gameManager.ChangeGameState();
						_gridManager.RefreshGameManager(_gameManager);
						_gridManager.RefreshGrid();
					}
					catch (Exception e)
					{
						Debug.Log(e);
					}
				});
			
			var subject1 = _gridManager.BlackPieceCount;
			var subject2 = _gridManager.WhitePieceCount;
			subject1
				.CombineLatest(subject2, (item1, item2) => new Tuple<int, int>(item1, item2))
				.Subscribe(tuple => _reversiGUI.SetPieceCount(tuple.Item1, tuple.Item2))
				.AddTo(this);

			subject1
				.CombineLatest(subject2, (item1, item2) => new Tuple<int, int>(item1, item2))
				.Where(tuple => tuple.Item1 + tuple.Item2 == GridManager.GridCount)
				.TakeUntil(_gameManager.NowGameState.Where(state => state == GameState.GameSet))
				.Subscribe(_ =>
				{
					_gameManager.GameSet();
				});

			_gridManager.CanPutGridCount
				.Where(value => value == 0)
				.Subscribe(value =>
				{
					_gameManager.ChangeGameState();
					_gridManager.RefreshGameManager(_gameManager);
					_gridManager.RefreshGrid();
					
					if (_gridManager.CanPutGridCount.Value == 0)
					{
						_gameManager.GameSet();
					}
				});

			_gameManager.NowGameState
				.Subscribe(gameState =>
				{
					_reversiGUI.ShowNowTurn(gameState);
				}).AddTo(this);

			_gameManager.NowGameState
				.Where(state => state == GameState.GameSet)
				.Subscribe(_ =>
				{
					var outcome = _gridManager.JudgeWinner();
					_reversiGUI.ShowWinner(outcome);
				}).AddTo(this);
		}
	}
}