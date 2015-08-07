namespace SomeSecretProject.Logic
{
	public class Map
	{
		public Cell[,] Cells;

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
			Cells = new Cell[width, height];
		}
	}
}