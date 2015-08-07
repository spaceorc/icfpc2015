namespace SomeSecretProject.Logic
{
	public class Map
	{
		public Cell[,] Cells;

		public Map(int width, int height)
		{
			Cells = new Cell[width, height];
		}
	}
}