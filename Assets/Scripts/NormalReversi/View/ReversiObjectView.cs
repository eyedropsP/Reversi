using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using UniRx;
using UnityEngine;

namespace NormalReversi.View
{
    public class ReversiObjectView : MonoBehaviour
    {
        private Transform _putLocationTransform;
        private AssetBundle _assetBundle;

        private async void Awake()
        {
            _assetBundle = await AssetBundle.LoadFromFileAsync("Assets/AssetBundles/environments");
        }

        public IObservable<IGridData> OnGridClicked()
        {
            
            var gridDataObservable = Observable
                .EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Select(_ => Input.mousePosition)
                .Select(mousePosition =>
                {
                    var raycastHit2D = new RaycastHit2D();
                    if (Camera.main == null) return raycastHit2D;
                    var tmpRay = Camera.main.ScreenPointToRay(mousePosition);
                    raycastHit2D = Physics2D.Raycast(tmpRay.origin, tmpRay.direction);

                    return raycastHit2D;
                })
                .Where(hit2D => hit2D.transform)
                .Select(hit2D =>
                {
                    _putLocationTransform = hit2D.transform;
                    _putLocationTransform.TryGetComponent(out IGridData gridData);
                    return gridData;
                });
            return gridDataObservable;
        }

        private async Task<IGridData> PutPieceObjectAsync(IGridData gridData, GameState gameState)
        {
            var pieceObject = (GameObject)await _assetBundle.LoadAssetAsync<GameObject>("Piece");
            Instantiate(pieceObject, gridData.Location, Quaternion.identity)
                .TryGetComponent(out IPiece piece);
            SetPieceColor(piece, gameState);
            gridData.AcceptPiece(piece);
            return gridData;
        }

        private static void SetPieceColor(IPiece piece, GameState gameState)
        {
            switch (gameState)
            {
                case GameState.BlackTurn:
                    piece.InitColor(Color.black);
                    break;
                case GameState.WhiteTurn:
                    piece.InitColor(Color.white);
                    break;
                case GameState.GameSet:
                    break;
                default:
                    throw new Exception("異常なゲームステートです。");
            }
        }
    }
}