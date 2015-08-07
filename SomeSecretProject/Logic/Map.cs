using NUnit.Framework;

namespace SomeSecretProject.Logic
{
	public class Map
	{
		public readonly Cell[,] cells;

		public Map(int width, int height)
		{
			cells = new Cell[width, height];
		}
	}
}