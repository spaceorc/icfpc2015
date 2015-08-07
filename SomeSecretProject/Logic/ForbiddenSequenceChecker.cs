using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace SomeSecretProject.Logic
{
	public class ForbiddenSequenceChecker
	{
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

		public bool CheckLastMove([NotNull] IList<MoveType> moves, MoveType lastMove)
		{
			if (lastMove == MoveType.NW || lastMove == MoveType.NE)
				return false;
			var prevMove = moves.Any() ? moves.Last() : (MoveType?)null;
			if (lastMove == MoveType.E && prevMove == MoveType.W)
				return false;
			if (lastMove == MoveType.W && prevMove == MoveType.E)
				return false;
			if (lastMove == MoveType.RotateCCW && (symmetric == 1 || prevMove == MoveType.RotateCW))
				return false;
			if (lastMove == MoveType.RotateCW && (symmetric == 1 || prevMove == MoveType.RotateCCW))
				return false;
			if (lastMove == MoveType.RotateCCW || lastMove == MoveType.RotateCW)
			{
				var rotateCount = 1;
				for (int i = moves.Count - 1; i >= 0; --i)
				{
					var prevMoveType = moves[i];
					if (prevMoveType == lastMove)
					{
						if (symmetric == ++rotateCount)
							return false;
					}
					else
						break;
				}
			}
			return true;
		}
	}
}