using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SomeSecretProject.Logic;

namespace Emulator.Drawing
{
	public class Drawer : IDisposable
	{
		private static readonly ConsoleColor[] availableColors =
		{
			ConsoleColor.Gray,
			ConsoleColor.DarkGray,
			ConsoleColor.DarkYellow,
			ConsoleColor.DarkCyan
		};

		private readonly FastConsole.FastConsoleWriter console;

		public Drawer([NotNull] FastConsole console)
		{
			this.console = console.BeginWrite();
		}

		private static readonly Dictionary<Tuple<int, int>, ConsoleColor> cellColors = new Dictionary<Tuple<int, int>, ConsoleColor>();

		private static ConsoleColor GetColor(int row, int col)
		{
			ConsoleColor result;
			var key = Tuple.Create(row, col);
			if (cellColors.TryGetValue(key, out result))
				return result;
			return cellColors[key] = availableColors[row%2 * 2 + col%2];
		}

		public void Dispose()
		{
			console.EndWrite();
		}

		private class CellViewInfo
		{
			public ConsoleColor ForegroundColor;
			public ConsoleColor BackgroundColor;
			public char Char;
		}

		private CellViewInfo GetMapViewInfo(Cell cell)
		{
			if (cell.filled)
				return new CellViewInfo
				{
					Char = ' ',
					ForegroundColor = ConsoleColor.Black,
					BackgroundColor = GetColor(cell.y, cell.x)
				};
			return new CellViewInfo
			{
				Char = '·',
				ForegroundColor = ConsoleColor.DarkGray,
				BackgroundColor = ConsoleColor.Black
			};
		}

		public void DrawMap(Map map, Unit unit)
		{
			for (int i = 0; i < 3 * map.Height + 1; i++)
			{
				var row = i / 3;
				if (i % 3 == 0)
				{
					console.BackgroundColor = ConsoleColor.Black;
					console.Write(" ");
					for (int col = 0; col < map.Width; ++col)
					{
						if (row % 2 == 1)
						{
							var viewInfo = GetViewInfo(map, unit, col, row - 1);
							console.ForegroundColor = viewInfo.ForegroundColor;
							console.BackgroundColor = viewInfo.BackgroundColor;
							console.Write(new string(viewInfo.Char, 2));
						}
						if (row < map.Height)
						{
							var viewInfo = GetViewInfo(map, unit, col, row);
							console.ForegroundColor = viewInfo.ForegroundColor;
							console.BackgroundColor = viewInfo.BackgroundColor;
							console.Write(new string(viewInfo.Char, 2));
						}
						else
						{
							console.BackgroundColor = ConsoleColor.Black;
							console.Write("  ");
						}
						if (row == 0)
						{
							console.BackgroundColor = ConsoleColor.Black;
							console.Write("  ");
						}
						else if (row % 2 == 0)
						{
							var viewInfo = GetViewInfo(map, unit, col, row - 1);
							console.ForegroundColor = viewInfo.ForegroundColor;
							console.BackgroundColor = viewInfo.BackgroundColor;
							console.Write(new string(viewInfo.Char, 2));
						}
					}
				}
				else
				{
					if (row % 2 == 1)
					{
						console.BackgroundColor = ConsoleColor.Black;
						console.Write("  ");
					}
					for (int col = 0; col < map.Width; ++col)
					{
						var viewInfo = GetViewInfo(map, unit, col, row);
						console.ForegroundColor = viewInfo.ForegroundColor;
						console.BackgroundColor = viewInfo.BackgroundColor;
						console.Write(new string(viewInfo.Char, 4));
					}
				}
				console.WriteLine();
			}
		}

		private CellViewInfo GetViewInfo(Map map, Unit unit, int col, int row)
		{
			var result = GetMapViewInfo(map.cells[col, row]);
			if (unit != null)
			{
				var unitCell = unit.cells.SingleOrDefault(x => x.x == col && x.y == row);
				if (unitCell != null)
				{
					result = new CellViewInfo
					{
						BackgroundColor = ConsoleColor.Red,
						Char = ' '
					};
				}
				var isPivot = unit.pivot.x == col && unit.pivot.y == row;
				if (isPivot)
					result.Char = 'X';
			}
			return result;
		}
	}
}