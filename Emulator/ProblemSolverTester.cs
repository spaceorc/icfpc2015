using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using SomeSecretProject;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace Emulator
{
    public class ProblemSolverTester
    {
        public Result CountScore(Problem problem, IProblemSolver solver)
        {
            long sum = 0;
            List<Output> outputs = new List<Output>();
            List<Game.State> states = new List<GameBase.State>();
            List<int> scores = new List<int>();
            for (int seedInd = 0; seedInd < problem.sourceSeeds.Length; ++seedInd)
            {
                var seed = problem.sourceSeeds[seedInd];
                var solution = solver.Solve(problem, seed);
                var output = new Output() {seed = seed, solution = solution};
                var game = new Game(problem, output);
                var emulator = new Emulator(game, 0);
                emulator.Run();
                scores.Add(game.CurrentScore);
                states.Add(game.state);
                outputs.Add(output);
            }
            return new Result(){Outputs = outputs.ToArray(), EndStates = states.ToArray()};
        }

        public class Result
        {
            public int TotalScore { get { return Scores.Sum()/Outputs.Length; } }
            public int[] Scores;
            public Output[] Outputs;
            public Game.State[] EndStates;
        }

        public static void SaveResult(Result result, string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            for(int i=0; i<result.Outputs.Length; ++i)
            {
                var output = result.Outputs[i];
                var path = Path.Combine(directory, "" + output.seed);
                File.WriteAllLines(path, new []{"score="+result.Scores[i], "state="+result.EndStates, "seed="+output.seed, output.solution});
            }
        }
    }
}
