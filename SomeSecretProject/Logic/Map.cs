using System;
using System.Collections.Generic;
using System.Linq;

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

		public Map(string map)
		{
			cells = map.Split('\n')
				.Select(row => row
					.Where(c => c == '.' || c == 'X')
					.Select(c => new Cell {filled = c == 'X'})
					.ToArray())
				.ToList();
			Height = cells.Count;
			Width = cells.Max(row => row.Length);
			RefineMap();
		}
		
		public Map(int width, int height)
		{
			Width = width;
			Height = height;
			cells = new List<Cell[]>();
			RefineMap();
		}

		public Map Clone()
		{
			var clone = new Map(Width, Height);
			for (int r = 0; r < clone.cells.Count; r++)
			{
				var row = clone.cells[r];
				for (int c = 0; c < row.Length; c++)
					row[c] = cells[r][c].Clone();
			}
			return clone;
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
			foreach (var cell in unit.members)
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

            for (int i=0; i<lockedLines.Count; i++)
                cells.Insert(0, new Cell[Width]);

			RefineMap();

			return lockedLines.Count;
		}

		public override string ToString()
		{
			return string.Join("\r\n", cells.Select((row, i) => (i%2 == 0 ? "" : " ") + String.Join(" ", row.Select(cell => cell.filled ? "X" : "."))));
		}

	}
}