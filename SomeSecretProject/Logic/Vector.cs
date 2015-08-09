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

		public Vector(MoveType move, int y, int length = 1)
		{
			switch (move)
			{
				case MoveType.E:
					X = length;
					break;
				case MoveType.W:
					X = -length;
					break;
				case MoveType.SW:
					Y = length;
			        X = GetWestDX(y, length);
			        break;
				case MoveType.SE:
					Y = length;
			        X = GetEastDX(y, length);
			        break;
				case MoveType.NW:
                    Y = -length;
                    X = GetWestDX(y, length);
					break;
				case MoveType.NE:
                    Y = -length;
                    X = GetEastDX(y, length);
					break;
				default:
					throw new ArgumentOutOfRangeException(move.ToString(), move, null);
			}
		}

	    private static int GetEastDX(int y, int length)
	    {
	        return y%2 == 0 ? length/2 : (length + 1)/2;
	    }

	    private static int GetWestDX(int y, int length)
	    {
	        return y%2 == 0 ? -(length + 1)/2 : -length/2;
	    }
	}
}
