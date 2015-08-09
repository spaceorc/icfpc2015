using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SomeSecretProject.Logic
{
	[JsonConverter(typeof(StringEnumConverter))]
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
        public static MoveType[] LinearMoves = new MoveType[]{MoveType.E, MoveType.NE, MoveType.NW, MoveType.SE, MoveType.SW, MoveType.W};
        private const string moveW = "p'!.03";
        private const string moveE = "bcefy2";
        private const string moveSW = "aghij4";
        private const string moveSE = "lmno 5";
        private const string rotateCW = "dqrvz1";
        private const string rotateCCW = "kstuwx";
        private const string ignored = "\t\n\r";

        public static MoveType? Convert(char c)
        {
	        c = Char.ToLower(c);
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
            return moveType;
        }
		
		public static MoveType AntiMove(MoveType moveType)
        {
			switch (moveType)
			{
				case MoveType.E:
					return MoveType.W;
				case MoveType.W:
					return MoveType.E;
				case MoveType.SW:
					return MoveType.NE;
				case MoveType.SE:
					return MoveType.NW;
				case MoveType.NW:
					return MoveType.SE;
				case MoveType.NE:
					return MoveType.SW;
				case MoveType.RotateCW:
					return MoveType.RotateCCW;
				case MoveType.RotateCCW:
					return MoveType.RotateCW;
				default:
					throw new ArgumentOutOfRangeException("moveType", moveType, null);
			}
        }

        public static bool IsIgnored(char c)
        {
            return ignored.IndexOf(c) >= 0;
        }

	    public static readonly MoveType[] allowedMoves =
	    {
		    MoveType.E, MoveType.W,
		    MoveType.SE, MoveType.SW,
		    MoveType.RotateCW, MoveType.RotateCCW,
	    };
    }
}