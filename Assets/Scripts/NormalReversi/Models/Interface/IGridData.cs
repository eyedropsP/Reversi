﻿using System;
using System.Collections.Generic;
using NormalReversi.Models.Enum;
using NormalReversi.Models.Struct;
using UnityEngine;

namespace NormalReversi.Models.Interface
{
	public interface IGridData
	{
		IPiece Piece { get; }
		Vector2 Location { get; }
		GridState GridState { get; }
		Point Point { get; }
		HashSet<Tuple<int, int>> DirectionOffset { get; }
		void ChangeGridState(GridState gridState);
		void Init(Point point, Vector2 location, GridState gridState);
		void AcceptPiece(IPiece piece);
	}
}