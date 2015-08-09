using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
    class MagicProblemSover_deep
    {
        private readonly IPowerPhraseBuilder staticPowerPhraseBuilder;
        private string[] powerPhrases;
        
		public MagicProblemSover_deep()
		{
			staticPowerPhraseBuilder = new StaticPowerPhraseBuilder();
            
		}

		public event Action<GameBase, string> SolutionAdded = (g, s) => { }; 
        private Unit[] units;
		
        private class State
        {
            public Map lastmap; // мапа перед применением lastUnit
            public Unit lastUnit; //  последний юнит в своем конечном состоянии
            public int lastDroopedLines; //количество линий удаляемых последним юнитом
            public int nunits; // количество юнитов которые уже применены (включая последний)
            public MoveType[] solution; // решение, получаемое последним юнитом
            public double score; // счет, получаемый последним юнитом
            public double wordscore; // cчет получаемый за слова
            public double sumPositionEstimates;
            public bool[] spelledPhrases; //все произнесенные фразы
        }

        private IEnumerable<State> FindNextStates(State state)
        {
            var map = state.lastmap.Clone();
            map.LockUnit(state.lastUnit);
            map.RemoveLines();

            var nextUnit = units[state.nunits];
            var reachablePositions = new ReachablePositionsWithWords(map, powerPhrases, state.spelledPhrases);
            var evaluatePositions = new EvaluatePositions2(map);
            var endPositions = reachablePositions.EndPositions(nextUnit);
            var estimated = new Dictionary<Unit, Tuple<double, int>>();
            Tuple<Unit, ReachablePositionsWithWords.VisitedInfo>[] bestPositions = endPositions.OrderByDescending(
                p => {
                    Tuple<double, int> value;
                         var unit = p.Item1;
                         if (!estimated.TryGetValue(unit, out value))
                             value = estimated[p.Item1] = evaluatePositions._Evaluate(p.Item1);
                         return value.Item1;
                     }).Take(100).ToArray();

            foreach (var position in bestPositions)
            {
                var unit = position.Item1;
                var ndropped = estimated[unit].Item2;
                var move_score = CountPoints(unit.members.Count, state.lastDroopedLines, estimated[unit].Item2);
                var score = state.score + move_score;
                yield return new State()
                {
                    lastmap = map,
                    lastUnit = unit,
                    nunits = state.nunits + 1,
                    lastDroopedLines = ndropped,
                    score = score,
                    solution = state.solution.Concat(position.Item2.path).ToArray(),
                    spelledPhrases = position.Item2.spelledWords,
                    sumPositionEstimates = state.sumPositionEstimates + 10 + estimated[unit].Item1
                };
            }
        }

        private int CountPoints(int unitSize, int prevDroopedLines, int nowDroppedLines)
        {
            var points = unitSize + 100*(1 + nowDroppedLines)*nowDroppedLines/2;
            if (prevDroopedLines > 1)
                points += (int) Math.Floor((double)(prevDroopedLines - 1)*points/10);
            return points;
        }

        /*
        public string Solve(Problem problem, int seed, string[] powerPhrases)
		{
            var randomGenerator = new LinearCongruentalGenerator(seed);
			units = new Unit[problem.sourceLength];
	        for (int i = 0; i < problem.sourceLength; i++)
	            units[i] = problem.units[randomGenerator.GetNext()%problem.units.Length];
	        
			var finalPowerPhraseBuilder = new SimplePowerPhraseBuilder(powerPhrases);
			var spelledPhrases = new bool[powerPhrases.Length];
			var solution = new List<MoveType>();
			
			while (true)
			{
				switch (game.state)
				{
					case GameBase.State.WaitUnit:
						game.Step();
						break;
					case GameBase.State.UnitInGame:
						var reachablePositions = new ReachablePositionsWithWords(game.map, powerPhrases, spelledPhrases);
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
						
						var wayToBestPosition = bestPosition.Item2;
						var unitSolution = staticPowerPhraseBuilder.Build(wayToBestPosition.path);
						SolutionAdded(game, unitSolution);
						game.ApplyUnitSolution(unitSolution);
						spelledPhrases = wayToBestPosition.spelledWords;
						solution.AddRange(wayToBestPosition.path);
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
         *  * */
    }
}
