namespace SomeSecretProject
{
    public static class ProblemSolverFactory
    {
        public static IProblemSolver GetSolver()
        {
            return new SimpleProblemSolver();
        }
    }
}