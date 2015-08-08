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
        private HttpHelper httpHelper;

        [SetUp]
        public void SetUp()
        {
            httpHelper = new HttpHelper(DavarAccounts.TEST_TEAM_TOKEN, DavarAccounts.TEST_TEAM_ID);
        }

        [TestCase]
        public void TestLoadProblem()
        {
            var problem = httpHelper.GetProblem(0);
            Console.WriteLine(problem.width +" x "+problem.height);
        }

        [TestCase]
        public void TestSendSolution()
        {
            var solution = new Output
            {
                problemId = 0,
                seed = 0,
                tag = "re",
                solution = "Ei!Ei!"
            };
            var result = httpHelper.SendOutput(solution);
            Assert.IsTrue(result);
        }

        [TestCase]
        public void LoadAllProblems()
        {
            for (var i = 0; i < 24; ++i)
            {
                ProblemServer.GetProblem(i);
            }
        }
    }
}
