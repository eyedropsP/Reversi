using UnityEngine;
using UnityEngine.Serialization;

namespace OneMonoBehaviourReversi
{
	public class ReversiPresenter : MonoBehaviour
	{
		[FormerlySerializedAs("reversiView")] [SerializeField] private ReversiView _reversiView;
		[FormerlySerializedAs("reversiGUI")] [SerializeField] private ReversiGUI _reversiGUI;

		private void Awake()
		{
			
		}

		private void Update()
		{
			
		}
	}
}