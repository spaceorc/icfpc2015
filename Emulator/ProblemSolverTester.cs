using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Emulator.Posting;
using SomeSecretProject;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace Emulator
{
	public class ProblemSolverTester
	{
		public Result CountScore(Problem problem, IProblemSolver solver, string[] powerPhrases)
		{
			long sum = 0;
			List<Output> outputs = new List<Output>();
			List<GameBase.State> states = new List<GameBase.State>();
			List<int> scores = new List<int>();
			for (int seedInd = 0; seedInd < problem.sourceSeeds.Length; ++seedInd)
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
				SaveOutput("output\\output" + problem.id, output);
			}
			var result = new Result { Outputs = outputs.ToArray(), EndStates = states.ToArray(), Scores = scores.ToArray() };
			//SaveResult(result, "output\\output" + problem.id);
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

		public static void SaveResult(Result result, string directory)
		{
			for (int i = 0; i < result.Outputs.Length; ++i)
			{
				var output = result.Outputs[i];
				SaveOutput(directory, output);
			}
		}

		private static void SaveOutput(string directory, Output output)
		{
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			var path = Path.Combine(directory, "" + output.seed);
			File.WriteAllText(path, output.ToJson());
		}

		public static void SendResult(Result result)
		{
			HttpHelper.SendOutput(DavarAccount.MainTeam, result.Outputs.ToArray());
		}

		public int ScoreOverAllProblems(IProblemSolver solver, string[] powerPhrases)
		{
			long sum = 0;
			for (int i = 0; i < 24; ++i)
			{
				var problem = ProblemServer.GetProblem(i);
				Console.WriteLine("Problem: {0}, w:{1}, h:{2}, seeds:{3}", i, problem.width, problem.height, problem.sourceSeeds.Length);
				var results = CountScore(problem, solver, powerPhrases);
				sum += results.TotalScore;
				for (int k = 0; k < results.EndStates.Length; ++k)
				{
					if (results.EndStates[k] != GameBase.State.End)
						Console.WriteLine("Problem: {0}  seed: {1}  End state: {2} Score: {3}", results.Outputs[k].problemId, results.Outputs[k].seed, results.EndStates[k].ToString(), results.Scores[k]);
				}
				Console.WriteLine("Problem score: {0}", results.TotalScore);
			}
			Console.WriteLine("Total score: {0}", sum);
			return (int)(sum / 24);
		}
	}
}