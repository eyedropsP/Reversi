using System;
using System.IO;
using NormalReversi.Models.Interface;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace NormalReversi.View
{
	public class ReversiView : MonoBehaviour
	{
		private Transform _putLocationTransform;
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

		public GameObject SetPieceColor(Color color)
		{
			var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Piece");
			var spriteRenderer = loadAssetAtPath.GetComponent<SpriteRenderer>();
			spriteRenderer.color = color;
			return loadAssetAtPath;
		}
	}
}