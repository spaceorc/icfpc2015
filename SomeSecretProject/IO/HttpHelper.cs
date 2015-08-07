
using System;
using System.IO;
using System.Net;
using System.Text;


namespace SomeSecretProject.IO
{
    public static class HttpHelper
    {
        public static string API_TOKEN = "LZPdLA9OaBh2PJO7Kogh6X+/sDzqX4nPdN+cNlAO39w=";
        public static int TEAM_ID = 127;

       
        private static WebClient getclient = new WebClient();
        private static WebClient postclient = new WebClient();
        static HttpHelper()
        {
            string authInfo = ":"+API_TOKEN;
            //postclient.Credentials = new NetworkCredential("", API_TOKEN);
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            postclient.Headers.Add("Authorization", "Basic " + authInfo);
            postclient.Headers.Add("Content-Type", "application/json");
            
        }

        public static Problem GetProblem(int problemnum)
        {
            var url = @"http://icfpcontest.org/problems/problem_" + problemnum + ".json";
            var data = getclient.DownloadString(url);
            return data.ParseAsJson<Problem>();
        }

        public static bool SendOutput2(Output output)
        {
            var url = @"https://davar.icfpcontest.org/teams/" + TEAM_ID + @"/solutions";
            var data = "[" + output.ToJson() + "]";
            var result = postclient.UploadString(url, "POST", data);
            return result == "created";
        }
    }
}
