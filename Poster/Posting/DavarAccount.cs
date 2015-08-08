namespace Emulator.Posting
{
    public class DavarAccount
    {
        public static DavarAccount MainTeam = new DavarAccount("LZPdLA9OaBh2PJO7Kogh6X+/sDzqX4nPdN+cNlAO39w=", 127);
        public static DavarAccount TestTeam = new DavarAccount("MAXOFfmF0wuQ4QYjz0mpShQckV6x/64vQuvT8aSI4Xg=", 188);

        public DavarAccount(string apiToken, int teamId)
        {
            ApiToken = apiToken;
            TeamId = teamId;
        }

        public string ApiToken { get; private set; }
        public int TeamId { get; private set; }
    }
}