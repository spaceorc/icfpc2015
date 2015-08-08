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
        public void MoveCellTests_Even_One()
        {
            TestMove(2, 4, MoveType.E,  3, 4);
            TestMove(2, 4, MoveType.SE, 2, 5);
            TestMove(2, 4, MoveType.SW, 1, 5);
            TestMove(2, 4, MoveType.W,  1, 4);
            TestMove(2, 4, MoveType.NW, 1, 3);
            TestMove(2, 4, MoveType.NE, 2, 3);
        }

        [Test]
        public void MoveCellTests_Odd_One()
        {
            TestMove(2, 5, MoveType.E,  3, 5);
            TestMove(2, 5, MoveType.SE, 3, 6);
            TestMove(2, 5, MoveType.SW, 2, 6);
            TestMove(2, 5, MoveType.W,  1, 5);
            TestMove(2, 5, MoveType.NW, 2, 4);
            TestMove(2, 5, MoveType.NE, 3, 4);
        }

        [Test]
        public void MoveCellTests_Even_Three()
        {
            TestMove(2, 4, MoveType.E,  5, 4, length: 3);
            TestMove(2, 4, MoveType.SE, 3, 7, length: 3);
            TestMove(2, 4, MoveType.SW, 0, 7, length: 3);
            TestMove(2, 4, MoveType.W, -1, 4, length: 3);
            TestMove(2, 4, MoveType.NW, 0, 1, length: 3);
            TestMove(2, 4, MoveType.NE, 3, 1, length: 3);
        }
        
        [Test]
        public void MoveCellTests_Odd_Three()
        {
            TestMove(2, 5, MoveType.E,  5, 5, length: 3);
            TestMove(2, 5, MoveType.SE, 4, 8, length: 3);
            TestMove(2, 5, MoveType.SW, 1, 8, length: 3);
            TestMove(2, 5, MoveType.W, -1, 5, length: 3);
            TestMove(2, 5, MoveType.NW, 1, 2, length: 3);
            TestMove(2, 5, MoveType.NE, 4, 2, length: 3);
        }
        
        [Test]
        public void MoveCellTests_Even_Four()
        {
            TestMove(2, 4, MoveType.E,  6, 4, length: 4);
            TestMove(2, 4, MoveType.SE, 4, 8, length: 4);
            TestMove(2, 4, MoveType.SW, 0, 8, length: 4);
            TestMove(2, 4, MoveType.W, -2, 4, length: 4);
            TestMove(2, 4, MoveType.NW, 0, 0, length: 4);
            TestMove(2, 4, MoveType.NE, 4, 0, length: 4);
        }
        
        [Test]
        public void MoveCellTests_Odd_Four()
        {
            TestMove(2, 5, MoveType.E,  6, 5, length: 4);
            TestMove(2, 5, MoveType.SE, 4, 9, length: 4);
            TestMove(2, 5, MoveType.SW, 0, 9, length: 4);
            TestMove(2, 5, MoveType.W, -2, 5, length: 4);
            TestMove(2, 5, MoveType.NW, 0, 1, length: 4);
            TestMove(2, 5, MoveType.NE, 4, 1, length: 4);
        }

	    private void TestMove(int fX, int fY, MoveType moveType, int tX, int tY, int length = 1)
	    {
            var cell = new Cell { x = fX, y = fY, filled = false };
	        var actual = cell.Move(new Vector(moveType, cell.y, length));
            Assert.AreEqual(actual.x, tX);
            Assert.AreEqual(actual.y, tY);
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

		[Test]
		public void RotateTests_Bug()
		{
			var pivot = new Cell {x = 1, y = -1};
			var cell = new Cell {x = 3, y = 0};
			cell = cell.RotateCW(pivot);
			CheckEqual(cell, 1, 1);
			cell = new Cell {x = 2, y = 1};
			cell = cell.RotateCW(pivot);
			CheckEqual(cell, 0, 1);
			cell = new Cell {x = 1, y = 1};
			cell = cell.RotateCW(pivot);
			CheckEqual(cell, 0, 0);
		}

		private void CheckEqual(Cell cell, int x, int y)
		{
			Assert.AreEqual(cell.x, x, String.Format("expected {0} {1}, but found {2} {3}", x, y, cell.x, cell.y));
			Assert.AreEqual(cell.y, y, String.Format("expected {0} {1}, but found {2} {3}", x, y, cell.x, cell.y));
		}
	}
}
