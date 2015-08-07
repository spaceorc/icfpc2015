using System.Collections.Generic;
using JetBrains.Annotations;

namespace SomeSecretProject.Logic
{
	public class ForbiddenSequenceChecker
	{
		private MoveType prevMove = MoveType.NE;
		private int rotateCount;
		private readonly int symmetric = 6;
		
		public ForbiddenSequenceChecker([NotNull] Unit unit)
		{
			var newUnit = unit;
			for (int i = 1; i <= 3; i++)
			{
				newUnit = newUnit.Move(MoveType.RotateCCW);
				var newMembersSet = new HashSet<Cell>(newUnit.members);
				newMembersSet.ExceptWith(unit.members);
				if (newMembersSet.Count == 0)
				{
					symmetric = i;
					break;
				}
			}
		}

		public bool CheckLastMove(IList<MoveType> moves, MoveType lastMove)
		{
			if (move == MoveType.E && prevMove == MoveType.W)
				return false;
			if (move == MoveType.W && prevMove == MoveType.E)
				return false;
			if (move == MoveType.RotateCCW && prevMove == MoveType.RotateCW)
				return false;
			if (move == MoveType.RotateCW && prevMove == MoveType.RotateCCW)
				return false;
			switch (move)
			{
				case MoveType.NW:
				case MoveType.NE:
					return false;
				case MoveType.RotateCCW:
				case MoveType.RotateCW:
					if (prevMove == move)
						rotateCount++;
					else
						rotateCount = 1;
					if (symmetric == rotateCount)
					{
						rotateCount--;
						return false;
					}
					break;
			}
			prevMove = move;
			return true;
		}
	}
}