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
			var hexCoords = HexagonalVector.GetHexagonalVectorFromInGameCoords(x - pivot.x, y - pivot.y);
			hexCoords.RotateCW();
			var newCell = new Cell();
			hexCoords.GetInGameCoords(out newCell.x, out newCell.y);
			newCell.x += pivot.x;
			newCell.y += pivot.y;
			return newCell;
		}

		public Cell RotateCCW(Cell pivot)
		{
			var hexCoords = HexagonalVector.GetHexagonalVectorFromInGameCoords(x - pivot.x, y - pivot.y);
			hexCoords.RotateCW();
			var newCell = new Cell();
			hexCoords.GetInGameCoords(out newCell.x, out newCell.y);
			newCell.x += pivot.x;
			newCell.y += pivot.y;
			return newCell;
		}
	}
}
