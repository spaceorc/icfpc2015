using NUnit.Framework;
using Poster.Posting;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    [Ignore]
    public class HttpPosterTest
    {
        [Test]
		[Explicit]
        public void PostAll()
        {
            HttpPoster.PostAll(DavarAccount.MainTeam, "best-2015-08-10T15_22_48");
        }
    }
}