using NUnit.Framework;
using SomeSecretProject.IO;

namespace SomeSecretProject.Tests
{
    [TestFixture]
    public class EmulatorViaTest
    {
        [Test]
        public void RunAll()
        {
            Emulator.Program.SolveAll(PowerDatas.GetPowerPhrases());
        }
    }
}