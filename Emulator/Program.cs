using System;
using Emulator.Drawing;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace Emulator
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var console = new FastConsole();
		    for (int p = 0; p < 24; )
		    {
		        var problem = ProblemServer.GetProblem(p);
		        var game = new Game(problem, new Output() {solution = ""});
		        var map = game.map;
		        using (var drawer = new Drawer(console))
		        {
                    drawer.console.WriteLine(string.Format("problem {0}", p));
		            drawer.DrawMap(map, null);
		        }
                var key = Console.ReadKey();
		        if (key.Key == ConsoleKey.LeftArrow) --p;
		        else ++p;
		    }
		    //Console.SetCursorPosition(0, map.Height * 3 + 1);
		}
	}
}