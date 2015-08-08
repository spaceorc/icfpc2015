using System.Collections.Generic;
using NUnit.Framework;
using SomeSecretProject.Logic;
using SomeSecretProject.Logic.Score;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    public class ScoreCounterTest
    {
        [Test]
        public void CountUnit()
        {
            var unit = new Unit {members = new HashSet<Cell>(new[] {new Cell(), new Cell()})};

            Assert.AreEqual(2, ScoreCounter.GetMoveScore(unit, 0, 0));
            Assert.AreEqual(102, ScoreCounter.GetMoveScore(unit, 1, 0));
            Assert.AreEqual(302, ScoreCounter.GetMoveScore(unit, 2, 0));
            Assert.AreEqual(602, ScoreCounter.GetMoveScore(unit, 3, 0));
            Assert.AreEqual(102, ScoreCounter.GetMoveScore(unit, 1, 1));
            Assert.AreEqual(112, ScoreCounter.GetMoveScore(unit, 1, 2));
            Assert.AreEqual(362, ScoreCounter.GetMoveScore(unit, 2, 3));
        }

        [Test]
        public void CountMagicWords()
        {
            Assert.AreEqual(612, ScoreCounter.GetPowerScores(new[] {"aaa", "bbb"}));
            Assert.AreEqual(312, ScoreCounter.GetPowerScores(new[] {"aaa", "aaa"}));
            Assert.AreEqual(318, ScoreCounter.GetPowerScores(new[] {"aaa", "aaa", "aaa"}));
            Assert.AreEqual(310, ScoreCounter.GetPowerScores(new[] {"qqqqq"}));
        }
    }
}