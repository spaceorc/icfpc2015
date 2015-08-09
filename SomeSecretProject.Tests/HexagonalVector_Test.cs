using NUnit.Framework;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    public class HexagonalVector_Test
    {
        [Test]
        public void Div2()
        {
            Assert.AreEqual(-2, HexagonalVector.Div2(-4));
            Assert.AreEqual(-2, HexagonalVector.Div2(-3));
            Assert.AreEqual(-1, HexagonalVector.Div2(-2));
            Assert.AreEqual(-1, HexagonalVector.Div2(-1));
            Assert.AreEqual(0, HexagonalVector.Div2(0));
            Assert.AreEqual(0, HexagonalVector.Div2(1));
            Assert.AreEqual(1, HexagonalVector.Div2(2));
            Assert.AreEqual(1, HexagonalVector.Div2(3));
        }
    }
}