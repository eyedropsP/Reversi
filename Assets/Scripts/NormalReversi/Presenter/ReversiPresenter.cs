using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Manager;
using NormalReversi.View;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace NormalReversi.Presenter
{
    public class ReversiPresenter : IDisposable, IAsyncStartable
    {
        private readonly ReversiView _reversiView;
        private readonly ReversiGUI _reversiGUI;
        private readonly IGameStateManager _gameStateManager;
        private readonly IGridManager _gridManager;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        public ReversiPresenter(IGameStateManager gameStateManager, IGridManager gridManager,
            ReversiView reversiView, ReversiGUI reversiGUI)
        {
            _gameStateManager = gameStateManager;
            _gridManager = gridManager;
            _reversiView = reversiView;
            _reversiGUI = reversiGUI;
        }

        public void Dispose() => _disposable.Dispose();

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            var assetBundle = await AssetBundle.LoadFromFileAsync("Assets/AssetBundles/environments");
            var pieceObject = await assetBundle.LoadAssetAsync<GameObject>("Piece");
            
            Debug.Log(pieceObject.name);
            
            // var handle = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/MainPage.prefab");
            // handle.Completed += op =>
            // {
            //     if (op.Status == AsyncOperationStatus.Succeeded)
            //         Object.Instantiate(op.Result);
            // };
            
            _gridManager.RefreshGameManager(_gameStateManager);
            _gridManager.Initialize();

            _reversiView.OnGridClicked()
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
                .Subscribe(tuple => _reversiGUI.SetPieceCount(tuple.Item1, tuple.Item2))
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
                .Subscribe(gameState => { _reversiGUI.ShowNowTurn(gameState); })
                .AddTo(_disposable);

            _gameStateManager.NowGameState
                .Where(state => state == GameState.GameSet)
                .Subscribe(_ =>
                {
                    var outcome = _gridManager.JudgeWinner();
                    _reversiGUI.ShowWinner(outcome);
                })
                .AddTo(_disposable);
        }
    }
}