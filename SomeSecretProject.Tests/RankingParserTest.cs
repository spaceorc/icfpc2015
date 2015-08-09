using NUnit.Framework;
using Poster.Posting;
using Poster.Ranking;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    public class RankingParserTest
    {
        [Test]
        public void Test()
        {
            RankingParser.Parse(DavarAccount.MainTeam.TeamId);
        }
    }
}