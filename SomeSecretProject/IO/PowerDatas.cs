using System.IO;

namespace SomeSecretProject.IO
{
    public static class PowerDatas
    {
        public static string[] GetPowerPhrases(string filename = "default.txt")
        {
            return File.ReadAllLines(Path.Combine(@"..\..\..\data\power", filename));
        }
    }
}