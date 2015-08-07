using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SomeSecretProject.IO;

namespace SomeSecretProject.Logic
{
	public class Game
	{
		private const string moveW = "p'!.03";
		private const string moveE = "bcefy2";
		private const string moveSW = "aghij4";
		private const string moveSE = "lmno 5";
		private const string rotateCW = "dqrvz1";
		private const string rotateCCW = "kstuwx";
		private const string ignored = "\t\n\r";

		private static readonly MoveType[][] forbiddenSequences =
		{
			new[] { MoveType.E, MoveType.W },
			new[] { MoveType.W, MoveType.E },
			new[] { MoveType.RotateCCW, MoveType.RotateCW },
			new[] { MoveType.RotateCW, MoveType.RotateCCW },
			new[] { MoveType.RotateCW, MoveType.RotateCW, MoveType.RotateCW, MoveType.RotateCW, MoveType.RotateCW, MoveType.RotateCW },
			new[] { MoveType.RotateCCW, MoveType.RotateCCW, MoveType.RotateCCW, MoveType.RotateCCW, MoveType.RotateCCW, MoveType.RotateCCW }
		};

		private readonly int[] forbiddenSequencePositions = {
			0, 0, 0, 0, 0, 0
		};

		private enum State
		{
			WaitUnit,
			UnitInGame,
			EndInvalidCommand,
			EndPositionRepeated,
			End
		}

		private readonly Problem problem;
		private readonly Output output;
		private int currentCommand;
	    public readonly Map map;
		private State state;
		private readonly List<Unit> units;
		private Unit currentUnit;
		private readonly LinearCongruentalGenerator randomGenerator;

		public Game([NotNull] Problem problem, [NotNull] Output output)
		{
			this.problem = problem;
			this.output = output;
			var filledCells = problem.filled.ToDictionary(cell => Tuple.Create<int, int>(cell.x, cell.y), cell => cell.Fill());
			map = new Map(problem.width, problem.height);
			for (int x = 0; x < problem.width; x++)
				for (int y = 0; y < problem.height; y++)
				{
					Cell cell;
					if (filledCells.TryGetValue(Tuple.Create(x, y), out cell))
						map[x, y] = cell;
					else
						map[x, y] = new Cell { x = x, y = y };
				}
			units = problem.units.ToList();
			randomGenerator = new LinearCongruentalGenerator(problem.sourceSeeds[output.seed]);
			state = State.WaitUnit;
			currentCommand = 0;
		}

		public void Step()
		{
			switch (state)
			{
				case State.WaitUnit:
					if (units.Count == 0)
					{
						state = State.End;
						return;
					}
					var unitIndex = randomGenerator.GetNext() % units.Count;
					currentUnit = TryPlaceUnit(SpawnUnit(units[unitIndex], problem));
					if (currentUnit == null)
					{
						state = State.End;
						return;
					}
					units.RemoveAt(unitIndex);
					state = State.UnitInGame;
					return;
				case State.UnitInGame:
					if (currentCommand >= output.solution.Length)
					{
						state = State.End;
						return;
					}
					MoveType moveType;
					if (moveW.IndexOf(output.solution[currentCommand]) >= 0)
						moveType = MoveType.W;
					else if (moveE.IndexOf(output.solution[currentCommand]) >= 0)
						moveType = MoveType.E;
					else if (moveSW.IndexOf(output.solution[currentCommand]) >= 0)
						moveType = MoveType.SW;
					else if (moveSE.IndexOf(output.solution[currentCommand]) >= 0)
						moveType = MoveType.SE;
					else if (rotateCW.IndexOf(output.solution[currentCommand]) >= 0)
						moveType = MoveType.RotateCW;
					else if (rotateCCW.IndexOf(output.solution[currentCommand]) >= 0)
						moveType = MoveType.RotateCCW;
					else if (ignored.IndexOf(output.solution[currentCommand]) >= 0)
					{
						currentCommand++;
						return;
					}
					else
					{
						state = State.EndInvalidCommand;
						return;
					}
					var movedUnit = TryPlaceUnit(currentUnit.Move(moveType));
					if (movedUnit == null)
					{
						LockUnit(currentUnit);
						state = State.WaitUnit;
						return;
					}
					for (int forbiddenIndex = 0; forbiddenIndex < forbiddenSequences.Length; forbiddenIndex++)
					{
						var forbiddenSequence = forbiddenSequences[forbiddenIndex];
						if (moveType != forbiddenSequence[forbiddenSequencePositions[forbiddenIndex]])
							forbiddenSequencePositions[forbiddenIndex] = 0;
					}
					for (int forbiddenIndex = 0; forbiddenIndex < forbiddenSequences.Length; forbiddenIndex++)
					{
						var forbiddenSequence = forbiddenSequences[forbiddenIndex];
						if (moveType == forbiddenSequence[forbiddenSequencePositions[forbiddenIndex]])
						{
							if (++forbiddenSequencePositions[forbiddenIndex] >= forbiddenSequence.Length)
							{
								state = State.EndPositionRepeated;
								return;
							}
						}
					}
					currentUnit = movedUnit;
					return;
				case State.EndInvalidCommand:
					return;
				case State.End:
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void LockUnit([NotNull] Unit unit)
		{
			foreach (var cell in unit.cells)
				map[cell.x, cell.y] = cell.Fill();
			// todo score!
		}

		[CanBeNull]
		private Unit TryPlaceUnit([NotNull] Unit unit)
		{
			throw new NotImplementedException();
		}

		[NotNull]
		public static Unit SpawnUnit([NotNull] Unit unit, [NotNull] Problem problem)
		{
		    var surr = unit.GetSurroundingRectangle();
		    var unitWidth = surr.Item2.x - surr.Item1.x + 1;
		    var leftShiftFromZero = ((problem.width - unitWidth) / 2);

		    var upped = unit.Move(MoveType.NW, surr.Item1.y);
            var uppedSurr = upped.GetSurroundingRectangle();
		    var leftShift = leftShiftFromZero - uppedSurr.Item1.x;

            return upped.Move(MoveType.E, leftShift);
        }
	}
}