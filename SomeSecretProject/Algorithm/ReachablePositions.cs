using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public class ReachablePositions
	{
		private readonly Map map;

		public ReachablePositions(Map map)
		{
			this.map = map;
		}

		public IList<Unit> AllPositions(Unit currentUnit)
		{
			var allowedMoves = new[]
				{
					MoveType.E, MoveType.W,
					MoveType.SE, MoveType.SW,
					MoveType.RotateCW, MoveType.RotateCCW,
				};
			if (!currentUnit.CheckCorrectness(map))
			{
				return new Unit[0];
			}
			var chacker = new ForbiddenSequenceChecker(currentUnit);
			var visited = new Dictionary<Unit, List<MoveType>>
			              {
				              {currentUnit, new List<MoveType>()},
			              };
			var queue = new Queue<Unit>(new [] {currentUnit});
			while (queue.Any())
			{
				var unit = queue.Dequeue();
				foreach (var move in allowedMoves.Where(m => chacker.CheckLastMove(visited[unit], m)))
				{
					var next = unit.Move(move);
					if (!next.CheckCorrectness(map) || visited.ContainsKey(next))
						continue;
					queue.Enqueue(next);
					visited[next] = visited[unit].Concat(new[] {move}).ToList();
				}
			}
			return visited.Keys.ToList();
		}
	}
}
