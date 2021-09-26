using NormalReversi.Models.Interface;
using UnityEngine;
using UnityEngine.Serialization;

namespace NormalReversi.Models
{
	public class Piece : MonoBehaviour, IPiece
	{
		[FormerlySerializedAs("spriteRenderer")] [SerializeField] private SpriteRenderer _spriteRenderer;
		
		public void InitColor(Color color)
		{
			_spriteRenderer.color = color;
		}
	}
}