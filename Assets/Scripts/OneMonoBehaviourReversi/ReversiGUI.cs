using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace OneMonoBehaviourReversi
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class ReversiGUI : MonoBehaviour
	{
		[FormerlySerializedAs("blackPieceCountText")] [SerializeField] private TextMeshProUGUI _blackPieceCountText;
		[FormerlySerializedAs("whitePieceCountText")] [SerializeField] private TextMeshProUGUI _whitePieceCountText;

		public void SetPieceCount(int blackPieceCount, int whitePieceCount)
		{
			_blackPieceCountText.text = $"BLACK : {blackPieceCount.ToString()}";
			_whitePieceCountText.text = $"WHITE : {whitePieceCount.ToString()}";
		}
	}
}