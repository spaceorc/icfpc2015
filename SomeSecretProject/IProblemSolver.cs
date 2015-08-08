using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeSecretProject.IO;

namespace SomeSecretProject
{
    public interface IProblemSolver
    {
        string Solve(Problem problem, int seed);
    }

    public static class ProblemSolverFactory
    {
        public static IProblemSolver GetSolver()
        {
            throw new Exception();
        }
    }
}
