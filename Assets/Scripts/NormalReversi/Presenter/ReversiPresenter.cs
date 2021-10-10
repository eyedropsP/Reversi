using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Manager;
using NormalReversi.View;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace NormalReversi.Presenter
{
    public class ReversiPresenter : IDisposable, IAsyncStartable
    {
        private readonly ReversiObjectView _reversiObjectView;
        private readonly ReversiGUIView _reversiGUIView;
        private readonly IGameStateManager _gameStateManager;
        private readonly IGridManager _gridManager;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public ReversiPresenter(IGameStateManager gameStateManager, IGridManager gridManager,
            ReversiObjectView reversiObjectView, ReversiGUIView reversiGUIView)
        {
            _gameStateManager = gameStateManager;
            _gridManager = gridManager;
            _reversiObjectView = reversiObjectView;
            _reversiGUIView = reversiGUIView;
        }

        public void Dispose() => _disposable.Dispose();

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            _gridManager.RefreshGameManager(_gameStateManager);
            _gridManager.Initialize();

            _reversiObjectView.OnGridClicked()
                .Where(_ => _gridManager.CanPutGridCount.Value > 0)
                .Where(gridData => gridData.IsCanPut)
                .TakeUntil(_gameStateManager.NowGameState.Where(state => state == GameState.GameSet))
                .Subscribe(gridData =>
                {
                    try
                    {
                        _gridManager.SetPiece(gridData);
                        _gridManager.FlipPiece(gridData);
                        _gameStateManager.ChangeGameState();
                        _gridManager.RefreshGameManager(_gameStateManager);
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
                .Subscribe(tuple => _reversiGUIView.SetPieceCount(tuple.Item1, tuple.Item2))
                .AddTo(_disposable);

            blackPieceCountReactiveProperty
                .CombineLatest(whitePieceCountReactiveProperty, (item1, item2) => new Tuple<int, int>(item1, item2))
                .Where(tuple => tuple.Item1 + tuple.Item2 == GridManager.GridCount)
                .TakeUntil(_gameStateManager.NowGameState.Where(state => state == GameState.GameSet))
                .Subscribe(_ => { _gameStateManager.GameSet(); })
                .AddTo(_disposable);

            _gridManager.CanPutGridCount
                .Where(value => value == 0)
                .Subscribe(value =>
                {
                    _gameStateManager.ChangeGameState();
                    _gridManager.RefreshGameManager(_gameStateManager);
                    _gridManager.RefreshGrid();

                    if (_gridManager.CanPutGridCount.Value == 0)
                    {
                        _gameStateManager.GameSet();
                    }
                })
                .AddTo(_disposable);

            _gameStateManager.NowGameState
                .Subscribe(gameState => { _reversiGUIView.ShowNowTurn(gameState); })
                .AddTo(_disposable);

            _gameStateManager.NowGameState
                .Where(state => state == GameState.GameSet)
                .Subscribe(_ =>
                {
                    var outcome = _gridManager.JudgeWinner();
                    _reversiGUIView.ShowWinner(outcome);
                })
                .AddTo(_disposable);
        }
    }
}