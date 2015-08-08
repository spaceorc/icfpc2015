using NUnit.Framework;
using Poster.Posting;
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
            httpPoster = new HttpPoster();
        }

        [Test]
        public void PostAll()
        {
            httpPoster.PostAll(DavarAccount.MainTeam, "1_Muddle_7cbff25c");
        }
    }
}