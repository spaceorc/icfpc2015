using System;
using System.Linq;

namespace SomeSecretProject.Logic
{
	public class Unit
	{
		public Cell pivot;
		public Cell[] cells;

		public Unit Move(MoveType move)
		{
			switch (move)
			{
				case MoveType.E:
				case MoveType.W:
				case MoveType.SW:
				case MoveType.SE:
					return new Unit { pivot = pivot.Move(new Vector(move, pivot.y)), cells = cells.Select(cell => cell.Move(new Vector(move, cell.y))).ToArray() };
				case MoveType.RotateCW:
					return new Unit { pivot = pivot, cells = cells.Select(cell => cell.RotateCW(pivot)).ToArray() };
				case MoveType.RotateCCW:
					return new Unit { pivot = pivot, cells = cells.Select(cell => cell.RotateCCW(pivot)).ToArray() };
				default:
					throw new ArgumentOutOfRangeException(move.ToString(), move, null);
			}
		}
	}
}