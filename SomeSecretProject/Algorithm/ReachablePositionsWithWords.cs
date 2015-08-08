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

		private readonly MoveType[] allowedMoves =
		{
			MoveType.E, MoveType.W,
			MoveType.SE, MoveType.SW,
			MoveType.RotateCW, MoveType.RotateCCW,
		};

		public ReachablePositionsWithWords(Map map, string[] powerPhrases)
		{
			this.map = map;
			this.powerPhrases = powerPhrases;
		}
		
		public IEnumerable<Tuple<Unit, IList<MoveType>>> SingleEndPositions(Unit currentUnit)
		{
			return EndPositions(currentUnit).GroupBy(t => t.Item1).Select(g => g.First());
		}

		public IList<Tuple<Unit, IList<MoveType>>> EndPositions(Unit currentUnit)
		{
			return AllPositions(currentUnit)
				.SelectMany(p => FinalMoves(p.Item1).Select(f => Tuple.Create(p.Item1, (IList<MoveType>)p.Item2.Concat(new[] { f }).ToList()))).ToList();
		}

		private List<MoveType> FinalMoves(Unit unit)
		{
			return allowedMoves.Where(m => !unit.Move(m).IsCorrect(map)).ToList();
		}

		public IList<Tuple<Unit, IList<MoveType>>> AllPositions(Unit currentUnit)
		{
			if (!currentUnit.IsCorrect(map))
			{
				return new Tuple<Unit, IList<MoveType>>[0];
			}
			var checker = new ForbiddenSequenceChecker(currentUnit);
			var visited = new Dictionary<Unit, List<MoveType>>
			{
				{ currentUnit, new List<MoveType>() },
			};
			var queue = new Queue<Unit>(new[] { currentUnit });
			while (queue.Any())
			{
				var unit = queue.Dequeue();
				foreach (var powerPhrase in powerPhrases)
				{
					var next = unit;
					bool invalid = false;
					var moves = visited[unit].ToList();
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
						queue.Enqueue(next);
						visited[next] = moves;
					}
				}
				foreach (var move in allowedMoves.Where(m => checker.CheckLastMove(visited[unit], m)))
				{
					var next = unit.Move(move);
					if (!next.IsCorrect(map) || visited.ContainsKey(next))
						continue;
					queue.Enqueue(next);
					visited[next] = visited[unit].Concat(new[] { move }).ToList();
				}
			}
			return visited.Keys.Select(u => Tuple.Create(u, (IList<MoveType>)visited[u])).ToList();
		}
	}
}