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

            var httpHelper = new HttpHelper(DavarAccounts.MAIN_TEAM_TOKEN, DavarAccounts.MAIN_TEAM_ID);

            var problem = httpHelper.GetProblem(problemnum);
            File.WriteAllText(path, problem.ToJson());
            return problem;
        }
    }

    public static class DavarAccounts
    {
        public const string MAIN_TEAM_TOKEN = "LZPdLA9OaBh2PJO7Kogh6X+/sDzqX4nPdN+cNlAO39w=";
        public const int MAIN_TEAM_ID = 127;

        public const string TEST_TEAM_TOKEN = "MAXOFfmF0wuQ4QYjz0mpShQckV6x/64vQuvT8aSI4Xg=";
        public const int TEST_TEAM_ID = 188;
    }
}
