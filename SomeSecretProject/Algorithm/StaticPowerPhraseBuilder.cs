using System;
using System.Collections.Generic;
using System.Linq;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public class StaticPowerPhraseBuilder : IPowerPhraseBuilder
	{
		private const string moveW = "p'!.03";
		private const string moveE = "bcefy2";
		private const string moveSW = "aghij4";
		private const string moveSE = "lmno 5";
		private const string rotateCW = "dqrvz1";
		private const string rotateCCW = "kstuwx";

		public string Build(IList<MoveType> moveTypes)
		{
			return new string(moveTypes.Select(GetMoveString).Select(x => x[0]).ToArray());
		}

		private static string GetMoveString(MoveType moveType)
		{
			switch (moveType)
			{
				case MoveType.E:
					return moveE;
				case MoveType.W:
					return moveW;
				case MoveType.SW:
					return moveSW;
				case MoveType.SE:
					return moveSE;
				case MoveType.RotateCW:
					return rotateCW;
				case MoveType.RotateCCW:
					return rotateCCW;
				default:
					throw new ArgumentOutOfRangeException("moveType", moveType, null);
			}
		}
	}
}