using NormalReversi.Models.Interface;
using UnityEngine;

namespace NormalReversi.Models
{
	public class Piece : MonoBehaviour, IPiece
	{
		[SerializeField] private SpriteRenderer spriteRenderer;
		
		public void InitColor(Color color)
		{
			spriteRenderer.color = color;
		}
	}
}