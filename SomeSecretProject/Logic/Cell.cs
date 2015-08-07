using Newtonsoft.Json;

namespace SomeSecretProject.Logic
{
	[JsonObject]
	public class Cell
	{
		[JsonIgnore]
		public bool filled;

		[JsonProperty("x")]
		public int x;

		[JsonProperty("y")]
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
			var hexCoords = HexagonalVector.GetHexagonalVectorFromInGameCoords(x, y);
			var pivotHexCoords = HexagonalVector.GetHexagonalVectorFromInGameCoords(pivot.x, pivot.y);
			hexCoords.X -= pivotHexCoords.X;
			hexCoords.Y -= pivotHexCoords.Y;
			hexCoords.RotateCW();
			hexCoords.X += pivotHexCoords.X;
			hexCoords.Y += pivotHexCoords.Y;
			var newCell = new Cell();
			hexCoords.GetInGameCoords(out newCell.x, out newCell.y);
			return newCell;
		}

		public Cell RotateCCW(Cell pivot)
		{
			var hexCoords = HexagonalVector.GetHexagonalVectorFromInGameCoords(x, y);
			var pivotHexCoords = HexagonalVector.GetHexagonalVectorFromInGameCoords(pivot.x, pivot.y);
			hexCoords.X -= pivotHexCoords.X;
			hexCoords.Y -= pivotHexCoords.Y;
			hexCoords.RotateCCW();
			hexCoords.X += pivotHexCoords.X;
			hexCoords.Y += pivotHexCoords.Y;
			var newCell = new Cell();
			hexCoords.GetInGameCoords(out newCell.x, out newCell.y);
			return newCell;
		}
	}
}
