using TMPro;
using UnityEngine;

namespace OneMonoBehaviourReversi
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class ReversiGUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI blackPieceCountText = default;
		[SerializeField] private TextMeshProUGUI whitePieceCountText = default;

		public void SetPieceCount(int blackPieceCount, int whitePieceCount)
		{
			blackPieceCountText.text = $"BLACK : {blackPieceCount.ToString()}";
			whitePieceCountText.text = $"WHITE : {whitePieceCount.ToString()}";
		}
	}
}