using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public class SimplePowerPhraseBuilder : IPowerPhraseBuilder
	{
		public SimplePowerPhraseBuilder(string[] powerPhrases)
		{
			PowerPhrases = powerPhrases == null ? new string[0] : powerPhrases.Select(phrase => phrase.ToLower()).OrderByDescending(phrase => phrase.Length).ToArray();
		}

		public string Build(IList<MoveType> moves)
		{
			var cur = new PhrasePosition {Index = -1};

			var positions = FindOrderedPositions(moves).GetEnumerator();
			if(positions.MoveNext())
				cur = positions.Current;

			var builder = new StringBuilder();
			for(int i = 0; i < moves.Count;)
			{
				if(cur.Index == i)
				{
					builder.Append(cur.Phrase);
					i += cur.Length;
					if(positions.MoveNext())
						cur = positions.Current;
					continue;
				}
				var move = SelectMove(moves[i]);
				builder.Append(move.First());
				i++;
			}
			return builder.ToString();
		}

		private IEnumerable<PhrasePosition> FindOrderedPositions(IList<MoveType> moves)
		{
			var cur = new PhrasePosition {Index = -1, Length = 0};
			return
				PowerPhrases
					.SelectMany(phrase => FindAllPositions(moves, phrase))
					.Where(pos => pos.Index >= 0)
					.OrderBy(pos => pos.Index)
					.Where(pos =>
					{
						var result = !PhrasePosition.Intersected(cur, pos);
						if(result) cur = pos;
						return result;
					});
		}

		private IEnumerable<PhrasePosition> FindAllPositions(IList<MoveType> moves, string phrase)
		{
			int idx = -1;
			while((idx = IndexOf(moves, phrase, idx + 1)) >= 0)
				yield return new PhrasePosition {Index = idx, Length = phrase.Length, Phrase = phrase};
		}

		private int IndexOf(IList<MoveType> moves, string phrase, int idx = 0)
		{
			for(int i = idx; i <= moves.Count - phrase.Length; i++)
			{
				var found = true;
				for(int j = 0; j < phrase.Length; j++)
				{
					var c = phrase[j];
					var move = SelectMove(moves[i + j]);
					if(!move.Contains(c))
					{
						found = false;
						break;
					}
				}
				if(found)
					return i;
			}
			return -1;
		}

		private static HashSet<char> SelectMove(MoveType move)
		{
			switch(move)
			{
				case MoveType.E:
					return MoveE;
				case MoveType.W:
					return MoveW;
				case MoveType.SW:
					return MoveSW;
				case MoveType.SE:
					return MoveSE;
				case MoveType.RotateCW:
					return RotateCW;
				case MoveType.RotateCCW:
					return RotateCCW;
				default:
					throw new ArgumentOutOfRangeException("move", move, null);
			}
		}

		private struct PhrasePosition
		{
			public int Index;
			public int Length;
			public string Phrase;

			public static bool Intersected(PhrasePosition x, PhrasePosition y)
			{
				return y.Index >= x.Index && y.Index < x.Index + x.Length || x.Index >= y.Index && x.Index < y.Index + y.Length;
			}
		}

		private static readonly HashSet<char> MoveW = new HashSet<char>("p'!.03".ToCharArray());
		private static readonly HashSet<char> MoveE = new HashSet<char>("bcefy2".ToCharArray());
		private static readonly HashSet<char> MoveSW = new HashSet<char>("aghij4".ToCharArray());
		private static readonly HashSet<char> MoveSE = new HashSet<char>("lmno 5".ToCharArray());
		private static readonly HashSet<char> RotateCW = new HashSet<char>("dqrvz1".ToCharArray());
		private static readonly HashSet<char> RotateCCW = new HashSet<char>("kstuwx".ToCharArray());

		private readonly string[] PowerPhrases;
	}
}