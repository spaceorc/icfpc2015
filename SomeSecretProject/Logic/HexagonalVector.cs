using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeSecretProject.Logic
{
	class HexagonalVector
	{
		public int X;
		public int Y;

		public static HexagonalVector GetHexagonalVectorFromInGameCoords(int x, int y)
		{
			return new HexagonalVector {Y = y, X = x - y/2};
		}

		public void GetInGameCoords(out int x, out int y)
		{
			y = Y;
			x = X + y/2;
		}

		public void RotateCW()
		{
			var newY = X + Y;
			X = -Y;
			Y = newY;
		}
		
		public void RotateCCW()
		{
			var newX = X + Y;
			Y = -X;
			X = newX;
		}
	}
}
