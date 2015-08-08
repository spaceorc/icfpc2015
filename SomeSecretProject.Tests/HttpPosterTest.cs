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
            httpPoster.PostAll(DavarAccount.MainTeam, "3_magic_2015-08-09_00_00_45");
        }
    }
}