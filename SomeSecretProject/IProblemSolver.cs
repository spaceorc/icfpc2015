using SomeSecretProject.IO;

namespace SomeSecretProject
{
    public interface IProblemSolver
    {
        string Solve(Problem problem, int seed, string[] magicSpells);
    }
}