using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Tests
{
	[TestFixture]
	class CellMoveTests
	{
		[Test]
		public void MoveCellTests()
		{
			var cell = new Cell {x = 5, y = 0, filled = false};
			CheckEqual(cell, 5, 0);
			cell = cell.Move(new Vector(MoveType.E, cell.y));
			CheckEqual(cell, 6, 0);
			cell = cell.Move(new Vector(MoveType.SE, cell.y));
			CheckEqual(cell, 6, 1);
			cell = cell.Move(new Vector(MoveType.SE, cell.y));
			CheckEqual(cell, 7, 2);
			cell = cell.Move(new Vector(MoveType.SE, cell.y));
			CheckEqual(cell, 7, 3);
			cell = cell.Move(new Vector(MoveType.SE, cell.y));
			CheckEqual(cell, 8, 4);
			cell = cell.Move(new Vector(MoveType.W, cell.y));
			CheckEqual(cell, 7, 4);
			cell = cell.Move(new Vector(MoveType.W, cell.y));
			CheckEqual(cell, 6, 4);
			cell = cell.Move(new Vector(MoveType.W, cell.y));
			CheckEqual(cell, 5, 4);
			cell = cell.Move(new Vector(MoveType.W, cell.y));
			CheckEqual(cell, 4, 4);
			cell = cell.Move(new Vector(MoveType.SW, cell.y));
			CheckEqual(cell, 3, 5);
			cell = cell.Move(new Vector(MoveType.SW, cell.y));
			CheckEqual(cell, 3, 6);
			cell = cell.Move(new Vector(MoveType.SW, cell.y));
			CheckEqual(cell, 2, 7);
		}

		[Test]
		public void RotateTests()
		{
			var cell = new Cell { x = 0, y = 0, filled = false };
			var pivot = new Cell {x = 0, y = 1};
			cell = cell.RotateCW(pivot);
			CheckEqual(cell, 1, 0);
			cell = cell.RotateCW(pivot);
			CheckEqual(cell, 1, 1);
			cell = cell.RotateCW(pivot);
			CheckEqual(cell, 1, 2);
			cell = cell.RotateCW(pivot);
			CheckEqual(cell, 0, 2);
			cell = cell.RotateCCW(pivot);
			CheckEqual(cell, 1, 2);
			
			pivot = new Cell { x = 2, y = 3 };
			cell = cell.RotateCCW(pivot);
			CheckEqual(cell, 1, 4);
			cell = cell.RotateCCW(pivot);
			CheckEqual(cell, 2, 5);
			cell = cell.RotateCCW(pivot);
			CheckEqual(cell, 4, 4);
			
			pivot = new Cell { x = 1, y = 5 };
			cell = cell.RotateCW(pivot);
			CheckEqual(cell, 3, 7);
		}

		private void CheckEqual(Cell cell, int x, int y)
		{
			Assert.AreEqual(cell.x, x, String.Format("expected {0} {1}, but found {2} {3}", x, y, cell.x, cell.y));
			Assert.AreEqual(cell.y, y, String.Format("expected {0} {1}, but found {2} {3}", x, y, cell.x, cell.y));
		}
	}
}
