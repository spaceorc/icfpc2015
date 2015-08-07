using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeSecretProject.Logic
{
	public class Vector
	{
		public int X;
		public int Y;

		public Vector(MoveType move, int y)
		{
			switch (move)
			{
				case MoveType.E:
					X = 1;
					break;
				case MoveType.W:
					X = -1;
					break;
				case MoveType.SW:
					Y = 1;
					X = y%2 == 0 ? -1 : 0;
					break;
				case MoveType.SE:
					Y = 1;
					X = y % 2 == 0 ? 0 : 1;
					break;
				case MoveType.NW:
					Y = -1;
					X = y%2 == 0 ? -1 : 0;
					break;
				case MoveType.NE:
					Y = -1;
					X = y % 2 == 0 ? 0 : 1;
					break;
				default:
					throw new ArgumentOutOfRangeException(move.ToString(), move, null);
			}
		}
	}
}
