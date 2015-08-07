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
			throw new InvalidOperationException("TODO");
		}

		public Cell RotateCCW(Cell pivot)
		{
			throw new InvalidOperationException("TODO");
		}
	}
}