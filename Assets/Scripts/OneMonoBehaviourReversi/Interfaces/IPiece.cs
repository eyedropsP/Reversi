using OneMonoBehaviourReversi.Enums;

namespace OneMonoBehaviourReversi.Interfaces
{
	public interface IPiece
	{
		PieceState PieceState { get; set; }
		void ReversePieceType();
	}
}