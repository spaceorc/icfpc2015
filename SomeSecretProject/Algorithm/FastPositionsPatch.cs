using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public class FastPositionsPatch : FastPositionsBase
	{
		public readonly FastPositions previousPositions;
		public readonly Dictionary<Key, Item> patchedPositions = new Dictionary<Key, Item>();
		public readonly Dictionary<Key, bool> patchedKeys = new Dictionary<Key, bool>();

		public FastPositionsPatch([NotNull] Map updatedMap, [NotNull] FastPositions previousPositions)
			: base(updatedMap, previousPositions.originalUnit, previousPositions.powerPhrases)
		{
			this.previousPositions = previousPositions;
		}
		
		private readonly MoveType[] allowedAntiMoves =
		{
			MoveType.E, MoveType.W,
			MoveType.NE, MoveType.NW,
			MoveType.RotateCW, MoveType.RotateCCW,
		};

		public override void InitQueue([NotNull] Queue<KeyValuePair<Key, Item>> queue)
		{
			var newQueueItems = new Dictionary<Key, Item>();
			foreach (var previousPosition in previousPositions.EnumerateAllPositions())
			{
				var key = previousPosition.Key;
				var unit = previousPosition.Value.unit;
				var way = previousPosition.Value.way;
				var keys = new List<Key>();
				while (true)
				{
					bool good;
					if (patchedKeys.TryGetValue(key, out good))
					{
						if (good)
							AddGoodWay(keys);
						else
							AddBadWay(keys, newQueueItems);
						break;
					}
					keys.Add(key);
					if (!unit.IsCorrect(map))
						AddBadWay(keys, newQueueItems);
					if (way == null)
					{
						foreach (var passedKey in keys)
							patchedKeys.Add(passedKey, true);
						break;
					}
					var antiMoveType = MoveTypeExt.AntiMove(way.moveType);
					key = MoveKey(key, antiMoveType);
					Item item;
					if (!previousPositions.TryGetVisited(key, out item))
						throw new InvalidOperationException(string.Format("!previousPositions.TryGetVisited(key:{0}, out item)", key));
					unit = item.unit;
					way = way.parent;
				}
			}
			foreach (var newQueueItem in newQueueItems)
				queue.Enqueue(newQueueItem);
		}

		private void AddGoodWay([NotNull] List<Key> keys)
		{
			foreach (var passedKey in keys)
				patchedKeys.Add(passedKey, true);
			keys.Clear();
		}

		private void AddBadWay([NotNull] List<Key> keys, [NotNull] Dictionary<Key, Item> newQueueItems)
		{
			foreach (var passedKey in keys)
			{
				patchedKeys.Add(passedKey, false);
				newQueueItems.Remove(passedKey);
			}
			foreach (var passedKey in keys)
			{
				foreach (var allowedAntiMove in allowedAntiMoves)
				{
					var antiKey = MoveKey(passedKey, allowedAntiMove);
					bool antiGood;
					if (!patchedKeys.TryGetValue(antiKey, out antiGood) || antiGood)
					{
						Item antiItem;
						if (previousPositions.TryGetVisited(antiKey, out antiItem) && !newQueueItems.ContainsKey(antiKey))
							newQueueItems.Add(antiKey, antiItem);
					}
				}
			}
			keys.Clear();
		}

		[NotNull]
		private Key MoveKey([NotNull] Key key, MoveType moveType)
		{
			var pivot = key.pivot;
			switch (moveType)
			{
				case MoveType.E:
				case MoveType.W:
				case MoveType.SW:
				case MoveType.SE:
				case MoveType.NW:
				case MoveType.NE:
					pivot = pivot.Move(moveType);
					break;
			}
			return new Key(pivot, Way.GetNewRotation(key.rotation, symmetric, moveType));
		}

		public override bool TryGetVisited([NotNull] Key key, out Item result)
		{
			if (patchedPositions.TryGetValue(key, out result))
				return true;
			bool good;
			if (patchedKeys.TryGetValue(key, out good) && !good)
				return false;
			return previousPositions.TryGetVisited(key, out result);
		}

		[NotNull]
		public override FastPositions Apply()
		{
			return new FastPositions(this);
		}

		[NotNull]
		public override IEnumerable<KeyValuePair<Key, Item>> EnumerateAllPositions()
		{
			return patchedPositions
				.Concat(previousPositions.EnumerateAllPositions().Where(p =>
				{
					bool good;
					return !patchedKeys.TryGetValue(p.Key, out good) || good;
				}));
		}

		protected override void DoUpdatePositionItem([NotNull] Key key, [NotNull] Item item)
		{
			patchedPositions[key] = item;
			patchedKeys[key] = false;
		}
	}
}