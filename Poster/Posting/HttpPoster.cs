using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace Poster.Posting
{
    public static class HttpPoster
    {
        public static void PostAll(DavarAccount account, string solveName, string tag = null)
        {
            var directoryMain = @"..\..\..\importantSolves\" + solveName;
            var problems = Directory.EnumerateDirectories(directoryMain);
            foreach (var p in problems.Select(Path.GetFileName))
            {
                Console.WriteLine("Problemm{0}", p);
                StringBuilder sb = new StringBuilder();
                var bKTULHUftagghen = new List<int>();
                foreach (var f in Directory.EnumerateFiles(Path.Combine(directoryMain, p)).Select(Path.GetFileName))
                {
                    if (f.StartsWith("score"))
                        continue;
                    var output = File.ReadAllText(Path.Combine(directoryMain, p, f)).ParseAsJson<Output>();
                    var game = new Game(ProblemsSet.GetProblem(int.Parse(p)), output, PowerDatas.GetPowerPhrases());
                    while (game.state == GameBase.State.UnitInGame || game.state == GameBase.State.WaitUnit)
                        game.Step();
                    sb.AppendLine(output.seed + " " + game.CurrentScore);
                    bKTULHUftagghen.Add(game.CurrentScore);
                    //output.tag = tag ?? solveName + " " + DateTime.Now.ToString("O");
                    //HttpHelper.SendOutput(account, output);
                    //Thread.Sleep(200);
                }
                File.WriteAllText(Path.Combine(directoryMain, p, "score_" + (bKTULHUftagghen.Sum() / bKTULHUftagghen.Count) + ".txt"), sb.ToString());
            }
        }
    }
}