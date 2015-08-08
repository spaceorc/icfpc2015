using SomeSecretProject.IO;

namespace SomeSecretProject
{
    public class SimpleProblemSolver : IProblemSolver
    {
        public string Solve(Problem problem, int seed, string[] magicSpells)
        {
            return magicSpells[0];
        }
    }
}