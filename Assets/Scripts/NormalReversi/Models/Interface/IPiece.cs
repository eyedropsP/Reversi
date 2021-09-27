using UnityEngine;

namespace NormalReversi.Models.Interface
{
	public interface IPiece
	{
		void InitColor(Color color);
		void SetSpriteRenderer(SpriteRenderer spriteRenderer);
	}
}