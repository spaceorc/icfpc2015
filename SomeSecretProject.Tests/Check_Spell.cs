using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using SomeSecretProject.Algorithm;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Tests
{
	public class Check_Spell
	{
		[Test]
		public void Test()
		{
            string word = "blue hades";
			var problemnum = 2;
			int unitIndex = 2;
			int seedIndex = 5;

			var problem = ProblemsSet.GetProblem(problemnum);
			var originalUnit = problem.units[unitIndex];
			var prefix = new List<MoveType>();
			var spawnedUnit = Game.SpawnUnit(originalUnit, problem).Move(MoveType.SE).Move(MoveType.SW);
			var unit = spawnedUnit;
			var units = new List<Unit> { unit };
			
			foreach (var c in word)
			{
				var moveType = MoveTypeExt.Convert(c);
				unit = unit.Move(moveType.Value);
				units.Add(unit);
			}
			var minX = units.SelectMany(x => x.members).Min(t => t.x);
			while (minX < 0)
			{
				prefix.Add(MoveType.E);
				minX++;
			}
			while (minX > 0)
			{
				prefix.Add(MoveType.W);
				minX--;
			}
			prefix.Add(MoveType.SE);
			prefix.Add(MoveType.SW);

			var game = new SolverGame(problem, problem.sourceSeeds[seedIndex], new string[0]);
			game.Step();
			var danananananananananananananan = "danananananananananananananan";
			game.ApplyUnitSolution(danananananananananananananan);
			if (game.state != GameBase.State.End && game.state != GameBase.State.WaitUnit)
				throw new InvalidOperationException(string.Format("Invalid game state: {0}", game.state));
			game.Step();
			Assert.That(game.spawnedUnitIndex, Is.EqualTo(unitIndex));
			var staticPowerPhraseBuilder = new StaticPowerPhraseBuilder();
			var solution = staticPowerPhraseBuilder.Build(prefix) + word;
			Console.Out.WriteLine(danananananananananananananan + solution);
			game.ApplyUnitSolution(solution);

			var output = new Output
			{
				problemId = problemnum,
				seed = problem.sourceSeeds[seedIndex],
				solution = danananananananananananananan + solution
			};
		}

		public class SolverGame : GameBase
		{
			public SolverGame([NotNull] Problem problem, int seed, string[] magicSpells)
				: base(problem, seed, magicSpells)
			{
			}

			private char? nextMove;

			protected override bool TryGetNextMove(out char move)
			{
				if (nextMove.HasValue)
				{
					move = nextMove.Value;
					return true;
				}
				move = default(char);
				return false;
			}

			public void ApplyUnitSolution(string solution)
			{
				if (state != State.UnitInGame)
					throw new InvalidOperationException(string.Format("Invalid game state: {0}", state));
				if (nextMove.HasValue)
					throw new InvalidOperationException(string.Format("Next move is not null: {0}", nextMove));
				for (int i = 0; i < solution.Length; i++)
				{
					nextMove = solution[i];
					Step();
					if (i < solution.Length - 1 && state != State.UnitInGame)
						throw new InvalidOperationException(string.Format("Invalid game state: {0}", state));
				}
				nextMove = null;
			}
		}

	}
}