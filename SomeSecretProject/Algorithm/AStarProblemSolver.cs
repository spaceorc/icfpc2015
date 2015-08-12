using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;
using SomeSecretProject.Utils;

namespace SomeSecretProject.Algorithm
{

    
    public class AStar
    {

        /// <summary>
        /// Текущее состояние системы. 
        /// Два состояния системы одинаковы, если возможности построения (и стоимость) последующих путей не зависят от того как мы пришли в это состояние.
        /// </summary>
        public interface IState
        {
            /// <summary>
            /// Примерная итоговая стоимость пути. 
            /// g(state) - стоимость (длинна) пути который уже пройден до текущего состояния
            /// h(state) - эвристическая оценка расстояния от текущего состояния до конечного.
            /// Функция h(x) должна быть допустимой эвристической оценкой, то есть не должна переоценивать расстояния к целевой вершине. 
            /// 
            /// Общее расстояние f(s) = g(s) + h(s)
            /// f(s) - неубывающая, т.е. для всех дочерних узлов s' f(s') >= f(s)  
            /// т.е. мы уточняем стоимость пути, и реальная его длинна может становиться лишь больше, чем предполагаемая с каждым шагом
            /// 
            /// Два состояния - одинаковы, если возможность и стоимость дальнейших путей не зависят от того как мы попали в данное состояние.
            /// При необходимости воостановления пути, состояние должно хранить ссылку на своего предка
            /// </summary>
            double EstimatedCost { get; }

            bool IsFinal { get; }
        }

        public T Search<T>(T[] startStates, Func<T, T[]> GetNextStates)
            where T : class, IState
        {
            PriorityQueue<T> queue = new PriorityQueue<T>();
            Dictionary<T, bool> processed = new Dictionary<T, bool>();

            foreach (var state in startStates)
            {
                queue.Enqueue(-state.EstimatedCost, state);
                processed[state] = false;

            }

            T optimal = null;
            while (queue.Count > 0)
            {
                var stateWithMinimalCost = queue.Dequeue();
                if (stateWithMinimalCost.IsFinal)
                {
                    if (optimal == null || stateWithMinimalCost.EstimatedCost < optimal.EstimatedCost)
                        optimal = stateWithMinimalCost;
                    continue;
                }
                if (processed[stateWithMinimalCost])
                    continue; // такое же состояние но с меньшей оценкой уже обрабатывали ранее
                var nextStates = GetNextStates(stateWithMinimalCost);
                bool isProcessed;
                foreach (var nextState in nextStates)
                    if (processed.TryGetValue(nextState, out isProcessed) && isProcessed)
                        continue; // такое состояние уже обрабатывали ранее (и оно очевидно было с меньшей оценкой)
                    else
                    {
                        if (nextState.EstimatedCost < stateWithMinimalCost.EstimatedCost)
                            throw new Exception("Next cost estimation is lower then previous!");
                        queue.Enqueue(-nextState.EstimatedCost, nextState);
                        processed[nextState] = false;
                    }
            }
            return optimal;
        }
    }

    public class AStarProblemSolver : IProblemSolver
    {
        private readonly IPowerPhraseBuilder staticPowerPhraseBuilder;

        public AStarProblemSolver()
		{
			staticPowerPhraseBuilder = new StaticPowerPhraseBuilder();
		}

        public string Solve(Problem problem, int seed, string[] powerPhrases)
        {
            this.powerPhrases = powerPhrases;
            var spelledPhrases = new bool[powerPhrases.Length];
            var game = new MagicProblemSolver.SolverGame(problem, seed, powerPhrases);
            var state = new State()
            {
                game = game,
                futurescore = EvaluatePossibleScore(game),
                positionscore = 0,
                score = 0,
                spelledPhrases = spelledPhrases
            };
            AStar astar = new AStar();
            var best = astar.Search<State>(new State[] {state}, GetNextStates);
            return best.game.enteredString.ToString();
        }

        private class State : AStar.IState
        {
            public MagicProblemSolver.SolverGame game;
            public bool[] spelledPhrases;
            public double score, futurescore, positionscore;
            public double EstimatedScore { get { return score + futurescore + positionscore; } }
            public double EstimatedCost { get { return -EstimatedScore; } }
            public bool IsFinal { get{return game.state != GameBase.State.UnitInGame && game.state != GameBase.State.WaitUnit;} }
        }

        private string[] powerPhrases;
        private State[] GetNextStates(State state)
        {
            var game = state.game;
            List<State> nextStates = new List<State>();
            while (nextStates.Count == 0)
            {
                switch (game.state)
                {
                    case GameBase.State.WaitUnit:
                        game.Step();
                        continue;
                    case GameBase.State.UnitInGame:
                        var reachablePositions = new ReachablePositionsWithWords(game.map, powerPhrases, state.spelledPhrases);
                        var evaluatePositions = new EvaluatePositions2(game.map);
                        var endPositions = reachablePositions.EndPositions(game.currentUnit);
                        var estimated = new Dictionary<Unit, double>();
                        var bestPositions = endPositions.OrderByDescending(p =>
                        {
                            double value;
                            if (estimated.TryGetValue(p.Item1, out value))
                                return value;
                            return estimated[p.Item1] = evaluatePositions.Evaluate(p.Item1);
                        }).Take(3);
                        foreach (var position in bestPositions)
                        {
                            var wayToBestPosition = position.Item2;
                            var unitSolution = staticPowerPhraseBuilder.Build(wayToBestPosition.path);
                            var copyOfGame = game.Clone();
                            copyOfGame.ApplyUnitSolution(unitSolution);
                            var nextstate = new State()
                            {
                                game = copyOfGame,
                                spelledPhrases = wayToBestPosition.spelledWords,
                                score = copyOfGame.currentMovesScore,
                                futurescore = EvaluatePossibleScore(copyOfGame),
                                positionscore = state.positionscore -1 + (estimated[position.Item1] + 10)/100,
                            };
                            nextStates.Add(nextstate);
                         }
                        break;
                    case GameBase.State.EndInvalidCommand:
                    case GameBase.State.EndPositionRepeated:
                        throw new InvalidOperationException(string.Format("Invalid state: {0}", game.state));
                    case GameBase.State.End:
                        nextStates.Add(new State()
                        {
                            game = state.game,
                            spelledPhrases = state.spelledPhrases,
                            score = game.CurrentScore,
                            futurescore = 0,
                            positionscore = 0,
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return nextStates.ToArray();
        }

        private double EvaluatePossibleScore(GameBase game)
        {
            // TODO: Если здесь удастся написать хоть сколько-нибудь адекватную оценку количества очеков, которое возможно набрать до конца игры, то может быть AStar взлетит.
            // но пока нет.

            int filledCells = 0;
            for (int x=0; x<game.map.Width; ++x)
                for (int y=0; y<game.map.Height; ++y)
                    if (game.map[x, y].filled) filledCells++;

            // Оценим, сколько мы можем сжечь линий
            List<LineState> lines = new List<LineState>();
            int lineToDrop = 0;
            for (int y=0; y < game.map.Height; ++y)
            {
                var state = GetStateOfLine(game.map, y);
                lines.Add(state);
                if (!state.IsBlockedLine() && state.emptyCells <= lines[lineToDrop].emptyCells)
                    lineToDrop = y;
                if (state.IsBlockingLine(game.map.Width))
                    break;
            }

            int step = game.currentUnitIndex;
            List<int> droppedLines = new List<int>();
            int cellsAdded = 0, cellsDropped = 0;
            for (; step < game.UnitIndeces.Length; ++step)
            {
                var unit = game.units[game.UnitIndeces[step]];
                cellsAdded += unit.members.Count;
                if (cellsAdded >= lines[lineToDrop].emptyCells)
                {
                    droppedLines.Add(lineToDrop);
                    cellsDropped += lines[lineToDrop].emptyCells;
                    for (int shiftLine = 1; shiftLine <= 2; ++shiftLine)
                        
                        if (lineToDrop + shiftLine >= lines.Count)break;
                        else if (lines[lineToDrop + shiftLine].IsBlockedLine()) continue;
                        else if (lines[lineToDrop+shiftLine].CanBeRemoved(game.map.Width, cellsAdded-cellsDropped))
                        {
                             droppedLines.Add(lineToDrop+shiftLine);
                             cellsDropped += lines[lineToDrop+shiftLine].emptyCells;
                        }
                    for (int shiftLine = -1; shiftLine >= -2; --shiftLine)
                        if (lineToDrop + shiftLine < 0 ) break;
                        else if (lines[lineToDrop + shiftLine].IsBlockedLine()) continue;
                        else if (lines[lineToDrop + shiftLine].CanBeRemoved(game.map.Width, cellsAdded-cellsDropped))
                        {
                             droppedLines.Add(lineToDrop+shiftLine);
                             cellsDropped += lines[lineToDrop + shiftLine].emptyCells;
                        }
                    ++step;
                    break;
                }
            }

            var scores = 100 * (1 + droppedLines.Count) * droppedLines.Count / 2;
            if (step == game.currentUnitIndex + 1 && game.previouslyExplodedLines > 1)
                scores += scores * (game.previouslyExplodedLines - 1) / 10;

            // cуммарное количество пустых клеток в непустых линиях.
            int sumEmptyCells = 0, nLinesWithEmptyCells = 0, nBlockedLines = 0;
            for (int i=0; i<lines.Count;++i)
                if (droppedLines.Contains(i) || lines[i].emptyCells == game.map.Width) continue;
                else
                {
                    sumEmptyCells += lines[i].emptyCells;
                    nLinesWithEmptyCells ++;
                    if (lines[i].IsBlockedLine()) nBlockedLines++;
                }

            for (; step < game.UnitIndeces.Length; ++step)
            {
                var unit = game.units[game.UnitIndeces[step]];
                cellsAdded += unit.members.Count;
            }
            int nlinesDropped;
            if (cellsAdded - cellsDropped > sumEmptyCells)
                nlinesDropped = nLinesWithEmptyCells + (cellsAdded - cellsDropped - sumEmptyCells)/game.map.Width;
            else
                nlinesDropped = (cellsAdded - cellsDropped)/(sumEmptyCells/nLinesWithEmptyCells);
            
            scores += 100*nlinesDropped;
            scores += cellsAdded;

            return scores;
        }

        private static MoveType[] up = new[] { MoveType.E, MoveType.NE, MoveType.NW, MoveType.W, };
        private static MoveType[] down = new[]{MoveType.SW, MoveType.SE};

        private class LineState
        {
            public int blockedCells, blockingCells, emptyCells;
            public bool IsBlockingLine(int width) {return blockedCells + blockingCells == width;}
            public bool IsBlockedLine() {return blockedCells > 0;}
            public bool CanBeRemoved(int width, int countCellsAdded) {return !IsBlockedLine() && emptyCells <= countCellsAdded;}
        }

        private LineState GetStateOfLine(Map map, int y)
        {
            int blocked=0, blocking=0, empty = 0;
            for (int x = 0; x < map.Width; ++x)
            {
                if (IsBlockedCell(map, map[x, y])) blocked++;
                else if (IsBlockingCell(map, map[x, y])) blocking++;
                if (!map[x, y].filled) empty++;
            }
            return new LineState() {blockedCells = blocked, blockingCells = blocking, emptyCells = empty};
        }
        // Заблокированная ячейка (нет подхода сверху)
        private bool IsBlockedCell(Map map, Cell cell)
        {
            foreach (var move in up)
            {
                var c = cell.Move(move);
                if (c.x < 0 || c.y < 0 || c.x >= map.Width || c.y >= map.Height) continue;
                if (!map[c].filled) return false;
            }
            return true;
        }

        // Блокирующая ячейка (через нее нельзя пройти к нижним рядам, если они есть)
        private bool IsBlockingCell(Map map, Cell cell)
        {
            if (cell.filled) return true;
            foreach (var move in down)
            {
                var c = cell.Move(move);
                if (c.x < 0 || c.y < 0 || c.x >= map.Width || c.y >= map.Height) continue;
                if (!map[c].filled) return false;
            }
            return false;
        }
    }
}
