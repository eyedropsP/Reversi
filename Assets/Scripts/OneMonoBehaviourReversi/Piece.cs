using OneMonoBehaviourReversi.Enums;
using OneMonoBehaviourReversi.Interfaces;

namespace OneMonoBehaviourReversi
{
	public class Piece : IPiece
	{
		private PieceState PieceState { get; set; }

		PieceState IPiece.PieceState
		{
			get => this.PieceState;
			set => this.PieceState = value;
		}

		public void ReversePieceType()
		{
			if (PieceState == PieceState.Black)
			{
				PieceState = PieceState.White;
				return;
			}

			PieceState = PieceState.Black;
		}
	}
}