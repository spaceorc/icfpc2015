using System;
using System.Collections.Generic;
using System.Linq;
using SomeSecretProject.Logic;
using SomeSecretProject.Utils;

namespace SomeSecretProject.Algorithm
{
	public class DynPowerPhraseBuilder : IPowerPhraseBuilder
	{
		public DynPowerPhraseBuilder(string[] powerPhrases)
		{
			PowerPhrases = powerPhrases == null ? new string[0] : powerPhrases.Select(phrase => phrase.ToLower()).OrderByDescending(phrase => phrase.Length).ToArray();
		}

		public string Build(IList<MoveType> moves)
		{
			int score;
			return Build(moves, out score);
		}

		public string Build(IList<MoveType> moves, out int score)
		{
			if(moves.Count == 0)
			{
				score = 0;
				return string.Empty;
			}

			var scores = new Score[moves.Count];
			var phrases = new string[moves.Count];
			for(int idx = 0; idx < moves.Count; idx++)
			{
				var items = PowerPhrases
					.Where(phrase => EndsWithPhrase(moves, phrase, idx))
					.Select(phrase => new {Phrase = phrase, Score = idx < phrase.Length ? new Score() : scores[idx - phrase.Length]});

				if(idx > 0)
					items = items.Concat(new[] {new {Phrase = (string)null, Score = scores[idx - 1]}});

				var item = items
					.MaxOrDefault(elem => elem.Score.CountNewScore(elem.Phrase));

				scores[idx] = item == null ? (idx == 0 ? new Score() : scores[idx - 1]) : item.Score.GetUpdatedScore(item.Phrase);
				phrases[idx] = item == null ? null : item.Phrase;
			}

			score = scores[moves.Count - 1].TotalScore;

			var result = new string[moves.Count];
			for(int idx = moves.Count - 1; idx >= 0;)
			{
				var phrase = phrases[idx];
				if(phrase != null)
				{
					result[idx] = phrase;
					idx -= phrase.Length;
					continue;
				}
				result[idx] = SelectMove(moves[idx]).First().ToString();
				idx--;
			}

			return string.Join(null, result);
		}

		private bool EndsWithPhrase(IList<MoveType> moves, string phrase, int idx)
		{
			var lastCharPos = phrase.Length - 1;
			if(idx < lastCharPos)
				return false;
			for(int i = idx; i >= idx - lastCharPos; i--)
			{
				var move = SelectMove(moves[i]);
				if(!move.Contains(phrase[lastCharPos - (idx - i)]))
					return false;
			}
			return true;
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

		private static readonly HashSet<char> MoveW = new HashSet<char>("p'!.03".ToCharArray());
		private static readonly HashSet<char> MoveE = new HashSet<char>("bcefy2".ToCharArray());
		private static readonly HashSet<char> MoveSW = new HashSet<char>("aghij4".ToCharArray());
		private static readonly HashSet<char> MoveSE = new HashSet<char>("lmno 5".ToCharArray());
		private static readonly HashSet<char> RotateCW = new HashSet<char>("dqrvz1".ToCharArray());
		private static readonly HashSet<char> RotateCCW = new HashSet<char>("kstuwx".ToCharArray());

		private readonly string[] PowerPhrases;
	}

	public class Score
	{
		public int CountNewScore(string phrase)
		{
			return phrase == null ? TotalScore : TotalScore + (PhraseCounts != null && PhraseCounts.ContainsKey(phrase) ? 0 : 300) + 2 * phrase.Length;
		}

		public Score GetUpdatedScore(string phrase)
		{
			var score = new Score
			{
				PhraseCounts = PhraseCounts == null ? new Dictionary<string, int>() : new Dictionary<string, int>(PhraseCounts),
				TotalScore = CountNewScore(phrase)
			};

			if(phrase != null)
			{
				if(!score.PhraseCounts.ContainsKey(phrase))
					score.PhraseCounts[phrase] = 0;
				score.PhraseCounts[phrase]++;
			}

			return score;
		}

		public int TotalScore;

		private Dictionary<string, int> PhraseCounts;
	}
}