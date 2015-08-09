using System;
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
			if (lastMove == MoveType.SW || lastMove == MoveType.SE)
				return true;
			if (symmetric == 1)
			{
				if (lastMove == MoveType.RotateCCW || lastMove == MoveType.RotateCW)
					return false;
				var prevMove = moves.Any() ? moves.Last() : (MoveType?)null;
				if (lastMove == MoveType.E && prevMove == MoveType.W)
					return false;
				if (lastMove == MoveType.W && prevMove == MoveType.E)
					return false;
				return true;
			}
			var used = new HashSet<Tuple<int, int>>();
			var position = Tuple.Create(0, 0);
			used.Add(position);
			position = ApplyMove(lastMove, position);
			if (position == null)
				throw new InvalidOperationException("Position couldn't be null here");
			used.Add(position);
			for (var i = moves.Count - 1; i >= 0; i--)
			{
				position = ApplyMove(moves[i], position);
				if (position == null)
					return true;
				if (!used.Add(position))
					return false;
			}
			return true;
		}

		[CanBeNull]
		private Tuple<int, int> ApplyMove(MoveType moveType, [NotNull] Tuple<int, int> position)
		{
			switch (moveType)
			{
				case MoveType.E:
					return Tuple.Create(position.Item1 + 1, position.Item2);
				case MoveType.W:
					return Tuple.Create(position.Item1 - 1, position.Item2);
				case MoveType.SW:
				case MoveType.SE:
					return null;
				case MoveType.RotateCW:
					return Tuple.Create(position.Item1, (position.Item2 + 1) % symmetric);
				case MoveType.RotateCCW:
					return Tuple.Create(position.Item1, (position.Item2 + symmetric - 1) % symmetric);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}