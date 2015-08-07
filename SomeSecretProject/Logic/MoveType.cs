namespace SomeSecretProject.Logic
{
	public enum MoveType
	{
		E,
		W,
		SW,
		SE,
		NW,
		NE,
		RotateCW,
		RotateCCW
	}

    public static class MoveTypeExt
    {
        private const string moveW = "p'!.03";
        private const string moveE = "bcefy2";
        private const string moveSW = "aghij4";
        private const string moveSE = "lmno 5";
        private const string rotateCW = "dqrvz1";
        private const string rotateCCW = "kstuwx";
        private const string ignored = "\t\n\r";

        public static MoveType? Convert(char c)
        {
            MoveType? moveType = null;
            if (moveW.IndexOf(c) >= 0)
                moveType = MoveType.W;
            else if (moveE.IndexOf(c) >= 0)
                moveType = MoveType.E;
            else if (moveSW.IndexOf(c) >= 0)
                moveType = MoveType.SW;
            else if (moveSE.IndexOf(c) >= 0)
                moveType = MoveType.SE;
            else if (rotateCW.IndexOf(c) >= 0)
                moveType = MoveType.RotateCW;
            else if (rotateCCW.IndexOf(c) >= 0)
                moveType = MoveType.RotateCCW;
            return null;
        }

        public static bool IsIgnored(char c)
        {
            return ignored.IndexOf(c) >= 0;
        }
    }
}