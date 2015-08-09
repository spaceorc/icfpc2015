using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public class FastPositions : FastPositionsBase
	{
		private readonly Unit originalUnit;
		private readonly bool[] spelledWords;
		public readonly Dictionary<Key, Item> allPositions = new Dictionary<Key, Item>();
		
		public FastPositions([NotNull] Map map, [NotNull] Unit originalUnit, [NotNull] string[] powerPhrases, bool[] spelledWords)
			: base(map, originalUnit, powerPhrases)
		{
			this.originalUnit = originalUnit;
			this.spelledWords = spelledWords;
		}
		
		public FastPositions([NotNull] FastPositionsBase source)
			: base(source.map, source.originalUnit, source.powerPhrases)
		{
			allPositions = source.EnumerateAllPositions().ToDictionary(x => x.Key, x => x.Value);
			foreach (var endPosition in source.endPositions)
				endPositions.Add(endPosition.Key, endPosition.Value);
		}

		public override void InitQueue([NotNull] Queue<KeyValuePair<Key, Item>> queue)
		{
			if (originalUnit.IsCorrect(map))
				queue.Enqueue(UpdatePositionItem(new Key(originalUnit, 0), new Item
				{
					unit = originalUnit,
					spelledWords = spelledWords
				}));
		}

		public override bool TryGetVisited([NotNull] Key key, out Item result)
		{
			return allPositions.TryGetValue(key, out result);
		}

		[NotNull]
		public override FastPositions Apply()
		{
			return this;
		}

		protected override void DoUpdatePositionItem([NotNull] Key key, [NotNull] Item item)
		{
			allPositions[key] = item;
		}

		[NotNull]
		public override IEnumerable<KeyValuePair<Key, Item>> EnumerateAllPositions()
		{
			return allPositions;
		}
	}
}