using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeSecretProject.Logic.Score
{
    public static class ScoreCounter
    {
        public static int GetMoveScore(Unit unit, int currentStepClearedLinesAmount, int previousStepClearedLinesAmount)
        {
            var size = unit.members.Length;
            var ls = currentStepClearedLinesAmount;
            var ls_old = previousStepClearedLinesAmount;

            var points = size + 100*(1 + ls)*ls/2;
            var lineBonus = ls_old > 1 ? (int) Math.Floor((ls_old - 1)*points/10.0) : 0;

            return points + lineBonus;
        }

        public static int GetPowerScores(IList<string> powerPhrases)
        {
            return powerPhrases
                .GroupBy(e => e)
                .Select(e => new {PowerPhrase = e.Key, Amount = e.Count()})
                .Aggregate(0, (sum, info) => sum + GetPowerScore(info.PowerPhrase, info.Amount));
        }

        private static int GetPowerScore(string powerPhrase, int reps)
        {
            var len = powerPhrase.Length;

            var powerBonus = reps > 0 ? 300 : 0;

            return 2 * len * reps + powerBonus;
        }
    }
}