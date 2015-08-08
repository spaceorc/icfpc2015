using System;
using System.IO;
using System.Linq;
using SomeSecretProject;
using SomeSecretProject.IO;

namespace Emulator.Posting
{
    public class HttpPoster
    {
        private readonly IProblemSolver problemSolver;

        public HttpPoster(IProblemSolver problemSolver)
        {
            this.problemSolver = problemSolver;
        }

        public void PostAll(DavarAccount account, string tag = null)
        {
			string[] powerPhrases = File.ReadAllLines(Path.Combine(@"..\..\..\data\power", "default.txt"));
            var outputs = Enumerable.Range(0, 24)
                .Select(ProblemServer.GetProblem)
                .SelectMany(problem => problem.sourceSeeds
                    .Select(seed =>
                    {
						var ans = problemSolver.Solve(problem, seed, powerPhrases);
                        Console.WriteLine("{0} {1} {2}", problem.id, seed, ans);
                        return new Output
                        {
                            problemId = problem.id,
                            seed = seed,
                            solution = ans,
                            tag = tag
                        };
                    }));

            foreach (var output in outputs)
            {
                HttpHelper.SendOutput(account, output);
            }
        }
    }
}