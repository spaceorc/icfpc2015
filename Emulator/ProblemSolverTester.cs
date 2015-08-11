using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SomeSecretProject;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace Emulator
{
	public class ProblemSolverTester
	{
		public Result CountScore(Problem problem, Func<IProblemSolver> solverFactory, string[] powerPhrases, string folder)
		{
			long sum = 0;
            var lockerForLists = new object();
			List<Output> outputs = new List<Output>();
			List<GameBase.State> states = new List<GameBase.State>();
			List<int> scores = new List<int>();

            Parallel.For(0, problem.sourceSeeds.Length, new ParallelOptions() { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount) }, seedInd =>
            {
                var solver = solverFactory();
                var seed = problem.sourceSeeds[seedInd];
                try
                {
                    Console.WriteLine("Problem {0}: seed {1} started counting", problem.id, seed);
                    var stopwatch = Stopwatch.StartNew();
                    var solution = solver.Solve(problem, seed, powerPhrases);
                    stopwatch.Stop();
                    var output = new Output() { problemId = problem.id, seed = seed, solution = solution };
                    var game = new Game(problem, output, powerPhrases);
                    while (game.state == GameBase.State.UnitInGame || game.state == GameBase.State.WaitUnit)
                    {
                        game.Step();
                    }
                    lock (lockerForLists)
                    {
                        scores.Add(game.CurrentScore);
                        states.Add(game.state);
                        outputs.Add(output);
                    }
                    Console.WriteLine("Problem {0}: seed: {1}, score: {2}, time: {3}", problem.id, seed, game.CurrentScore,
                        stopwatch.Elapsed);
                    SaveOutput(folder + problem.id, output);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Problem {0}: seed {1} crashed {2}", problem.id, seed, ex.Message);
                    throw;
                }
            });
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
            File.WriteAllText(Path.Combine(directory,"score"+score) + ".txt", result.Scores.Select((s,i) => string.Format("{0} {1}\r\n", result.Outputs[i].seed, s)).Aggregate((s,a) => s + a));
		}

		private static void SaveOutput(string directory, Output output)
		{
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			var path = Path.Combine(directory, "" + output.seed);
			File.WriteAllText(path, output.ToJson());
		}

		public void ScoreOverAllProblems(Func<Problem, IProblemSolver> solver, string[] powerPhrases)
		{
		    var resultingFolder = @"..\..\..\solves\" + DateTime.Now.ToString("O").Replace(":", "_") + @"\";

			long sum = 0;
		    var agg = new StringBuilder();
		    for (int i = 0; i < 24; ++i)
			{
				var problem = ProblemsSet.GetProblem(i);
				WriteMessage(agg, "Problem {0}: w:{1}, h:{2}, seeds:{3}", i, problem.width, problem.height, problem.sourceSeeds.Length);
				var results = CountScore(problem, () => solver(problem), powerPhrases, resultingFolder);
				sum += results.TotalScore;
				for (int k = 0; k < results.EndStates.Length; ++k)
				{
					if (results.EndStates[k] != GameBase.State.End)
						WriteMessage(agg, "Problem {0}: {1}  seed: {2}  End state: {3} Score: {4}", i, results.Outputs[k].problemId, results.Outputs[k].seed, results.EndStates[k].ToString(), results.Scores[k]);
				}
				WriteMessage(agg, "Problem {0}: score {1}", i, results.TotalScore);
			}
			WriteMessage(agg, "Total score: {0}", sum);
            File.WriteAllText(resultingFolder + @"\" + "totalog.txt", agg.ToString());
		}

	    private void WriteMessage(StringBuilder aggregator, string f, params object[] args)
	    {
	        var line = string.Format(f, args);
	        Console.WriteLine(line);
	        aggregator.AppendLine(line);
	    }
	}
}