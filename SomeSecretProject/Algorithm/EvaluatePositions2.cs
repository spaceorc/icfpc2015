using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
    class EvaluatePositions2
    {
        private List<List<Cell>> emptyCellsInLines = new List<List<Cell>>();
        private Dictionary<Cell, int> freeInputsForCells = new Dictionary<Cell, int>(); 
        private Dictionary<Cell, double> UsabilityForCells = new Dictionary<Cell, double>(); 
        private Map map;

       public EvaluatePositions2(Map map)
        {
            this.map = map;
            for (int y = 0; y < map.Height; ++y)
            {
                var emptyCellsInLine = new List<Cell>();
                for (int x = 0; x < map.Width; ++x)
                    if (!map[x, y].filled)
                    {
                        emptyCellsInLine.Add(map[x, y]);
                        freeInputsForCells[map[x, y]] = CountFreeInputs(map[x,y]);
                    }
                emptyCellsInLines.Add(emptyCellsInLine);
                int countLockedInLine = emptyCellsInLine.Count(c => freeInputsForCells[c] == 0);
                foreach (var c in emptyCellsInLine)
                    UsabilityForCells[c] = 0.1 + 1.0/emptyCellsInLine.Count - ((double)countLockedInLine)/map.Width;
            }
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

        private int CountFreeInputs(Cell cell, Unit unit = null)
        {
            int nfree = 0;
            foreach (var move in up)
            {
                var c = cell.Move(move);
                if (c.x < 0 || c.y < 0 || c.x >= map.Width || c.y >= map.Height) continue;
                if (!map[c].filled && (unit == null || !unit.members.Contains(c))) nfree++;
            }
            return nfree;
        }

        private MoveType[] up = new[] { MoveType.E, MoveType.NE, MoveType.NW, MoveType.W, };
        private Cell[] GetFreeSurroundingCells(Unit unit)
        {
            return unit.GetSurroundingCells().Where(c => freeInputsForCells.ContainsKey(c)).ToArray();
        }

        public double Evaluate(Unit unit)
        {
            return _Evaluate(unit).Item1;
        }

        public Tuple<double,int> _Evaluate(Unit unit)
        {
            // Уничтожаемые линии это хорошо
            var dropped = DroppedLines(unit);
            double scoreDropped = Math.Pow(dropped.Count(isDrop => isDrop)-1, 2) + dropped.Select((isDrop, i) => isDrop ? 1.0 +  ((double)i)/map.Height : 0.0).Sum();
            var droppedLines = dropped.Count(isDrop => isDrop);
            // Занимаем полезные клетки - это хорошо
            var usabilities = unit.members.Select(m => UsabilityForCells[m]).ToArray();
            double scoreOccupied = unit.members.Sum(m => UsabilityForCells[m]);
            // Ухудшаем возможность занять полезные клетки - это плохо
            var freeSurroundingCells = GetFreeSurroundingCells(unit);
            Dictionary<Cell, int> newFreeInputCells = freeSurroundingCells.ToDictionary(c => c, c => CountFreeInputs(c, unit));
            double scoreClosed = freeSurroundingCells.Select(c => (newFreeInputCells[c] - freeInputsForCells[c])*UsabilityForCells[c]).Sum();
            // Некомпактность - слишком много свободных клеток вокруг - это плохо
            double scoreCompact = ((double)-freeSurroundingCells.Length)/unit.GetSurroundingCells().Length;
            // Чем ниже тем лучше
            double scorePosHeigh = unit.members.Average(m =>((double)m.y)/map.Height);


            var score = 0.1*scoreDropped +
                        scoreOccupied +
                        0.5*scoreClosed +
                        0.2*scoreCompact +
                        0.1*scorePosHeigh+
                        0;
            return Tuple.Create(score, droppedLines);
        }



    }
}
