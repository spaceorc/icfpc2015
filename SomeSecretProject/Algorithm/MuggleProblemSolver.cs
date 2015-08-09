using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public class MuggleProblemSolver : IProblemSolver
	{
		protected readonly IPowerPhraseBuilder staticPowerPhraseBuilder;
		
		public MuggleProblemSolver()
		{
			staticPowerPhraseBuilder = new StaticPowerPhraseBuilder();
		}

		public event Action<GameBase, string> SolutionAdded = (g, s) => { }; 

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

		protected void CallEvent(GameBase game, string unitSolution)
		{
			SolutionAdded(game, unitSolution);
		}

		public virtual string Solve(Problem problem, int seed, string[] powerPhrases)
		{
			var finalPowerPhraseBuilder = new SimplePowerPhraseBuilder(powerPhrases);
			var solution = new List<MoveType>();
            var game = new SolverGame(problem, seed, powerPhrases);
			while (true)
			{
				switch (game.state)
				{
					case GameBase.State.WaitUnit:
						game.Step();
						break;
					case GameBase.State.UnitInGame:
						var reachablePositions = new ReachablePositions(game.map);
						var evaluatePositions = new EvaluatePositions2(game.map);
						var endPositions = reachablePositions.EndPositions(game.currentUnit);
						var estimated = new Dictionary<Unit, double>();
						var bestPosition = endPositions.ArgMax(p =>
						{
							double value;
							if (estimated.TryGetValue(p.Item1, out value))
								return value;
							return estimated[p.Item1] = evaluatePositions.Evaluate(p.Item1);
						});
                        var score = evaluatePositions.Evaluate(bestPosition.Item1);
						var wayToBestPosition = bestPosition.Item2;
						var unitSolution = staticPowerPhraseBuilder.Build(wayToBestPosition);
						CallEvent(game, unitSolution);
						game.ApplyUnitSolution(unitSolution);
						solution.AddRange(wayToBestPosition);
						break;
					case GameBase.State.EndInvalidCommand:
					case GameBase.State.EndPositionRepeated:
						throw new InvalidOperationException(string.Format("Invalid state: {0}", game.state));
					case GameBase.State.End:
						return finalPowerPhraseBuilder.Build(solution);
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}