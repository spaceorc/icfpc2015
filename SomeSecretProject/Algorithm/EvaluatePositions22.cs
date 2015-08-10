using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
    class EvaluatePositions22
    {
        private List<List<Cell>> emptyCellsInLines = new List<List<Cell>>();
        private List<int> countLockedCellsInLines = new List<int>(); 
        private Dictionary<Cell, int> freeInputsForCells = new Dictionary<Cell, int>(); 
        private Dictionary<Cell, double> usabilityOfCells = new Dictionary<Cell, double>(); 
        private Dictionary<Cell, Access> accessablityOfCell = new Dictionary<Cell, Access>();
        private Map map;

       public EvaluatePositions22(Map map)
        {
            this.map = map;
           this.maxLenPath = Math.Min(Math.Min(map.Width, map.Height)/3, 6);
            for (int y = 0; y < map.Height; ++y)
            {
                var emptyCellsInLine = new List<Cell>();
                for (int x = 0; x < map.Width; ++x)
                {
                    var cell = map[x, y];

                    if (!cell.filled)
                    {
                        emptyCellsInLine.Add(map[x, y]);
                        freeInputsForCells[map[x, y]] = CountFreeInputs(map[x, y]);
                        accessablityOfCell[cell] = new Access()
                        {
                            E = FindFreeLinearPath(cell, MoveType.E),
                            W = FindFreeLinearPath(cell, MoveType.W),
                            NE = FindFreeLinearPath(cell, MoveType.NE),
                            NW = FindFreeLinearPath(cell, MoveType.NW)
                        };
                    }
                }
                emptyCellsInLines.Add(emptyCellsInLine);
                int countLockedInLine = emptyCellsInLine.Count(c => freeInputsForCells[c] == 0);
                countLockedCellsInLines.Add(countLockedInLine);
                int countBadAccessedInLine = emptyCellsInLine.Count(c => accessablityOfCell[c].Accessability().Max() < maxLenPath);
                foreach (var c in emptyCellsInLine)
                {
                    usabilityOfCells[c] = 0.1 + 1.0/emptyCellsInLine.Count - ((double)countLockedInLine) / map.Width;
                   
                }
            }
           
            for (int y = 0; y < map.Height/3; ++y)
                for (int x = map.Width / 3; x <= map.Width * 3 / 4; ++x)
                    usabilityOfCells[new Cell() { x = x, y = y }] = -map.Height/4 /(1.0 + Math.Min(y, x));
            

        }

        private bool[] DroppedLines(Unit unit)
        {
            bool[] dropped = emptyCellsInLines.Select(c => false).ToArray();
            var rect = unit.GetSurroundingRectangle();
            for (int l = rect.Item1.y; l <= rect.Item2.y; ++l)
            {
                var emptyCellsInLine = emptyCellsInLines[l];
                if (emptyCellsInLine.Count > unit.members.Count) continue;
                if (emptyCellsInLine.TrueForAll(c => unit.members.Contains(c)))
                    dropped[l] = true;
            }
            return dropped;
        }

        private bool IsCorrect(Cell cell)
        {
            return cell.x >= 0 && cell.y >= 0 && cell.x < map.Width && cell.y < map.Height;
        }

        private int CountFreeInputs(Cell cell, Unit unit = null)
        {
            int nfree = 0;
            foreach (var move in up)
            {
                var c = cell.Move(move);
                if (!IsCorrect(c) || map[c].filled) continue;
                if (unit == null || !unit.members.Contains(c)) nfree++;
            }
            return nfree;
        }

        private int maxLenPath;
        private Cell[] FindFreeLinearPath(Cell cell, MoveType move)
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

        private struct Access
        {
            
            public Cell[] E, W, NE, NW;

            public int[] Accessability(Unit unit = null)
            {
                if (unit == null) return new []{E.Length, W.Length, NE.Length,NW.Length};
                else return new []{FirstInUnit(E, unit),FirstInUnit(W, unit), FirstInUnit(NE, unit), FirstInUnit(NW, unit)};
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

        private MoveType[] up = new[] { MoveType.E, MoveType.NE, MoveType.NW, MoveType.W, };
        private Cell[] GetFreeSurroundingCells(Unit unit)
        {
            return unit.GetSurroundingCells().Where(c => freeInputsForCells.ContainsKey(c)).ToArray();
        }

        private double GoodnessOfPosition(Cell c)
        {
            double yp = ((double)c.y) / map.Height;
            double xp = ((double)c.x) / map.Height;
            if (xp > 0.5) xp = 1.0 - xp;
            xp = 0.5 - xp;
	        return yp;
        }

        private double AccessabilityScore(Cell cell, Unit unit)
        {
            var access = accessablityOfCell[cell];
            return (access.Accessability(unit).Average() - access.Accessability().Average() + CountFreeInputs(cell, unit) - CountFreeInputs(cell))/2;
        }

        private bool IsLocked(Cell cell, Unit unit)
        {
            return CountFreeInputs(cell) == 0;
        }

        public double Evaluate(Unit unit)
        {
            // Уничтожаемые линии это хорошо
            
            
            var dropped = DroppedLines(unit);
            double scoreDropped = Math.Pow(dropped.Count(isDrop => isDrop)-1, 2) + dropped.Select((isDrop, i) => isDrop ? 1.0 +  ((double)i)/map.Height : 0.0).Sum();
            // Занимаем полезные клетки - это хорошо
            var usabilities = unit.members.Select(m => usabilityOfCells[m]).ToArray();
            double scoreOccupied = unit.members.Sum(m => usabilityOfCells[m]);
            // Ухудшаем возможность занять полезные клетки - это плохо
            var freeSurroundingCells = GetFreeSurroundingCells(unit);
            Dictionary<Cell, int> newFreeInputCells = freeSurroundingCells.ToDictionary(c => c, c => CountFreeInputs(c, unit));
            double scoreClosed = freeSurroundingCells.Select(c => AccessabilityScore(c, unit)*Math.Max(usabilityOfCells[c], 0)).Sum();
            // Некомпактность - слишком много свободных клеток вокруг - это плохо
            double scoreCompact = ((double)-freeSurroundingCells.Length)/unit.GetSurroundingCells().Length;
            // Чем ниже тем лучше
            double scorePosHeigh = unit.members.Average(m => GoodnessOfPosition(m));


            var score = 0.1*scoreDropped +
                        scoreOccupied +
                        0.5*scoreClosed +
                        0.2*scoreCompact +
                        0.1*scorePosHeigh+
                        0;
            return score;
        }



    }
}
