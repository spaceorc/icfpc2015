using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SomeSecretProject.Algorithm;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Tests
{
	[TestFixture]
	public class FastPositions_Test
	{
		[Test]
		public void Test()
		{
			var map = new Map(3, 3);
			var originalUnit = new Unit
			{
				pivot = new Cell { x = 1, y = 0 },
				members = new HashSet<Cell> { new Cell { x = 1, y = 0 } }
			};
			var fastPositions = new FastPositions(map, originalUnit, new string[0], new bool[0]);
			fastPositions.BuildAllPositions();

			var updatedMap = map.Clone();
			var endPosition = fastPositions.endPositions.ElementAt(0);
			updatedMap.LockUnit(endPosition.Value.item.unit);

			var patch = new FastPositionsPatch(updatedMap, fastPositions);
			patch.BuildAllPositions();

			Assert.AreEqual(7, patch.endPositions.Count);

			var appliedPositions = patch.Apply();


		}
	}
}