using System;
using System.IO;
using System.Linq;
using System.Threading;
using SomeSecretProject;
using SomeSecretProject.IO;

namespace Poster.Posting
{
    public class HttpPoster
    {
        public void PostAll(DavarAccount account, string solveName, string tag = null)
        {
            var directoryMain = @"..\..\..\importantSolves\" + solveName;
            var problems = Directory.EnumerateDirectories(directoryMain);
            foreach (var p in problems.Select(Path.GetFileName))
            {
                foreach (var f in Directory.EnumerateFiles(Path.Combine(directoryMain, p)).Select(Path.GetFileName))
                {
                    if (f.StartsWith("score"))
                        continue;
                    var output = File.ReadAllText(Path.Combine(directoryMain, p, f)).ParseAsJson<Output>();
                    output.tag = tag ?? solveName + " " + DateTime.Now.ToString("O");
                    HttpHelper.SendOutput(account, output);
                    Thread.Sleep(200);
                }
            }
        }
    }
}