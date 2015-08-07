using System;
using Emulator.Drawing;
using SomeSecretProject.Logic;

namespace Emulator
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var console = new FastConsole();
			var map = new Map(5, 5);
			for (int i = 0; i < map.Width; i++)
			{
				for (int j = 0; j < map.Height; j++)
					map.cells[i, j] = new Cell { x = i, y = j, filled = j >= 3 };
			}
			var unit = new Unit
			{
				cells = new[] { new Cell { x = 1, y = 1 }, new Cell { x = 3, y = 2 } },
				pivot = new Cell { x = 1, y = 1 }
			};
			using (var drawer = new Drawer(console))
			{
				drawer.DrawMap(map, unit);
			}
			Console.SetCursorPosition(0, map.Height * 3 + 1);
		}
	}
}