using NUnit.Framework;
using Poster.Posting;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    [Ignore]
    public class HttpPosterTest
    {
        [Test]
        public void PostAll()
        {
            HttpPoster.PostAll(DavarAccount.MainTeam, "3_magic_2015-08-09_00_00_45");
        }
    }
}