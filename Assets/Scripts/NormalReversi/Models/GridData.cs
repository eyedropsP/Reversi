using System;
using System.Collections.Generic;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Struct;
using UnityEngine;

namespace NormalReversi.Models
{
	public class GridData : MonoBehaviour, IGridData
	{
		[SerializeField] private SpriteRenderer spriteRenderer;
		private Color normalGridColor = new Color(0.2312f, 0.68f, 0.4556f, 1f);
		private Color highLightGridColor = Color.green;
		
		public GridState GridState { get; private set; }
		public Point Point { get; private set; }
		public HashSet<Tuple<int, int>> DirectionOffset { get; private set; }
		public IPiece Piece { get; private set; }
		public Vector2 Location { get; private set; }
		
		public void Init(Point point, Vector2 location, GridState gridState)
		{
			Point = point;
			GridState = gridState;
			Location = location;
			spriteRenderer.color = normalGridColor;
			DirectionOffset = new HashSet<Tuple<int, int>>();
			Piece = null;
		}

		public void AcceptPiece(IPiece piece)
		{
			Piece = piece;
		}

		public void ChangeGridState(GridState gridState)
		{
			GridState = gridState;
			spriteRenderer.color = gridState == GridState.CanPut ? highLightGridColor : normalGridColor;
		}
	}
}