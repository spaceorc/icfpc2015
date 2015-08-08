using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    public class CellSurroundingTest
    {
        [Test]
        public void TestGetSurroundingCells()
        {
            Test(new HashSet<Cell>(){new Cell(){y=3, x=1}, new Cell(){y=2, x=2}, new Cell(){y=2, x=3}},
                new HashSet<Cell>()
                {
                    new Cell(){y=1, x=1}, new Cell(){y=1, x=2}, new Cell(){y=1,x=3},
                    new Cell(){y=2, x=1}, new Cell(){y=2, x=4},
                    new Cell(){y=3, x=0}, new Cell(){y=3,x=2}, new Cell(){y=3,x=3},
                    new Cell(){y=4, x=1}, new Cell(){y=4, x=2}
                });
        }

        public void Test(HashSet<Cell> unitCells, HashSet<Cell> surround)
        {
            Unit unit = new Unit(){members = unitCells, pivot = new Cell(){x=0,y=0}};
            var surrounding = unit.GetSurroundingCells();
            Assert.IsTrue(surround.Count == surrounding.Length);
            foreach (var surroundCell in surrounding)
                Assert.IsTrue(surround.Contains(surroundCell));
        }
    }
}
