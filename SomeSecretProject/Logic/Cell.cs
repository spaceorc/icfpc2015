using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeSecretProject.Logic
{
	public class Cell
	{
		public bool Locked;
		public int X;
		public int Y;

		public Cell Move(Vector vector)
		{
			return new Cell {X = X + vector.X, Y = Y + vector.Y, Locked = Locked};
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
