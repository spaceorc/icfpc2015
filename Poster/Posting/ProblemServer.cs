using System.IO;
using SomeSecretProject.IO;

namespace Emulator.Posting
{
    public static class ProblemServer
    {
        public static string directory = "../../../data/problems/";

        static ProblemServer()
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static Problem GetProblem(int problemnum)
        {
            var path = Path.Combine(directory, "problem" + problemnum + ".txt");
            if (File.Exists(path))
            {
                return File.ReadAllText(path).ParseAsJson<Problem>();
            }

            var problem = HttpHelper.GetProblem(problemnum);
            File.WriteAllText(path, problem.ToJson());
            return problem;
        }
    }
}
