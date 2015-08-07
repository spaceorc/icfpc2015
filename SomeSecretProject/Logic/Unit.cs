using System;
using System.Linq;
using Newtonsoft.Json;

namespace SomeSecretProject.Logic
{
	[JsonObject]
	public class Unit
	{
		[JsonProperty("pivot")]
		public Cell pivot;

		[JsonProperty("members")]
		public Cell[] members;

		public Unit Move(MoveType move)
		{
			switch (move)
			{
				case MoveType.E:
				case MoveType.W:
				case MoveType.SW:
				case MoveType.SE:
					return new Unit { pivot = pivot.Move(new Vector(move, pivot.y)), members = members.Select(cell => cell.Move(new Vector(move, cell.y))).ToArray() };
				case MoveType.RotateCW:
					return new Unit { pivot = pivot, members = members.Select(cell => cell.RotateCW(pivot)).ToArray() };
				case MoveType.RotateCCW:
					return new Unit { pivot = pivot, members = members.Select(cell => cell.RotateCCW(pivot)).ToArray() };
				default:
					throw new ArgumentOutOfRangeException(move.ToString(), move, null);
			}
		}

		public bool CheckCorrectness(Map map)
		{
			return !members.Any(cell => cell.x < 0 || cell.y < 0 || cell.x >= map.Width || cell.y >= map.Height || map[cell.x, cell.y].filled);
		}
	}
}