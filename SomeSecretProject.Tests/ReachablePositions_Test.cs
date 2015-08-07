using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SomeSecretProject.Algorithm;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Tests
{
	[TestFixture]
	public class ReachablePositions_Test
	{
		[Test]
		public void SimpleTest()
		{
			var map = new Map(3, 5);
			map[0,2] = map[0,2].Fill();
			map[1,2] = map[1,2].Fill();
			map[1,3] = map[1,3].Fill();
			var unit = "{pivot:{x:1,y:0},members:[{x:1,y:0}]}".ParseAsJson<Unit>();
			var reacher = new ReachablePositions(map);
			var expected = new[]
			               {
				               "{pivot:{x:0,y:0},members:[{x:0,y:0}]}".ParseAsJson<Unit>(),
				               "{pivot:{x:1,y:0},members:[{x:1,y:0}]}".ParseAsJson<Unit>(),		// start position
				               "{pivot:{x:2,y:0},members:[{x:2,y:0}]}".ParseAsJson<Unit>(),

				               "{pivot:{x:0,y:1},members:[{x:0,y:1}]}".ParseAsJson<Unit>(),
				               "{pivot:{x:1,y:1},members:[{x:1,y:1}]}".ParseAsJson<Unit>(),
				               "{pivot:{x:2,y:1},members:[{x:2,y:1}]}".ParseAsJson<Unit>(),

//				               "{pivot:{x:0,y:2},members:[{x:0,y:2}]}".ParseAsJson<Unit>(),		// wall
//				               "{pivot:{x:1,y:2},members:[{x:1,y:2}]}".ParseAsJson<Unit>(),		// wall
				               "{pivot:{x:2,y:2},members:[{x:2,y:2}]}".ParseAsJson<Unit>(),

//				               "{pivot:{x:0,y:3},members:[{x:0,y:3}]}".ParseAsJson<Unit>(),		// empty but unreachable!
//				               "{pivot:{x:1,y:3},members:[{x:1,y:3}]}".ParseAsJson<Unit>(),		// wall
				               "{pivot:{x:2,y:3},members:[{x:2,y:3}]}".ParseAsJson<Unit>(),

				               "{pivot:{x:0,y:4},members:[{x:0,y:4}]}".ParseAsJson<Unit>(),
				               "{pivot:{x:1,y:4},members:[{x:1,y:4}]}".ParseAsJson<Unit>(),
				               "{pivot:{x:2,y:4},members:[{x:2,y:4}]}".ParseAsJson<Unit>(),
			               };
			var actual = reacher.AllPositions(unit).OrderBy(u => Tuple.Create(u.pivot.y, u.pivot.x));
			CollectionAssert.AreEqual(expected, actual);
		}
	}
}
