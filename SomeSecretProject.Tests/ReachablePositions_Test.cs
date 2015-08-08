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
			var expected = new[]
			               {GetUnit(@"
@..##
.....
##...
.#...
....."),
							GetUnit(@"
..@##
.....
##...
.#...
....."),
							GetUnit(@"
...##
@....
##...
.#...
....."),
							GetUnit(@"
...##
.@...
##...
.#...
....."),
							GetUnit(@"
...##
....@
##...
.#...
....."),
							GetUnit(@"
...##
.....
##@..
.#...
....."),
							GetUnit(@"
...##
.....
##..@
.#...
....."),
							GetUnit(@"
...##
.....
##...
.#@..
....."),
							GetUnit(@"
...##
.....
##...
.#..@
....."),
							GetUnit(@"
...##
.....
##...
.#...
@...."),
							GetUnit(@"
...##
.....
##...
.#...
.@..."),
							GetUnit(@"
...##
.....
##...
.#...
..@.."),
							GetUnit(@"
...##
.....
##...
.#...
...@."),
							GetUnit(@"
...##
.....
##...
.#...
....@"),
			               };
			var actual = reacher.EndPositions(unit).Select(t => t.Item1).OrderBy(u => Tuple.Create(u.pivot.y, u.pivot.x));
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
			var unit = GetUnit(@"
+
..*
**
");
			var reacher = new ReachablePositions(map);
			var expected = new[]
			               {GetUnit(@"
+
..*.
**..
##..
.#..
...."),
	 GetUnit(@"
 +
...*
.**.
##..
.#..
...."),
	 GetUnit(@"
 +
*...
**..
##..
.#..
...."),
	 GetUnit(@"
  +
.*..
.**.
##..
.#..
...."),
	 GetUnit(@"
   +
..*.
..**
##..
.#..
...."),
			               };
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

		public Unit GetUnit(string source)
		{
			var rows = source.Split(new []{Environment.NewLine}, StringSplitOptions.None)
				.SkipWhile(String.IsNullOrWhiteSpace).Reverse()
				.SkipWhile(String.IsNullOrWhiteSpace).Reverse().ToList();
			Cell pivot = null;
			var cells = new List<Cell>();
			var width = rows.Where(x => x.Trim().Length > 1).Min(x => x.Trim().Length);
			if (rows[0].Trim().Length == 1)
			{
				pivot = new Cell {x = rows[0].IndexOf('+') - rows.First(x => x.Trim().Length > 1).TakeWhile(c => c == ' ').Count(), 
					y = -rows.TakeWhile(r => r.Trim().Length <= 1).Count()};
				rows = rows.SkipWhile(r => r.Trim().Length <= 1).ToList();
			}
			if (rows.Last().Trim().Length == 1)
			{
				pivot = new Cell {x = rows[0].IndexOf('+') - rows.First(x => x.Trim().Length > 1).TakeWhile(c => c == ' ').Count(),
					y = rows.Count - 1};
				rows = rows.TakeWhile(r => r.Trim().Length > 1).ToList();
			}
			for (int y = 0; y < rows.Count; y++)
			{
				var row = rows[y].Trim();
				if (row.Length > width)
				{
					if (row[0] == '+')
					{
						pivot = new Cell
						{
							x = -row.TakeWhile(c => c == '+' || c == ' ').Count(),
							y = y
						};
						row = row.Substring(1).Trim();
					}
					if (row.Last() == '+')
					{
						pivot = new Cell
						{
							x = row.Length - 1,
							y = y
						};
						row = row.Substring(0, width);
					}
				}
				for (int x = 0; x < row.Length; x++)
				{
					if (row[x] == '*')
					{
						cells.Add(new Cell { x = x, y = y });
					}
					else if (row[x] == '@')
					{
						pivot = new Cell { x = x, y = y };
						cells.Add(pivot);
					}
					else if (row[x] == '+')
					{
						pivot = new Cell { x = x, y = y };
					}
				}

			}
			return new Unit{pivot = pivot, members = new HashSet<Cell>(cells.ToArray())};
		}
	}
}
