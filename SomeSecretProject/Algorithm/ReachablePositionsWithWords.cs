using System;
using System.Collections.Generic;
using System.Linq;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public class ReachablePositionsWithWords
	{
		private readonly Map map;
		private readonly string[] powerPhrases;
		private readonly bool[] powerPhrasesSpelled;

		private readonly MoveType[] allowedMoves =
		{
			MoveType.E, MoveType.W,
			MoveType.SE, MoveType.SW,
			MoveType.RotateCW, MoveType.RotateCCW,
		};

		public ReachablePositionsWithWords(Map map, string[] powerPhrases, bool[] powerPhrasesSpelled)
		{
			this.map = map;
			this.powerPhrases = powerPhrases;
			this.powerPhrasesSpelled = powerPhrasesSpelled;
		}

		public IEnumerable<Tuple<Unit, VisitedInfo>> SingleEndPositions(Unit currentUnit)
		{
			return EndPositions(currentUnit).GroupBy(t => t.Item1).Select(g => g.First());
		}

		public IList<Tuple<Unit, VisitedInfo>> EndPositions(Unit currentUnit)
		{
			return AllPositions(currentUnit)
				.SelectMany(p => FinalMoves(p.Item1).Select(f => Tuple.Create(p.Item1, 
					new VisitedInfo
					{
						path = p.Item2.path.Concat(new[] { f }).ToList(),
						score = p.Item2.score,
						spelledWords = p.Item2.spelledWords
					}))).ToList();
		}

		private List<MoveType> FinalMoves(Unit unit)
		{
			return allowedMoves.Where(m => !unit.Move(m).IsCorrect(map)).ToList();
		}

		public class VisitedInfo
		{
			public List<MoveType> path;
			public bool[] spelledWords;
			public int score;
		}

		public IList<Tuple<Unit, VisitedInfo>> AllPositions(Unit currentUnit)
		{
			if (!currentUnit.IsCorrect(map))
			{
				return new Tuple<Unit, VisitedInfo>[0];
			}
			var checker = new ForbiddenSequenceChecker(currentUnit);
			var visited = new Dictionary<Unit, VisitedInfo>
			{
				{ currentUnit, new VisitedInfo
				{
					path = new List<MoveType>(),
					spelledWords = powerPhrasesSpelled
				} }
			};
			var queue = new Queue<Unit>(new[] { currentUnit });
			while (queue.Any())
			{
				var unit = queue.Dequeue();
				var currentInfo = visited[unit];
				for (int wordIndex = 0; wordIndex < powerPhrases.Length; wordIndex++)
				{
					var powerPhrase = powerPhrases[wordIndex];
					var next = unit;
					bool invalid = false;
					var moves = currentInfo.path.ToList();
					foreach (var c in powerPhrase)
					{
						var move = MoveTypeExt.Convert(c).Value;
						if (!checker.CheckLastMove(moves, move))
						{
							invalid = true;
							break;
						}
						next = next.Move(move);
						if (!next.IsCorrect(map) || visited.ContainsKey(next))
						{
							invalid = true;
							break;
						}
						moves.Add(move);
					}
					if (!invalid)
					{
						VisitedInfo prevInfo;
						var newScore = currentInfo.score;
						if (!currentInfo.spelledWords[wordIndex])
							newScore += 300;
						newScore += 2 * powerPhrase.Length;
						if (!visited.TryGetValue(next, out prevInfo) || prevInfo.score < newScore)
						{
							queue.Enqueue(next);
							var newSpelledWords = new bool[currentInfo.spelledWords.Length];
							Array.Copy(currentInfo.spelledWords, newSpelledWords, newSpelledWords.Length);
							newSpelledWords[wordIndex] = true;
							visited[next] = new VisitedInfo
							{
								path = moves,
								score = newScore,
								spelledWords = newSpelledWords
							};
						}
					}
				}
				foreach (var move in allowedMoves.Where(m => checker.CheckLastMove(currentInfo.path, m)))
				{
					var next = unit.Move(move);
					if (!next.IsCorrect(map))
						continue;
					VisitedInfo prevInfo;
					if (!visited.TryGetValue(next, out prevInfo) || prevInfo.score < currentInfo.score)
					{
						queue.Enqueue(next);
						visited[next] = new VisitedInfo
						{
							path = currentInfo.path.Concat(new[] { move }).ToList(),
							score = currentInfo.score,
							spelledWords = currentInfo.spelledWords
						};
					}
				}
			}
			return visited.Keys.Select(u => Tuple.Create(u, visited[u])).ToList();
		}
	}
}