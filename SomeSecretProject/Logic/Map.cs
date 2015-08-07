using NUnit.Framework;

namespace SomeSecretProject.Logic
{
	public class Map
	{
		public readonly Cell[,] cells;

		public Cell this[int x, int y]
		{
			get { return Cells[x, y]; }
			set { Cells[x, y] = value; }
		}

		public int Width
		{
			get { return Cells.GetLength(0); }
		}

		public int Height
		{
			get { return Cells.GetLength(1); }
		}

		public Map(int width, int height)
		{
			cells = new Cell[width, height];
		}
	}
}