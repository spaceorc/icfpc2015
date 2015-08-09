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
            HttpPoster.PostAll(DavarAccount.MainTeam, "bestBySeeds_2015-08-09_14_54");
        }
    }
}