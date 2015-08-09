using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Threading;
using Emulator.Drawing;
using NDesk.Options;
using SomeSecretProject.Algorithm;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace Emulator
{
	public class Program
	{
		private static void Main(string[] args)
		{
			string action = "help";
			string solution = "";
		    string[] powerPhrases = PowerDatas.GetPowerPhrases();
			int seed = 0;
			int problem = 0;
			int delay = 0;
			bool visualize = true;
			var options = new OptionSet
			{
				{ "show|play|solve|solveall|debug|help", v => action = v },
				{ "solution=", v => solution = v },
				{ "delay=", (int v) => delay = v },
				{ "power=", v => powerPhrases = v.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries) },
				{ "powerfile=", v => powerPhrases = PowerDatas.GetPowerPhrases(v) },
				{ "seed=", (int v) => seed = v },
				{ "problem=", (int v) => problem = v },
				{ "fast", v => visualize = false },
			};
			try
			{
				options.Parse(args);
			}
			catch (OptionException e)
			{
				Console.Error.WriteLine("Option '{0}' value is invalid: {1}", e.OptionName, e.Message);
				Console.Error.WriteLine();
			}
			switch (action)
			{
				case "help":
					Console.WriteLine("USAGE:");
					options.WriteOptionDescriptions(Console.Out);
					break;
				case "show":
					ShowProblems();
					break;
				case "play":
					if (string.IsNullOrEmpty(solution))
						PlayManual(problem, seed, powerPhrases, delay);
					else
						PlayAuto(problem, solution, seed, powerPhrases, delay);
					break;
				case "solve":
					Solve(problem, seed, powerPhrases, delay, visualize);
					break;
				case "solveall":
					SolveAll(powerPhrases);
					break;
				case "debug":
					DebugSolve(problem, seed, powerPhrases, delay);
					break;
                
			}
		}

		public static void ShowProblems()
		{
			var console = new FastConsole();
			var p = 0;
			while(true)
			{
				var problem = ProblemsSet.GetProblem(p);
				var game = new Game(problem, new Output() { solution = "" }, new string[0]);
				var map = game.map;
				using (var drawer = new Drawer(console))
				{
					drawer.console.WriteLine(string.Format("problem {0}", p));
					drawer.DrawMap(map, null);
					foreach (var unit in game.problem.units)
					{
						drawer.DrawUnit(unit);
					}
				}
				Console.SetWindowPosition(0, 0);
				var key = Console.ReadKey(false);
				if (key.Key == ConsoleKey.LeftArrow)
					--p;
				else
					++p;
				if (p < 0)
					p = 0;
				if (p > 24)
					p = 24;
			}
		}


		public static void PlayManual(int problemnum, int seed, string[] powerPhrases, int delay)
		{
            var problem = ProblemsSet.GetProblem(problemnum);
			var game = new ConsoleGame(problem, problem.sourceSeeds[seed], powerPhrases);
			var emulator = new Emulator(game, delay);
			emulator.Run();
		}

		public static void PlayAuto(int problemnum, string solution, int seed, string[] powerPhrases, int delay)
		{
            var problem = ProblemsSet.GetProblem(problemnum);
			var game = new Game(problem, new Output() { solution = solution, seed = problem.sourceSeeds[seed] }, powerPhrases);
			var emulator = new Emulator(game, delay);
			emulator.Run();
		}

		public static void SolveAll(string[] powerPhrases)
		{
			var tester = new ProblemSolverTester();
			//var problemSolver = new MuggleProblemSolver();
			tester.ScoreOverAllProblems(() => new MuggleProblemSolver_MultiUnit(1), powerPhrases);
		}

		public static void Solve(int problemnum, int seed, string[] magicSpells, int delay, bool visualize)
		{
			var problem = ProblemsSet.GetProblem(problemnum);
//			var muggleProblemSolver = new MuggleProblemSolver();
			//var muggleProblemSolver = new MagicProblemSolver();
			var solver = new MuggleProblemSolver_MultiUnit(1);

			var stopwatch = Stopwatch.StartNew();
			var solution = solver.Solve(problem, problem.sourceSeeds[seed], magicSpells);
			stopwatch.Stop();

			var game = new Game(problem, new Output { seed = problem.sourceSeeds[seed], solution = solution }, magicSpells);
			if (visualize)
			{
				var emulator = new Emulator(game, delay);
				emulator.Run();
			}
			else
			{
				while (game.state == GameBase.State.UnitInGame || game.state == GameBase.State.WaitUnit)
				{
					game.Step();
				}
				Console.WriteLine("Score=" + game.CurrentScore);
				Console.WriteLine("Time=" + stopwatch.Elapsed);
				Console.ReadKey();
			}
		}

		public static void DebugSolve(int problemnum, int seed, string[] magicSpells, int delay)
		{
            var problem = ProblemsSet.GetProblem(problemnum);
			//var problemSolver = new MagicProblemSolver();
			var problemSolver = new MuggleProblemSolver_MultiUnit(1);
			var fastConsole = new FastConsole();
			problemSolver.SolutionAdded += (g, s) =>
			{
			    using (var drawer = new Drawer(fastConsole))
			    {
			        drawer.DrawMap(g.map, g.currentUnit);
                    drawer.console.WriteLine("Score: " + g.CurrentScore);
			    }
			    Console.ReadKey(true);
				var unit = g.currentUnit;
				foreach (var c in s.Where(x => !MoveTypeExt.IsIgnored(x)))
				{
					var moveType = MoveTypeExt.Convert(c);
					if (!moveType.HasValue)
						throw new InvalidOperationException(string.Format("Invalid char in solution: {0}. Char: '{1}'", s, c));
					var newUnit = unit.Move(moveType.Value);
					using (var drawer = new Drawer(fastConsole))
					{
						if (newUnit.IsCorrect(g.map))
							drawer.DrawMap(g.map, newUnit);
						else
							drawer.DrawMap(g.map, unit, locked: true);
                        drawer.console.WriteLine("Score: " + g.CurrentScore);
						unit = newUnit;
					}
					Thread.Sleep(delay < 0 ? 0 : delay);
				}
				Console.ReadKey(true);
			};
			problemSolver.Solve(problem, problem.sourceSeeds[seed], magicSpells);
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            { }
		}
	}
}