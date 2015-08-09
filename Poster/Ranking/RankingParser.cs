using System.Linq;
using System.Net;
using SomeSecretProject.IO;

namespace Poster.Ranking
{
    public static class RankingParser
    {
        private const string URL = "https://davar.icfpcontest.org/rankings.js";

        public static void Parse(int teamId = 127)
        {
            var client = new WebClient();

            var data = client.DownloadString(URL);

            var ranking = data.Substring("var data = ".Length).ParseAsJson<Ranking>();

            var stringsList = ranking.data.settings
                .Select(setting =>
                {
                    var rankingRanking = setting.rankings.First(e => e.teamId == teamId);
                    return string.Format("Problem: {0}, Score: {1} (max: {2}), PowerScore: {3}, Rank: {4}", setting.setting,
                        rankingRanking.score, setting.rankings.First(r => r.rank < 2).score, rankingRanking.power_score, rankingRanking.rank);
                })
                .ToList();
            var content = string.Join("\r\n", stringsList);

            var fileName = string.Format("{0}.txt", ranking.time.Replace(':', ' '));
            ScoresFileManager.Save(teamId.ToString("D"), fileName, content);
        }
    }
}