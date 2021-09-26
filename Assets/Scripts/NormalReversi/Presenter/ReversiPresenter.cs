using System;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Manager;
using NormalReversi.View;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace NormalReversi.Presenter
{
    public class ReversiPresenter : IStartable, IDisposable
    {
        private readonly ReversiView _reversiView;
        private readonly ReversiGUI _reversiGUI;
        private readonly IGameManager _gameManager;
        private readonly IGridManager _gridManager;
        private readonly IPlayer _player;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public ReversiPresenter(IGameManager gameManager, IGridManager gridManager, IPlayer player,
            ReversiView reversiView, ReversiGUI reversiGUI)
        {
            _gameManager = gameManager;
            _gridManager = gridManager;
            _player = player;
            _reversiView = reversiView;
            _reversiGUI = reversiGUI;
        }

        void IStartable.Start()
        {
            _gridManager.RefreshGameManager(_gameManager);
            _gridManager.Initialize();

            _reversiView.OnGridClicked()
                .Where(_ => _gridManager.CanPutGridCount.Value > 0)
                .Where(gridData => gridData.IsCanPut)
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
                })
                .AddTo(_disposable);

            var blackPieceCountReactiveProperty = _gridManager.BlackPieceCount;
            var whitePieceCountReactiveProperty = _gridManager.WhitePieceCount;

            blackPieceCountReactiveProperty
                .CombineLatest(whitePieceCountReactiveProperty, (item1, item2) => new Tuple<int, int>(item1, item2))
                .Subscribe(tuple => _reversiGUI.SetPieceCount(tuple.Item1, tuple.Item2))
                .AddTo(_disposable);

            blackPieceCountReactiveProperty
                .CombineLatest(whitePieceCountReactiveProperty, (item1, item2) => new Tuple<int, int>(item1, item2))
                .Where(tuple => tuple.Item1 + tuple.Item2 == GridManager.GridCount)
                .TakeUntil(_gameManager.NowGameState.Where(state => state == GameState.GameSet))
                .Subscribe(_ => { _gameManager.GameSet(); })
                .AddTo(_disposable);

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
                })
                .AddTo(_disposable);

            _gameManager.NowGameState
                .Subscribe(gameState => { _reversiGUI.ShowNowTurn(gameState); })
                .AddTo(_disposable);

            _gameManager.NowGameState
                .Where(state => state == GameState.GameSet)
                .Subscribe(_ =>
                {
                    var outcome = _gridManager.JudgeWinner();
                    _reversiGUI.ShowWinner(outcome);
                })
                .AddTo(_disposable);
        }

        public void Dispose() => _disposable.Dispose();
    }
}