using Emulator.Posting;
using NUnit.Framework;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    public class HttpPosterTest
    {
        private HttpPoster httpPoster;

        [SetUp]
        public void SetUp()
        {
            httpPoster = new HttpPoster(DavarAccounts.MAIN_TEAM_TOKEN, DavarAccounts.MAIN_TEAM_ID);
        }

        [Test]
        public void PostAll()
        {
            httpPoster.PostAll("test");
        }
    }
}