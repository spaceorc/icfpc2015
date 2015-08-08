using System;
using System.Net;
using System.Text;
using SomeSecretProject.IO;

namespace Emulator.Posting
{
    public class HttpHelper
    {
        private readonly string apiToken;
        private readonly int teamId;

        private static readonly WebClient client = new WebClient();

        public HttpHelper(string apiToken, int teamId)
        {
            this.apiToken = apiToken;
            this.teamId = teamId;
        }

        public Problem GetProblem(int problemnum)
        {
            var url = string.Format("http://icfpcontest.org/problems/problem_{0}.json", problemnum);
            var data = client.DownloadString(url);
            return data.ParseAsJson<Problem>();
        }

        public bool SendOutput(Output output)
        {
            var authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(":" + apiToken));
            client.Headers.Add("Authorization", "Basic " + authInfo);

            client.Headers.Add("Content-Type", "application/json");

            var url = string.Format("https://davar.icfpcontest.org/teams/{0}/solutions", teamId);
            var data = "[" + output.ToJson() + "]";
            var result = client.UploadString(url, "POST", data);

            return result == "created";
        }
    }
}