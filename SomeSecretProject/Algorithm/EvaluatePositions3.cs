using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
    //for linear accessability
    public class EvaluatePositions3
    {
        private struct Access
        {
            
            public Cell[] E, W, NE, NW;

            public int AccessabilityNENW(Unit unit = null)
            {
                if (unit == null) return Math.Max(NE.Length, NW.Length);
                return Math.Max(FirstInUnit(NE, unit), FirstInUnit(NW, unit));
            }

            public int AccessabilityEW(Unit unit = null)
            {
                if (unit == null) return Math.Max(E.Length, W.Length);
                return Math.Max(FirstInUnit(E, unit), FirstInUnit(W, unit));
            }

            

            private static int FirstInUnit(Cell[] cells, Unit unit)
            {
                int i = 0;
                foreach (var cell in cells)
                {
                    if (unit.members.Contains(cell)) return i;
                    ++i;
                }
                return i;
            }

            public override string ToString()
            {
                return String.Format("(NE: {0}, NW: {1}, E: {2}, W: {3})", NE.Length, NW.Length, E.Length, W.Length);
            }
        }


        private Dictionary<Cell, double> usabilityOfCell = new Dictionary<Cell, double>();
        private Cell[] cellsWithPositiveUsability;
        private Dictionary<Cell, Access> accessablityOfCell = new Dictionary<Cell, Access>();
        private Cell[][] emptyCells;
        private Map map;

        public EvaluatePositions3(Map map)
        {
            this.map = map;
            emptyCells = new Cell[map.Height][];
            for (int y = 0; y < map.Height; ++y)
            {
                var emptyCellsInLine = new List<Cell>();
                for (int x = 0; x < map.Width; ++x)
                    if (!map[x, y].filled)
                        emptyCellsInLine.Add(map[x, y]);
                        
                emptyCells[y] = emptyCellsInLine.ToArray();
                foreach (var cell in emptyCellsInLine)
                {
                    usabilityOfCell[cell] = 1.0 - ((double) emptyCellsInLine.Count - 1)/map.Width;
                    accessablityOfCell[cell] = new Access()
                    {
                        E = FindLinearPath(cell, MoveType.E),
                        W = FindLinearPath(cell, MoveType.W),
                        NE = FindLinearPath(cell, MoveType.NE),
                        NW = FindLinearPath(cell, MoveType.NW)
                    };
                }
            }
            var minvalue = 1.0 - 1.0/map.Width;
            for (int y=0; y <4; ++y)
                for (int x = map.Width/4; x <= map.Width*3/4; ++x)
                    usabilityOfCell[new Cell() {x = x, y = y}] = -1.0/(Math.Pow(x - map.Width/2, 2) + Math.Pow(y, 2) + 1);
            cellsWithPositiveUsability = usabilityOfCell.Keys.Where(c => usabilityOfCell[c] > 0).ToArray();
            // Полезность клеток возрастает если под ними тоже очень полезные клетки. 
            foreach (var cell in cellsWithPositiveUsability.OrderBy(c => c.y))
            {
                Cell under;
                under = cell.Move(MoveType.SW);
                if (IsCorrect(under) && !map[under].filled && usabilityOfCell[under] > 0.5)
                    usabilityOfCell[cell] += 0.2*usabilityOfCell[under];
                under = cell.Move(MoveType.SE);
                if (IsCorrect(under) && !map[under].filled && usabilityOfCell[under] > 0.5)
                    usabilityOfCell[cell] += 0.2 * usabilityOfCell[under];
            }
        }

       
        
        private int[] OccupiedByLines(IEnumerable<Cell> cells)
        {
            int[] occupied = new int[map.Height];
            foreach (var cell in cells)
                occupied[cell.y] ++;
            return occupied;
        }

        private bool IsCorrect(Cell cell)
        {
            return cell.x >= 0 && cell.y >= 0 && cell.x < map.Width && cell.y < map.Height;
        }

        private int maxLenPath = 6;
        private Cell[] FindLinearPath(Cell cell, MoveType move)
        {
            var next = cell.Move(move);
            List<Cell> path = new List<Cell>();
            while (IsCorrect(next) && !map[next].filled && path.Count < maxLenPath)
            {
                path.Add(next);
                next = next.Move(move);
            }
            return path.ToArray();
        }

        private double GoodnessOfPosition(Cell c)
        {
            double yp = ((double) c.y)/map.Height;
            double xp = ((double) c.x)/map.Height;
            if (xp > 0.5) xp = 1.0 - xp;
            xp = 0.5 - xp;
            return (yp + 2*xp)/2;
        }

        private Cell[] GetFreeSurroundingCells(Unit unit)
        {
            return unit.GetSurroundingCells().Where(c => usabilityOfCell.ContainsKey(c)).ToArray();
        }

        public double Evaluate(Unit unit)
        {
            var occupied = OccupiedByLines(unit.members);
            int maxIndLineDropped = 0;
            for (int l=0; l<occupied.Length; ++l)
                if (occupied[l] == emptyCells[l].Length)
                    maxIndLineDropped = l;
            // Оценка позиции - это сколько полезных клеток мы заняли + новая суммарная полезность всех клеток

            var scoreUsability = 0.0;
            foreach (var m in unit.members)
                scoreUsability += usabilityOfCell[m];

            double scoreDropped = 0;
            for (int i=0; i<occupied.Length; ++i)
                if (emptyCells[i].Length == occupied[i])
                    if (scoreDropped == 0) scoreDropped = 1.0 + ((double) i)/map.Height;
                    else scoreDropped *= 1.0 + ((double)i) / map.Height;
            
            var sumUsabilities = 0.0;
            int n = 0;
            foreach (var cell in GetFreeSurroundingCells(unit))
            {
                var access = accessablityOfCell[cell];
                var accessScore = ((double)access.AccessabilityNENW(unit))/maxLenPath;
                if (cell.y == maxIndLineDropped + 1) accessScore = 1.0;

                var currentScore = accessScore == 0 ? -10 : usabilityOfCell[cell]*accessScore;
                sumUsabilities += currentScore;
                ++n;
            }
            sumUsabilities = sumUsabilities/n;

            double scorePosition = unit.members.Average(m => GoodnessOfPosition(m));
           
            var score = 0.05*scoreDropped + scoreUsability + sumUsabilities + 0.05*scorePosition;
            return score;
        }

       
    }
}
