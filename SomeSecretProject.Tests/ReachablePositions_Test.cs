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
		public void TestEndPoints()
		{
			var map = GetMap(@"
...##
.....
##...
.#...
.....");
			var unit = "{pivot:{x:1,y:0},members:[{x:1,y:0}]}".ParseAsJson<Unit>();
			var reacher = new ReachablePositions(map);
			var expected = @"
				[{pivot: {x: 0, y: 0}, members: [{x: 0, y: 0}]},
				{pivot: {x: 2, y: 0}, members: [{x: 2, y: 0}]},
				{pivot: {x: 0, y: 1}, members: [{x: 0, y: 1}]},
				{pivot: {x: 1, y: 1}, members: [{x: 1, y: 1}]},
				{pivot: {x: 4, y: 1}, members: [{x: 4, y: 1}]},
				{pivot: {x: 2, y: 2}, members: [{x: 2, y: 2}]},
				{pivot: {x: 4, y: 2}, members: [{x: 4, y: 2}]},
				{pivot: {x: 2, y: 3}, members: [{x: 2, y: 3}]},
				{pivot: {x: 4, y: 3}, members: [{x: 4, y: 3}]},
				{pivot: {x: 0, y: 4}, members: [{x: 0, y: 4}]},
				{pivot: {x: 1, y: 4}, members: [{x: 1, y: 4}]},
				{pivot: {x: 2, y: 4}, members: [{x: 2, y: 4}]},
				{pivot: {x: 3, y: 4}, members: [{x: 3, y: 4}]},
				{pivot: {x: 4, y: 4}, members: [{x: 4, y: 4}]}]".ParseAsJson<Unit[]>();
			var actual = reacher.EndPositions(unit).Select(t => t.Item1).OrderBy(u => Tuple.Create(u.pivot.y, u.pivot.x));
			foreach (var u in actual)
			{
				Console.WriteLine(u);
				Console.WriteLine();
			}
			CollectionAssert.AreEqual(expected, actual.ToArray());
		}


		[Test]
		public void TestEndPoints_Complex()
		{
			var map = GetMap(@"
....
....
##..
.#..
....");
			var unit = "{pivot: {x: 0, y: -1}, members: [{x: 2, y: 0}, {x: 0, y: 1}, {x: 1, y: 1}]}".ParseAsJson<Unit>();
			var reacher = new ReachablePositions(map);
			var expected =
				@"[{pivot: {x: 0, y: -1}, members: [{x: 2, y: 0}, {x: 0, y: 1}, {x: 1, y: 1}]},
				{pivot: {x: 1, y: -1}, members: [{x: 3, y: 0}, {x: 1, y: 1}, {x: 2, y: 1}]},
				{pivot: {x: 1, y: -1}, members: [{x: 1, y: 1}, {x: 0, y: 0}, {x: 0, y: 1}]},
				{pivot: {x: 2, y: -1}, members: [{x: 2, y: 1}, {x: 1, y: 0}, {x: 1, y: 1}]},
				{pivot: {x: 3, y: -1}, members: [{x: 3, y: 1}, {x: 2, y: 0}, {x: 2, y: 1}]},
				{pivot: {x: 2, y: 0}, members: [{x: 3, y: 1}, {x: 2, y: 2}, {x: 3, y: 2}]},
				{pivot: {x: 3, y: 0}, members: [{x: 3, y: 2}, {x: 1, y: 1}, {x: 2, y: 2}]},
				{pivot: {x: 3, y: 1}, members: [{x: 3, y: 3}, {x: 2, y: 2}, {x: 2, y: 3}]},
				{pivot: {x: 3, y: 1}, members: [{x: 2, y: 2}, {x: 2, y: 0}, {x: 1, y: 1}]},
				{pivot: {x: 4, y: 1}, members: [{x: 3, y: 2}, {x: 3, y: 0}, {x: 2, y: 1}]},
				{pivot: {x: 4, y: 2}, members: [{x: 2, y: 3}, {x: 2, y: 1}, {x: 2, y: 2}]},
				{pivot: {x: 5, y: 2}, members: [{x: 3, y: 3}, {x: 3, y: 1}, {x: 3, y: 2}]},
				{pivot: {x: 4, y: 3}, members: [{x: 3, y: 4}, {x: 3, y: 2}, {x: 2, y: 3}]}]".ParseAsJson<Unit[]>();
			
			var actual = reacher.EndPositions(unit).Select(t => t.Item1).OrderBy(u => Tuple.Create(u.pivot.y, u.pivot.x));
			foreach (var u in actual)
			{
				Console.WriteLine(u);
				Console.WriteLine();				
			}
			CollectionAssert.AreEqual(expected, actual.ToArray());
		}

		public Map GetMap(string source)
		{
			var rows = source.Split(new []{Environment.NewLine}, StringSplitOptions.None)
				.SkipWhile(String.IsNullOrWhiteSpace).Reverse()
				.SkipWhile(String.IsNullOrWhiteSpace).Reverse().ToList();
			var cells = rows
				.Select((row, index) => row.Trim()
											.Select((c, i) => new Cell {filled = c == '#', x = i, y = index})
											.ToArray())
				.ToList();
			var map = new Map(cells[0].Length, cells.Count);
			for (int x = 0; x < map.Width; x++)
			{
				for (int y = 0; y < map.Height; y++)
				{
					map[x,y] = cells[y][x];
				}
			}
			return map;
		}
	}
}
