using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public class MuggleProblemSolver : IProblemSolver
	{
		private readonly IPowerPhraseBuilder powerPhraseBuilder;

		public MuggleProblemSolver()
		{
			powerPhraseBuilder = new RandomPowerPhraseBuilder(new Random(12345));
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
				if (state != State.End && state != State.WaitUnit)
					throw new InvalidOperationException(string.Format("Invalid game state: {0}", state));
			}
		}

		public string Solve(Problem problem, int seed, string[] magicSpells)
		{
			var solution = "";
            var game = new SolverGame(problem, seed, magicSpells);
			while (true)
			{
				switch (game.state)
				{
					case GameBase.State.WaitUnit:
						game.Step();
						break;
					case GameBase.State.UnitInGame:
						var reachablePositions = new ReachablePositions(game.map);
						var evaluatePositions = new EvaluatePositions(game.map);
						var endPositions = reachablePositions.EndPositions(game.currentUnit);
						var estimated = new Dictionary<Unit, double>();
						var bestPosition = endPositions.ArgMax(p =>
						{
							double value;
							if (estimated.TryGetValue(p.Item1, out value))
								return value;
							return estimated[p.Item1] = evaluatePositions.Evaluate(p.Item1);
						});
						var wayToBestPosition = bestPosition.Item2;
						var unitSolution = powerPhraseBuilder.Build(wayToBestPosition);
						game.ApplyUnitSolution(unitSolution);
						solution += unitSolution + "\r\n";
						break;
					case GameBase.State.EndInvalidCommand:
					case GameBase.State.EndPositionRepeated:
						throw new InvalidOperationException(string.Format("Invalid state: {0}", game.state));
					case GameBase.State.End:
						return solution;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}