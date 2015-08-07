using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeSecretProject.Logic
{
	class Map
	{
		private Cell[,] cells;

		public Map(int width, int height)
		{
			cells = new Cell[width, height];
		}

	}
}
