using System.IO;

namespace Poster.Ranking
{
    public static class ScoresFileManager
    {
        private const string BASE_DIRECTORY = "../../../data/scores/";

        static ScoresFileManager()
        {
            if (!Directory.Exists(BASE_DIRECTORY))
            {
                Directory.CreateDirectory(BASE_DIRECTORY);
            }
        }

        public static void Save(string directory, string fileName, string content)
        {
            var fullDirectoryPath = Path.Combine(BASE_DIRECTORY, directory);

            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            var fullFileName = Path.Combine(fullDirectoryPath, fileName);

            File.WriteAllText(fullFileName, content);
        }
    }
}