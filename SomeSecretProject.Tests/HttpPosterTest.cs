using Emulator.Posting;
using NUnit.Framework;

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
            httpPoster = new HttpPoster(new SimpleProblemSolver());
        }

        [Test]
        public void PostAll()
        {
            httpPoster.PostAll(DavarAccount.MainTeam, "test");
        }
    }
}