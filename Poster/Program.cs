using System;
using System.Linq;
using NDesk.Options;
using Poster.Posting;
using Poster.Ranking;

namespace Poster
{
    class Program
    {
        static void Main(string[] args)
        {
            var action = "help";
            var teamId = DavarAccount.MainTeam.TeamId;
            var solveName = Guid.NewGuid().ToString("N");
            string tag = null;

            var options = new OptionSet
            {
                {"post|rank|help", v => action = v},
                {"team=", (int v) => teamId = v},
                {"solveName=", v => solveName = v},
                {"tag=", v => tag = v}
            };
            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Error.WriteLine("Option '{0}' value is invalid: {1}", e.OptionName, e.Message);
                Console.Error.WriteLine();
            }
            switch (action)
            {
                case "help":
                    Console.WriteLine("USAGE:");
                    options.WriteOptionDescriptions(Console.Out);
                    break;
                case "post":
                    HttpPoster.PostAll(DavarAccount.Items.First(e => e.TeamId == teamId), solveName, tag);
                    break;
                case "rank":
                    RankingParser.Parse(teamId);
                    break;
            }
        }
    }
}
