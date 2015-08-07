using System;
using Emulator.ConsoleUtils;
using SomeSecretProject.Logic;

namespace Emulator
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var console = new FastConsole();
			var map = new Map(5, 5);
			for (int i = 0; i < map.Cells.GetLength(0); i++)
			{
				for (int j = 0; j < map.Cells.GetLength(1); j++)
					map.Cells[i, j] = new Cell { X = i, Y = j, Locked = j >= 3 };
			}
			var unit = new Unit
			{
				Cells = new[] { new Cell { X = 1, Y = 1 }, new Cell { X = 3, Y = 2 } },
				Pivot = new Cell { X = 1, Y = 1 }
			};
			using (var drawer = new Drawer(console))
			{
				drawer.DrawMap(map, unit);
			}
			Console.SetCursorPosition(0, map.Cells.GetLength(1) * 3 + 1);
		}
	}
}