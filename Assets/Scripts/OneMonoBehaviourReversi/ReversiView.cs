using System;
using UniRx;
using UnityEngine;

namespace OneMonoBehaviourReversi
{
	public class ReversiView : MonoBehaviour
	{

		public IObservable<Ray> OnGridClicked()
		{
			var rayObservable = Observable
				.EveryUpdate()
				.Where(_ => Input.GetMouseButtonDown(0))
				.Select(_ => Input.mousePosition)
				.Select(mousePosition =>
				{
					var tmpRay = new Ray();
					if (Camera.main != null)
					{
						tmpRay = Camera.main.ScreenPointToRay(mousePosition);
					}

					return tmpRay;
				});
			return rayObservable;
		} 
	}
}