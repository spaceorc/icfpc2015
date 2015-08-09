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
        private readonly int seed;
		public Map map;
		public State state { get; private set; }
		public Unit[] units;
        private readonly string[] knownMagicSpells;
        public Unit currentUnit;
	    public int currentUnitIndex;
        private int currentMovesScore;
        private int currentSpellsScore;
        private int previouslyExplodedLines;
        public StringBuilder enteredString;
        public List<string> EnteredMagicSpells;
        private LinearCongruentalGenerator randomGenerator;
        protected int step = 0;
	    private ForbiddenSequenceChecker forbiddenSequenceChecker;
	    private List<MoveType> moves;
	    public int spawnedUnitIndex;
	    public int[] UnitIndeces;

	    public int CurrentScore
        {
            get { return currentMovesScore + currentSpellsScore; }
        }

        public GameBase([NotNull] Problem problem, int seed, string[] knownMagicSpells)
        {
            this.problem = problem;
            this.seed = seed;
            this.knownMagicSpells = knownMagicSpells.Select(s => s.ToLower()).ToArray();
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
            units = problem.units;
			for (int i = 0; i < units.Length; i++)
			{
				units[i] = SpawnUnit(units[i], problem);
			}
			randomGenerator = new LinearCongruentalGenerator(seed);
			UnitIndeces = new int[problem.sourceLength];
	        for (int i = 0; i < problem.sourceLength; i++)
	        {
		        UnitIndeces[i] = randomGenerator.GetNext()%units.Length;
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
			yo:
					if (!TryGetNextMove(out move))
			        {
			            state = State.End;
			            return;
			        }
					var moveType = move == '#' ? MoveType.NW : move == '$' ?  MoveType.NE : MoveTypeExt.Convert(move);
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
						Console.Beep();
						if (Console.ReadKey(true).Key != ConsoleKey.Enter)
							goto yo;
						LockUnit(currentUnit);
						currentUnit = null;
						state = State.WaitUnit;
						return;
					}
//					if (!forbiddenSequenceChecker.CheckLastMove(moves, moveType.Value))
//					{
//						state = State.EndPositionRepeated;
//						return;
//					}
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