using System.Linq;
using SomeSecretProject;
using SomeSecretProject.IO;

namespace Emulator.Posting
{
    public class HttpPoster
    {
        private readonly IProblemSolver problemSolver;
        private readonly HttpHelper httpHelper;

        public HttpPoster(string apiToken, int teamId)
        {
            problemSolver = ProblemSolverFactory.GetSolver();
            httpHelper = new HttpHelper(apiToken, teamId);
        }

        public void PostAll(string tag = null)
        {
            var outputs = Enumerable.Range(0, 24)
                .Select(ProblemServer.GetProblem)
                .SelectMany(problem => problem.sourceSeeds
                    .Select(seed => new Output
                    {
                        problemId = problem.id,
                        seed = seed,
                        solution = problemSolver.Solve(problem, seed),
                        tag = tag
                    }));

            foreach (var output in outputs)
            {
                httpHelper.SendOutput(output);
            }
        }
    }
}