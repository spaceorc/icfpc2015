using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SomeSecretProject;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace Emulator
{
	public class ProblemSolverTester
	{
		public Result CountScore(Problem problem, IProblemSolver solver, string[] powerPhrases, string folder)
		{
			long sum = 0;
			List<Output> outputs = new List<Output>();
			List<GameBase.State> states = new List<GameBase.State>();
			List<int> scores = new List<int>();
		    for (var seedInd = 0; seedInd < problem.sourceSeeds.Length; ++seedInd)
			{
				var seed = problem.sourceSeeds[seedInd];
				var solution = solver.Solve(problem, seed, powerPhrases);
				var output = new Output() { problemId = problem.id, seed = seed, solution = solution };
				var game = new Game(problem, output, powerPhrases);
				while (game.state == GameBase.State.UnitInGame || game.state == GameBase.State.WaitUnit)
				{
					game.Step();
				}
				scores.Add(game.CurrentScore);
				states.Add(game.state);
				outputs.Add(output);
				Console.WriteLine("seed: {0}, score: {1}", seedInd, game.CurrentScore);
			    SaveOutput(folder + problem.id, output);
			}
			var result = new Result { Outputs = outputs.ToArray(), EndStates = states.ToArray(), Scores = scores.ToArray() };
			SaveResult(folder + problem.id, result);
			return result;
		}

		public class Result
		{
			public int TotalScore
			{
				get { return Scores.Sum() / Outputs.Length; }
			}

			public int[] Scores;
			public Output[] Outputs;
			public GameBase.State[] EndStates;
		}

	    private static void SaveResult(string directory, Result result)
		{
		    var score = result.TotalScore;
            File.WriteAllText(Path.Combine(directory,"score"+score) + ".txt", result.Scores.Select((s,i) => string.Format("{0} {1}\r\n", i, s)).Aggregate((s,a) => s + a));
		}

		private static void SaveOutput(string directory, Output output)
		{
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			var path = Path.Combine(directory, "" + output.seed);
			File.WriteAllText(path, output.ToJson());
		}

		public int ScoreOverAllProblems(IProblemSolver solver, string[] powerPhrases)
		{
		    var folder = @"..\..\..\solves\" + DateTime.Now.ToString("O").Replace(":", "_") + @"\";

			long sum = 0;
		    var agg = new StringBuilder();
		    for (int i = 0; i < 25; ++i)
			{
				var problem = ProblemsSet.GetProblem(i);
				WriteMessage(agg, "Problem: {0}, w:{1}, h:{2}, seeds:{3}", i, problem.width, problem.height, problem.sourceSeeds.Length);
				var results = CountScore(problem, solver, powerPhrases, folder);
				sum += results.TotalScore;
				for (int k = 0; k < results.EndStates.Length; ++k)
				{
					if (results.EndStates[k] != GameBase.State.End)
						WriteMessage(agg, "Problem: {0}  seed: {1}  End state: {2} Score: {3}", results.Outputs[k].problemId, results.Outputs[k].seed, results.EndStates[k].ToString(), results.Scores[k]);
				}
				WriteMessage(agg, "Problem score: {0}", results.TotalScore);
			}
			WriteMessage(agg, "Total score: {0}", sum);
            File.WriteAllText(folder + @"\" + "totalog.txt", agg.ToString());
			return (int)(sum / 25);
		}

	    private void WriteMessage(StringBuilder aggregator, string f, params object[] args)
	    {
	        var line = string.Format(f, args);
	        Console.WriteLine(line);
	        aggregator.AppendLine(line);
	    }
	}
}