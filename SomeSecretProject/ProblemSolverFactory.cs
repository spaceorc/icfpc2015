using SomeSecretProject.Algorithm;

namespace SomeSecretProject
{
    public static class ProblemSolverFactory
    {
        public static IProblemSolver GetSolver(bool big)
        {
            return new MuggleProblemSolver_MultiUnit(big ? 0 : 1);
        }
    }
}