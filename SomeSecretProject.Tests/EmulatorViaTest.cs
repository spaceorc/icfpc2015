﻿using System;
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
                var grouByName = result.GroupBy(r => r.Item1);
                Console.WriteLine("Problem" + result.Key);
                foreach (var res in grouByName.OrderByDescending(r => r.Sum(k => k.Item4)))
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
    }
}