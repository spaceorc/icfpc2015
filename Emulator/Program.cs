using System;
using Emulator.Drawing;
using Emulator.Posting;
using SomeSecretProject.Algorithm;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace Emulator
{
	public class Program
	{
		private static void Main(string[] args)
		{
			if (args[0]=="--show") ShowProblems();
            if (args[0]=="--play") 
                if (args.Length > 2) PlayAuto(int.Parse(args[1]), args[2]);
                else PlayManual(int.Parse(args[1]));
			if (args[0] == "--solve")
				Solve(int.Parse(args[1]), args.Length > 2 ? int.Parse(args[2]) : 0, new string[0]);
			//Console.SetCursorPosition(0, map.Height * 3 + 1);
		}

	    public static void ShowProblems()
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
			        foreach (var unit in game.units)
			        {
				        drawer.DrawUnit(unit);
			        }
		        }
				Console.SetWindowPosition(0, 0);
                var key = Console.ReadKey();
		        if (key.Key == ConsoleKey.LeftArrow) --p;
		        else ++p;
		    }
	    }


	    public static void PlayManual(int problemnum, int seed = 0)
	    {
            var problem = ProblemServer.GetProblem(problemnum);
            var game = new ConsoleGame(problem, problem.sourceSeeds[0]);
            var emulator = new Emulator(game, 0);
            emulator.Run();
	    }

	    public static void PlayAuto(int problemnum, string solution, int seed = 0)
	    {
	        var problem = ProblemServer.GetProblem(problemnum);
            var game = new Game(problem, new Output(){solution = solution});
            var emulator = new Emulator(game, 1000);
            emulator.Run();
	    }

		public static void Solve(int problemnum, int seed, string[] magicSpells)
	    {
			var problem = ProblemServer.GetProblem(problemnum);
		    var muggleProblemSolver = new MuggleProblemSolver();
			var solution = muggleProblemSolver.Solve(problem, seed, magicSpells);
			var game = new Game(problem, new Output { seed = seed, solution = solution });
			var emulator = new Emulator(game, -1);
			emulator.Run();
	    }
	}
}