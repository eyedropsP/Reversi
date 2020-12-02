using OneMonoBehaviourReversi.Enums;
using UniRx;

namespace OneMonoBehaviourReversi
{
	public class GameManager
	{
		private GameState GameState { get; set; }
		private IntReactiveProperty blackPieceCount = new IntReactiveProperty();
		private IntReactiveProperty whitePieceCount = new IntReactiveProperty();
		
		public IReadOnlyReactiveProperty<int> BlackPieceCount => blackPieceCount;
		public IReadOnlyReactiveProperty<int> WhitePieceCount => whitePieceCount;
		
		
	}
}