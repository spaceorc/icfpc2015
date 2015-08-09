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
		public class UnitResult : IEquatable<UnitResult>
		{
			public Unit Unit { get; set; }
			public HashSet<MoveType> FinalMoves { get; set; }

			public override string ToString()
			{
				return string.Format("Unit: {0}, FinalMoves: [{1}]", Unit, string.Join(", ", FinalMoves.Select(t => "'" + t + "'")));
			}

			public bool Equals(UnitResult other)
			{
				if (ReferenceEquals(null, other))
					return false;
				if (ReferenceEquals(this, other))
					return true;
				if (!Equals(Unit, other.Unit))
					return false;
				return FinalMoves.Count == other.FinalMoves.Count && FinalMoves.All(m => other.FinalMoves.Contains(m));
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;
				if (ReferenceEquals(this, obj))
					return true;
				if (obj.GetType() != this.GetType())
					return false;
				return Equals((UnitResult)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return FinalMoves.OrderBy(m => m).Aggregate(Unit.GetHashCode(), (s, n) => (n.GetHashCode() * 397) & s);
				}
			}
		}

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
			
			var expected = @"[
				{Unit: {pivot: {x: 0, y: 0}, members: [{x: 0, y: 0}]}, FinalMoves: ['W', 'SW']},
				{Unit: {pivot: {x: 2, y: 0}, members: [{x: 2, y: 0}]}, FinalMoves: ['E']},
				{Unit: {pivot: {x: 0, y: 1}, members: [{x: 0, y: 1}]}, FinalMoves: ['W', 'SE', 'SW']},
				{Unit: {pivot: {x: 1, y: 1}, members: [{x: 1, y: 1}]}, FinalMoves: ['SW']},
				{Unit: {pivot: {x: 4, y: 1}, members: [{x: 4, y: 1}]}, FinalMoves: ['E', 'SE']},
				{Unit: {pivot: {x: 2, y: 2}, members: [{x: 2, y: 2}]}, FinalMoves: ['W', 'SW']},
				{Unit: {pivot: {x: 4, y: 2}, members: [{x: 4, y: 2}]}, FinalMoves: ['E']},
				{Unit: {pivot: {x: 2, y: 3}, members: [{x: 2, y: 3}]}, FinalMoves: ['W']},
				{Unit: {pivot: {x: 4, y: 3}, members: [{x: 4, y: 3}]}, FinalMoves: ['E', 'SE']},
				{Unit: {pivot: {x: 0, y: 4}, members: [{x: 0, y: 4}]}, FinalMoves: ['W', 'SE', 'SW']},
				{Unit: {pivot: {x: 1, y: 4}, members: [{x: 1, y: 4}]}, FinalMoves: ['SE', 'SW']},
				{Unit: {pivot: {x: 2, y: 4}, members: [{x: 2, y: 4}]}, FinalMoves: ['SE', 'SW']},
				{Unit: {pivot: {x: 3, y: 4}, members: [{x: 3, y: 4}]}, FinalMoves: ['SE', 'SW']},
				{Unit: {pivot: {x: 4, y: 4}, members: [{x: 4, y: 4}]}, FinalMoves: ['E', 'SE', 'SW']}
			]".ParseAsJson<UnitResult[]>();

			var actual = reacher.EndPositions(unit)
				.GroupBy(x => x.Item1)
				.OrderBy(g => Tuple.Create(g.Key.pivot.y, g.Key.pivot.x))
				.Select(g => new UnitResult
				{
					Unit = g.Key,
					FinalMoves = new HashSet<MoveType>(g.Select(x => x.Item2.Last()))
				});
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
			var expected = @"[
				{Unit: {pivot: {x: 0, y: -1}, members: [{x: 2, y: 0}, {x: 0, y: 1}, {x: 1, y: 1}]}, FinalMoves: ['W', 'SE', 'SW', 'RotateCW', 'RotateCCW']},
				{Unit: {pivot: {x: 1, y: -1}, members: [{x: 3, y: 0}, {x: 1, y: 1}, {x: 2, y: 1}]}, FinalMoves: ['E', 'SW', 'RotateCCW']},
				{Unit: {pivot: {x: 1, y: -1}, members: [{x: 1, y: 1}, {x: 0, y: 0}, {x: 0, y: 1}]}, FinalMoves: ['W', 'SE', 'SW', 'RotateCW']},
				{Unit: {pivot: {x: 2, y: -1}, members: [{x: 2, y: 1}, {x: 1, y: 0}, {x: 1, y: 1}]}, FinalMoves: ['SW', 'RotateCW', 'RotateCCW']},
				{Unit: {pivot: {x: 3, y: -1}, members: [{x: 3, y: 1}, {x: 2, y: 0}, {x: 2, y: 1}]}, FinalMoves: ['E', 'SE', 'RotateCW', 'RotateCCW']},
				{Unit: {pivot: {x: 2, y: 0},  members: [{x: 3, y: 1}, {x: 2, y: 2}, {x: 3, y: 2}]}, FinalMoves: ['E', 'W', 'SE', 'SW', 'RotateCW', 'RotateCCW']},
				{Unit: {pivot: {x: 3, y: 0},  members: [{x: 3, y: 2}, {x: 1, y: 1}, {x: 2, y: 2}]}, FinalMoves: ['E', 'W', 'SW', 'RotateCW', 'RotateCCW']},
				{Unit: {pivot: {x: 3, y: 1},  members: [{x: 3, y: 3}, {x: 2, y: 2}, {x: 2, y: 3}]}, FinalMoves: ['E', 'W', 'SE', 'SW', 'RotateCCW']},
				{Unit: {pivot: {x: 3, y: 1},  members: [{x: 2, y: 2}, {x: 2, y: 0}, {x: 1, y: 1}]}, FinalMoves: ['W', 'SW', 'RotateCW']},
				{Unit: {pivot: {x: 4, y: 1},  members: [{x: 3, y: 2}, {x: 3, y: 0}, {x: 2, y: 1}]}, FinalMoves: ['E', 'RotateCW', 'RotateCCW']},
				{Unit: {pivot: {x: 4, y: 2},  members: [{x: 2, y: 3}, {x: 2, y: 1}, {x: 2, y: 2}]}, FinalMoves: ['W', 'SW', 'RotateCW', 'RotateCCW']},
				{Unit: {pivot: {x: 5, y: 2},  members: [{x: 3, y: 3}, {x: 3, y: 1}, {x: 3, y: 2}]}, FinalMoves: ['E', 'SE', 'RotateCW', 'RotateCCW']},
				{Unit: {pivot: {x: 4, y: 3},  members: [{x: 3, y: 4}, {x: 3, y: 2}, {x: 2, y: 3}]}, FinalMoves: ['E', 'W', 'SE', 'SW', 'RotateCW', 'RotateCCW']}
			]".ParseAsJson<UnitResult[]>();
			var actual = reacher.EndPositions(unit)
				.GroupBy(x => x.Item1)
				.OrderBy(g => Tuple.Create(g.Key.pivot.y, g.Key.pivot.x))
				.Select(g => new UnitResult
				{
					Unit = g.Key,
					FinalMoves = new HashSet<MoveType>(g.Select(x => x.Item2.Last()))
				});
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

		[Test]
		public void TestPaths()
		{
			var map = GetMap(@"
...##
.....
##...
.#...
.....");
			var unit = "{pivot:{x:1,y:0},members:[{x:1,y:0}]}".ParseAsJson<Unit>();
			var reacher = new ReachablePositions(map);

			var expected = @"[
				['E', 'E'],
				['W', 'W'],
				['W', 'SW'],
				['SE', 'SW'],
				['SW', 'W'],
				['SW', 'SE'],
				['SW', 'SW'],
				['SE', 'SE', 'W'],
				['SE', 'SE', 'SW'],
				['SE', 'SE', 'SE', 'W'],
				['E', 'SE', 'E', 'E', 'E'],
				['E', 'SE', 'E', 'E', 'SE'],
				['E', 'SE', 'E', 'SE', 'E'],
				['SE', 'SE', 'SE', 'SE', 'SE'],
				['SE', 'SE', 'SE', 'SE', 'SW'],
				['SE', 'SE', 'SE', 'SW', 'SE'],
				['SE', 'SE', 'SE', 'SW', 'SW'],
				['E', 'SE', 'E', 'SE', 'SE', 'E'],
				['E', 'SE', 'E', 'SE', 'SE', 'SE'],
				['E', 'SE', 'SE', 'SE', 'SE', 'E'],
				['E', 'SE', 'SE', 'SE', 'SE', 'SE'],
				['E', 'SE', 'SE', 'SE', 'SE', 'SW'],
				['SE', 'SE', 'SE', 'SW', 'W', 'SE'],
				['SE', 'SE', 'SE', 'SW', 'W', 'SW'],
				['SE', 'SE', 'SE', 'SW', 'W', 'W', 'W'],
				['SE', 'SE', 'SE', 'SW', 'W', 'W', 'SE'],
				['SE', 'SE', 'SE', 'SW', 'W', 'W', 'SW']
			]".ParseAsJson<MoveType[][]>();

			var forPrint = reacher.EndPositions(unit)
				.Select(g => g.Item1 + " - " +  "['" + string.Join("', '", g.Item2) + "'],");
			var actual = reacher.EndPositions(unit)
				.Select(x => x.Item2).ToArray();
			foreach (var u in forPrint)
			{
				Console.WriteLine(u);
			}
			CollectionAssert.AreEqual(expected, actual.ToArray());
			
		}
	}
}
