using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public abstract class FastPositionsBase
	{
		public readonly Map map;
		public readonly Unit originalUnit;
		public readonly string[] powerPhrases;
		public readonly int symmetric;
		public readonly Dictionary<Key, EndItem> endPositions = new Dictionary<Key, EndItem>();

		protected FastPositionsBase([NotNull] Map map, [NotNull] Unit originalUnit, [NotNull] string[] powerPhrases)
		{
			this.map = map;
			this.originalUnit = originalUnit;
			this.powerPhrases = powerPhrases;
			symmetric = Unit.GetSymmetric(originalUnit);
		}

		public void BuildAllPositions()
		{
			var queue = new Queue<KeyValuePair<Key, Item>>();
			InitQueue(queue);
			while (queue.Any())
			{
				var queueItem = queue.Dequeue();
				var currentItem = queueItem.Value;
				var currentKey = queueItem.Key;
				var currentUnit = currentItem.unit;
				var currentWay = currentItem.way;
				var currentRotation = currentKey.rotation;
				for (int wordIndex = 0; wordIndex < powerPhrases.Length; wordIndex++)
				{
					var powerPhrase = powerPhrases[wordIndex];
					var next = currentUnit;
					var nextWay = currentWay;
					var nextRotation = currentRotation;
					Key nextKey = null;
					var invalid = false;
					foreach (var c in powerPhrase)
					{
						var move = MoveTypeExt.Convert(c).Value;
						nextWay = nextWay.Add(move);
						if (!nextWay.CheckSequence(symmetric))
						{
							invalid = true;
							break;
						}
						nextRotation = Way.GetNewRotation(nextRotation, symmetric, move);
						next = next.Move(move);
						nextKey = new Key(next, nextRotation);
						if (!next.IsCorrect(map))
						{
							invalid = true;
							break;
						}
					}
					if (!invalid)
					{
						if (nextKey == null)
							throw new InvalidOperationException("nextKey == null");
						var nextScore = currentItem.score;
						if (!currentItem.spelledWords[wordIndex])
							nextScore += 300;
						nextScore += 2 * powerPhrase.Length;
						Item prevVisited;
						if (!TryGetVisited(nextKey, out prevVisited) || prevVisited.score < nextScore)
						{
							var nextSpelledWords = new bool[currentItem.spelledWords.Length];
							Array.Copy(currentItem.spelledWords, nextSpelledWords, nextSpelledWords.Length);
							nextSpelledWords[wordIndex] = true;
							queue.Enqueue(UpdatePositionItem(nextKey, new Item
							{
								unit = next,
								way = nextWay,
								score = nextScore,
								spelledWords = nextSpelledWords
							}));
						}
					}
				}
				foreach (var move in MoveTypeExt.allowedMoves)
				{
					var nextWay = currentWay.Add(move);
					if (!nextWay.CheckSequence(symmetric))
						continue;
					var nextRotation = Way.GetNewRotation(currentRotation, symmetric, move);
					var next = currentUnit.Move(move);
					var nextKey = new Key(next, nextRotation);
					if (!next.IsCorrect(map))
						continue;
					Item prevVisited;
					if (!TryGetVisited(nextKey, out prevVisited) || prevVisited.score < currentItem.score)
						queue.Enqueue(UpdatePositionItem(nextKey, new Item
						{
							unit = next,
							way = nextWay,
							score = currentItem.score,
							spelledWords = currentItem.spelledWords
						}));
				}
			}
		}

		public abstract void InitQueue([NotNull] Queue<KeyValuePair<Key, Item>> queue);
		protected abstract void DoUpdatePositionItem([NotNull] Key key, [NotNull] Item item);
		public abstract bool TryGetVisited([NotNull] Key key, out Item result);

		[NotNull]
		public abstract FastPositions Apply();

		[NotNull]
		public abstract IEnumerable<KeyValuePair<Key, Item>> EnumerateAllPositions();

		public KeyValuePair<Key, Item> UpdatePositionItem([NotNull] Key key, [NotNull] Item item)
		{
			DoUpdatePositionItem(key, item);
			var finalMoves = FinalMoves(item.unit);
			if (finalMoves.Any())
			{
				endPositions[key] = new EndItem
				{
					item = item,
					finalMoves = finalMoves
				};
			}
			return new KeyValuePair<Key, Item>(key, item);
		}

		[NotNull]
		public List<MoveType> FinalMoves([NotNull] Unit unit)
		{
			return MoveTypeExt.allowedMoves.Where(m => !unit.Move(m).IsCorrect(map)).ToList();
		}

		public class Key : IEquatable<Key>
		{
			[NotNull]
			public readonly Cell pivot;

			public readonly int rotation;

			public Key(Unit unit, int rotation)
			{
				pivot = unit.pivot;
				this.rotation = rotation;
			}

			public bool Equals(Key other)
			{
				if (ReferenceEquals(null, other))
					return false;
				if (ReferenceEquals(this, other))
					return true;
				return Equals(pivot, other.pivot) && rotation == other.rotation;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;
				if (ReferenceEquals(this, obj))
					return true;
				if (obj.GetType() != this.GetType())
					return false;
				return Equals((Key)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((pivot != null ? pivot.GetHashCode() : 0) * 397) ^ rotation;
				}
			}
		}

		public class Item
		{
			[NotNull]
			public Unit unit;

			[CanBeNull]
			public Way way;

			[NotNull]
			public bool[] spelledWords;

			public int score;
		}

		public class EndItem
		{
			[NotNull]
			public Item item;

			[NotNull]
			public List<MoveType> finalMoves;
		}
	}
}