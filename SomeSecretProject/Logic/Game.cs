using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using SomeSecretProject.IO;
using SomeSecretProject.Logic.Score;

namespace SomeSecretProject.Logic
{
    public abstract class GameBase
	{
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

		public enum State
		{
			WaitUnit,
			UnitInGame,
			EndInvalidCommand,
			EndPositionRepeated,
			End
		}

		private readonly Problem problem;
        private readonly int seed;
		public Map map;
		public State state { get; private set; }
		public Unit[] units;
        private string[] knownMagicSpells;
        public Unit currentUnit;
        private int currentUnitIndex;
        private int currentMovesScore;
        private int currentSpellsScore;
        private int previouslyExplodedLines;
        private StringBuilder enteredString;
        private List<string> enteredMagicSpells;
        private LinearCongruentalGenerator randomGenerator;
        protected int step = 0;

        public int CurrentScore
        {
            get { return currentMovesScore + currentSpellsScore; }
        }

        public GameBase([NotNull] Problem problem, int seed)
        {
            this.problem = problem;
            ReStart();
        }

        public void ReStart()
        {
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
            units = problem.units;
            randomGenerator = new LinearCongruentalGenerator(problem.sourceSeeds[seed]);
            state = State.WaitUnit;
            step = 0;
            currentUnitIndex = 0;
		    currentMovesScore = 0;
		    currentSpellsScore = 0;
            previouslyExplodedLines = 0;
            enteredString = new StringBuilder();
            enteredMagicSpells = new List<string>();
            knownMagicSpells = new [] {"Ei!"}; //todo constructor+
        }

        public void StepBack(int backSteps)
        {
            var newstep = Math.Max(0, step - backSteps);
            ReStart();
            while(step <  newstep)
                Step();
        }

        /// <summary>
        /// return true if exist some next move, and false if EndOfSequence
        /// moveType == null if there are is invalid nextMove.
        /// </summary>
        protected abstract bool TryGetNextMove(out char move);

	    public void Step()
	    {
	        ++step;
			switch (state)
			{
				case State.WaitUnit:
					if (currentUnitIndex++ >= problem.sourceLength)
					{
						state = State.End;
						return;
					}
					var unitIndex = randomGenerator.GetNext() % units.Length;
					var spawnedUnit = SpawnUnit(units[unitIndex], problem);
					if (!spawnedUnit.CheckCorrectness(map))
					{
						state = State.End;
						return;
					}
					currentUnit = spawnedUnit;
					state = State.UnitInGame;
					return;
				case State.UnitInGame:
                    char move;
			        if (!TryGetNextMove(out move))
			        {
			            state = State.End;
			            return;
			        }
			        var moveType = MoveTypeExt.Convert(move);
			        if (moveType == null)
			        {
			            state = State.EndInvalidCommand;
			            return;
			        }
			        enteredString.Append(move);
                    ParseNewMagicSpells();
			        var movedUnit = currentUnit.Move(moveType.Value);
					if (!movedUnit.CheckCorrectness(map))
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

        private void ParseNewMagicSpells()
        {
            foreach (var magicSpell in knownMagicSpells)
            {
                if (enteredString.ToString().EndsWith(magicSpell)) //performance slow to use tostring
                    enteredMagicSpells.Add(magicSpell);
            }
            currentSpellsScore = ScoreCounter.GetPowerScores(enteredMagicSpells);
        }

		private void LockUnit([NotNull] Unit unit)
		{
			map.LockUnit(unit);
		    var currentlyExploded = map.RemoveLines();
		    currentMovesScore += ScoreCounter.GetMoveScore(unit, currentlyExploded, previouslyExplodedLines);
		    previouslyExplodedLines = currentlyExploded;
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

    public class Game : GameBase
    {
        private readonly Output output;
        private int currentCommand;

        public Game([NotNull] Problem problem, [NotNull] Output output)
            :base(problem, output.seed)
		{
			this.output = output;
            currentCommand = 0;
		}

        protected override bool TryGetNextMove(out char move)
        {
            move = (char) 0;
            while (currentCommand < output.solution.Length)
            {
                if (MoveTypeExt.IsIgnored(output.solution[currentCommand]))
                {
                    currentCommand++;
                    continue;
                }
                move = output.solution[currentCommand];
                return true;
            }
            return false;
        }

        
    }
}