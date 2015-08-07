using NUnit.Framework;

namespace SomeSecretProject.Logic
{
	public class Map
	{
		public readonly Cell[,] cells;

		public Cell this[int x, int y]
		{
			get { return cells[x, y]; }
			set { cells[x, y] = value; }
		}

		public int Width
		{
			get { return cells.GetLength(0); }
		}

		public int Height
		{
			get { return cells.GetLength(1); }
		}

		public Map(int width, int height)
		{
			cells = new Cell[width, height];
		}
	}
}