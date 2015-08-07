
using System.Net;


namespace SomeSecretProject.IO
{
    public static class HttpHelper
    {
        public static string API_TOKEN;
        public static int TEAM_ID;

        private static WebClient client = new WebClient()
        {
            Headers = new WebHeaderCollection() {"Content-Type", "application/json"}
        };

        public static Problem GetProblem(int problemnum)
        {
            var url = @"http://icfpcontest.org/problems/problem_" + problemnum + ".json";
            var data = client.DownloadString(url);
            return data.ParseAsJson<Problem>();
        }

        public static void SendOutput(Output output)
        {
            var url = @"https://davar.icfpcontest.org/teams/" + TEAM_ID + @"/solutions";
            var data = output.ToJson();
            var result = client.UploadString(url, "post", data);
        }
    }
}
