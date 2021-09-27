using System;
using System.Collections.Generic;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Interface;
using NormalReversi.Models.Struct;
using UnityEngine;
using UnityEngine.Serialization;

namespace NormalReversi.Models
{
    public class GridData : MonoBehaviour, IGridData
    {
        [FormerlySerializedAs("spriteRenderer")] [SerializeField]
        private SpriteRenderer _spriteRenderer;

        private readonly Color _normalGridColor = new Color(0.2312f, 0.68f, 0.4556f, 1f);
        private readonly Color _highLightGridColor = Color.green;

        public GridState GridState { get; private set; }
        public Point Point { get; private set; }
        public HashSet<Tuple<int, int>> DirectionOffset { get; private set; }

        public bool IsCanPut => GridState == GridState.CanPut;

        public IPiece Piece { get; private set; }
        public Vector2 Location { get; private set; }

        public void SetSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            _spriteRenderer = spriteRenderer;
        }

        public void Initialize(Point point, Vector2 location, GridState gridState)
        {
            Point = point;
            GridState = gridState;
            Location = location;
            _spriteRenderer.color = _normalGridColor;
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
            _spriteRenderer.color = gridState == GridState.CanPut ? _highLightGridColor : _normalGridColor;
        }
    }
}