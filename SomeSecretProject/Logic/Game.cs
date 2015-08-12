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
		public enum State
		{
			WaitUnit,
			UnitInGame,
			EndInvalidCommand,
			EndPositionRepeated,
			End
		}

	    public readonly Problem problem;
        protected readonly int seed;
        protected readonly string[] knownMagicSpells;
        public readonly int[] UnitIndeces;
        public readonly Unit[] units;
		public Map map;
		public State state { get; protected set; }
		public Unit currentUnit;
	    public int currentUnitIndex;
        public int currentMovesScore;
        protected int currentSpellsScore;
        public int previouslyExplodedLines;
        public StringBuilder enteredString;
        public List<string> EnteredMagicSpells;

        public int step = 0;
	    protected ForbiddenSequenceChecker forbiddenSequenceChecker;
        public List<MoveType> moves;
	    public int spawnedUnitIndex;
	    

	    public int CurrentScore
        {
            get { return currentMovesScore + currentSpellsScore; }
        }

        public GameBase([NotNull] Problem problem, int seed, string[] knownMagicSpells)
        {
            this.problem = problem;
            this.seed = seed;
            this.knownMagicSpells = knownMagicSpells.Select(s => s.ToLower()).ToArray();
            units = problem.units;
            UnitIndeces = new int[problem.sourceLength];
            var randomGenerator = new LinearCongruentalGenerator(seed);
            for (int i = 0; i < problem.sourceLength; i++)
                UnitIndeces[i] = randomGenerator.GetNext() % units.Length;
            ReStart();
        }

        public virtual void ReStart()
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
            			for (int i = 0; i < units.Length; i++)
			{
				units[i] = SpawnUnit(units[i], problem);
			}
			state = State.WaitUnit;
            step = 0;
	        currentUnit = null;
	        forbiddenSequenceChecker = null;
	        moves = null;
            currentUnitIndex = 0;
		    currentMovesScore = 0;
		    currentSpellsScore = 0;
            previouslyExplodedLines = 0;
            enteredString = new StringBuilder();
            EnteredMagicSpells = new List<string>();
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
					spawnedUnitIndex = UnitIndeces[currentUnitIndex - 1];
					var spawnedUnit = units[spawnedUnitIndex];
					if (!spawnedUnit.IsCorrect(map))
					{
						state = State.End;
						return;
					}
					currentUnit = spawnedUnit;
					forbiddenSequenceChecker = new ForbiddenSequenceChecker(currentUnit);
					moves = new List<MoveType>();
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
					if (!movedUnit.IsCorrect(map))
					{
						LockUnit(currentUnit);
						currentUnit = null;
						state = State.WaitUnit;
						return;
					}
					if (!forbiddenSequenceChecker.CheckLastMove(moves, moveType.Value))
					{
						state = State.EndPositionRepeated;
						return;
					}
					moves.Add(moveType.Value);
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
                    EnteredMagicSpells.Add(magicSpell);
            }
            currentSpellsScore = ScoreCounter.GetPowerScores(EnteredMagicSpells);
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

	    public IEnumerable<Unit> GetAllRestUnits()
	    {
			return Enumerable.Range(currentUnitIndex, problem.sourceLength - currentUnitIndex)
				.Select(i => units[UnitIndeces[i]]);
	    }
	}

    public class Game : GameBase
    {
        private readonly Output output;
        private int currentCommand;

		public Game([NotNull] Problem problem, [NotNull] Output output, string[] magicSpells)
            :base(problem, output.seed, magicSpells)
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
				currentCommand++;
                return true;
            }
            return false;
        }

        
    }
}