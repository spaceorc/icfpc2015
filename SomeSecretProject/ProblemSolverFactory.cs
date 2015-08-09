using SomeSecretProject.Algorithm;

namespace SomeSecretProject
{
    public static class ProblemSolverFactory
    {
        public static IProblemSolver GetSolver()
        {
            return new MuggleProblemSolver_MultiUnit(1);
        }
    }
}