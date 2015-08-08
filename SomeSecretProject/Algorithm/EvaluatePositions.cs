using System.Collections.Generic;
using System.Linq;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	class EvaluatePositions
    {
        private List<List<Cell>> emptyCellsInLines = new List<List<Cell>>();
        private Map map;

        public EvaluatePositions(Map map)
        {
            this.map = map;
            for (int y = 0; y < map.Height; ++y)
            {
                var emptyCellsInLine = new List<Cell>();
                for (int x=0; x<map.Width; ++x)
                    if (!map[x, y].filled) 
                        emptyCellsInLine.Add(map[x,y]);
                
                emptyCellsInLines.Add(emptyCellsInLine);
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

        private MoveType[] up = new[] {MoveType.E, MoveType.NE, MoveType.NW, MoveType.W,};
        private int[] CountFreeInputsForSurroundingPoints(Unit unit)
        {
            List<int> countOfInputs = new List<int>();
            foreach (var cell in unit.GetSurroundingCells())
                if (cell.x < 0 || cell.y < 0 || cell.x >= map.Width || cell.y >= map.Height) continue;
                else if (map[cell].filled) continue;
                else
                {
                    int nfree = 0;
                    foreach (var move in up)
                    {
                        var c = cell.Move(move);
                        if (c.x < 0 || c.y < 0 || c.x >= map.Width || c.y >= map.Height) continue;
                        if (!map[c].filled && !unit.members.Contains(c)) nfree++;
                    }
                    countOfInputs.Add(nfree);
                }
            return countOfInputs.ToArray();
        }

        public double Evaluate(Unit unit)
        {
            var dropped = DroppedLines(unit);
            int[] freedomInts = CountFreeInputsForSurroundingPoints(unit);
            int[] byCountFree = new int[5];
            foreach (var nfree in freedomInts)
                byCountFree[nfree]++;
            int scoreDropped = dropped.Select((isDrop, i) => isDrop ? 100+10*i : 0).Sum();
			int scorePosHeigh = unit.GetSurroundingRectangle().Item1.y;
            var score = scoreDropped  + scorePosHeigh 
				//- 10*byCountFree[0] - 5*byCountFree[1] - 2*byCountFree[2] - 1*byCountFree[3]
				;
	        return score;
        }



    }
}
