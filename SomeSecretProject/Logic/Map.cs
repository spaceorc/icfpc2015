using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SomeSecretProject.Logic
{
	public class Map
	{
		private readonly List<Cell[]> cells;

		public readonly int Height;
		public readonly int Width;

		public Cell this[int x, int y]
		{
			get { return cells[y][x]; }
			set { cells[y][x] = value; }
		}
		public Cell this[Cell cell]
		{
			get { return cells[cell.y][cell.x]; }
			set { cells[cell.y][cell.x] = value; }
		}
		

		public Map(int width, int height)
		{
			cells = new List<Cell[]>();
			RefineMap();
		}

		private void RefineMap()
		{
			while(cells.Count < Height)
				cells.Add(new Cell[Width]);

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if(cells[y][x] == null)
						cells[y][x] = new Cell();
					cells[y][x].y = y;
					cells[y][x].x = x;
				}
			}
		}

		public void LockUnit(Unit unit)
		{
			foreach (var cell in unit.cells)
			{
				this[cell.x, cell.y].filled = true;
			}
		}

		public int RemoveLines()
		{
			List<int> lockedLines = new List<int>();
			for (int i = 0; i < cells.Count; i++)
			{
				if(cells[i].All(cell => cell.filled))
					lockedLines.Add(i);
			}

			for (int i = lockedLines.Count - 1; i >= 0; i--)
			{
				cells.RemoveAt(lockedLines[i]);
			}

			RefineMap();

			return lockedLines.Count;
		}
	}
}