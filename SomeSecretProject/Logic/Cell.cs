using System;
using Newtonsoft.Json;

namespace SomeSecretProject.Logic
{
	[JsonObject]
	public class Cell : IEquatable<Cell>
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

		public override string ToString()
		{
			return string.Format("X: {1}, Y: {2}, Filled: {0}", filled, x, y);
		}

		public bool Equals(Cell other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return filled == other.filled && x == other.x && y == other.y;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != GetType())
				return false;
			return Equals((Cell)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = filled.GetHashCode();
				hashCode = (hashCode * 397) ^ x;
				hashCode = (hashCode * 397) ^ y;
				return hashCode;
			}
		}
	}
}
