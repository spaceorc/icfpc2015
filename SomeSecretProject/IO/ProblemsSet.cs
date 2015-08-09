using System.IO;

namespace SomeSecretProject.IO
{
    public static class ProblemsSet
    {
        public static string directory = "../../../data/problems/";

        static ProblemsSet()
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static Problem GetProblem(int problemN)
        {
            var path = Path.Combine(directory, "problem" + problemN + ".txt");
            return File.ReadAllText(path).ParseAsJson<Problem>();
        }
    }
}