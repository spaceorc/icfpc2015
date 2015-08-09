using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public static class WayExtensions
	{
		[NotNull]
		public static Way Add([CanBeNull] this Way parent, MoveType moveType)
		{
			return new Way(parent, moveType);
		}

		[NotNull]
		public static IList<MoveType> ToList([CanBeNull] this Way way)
		{
			var list = new List<MoveType>();
			for (; way != null; way = way.parent)
				list.Add(way.moveType);
			list.Reverse();
			return list;
		}
	}

	public class Way
	{
		public readonly MoveType moveType;

		[CanBeNull]
		public readonly Way parent;

		public Way([CanBeNull] Way parent, MoveType moveType)
		{
			this.moveType = moveType;
			this.parent = parent;
		}

		public bool CheckSequence(int symmetric)
		{
			if (moveType == MoveType.NW || moveType == MoveType.NE)
				return false;
			if (moveType == MoveType.SW || moveType == MoveType.SE)
				return true;
			if (symmetric == 1)
			{
				if (moveType == MoveType.RotateCCW || moveType == MoveType.RotateCW)
					return false;
				if (parent != null)
				{
					if (moveType == MoveType.E && parent.moveType == MoveType.W)
						return false;
					if (moveType == MoveType.W && parent.moveType == MoveType.E)
						return false;
				}
				return true;
			}
			var used = new List<Tuple<int, int>>();
			var position = Tuple.Create(0, 0);
			used.Add(position);
			for (var current = this; current != null; current = current.parent)
			{
				position = ApplyMove(current.moveType, position, symmetric);
				if (position == null)
					return true;
				if (used.Any(x => x.Equals(position)))
					return false;
				used.Add(position);
			}
			return true;
		}

		[CanBeNull]
		private static Tuple<int, int> ApplyMove(MoveType moveType, [NotNull] Tuple<int, int> position, int symmetric)
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
				case MoveType.RotateCCW:
					return Tuple.Create(position.Item1, GetNewRotation(position.Item2, symmetric, moveType));
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static int GetNewRotation(int rotation, int symmetric, MoveType moveType)
		{
			switch (moveType)
			{
				case MoveType.E:
				case MoveType.W:
				case MoveType.SW:
				case MoveType.SE:
				case MoveType.NW:
				case MoveType.NE:
					return rotation;
				case MoveType.RotateCW:
					return (rotation + 1) % symmetric;
				case MoveType.RotateCCW:
					return (rotation + symmetric - 1) % symmetric;
				default:
					throw new ArgumentOutOfRangeException("moveType", moveType, null);
			}
		}

		public override string ToString()
		{
			return string.Format("{0}{1}", parent == null ? "" : parent.ToString() + "-", moveType);
		}
	}
}