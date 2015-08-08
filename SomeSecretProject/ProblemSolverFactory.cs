using System;

namespace SomeSecretProject
{
    public static class ProblemSolverFactory
    {
        public static IProblemSolver GetSolver()
        {
            throw new Exception();
        }
    }
}