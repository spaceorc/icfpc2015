using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using NUnit.Framework;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    public class EmulatorViaTest
    {
        private const string idealSolvesFolder = @"..\..\..\importantSolves";

        [Test]
		[Explicit]
        public void RunAll()
        {
            var time = Stopwatch.StartNew();
            Emulator.Program.SolveAll(PowerDatas.GetPowerPhrases());
            time.Stop();
            Console.WriteLine("Total Elapsed: {0}", time);
        }

        [Test]
        public void CompareSolves()
        {
            var groupBy = GetSolves();
            var bests = new List<Tuple<string, int>>();
            foreach (var result in groupBy)
            {
                var grouByName = result.GroupBy(r => r.Item1);
                Console.WriteLine("Problem" + result.Key);
                var solves = grouByName.OrderByDescending(r => r.Sum(k => k.Item4));
                var bsolve = solves.First();
                bests.Add(Tuple.Create(bsolve.Key, int.Parse(result.Key)));
                foreach (var res in solves)
                {
                    Console.WriteLine("{0}: {1}", res.Key, res.Sum(K => K.Item4));
                }
                foreach (var byS in result.GroupBy(r => r.Item3))
                {
                    Console.WriteLine("Seed {0}", byS.Key);
                    foreach (var oneItem in byS.OrderByDescending(b => b.Item4))
                    {
                        Console.WriteLine("{0}: {1}", oneItem.Item1, oneItem.Item4);
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("Who's BEST?");
            foreach (var taskresult in bests.GroupBy(b => b.Item1))
            {
                Console.WriteLine("{0}: {1}% ({2})", taskresult.Key, taskresult.Count() * 100 / bests.Count, string.Join(".", taskresult.Take(5).Select(b => b.Item2)));
            }
           
        }

        [Test]
        public void PrintBestSolves()
        {
            var groupBy = GetSolves();
            foreach (var result in groupBy.OrderBy(k => int.Parse(k.Key)))
            {
                var grouByName = result.GroupBy(r => r.Item1);
                Console.Write("Problem: " + result.Key + ", ");
                var best = grouByName.OrderByDescending(r => r.Sum(k => k.Item4)).First();
                Console.Write("Score: {0}", best.Sum(K => K.Item4) / best.Count());
                Console.WriteLine();
            }
        }

        private static IEnumerable<IGrouping<string, Tuple<string, string, string, int>>> GetSolves()
        {
            var dirs = Directory.EnumerateDirectories(idealSolvesFolder).Select(s => Path.GetFileName(s)).Where(s => !s.StartsWith("best")); //ignore solanka
            var groupBy = dirs.SelectMany(
                d =>
                    Directory.EnumerateDirectories(Path.Combine(idealSolvesFolder, d))
                        .Select(inode => Path.GetFileName(inode))
                        .SelectMany(
                            problem =>
                            {
                                var files =
                                    Directory.EnumerateFiles(Path.Combine(idealSolvesFolder, d, problem))
                                        .Select(f => Path.GetFileName(f))
                                        .ToArray();
                                var scoresFile = files.Single(f => f.Contains("score"));
                                return File.ReadAllLines(Path.Combine(idealSolvesFolder, d, problem, scoresFile)).Select(l => l.Split()).Select(l => Tuple.Create(d, problem, l[0], int.Parse(l[1]))); //name, pr, se, score
                            })).GroupBy(e => e.Item2);
            return groupBy.ToList();
        }

        [Test]
        public void ChooseBestSolves()
        {
            var groupBy = GetSolves();
            var newSolve = Path.Combine(idealSolvesFolder, "best-" + DateTime.Now.ToString("O").Replace(":", "_"));
            Directory.CreateDirectory(newSolve);
            foreach (var problemGroup in groupBy)
            {
                var newPAth = Path.Combine(newSolve, problemGroup.Key);
                Directory.CreateDirectory(newPAth);
                var bests = problemGroup.GroupBy(p => p.Item3).Select(s => Tuple.Create(s.Key, s.OrderByDescending(k => k.Item4).First().Item1));
                foreach (var best in bests)
                {
                    var path = Path.Combine(idealSolvesFolder, best.Item2, problemGroup.Key, best.Item1);
                    File.Copy(path, Path.Combine(newPAth, Path.GetFileName(path)));
                }
            }
        }

        [Test]
		[Explicit]
        public void RecountScores()
        {
            var groupBy = GetSolves();
            var ungroup = groupBy.SelectMany(g => g);
            var groupByProblemAndName = ungroup.GroupBy(g => g.Item1 + "<->" + g.Item2);
            foreach (var g in groupByProblemAndName)
            {
                var problFolder = Path.Combine(idealSolvesFolder, g.First().Item1, g.First().Item2);
                var cb = new StringBuilder();
                var sum = 0;
                foreach (var seedTuple in g)
                {
                    var seed = seedTuple.Item3;
                    var solution = File.ReadAllText(problFolder + @"\" + seed).ParseAsJson<Output>().solution;
                    var game = new Game(ProblemsSet.GetProblem(int.Parse(seedTuple.Item2)), new Output() { solution = solution, seed = int.Parse(seed) }, PowerDatas.GetPowerPhrases());
                    while (game.state == GameBase.State.UnitInGame || game.state == GameBase.State.WaitUnit)
                        game.Step();
                    var resultingScoring = game.CurrentScore;
                    sum += game.CurrentScore;
                    cb.AppendLine(seed + " " + resultingScoring);
                }
                var result = cb.ToString();
                File.WriteAllText(problFolder + @"\" + "score" + (sum / g.Count()) + ".txt", result);
            }
        }
    }
}