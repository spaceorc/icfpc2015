using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public class WaterMagicProblemSolver : MuggleProblemSolver
	{
		private readonly int unitsToEvaluate;

		public WaterMagicProblemSolver(int unitsToEvaluate)
		{
			this.unitsToEvaluate = unitsToEvaluate;
		}

		private class QueueItem
		{
			public int unitIndex;
			public double parentEvaluation;

			[NotNull]
			public List<FastPositionsBase> fastPositions;

			[CanBeNull]
			public FastPositionsBase.EndItem solutionEndItem;

			[NotNull]
			public Map map;
		}

		public override string Solve(Problem problem, int seed, string[] powerPhrases)
		{
			var finalPowerPhraseBuilder = new SimplePowerPhraseBuilder(powerPhrases);
			var spelledPhrases = new bool[powerPhrases.Length];
			var solution = new List<MoveType>();
			var game = new SolverGame(problem, seed, powerPhrases);
			var unitsFastPositions = new List<FastPositions>();
			while (true)
			{
				switch (game.state)
				{
					case GameBase.State.WaitUnit:
						game.Step();
						break;

					case GameBase.State.UnitInGame:
						var units = new[] { game.currentUnit }.Concat(game.GetAllRestUnits()).ToArray();
						while (unitsFastPositions.Count < unitsToEvaluate)
						{
							var fastPositions = new FastPositions(game.map, units[unitsFastPositions.Count], powerPhrases, spelledPhrases);
							fastPositions.BuildAllPositions();
							unitsFastPositions.Add(fastPositions);
						}
						var evaluationQueue = new Queue<QueueItem>();
						evaluationQueue.Enqueue(new QueueItem
						{
							unitIndex = 0,
							map = game.map,
							parentEvaluation = 0,
							solutionEndItem = null,
							fastPositions = new List<FastPositionsBase> { unitsFastPositions[0] }
						});
						var bestEvaluation = double.MinValue;
						FastPositionsBase.EndItem bestSolution = null;
						List<FastPositionsBase> bestFastPositions = null;
						while (evaluationQueue.Any())
						{
							var item = evaluationQueue.Dequeue();
							var evaluatePositions = new EvaluatePositions2(item.map);
							foreach (var endPosition in item.fastPositions[item.unitIndex].endPositions)
							{
								var evaluation = item.parentEvaluation + evaluatePositions.Evaluate(endPosition.Value.item.unit);
								if (item.unitIndex >= unitsToEvaluate - 1)
								{
									var solutionCandidate = item.solutionEndItem ?? endPosition.Value;
									if (bestSolution == null
									    || evaluation > bestEvaluation
									    || (evaluation > bestEvaluation - 0.01 && solutionCandidate.item.score > bestSolution.item.score))
									{
										bestSolution = solutionCandidate;
										bestEvaluation = Math.Max(evaluation, bestEvaluation);
										bestFastPositions = item.fastPositions;
									}
								}
								else
								{
									var childMap = item.map.Clone();
									childMap.LockUnit(endPosition.Value.item.unit);
									childMap.RemoveLines();
									var newFastPositions = item.fastPositions.ToList();
									if (newFastPositions.Count != item.unitIndex + 1)
										throw new InvalidOperationException("newFastPositions.Count != item.unitIndex + 1");
									newFastPositions.Add(new FastPositionsPatch(childMap, unitsFastPositions[item.unitIndex + 1]));
									evaluationQueue.Enqueue(new QueueItem
									{
										map = childMap,
										unitIndex = item.unitIndex + 1,
										parentEvaluation = evaluation,
										solutionEndItem = item.solutionEndItem,
										fastPositions = newFastPositions
									});
								}
							}
						}

						if (bestSolution == null)
							throw new InvalidOperationException("bestSolution == null");

						if (bestFastPositions.Count != unitsFastPositions.Count)
							throw new InvalidOperationException("bestFastPositions.Count != unitsFastPositions.Count");
						for (int i = 0; i < bestFastPositions.Count; i++)
							unitsFastPositions[i] = bestFastPositions[i].Apply();

						var wayToBestPosition = bestSolution.item.way.Add(bestSolution.finalMoves[0]).ToList();
						var unitSolution = staticPowerPhraseBuilder.Build(wayToBestPosition);
						CallEvent(game, unitSolution);
						game.ApplyUnitSolution(unitSolution);
						solution.AddRange(wayToBestPosition);
						unitsFastPositions.RemoveAt(0);
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