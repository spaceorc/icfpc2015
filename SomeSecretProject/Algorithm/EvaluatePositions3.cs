using System;
using System.Collections.Generic;
using System.Linq;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
    //for linear accessability
    public class EvaluatePositions3
    {
        private Cell[][] emptyCells;
        private Dictionary<Cell,Cell[]> inputsOfCells;
        private int[] blockedCells;
        private Map map;

        public EvaluatePositions3(Map map)
        {
            this.map = map;
            emptyCells = new Cell[map.Height][];
            blockedCells = new int[map.Height];
            inputsOfCells = new Dictionary<Cell, Cell[]>();

            for (int y = 0; y < map.Height; ++y)
            {
                var emptyCellsInLine = new List<Cell>();
                for (int x = 0; x < map.Width; ++x)
                    if (!map[x, y].filled)
                    {
                        emptyCellsInLine.Add(map[x, y]);
                        inputsOfCells[map[x, y]] = FreeInputs(map[x, y]).ToArray();
                    }

                emptyCells[y] = emptyCellsInLine.ToArray();
                blockedCells[y] = emptyCellsInLine.Count(c => CountFreeInputs(c) == 0);
            }
        }

        private MoveType[] up = new[] {MoveType.NE, MoveType.NW};
        private int CountFreeInputs(Cell cell, Unit unit = null)
        {
            if (unit == null) return inputsOfCells[cell].Length;
            int nfree = 0;
            foreach (var c in inputsOfCells[cell])
                if (unit.members.Contains(c)) continue;
                else nfree++;
            return nfree;
        }

        private IEnumerable<Cell> FreeInputs(Cell cell)
        {
            foreach (var move in up)
            {
                var c = cell.Move(move);
                if (c.x < 0 || c.y < 0 || c.x >= map.Width || c.y >= map.Height) continue;
                if (!map[c].filled) yield return c;
            }
        }

        private bool IsCorrect(Cell cell)
        {
            return cell.x >= 0 && cell.y >= 0 && cell.x < map.Width && cell.y < map.Height;
        }

        public double Evaluate(Unit unit)
        {
            //Ценность занимаемых ячеек
            //Чем меньше остается пустых ячеек в строке - тем лучше
            //Чем меньше в данной стороке "глухих ячек" - тем лучше
            int score = 0;
            int nLinesDropped = 0;
            int[] occupied = new int[map.Height];
            foreach (var occupiedCellsInLine in unit.members.GroupBy(m => m.y))
            {
                var y = occupiedCellsInLine.Key;
                occupied[y] = occupiedCellsInLine.Count();
                var newEmptyCells = emptyCells[y].Except(occupiedCellsInLine);
                int nEmpty = 0, nBlocked = 0;
                foreach (var c in newEmptyCells)
                    if (CountFreeInputs(c, unit) == 0) nBlocked++;
                    else nEmpty++;
                int lineScore = (int) (map.Width - nEmpty - nBlocked)*occupied[y];
                if (nEmpty + nBlocked == 0) nLinesDropped++;
                if (lineScore > 0) score += lineScore;
            }
            score += map.Width*nLinesDropped*(nLinesDropped+1);

            // Насколько хорошая позиция юнита
            // Если рядом стена или заполненная ячейка - отлично
            // Если рядом пустая ячейка, то ее ценность - сколько у нее входов
            int surroundingScore = 0;
            foreach (var surroundingCellsInLine in unit.GetSurroundingCells().GroupBy(c => c.y))
            {
                foreach (var cell in surroundingCellsInLine)
                {
               
                var cellScore = 0;
                if (!IsCorrect(cell) || map[cell].filled) cellScore= 10;
                else cellScore =  4*CountFreeInputs(cell, unit);
                    if (cellScore == 0) cellScore= -(map.Width - emptyCells[cell.y].Length + occupied[cell.y] + (map.Width - emptyCells[cell.y].Length - blockedCells[cell.y] + occupied[cell.y] > map.Width*3/4 ? 10 : 0));
                surroundingScore += cellScore;
                         
                }
            }

            // Не находится ли юнит рядом с входом
            int posScore = 0;
            if (unit.members.Any(cell =>
                (Math.Abs(cell.x - map.Width/2) < 3 && cell.y < 3)))
                if (nLinesDropped == 0) posScore = -2*map.Width;

            score = score + surroundingScore + posScore;
            return score;
        }

       
    }
}
