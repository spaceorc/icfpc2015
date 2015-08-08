using System;
using System.Linq;
using SomeSecretProject;
using SomeSecretProject.IO;

namespace Poster.Posting
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
            string[] powerPhrases = PowerDatas.GetPowerPhrases();
            var outputs = Enumerable.Range(0, 24)
                .Select(ProblemsSet.GetProblem)
                .SelectMany(problem => problem.sourceSeeds
                    .Select(seed => new Output
                    {
                        problemId = problem.id,
                        seed = seed,
                        solution = problemSolver.Solve(problem, seed, powerPhrases),
                        tag = tag
                    }));

            foreach (var output in outputs)
            {
                HttpHelper.SendOutput(account, output);
            }
        }
    }
}