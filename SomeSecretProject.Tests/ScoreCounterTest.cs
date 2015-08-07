using NUnit.Framework;
using SomeSecretProject.Logic.Score;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    public class ScoreCounterTest
    {
        [Test]
        public void Count()
        {
            var unit1 = new Logic.Unit
            {
                cells = new Logic.Cell[1]
            };

            var unitSteps1 = new[]
            {
                new UnitStep(unit1, false, 0, new[] {"Ei!"})
            };
            Assert.AreEqual(306, ScoreCounter.Count(unitSteps1));

            var unitSteps2 = new[]
            {
                new UnitStep(unit1, false, 0, new[] {"Ei!", "Ei!"})
            };
            Assert.AreEqual(312, ScoreCounter.Count(unitSteps2));
        }
    }
}