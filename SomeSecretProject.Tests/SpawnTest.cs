using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Tests
{
	[TestFixture]
	public class SpawnTest
	{
		[Test]
		public void SmallBacteriaTest()
		{
			var problem = new Problem { width = 10 };
			var unit = new Unit { members = new HashSet<Cell>(new[] { new Cell { x = 1, y = 1 } }), pivot = new Cell { x = 2, y = 2 } };

			var placedUnit = Game.SpawnUnit(unit, problem);
			Assert.AreEqual(new Cell { x = 4, y = 0 }, placedUnit.members.Single());
			Assert.AreEqual(new Cell { x = 4, y = 1 }, placedUnit.pivot);
		}

		[Test]
		public void SinglePointTest_5()
		{
			var problem = new Problem { width = 5 };
			var unit = new Unit { members = new HashSet<Cell>(new[] { new Cell { x = 5, y = 5 } }), pivot = new Cell { x = 5, y = 5 } };

			var placedUnit = Game.SpawnUnit(unit, problem);

			Assert.AreEqual(new Cell { x = 2, y = 0 }, placedUnit.members.Single());
			Assert.AreEqual(new Cell { x = 2, y = 0 }, placedUnit.pivot);
		}

		[Test]
		public void LongVTest()
		{
			var problem = new Problem { width = 6 };
			var unit = new Unit
			{
				members = new HashSet<Cell>(new[]
				                      {
					                      new Cell { x = 0, y = 2 },
					                      new Cell { x = 0, y = 3 },
					                      new Cell { x = 1, y = 4 },
					                      new Cell { x = 1, y = 3 },
					                      new Cell { x = 2, y = 2 }
				                      }),
				pivot = new Cell { x = 1, y = 0 }
			};

			var placedUnit = Game.SpawnUnit(unit, problem);

			var members = placedUnit.members.ToArray();
			Assert.AreEqual(new Cell { x = 1, y = 0 }, members[0]);
			Assert.AreEqual(new Cell { x = 1, y = 1 }, members[1]);
			Assert.AreEqual(new Cell { x = 2, y = 2 }, members[2]);
			Assert.AreEqual(new Cell { x = 2, y = 1 }, members[3]);
			Assert.AreEqual(new Cell { x = 3, y = 0 }, members[4]);
			Assert.AreEqual(new Cell { x = 2, y = -2 }, placedUnit.pivot);
		}
	}
}