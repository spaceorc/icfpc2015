using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public class FastPositionsPatch : FastPositionsBase
	{
		public readonly FastPositionsBase previousPositions;
		public readonly Dictionary<Key, Item> patchedPositions = new Dictionary<Key, Item>();
		public readonly Dictionary<Key, PatchItem> patchedKeys = new Dictionary<Key, PatchItem>();

		public FastPositionsPatch([NotNull] Map updatedMap, [NotNull] FastPositionsBase previousPositions)
			: base(updatedMap, previousPositions.originalUnit, previousPositions.powerPhrases)
		{
			this.previousPositions = previousPositions;
		}

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
					PatchItem patchItem;
					if (patchedKeys.TryGetValue(key, out patchItem))
					{
						foreach (var passedKey in keys)
						{
							patchedKeys.Add(passedKey, patchItem);
							newQueueItems.Remove(passedKey);
						}
						break;
					}
					keys.Add(key);
					bool parentIsCandidate = false;
					if (!unit.IsCorrect(map))
					{
						patchItem = new PatchItem { good = false };
						foreach (var passedKey in keys)
						{
							patchedKeys.Add(passedKey, patchItem);
							newQueueItems.Remove(passedKey);
						}
						parentIsCandidate = true;
						keys.Clear();
					}
					if (way == null)
					{
						patchItem = new PatchItem { good = true };
						foreach (var passedKey in keys)
							patchedKeys.Add(passedKey, patchItem);
						break;
					}
					var antiMoveType = MoveTypeExt.AntiMove(way.moveType);
					unit = unit.Move(antiMoveType);
					key = new Key(unit, Way.GetNewRotation(key.rotation, previousPositions.symmetric, antiMoveType));
					way = way.parent;
					if (parentIsCandidate)
					{
						if (!newQueueItems.ContainsKey(key))
							newQueueItems.Add(key, new Item
							{
								unit = unit,
								way = way,
								???
							});
					}
				}
			}
			foreach (var newQueueItem in newQueueItems)
				queue.Enqueue(newQueueItem);
		}

		public override bool TryGetVisited([NotNull] Key key, out Item result)
		{
			if (patchedPositions.TryGetValue(key, out result))
				return true;
			PatchItem patchItem;
			if (patchedKeys.TryGetValue(key, out patchItem) && !patchItem.good)
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
					PatchItem patchItem;
					return !patchedKeys.TryGetValue(p.Key, out patchItem) || patchItem.good;
				}));
		}

		protected override void DoUpdatePositionItem([NotNull] Key key, [NotNull] Item item)
		{
			patchedPositions[key] = item;
			patchedKeys.Remove(key);
		}

		public class PatchItem
		{
			public bool good;
		}
	}
}