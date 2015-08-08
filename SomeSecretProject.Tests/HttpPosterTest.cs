using Emulator.Posting;
using NUnit.Framework;
using SomeSecretProject.Algorithm;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    [Ignore]
    public class HttpPosterTest
    {
        private HttpPoster httpPoster;

        [SetUp]
        public void SetUp()
        {
            httpPoster = new HttpPoster(new MuggleProblemSolver());
        }

        [Test]
        public void PostAll()
        {
            httpPoster.PostAll(DavarAccount.MainTeam, "muggle1");
        }
    }
}