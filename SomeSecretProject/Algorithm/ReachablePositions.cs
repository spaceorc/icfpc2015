using System;
using System.Collections.Generic;
using System.Linq;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public class ReachablePositions
	{
		private readonly Map map;

		private readonly MoveType[] allowedMoves =
		{
			MoveType.E, MoveType.W,
			MoveType.SE, MoveType.SW,
			MoveType.RotateCW, MoveType.RotateCCW,
		};

		public ReachablePositions(Map map)
		{
			this.map = map;
		}

		public IEnumerable<Tuple<Unit, IList<MoveType>>> SingleEndPositions(Unit currentUnit)
		{
			return EndPositions(currentUnit).GroupBy(t => t.Item1).Select(g => g.First());
		}

		public IEnumerable<Tuple<Unit, IList<MoveType>>> EndPositions(Unit currentUnit)
		{
			return AllPositions(currentUnit)
				.SelectMany(p => FinalMoves(p.Item1)
					.Select(f => Tuple.Create(p.Item1, (IList<MoveType>)p.Item2.Concat(new[] { f })
						.ToList())));
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