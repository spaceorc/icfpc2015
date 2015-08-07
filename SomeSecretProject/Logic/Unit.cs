using System;
using System.Linq;

namespace SomeSecretProject.Logic
{
	public class Unit
	{
		public Cell Pivot;
		public Cell[] Cells;

		public Unit Move(MoveType move)
		{
			switch (move)
			{
				case MoveType.E:
				case MoveType.W:
				case MoveType.SW:
				case MoveType.SE:
					return new Unit { Pivot = Pivot.Move(new Vector(move, Pivot.Y)), Cells = Cells.Select(cell => cell.Move(new Vector(move, cell.Y))).ToArray() };
				case MoveType.RotateCW:
					return new Unit { Pivot = Pivot, Cells = Cells.Select(cell => cell.RotateCW(Pivot)).ToArray() };
				case MoveType.RotateCCW:
					return new Unit { Pivot = Pivot, Cells = Cells.Select(cell => cell.RotateCCW(Pivot)).ToArray() };
				default:
					throw new ArgumentOutOfRangeException(move.ToString(), move, null);
			}
		}

		public bool CheckCorrectness(Map map)
		{
			return !Cells.Any(cell => cell.X < 0 || cell.Y < 0 || cell.X >= map.Width || cell.Y >= map.Height || map[cell.X, cell.Y].Locked);
		}
	}
}