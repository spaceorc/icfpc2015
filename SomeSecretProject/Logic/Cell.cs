using System;

namespace SomeSecretProject.Logic
{
	public class Cell
	{
		public bool filled;
		public int x;
		public int y;

		public Cell Fill()
		{
			return new Cell { x = x, y = y, filled = true };
		}

		public Cell Move(Vector vector)
		{
			return new Cell { x = x + vector.X, y = y + vector.Y, filled = filled };
		}

		public Cell RotateCW(Cell pivot)
		{
			var hexCoords = HexagonalVector.GetHexagonalVectorFromInGameCoords(X - pivot.X, Y - pivot.Y);
			hexCoords.RotateCW();
			var newCell = new Cell();
			hexCoords.GetInGameCoords(out newCell.X, out newCell.Y);
			newCell.X += pivot.X;
			newCell.Y += pivot.Y;
			return newCell;
		}

		public Cell RotateCCW(Cell pivot)
		{
			var hexCoords = HexagonalVector.GetHexagonalVectorFromInGameCoords(X - pivot.X, Y - pivot.Y);
			hexCoords.RotateCW();
			var newCell = new Cell();
			hexCoords.GetInGameCoords(out newCell.X, out newCell.Y);
			newCell.X += pivot.X;
			newCell.Y += pivot.Y;
			return newCell;
		}
	}
}
