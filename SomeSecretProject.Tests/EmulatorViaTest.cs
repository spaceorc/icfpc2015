using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SomeSecretProject.IO;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    public class EmulatorViaTest
    {
        private const string idealSolvesFolder = @"..\..\..\importantSolves";

        [Test]
        public void RunAll()
        {
            Emulator.Program.SolveAll(PowerDatas.GetPowerPhrases());
        }

        [Test]
        public void CompareSolves()
        {
            var groupBy = GetSolves();
            foreach (var result in groupBy)
            {
                Console.WriteLine("Problem" + result.Key);
                foreach (var res in result.OrderByDescending(r => r.Item3))
                {
                    Console.WriteLine("{0}: {1}", res.Item1, res.Item3);
                }
                Console.WriteLine();
            }
        }

        private static IEnumerable<IGrouping<string, Tuple<string, string, int>>> GetSolves()
        {
            var dirs = Directory.EnumerateDirectories(idealSolvesFolder).Select(s => Path.GetFileName(s));
            var groupBy = dirs.SelectMany(
                d =>
                    Directory.EnumerateDirectories(Path.Combine(idealSolvesFolder, d))
                        .Select(inode => Path.GetFileName(inode))
                        .Select(
                            problem =>
                            {
                                var files =
                                    Directory.EnumerateFiles(Path.Combine(idealSolvesFolder, d, problem))
                                        .Select(f => Path.GetFileName(f))
                                        .ToArray();
                                var score = int.Parse(files.Single(f => f.Contains("score"))
                                    .Replace("score", "")
                                    .Replace(".txt", ""));
                                return Tuple.Create(d, problem, score);
                            })).GroupBy(e => e.Item2);
            return groupBy;
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
                var best = problemGroup.OrderByDescending(p => p.Item3).First();
                var path = Path.Combine(idealSolvesFolder, best.Item1, problemGroup.Key);
                foreach (var a in Directory.EnumerateFiles(path))
                    File.Copy(a, Path.Combine(newPAth, Path.GetFileName(a)));
            }
        }
    }
}