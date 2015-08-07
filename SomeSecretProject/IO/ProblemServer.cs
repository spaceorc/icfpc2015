using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SomeSecretProject.IO
{
    public static class ProblemServer
    {
        public static string directory = "../../../data/problems/";

        static ProblemServer()
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public static Problem GetProblem(int problemnum)
        {
            var path = Path.Combine(directory, "problem" + problemnum + ".txt");
            if (File.Exists(path))
                return File.ReadAllText(path).ParseAsJson<Problem>();
            var problem = HttpHelper.GetProblem(problemnum);
            File.WriteAllText(path, problem.ToJson());
            return problem;
        }
    }
}
