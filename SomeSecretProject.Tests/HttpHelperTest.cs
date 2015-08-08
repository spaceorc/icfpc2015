using System;
using Emulator.Posting;
using NUnit.Framework;
using SomeSecretProject.IO;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    [Ignore]
    public class HttpHelperTest
    {
        [Test]
        public void TestLoadProblem()
        {
            var problem = HttpHelper.GetProblem(0);
            Console.WriteLine(problem.width +" x "+problem.height);
        }

        [Test]
        public void TestSendSolution()
        {
            var solution = new Output
            {
                problemId = 0,
                seed = 0,
                tag = "re",
                solution = "Ei!"
            };
            var result = HttpHelper.SendOutput(DavarAccount.TestTeam, solution);
            Assert.IsTrue(result);
        }

        [Test]
        public void LoadAllProblems()
        {
            for (var i = 0; i < 24; ++i)
            {
                ProblemServer.GetProblem(i);
            }
        }
    }
}
