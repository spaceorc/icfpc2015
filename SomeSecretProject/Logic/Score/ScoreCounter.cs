using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeSecretProject.Logic.Score
{
    public static class ScoreCounter
    {
        public static int Count(IList<UnitStep> steps)
        {
            return steps
                .Select((e, index) => new {CurrentStep = e, PreviousStep = index > 0 ? e : UnitStep.Null})
                .Aggregate(0,
                    (sum, info) =>
                        GetMoveScore(info.CurrentStep.Item, info.CurrentStep.IsLocked, info.CurrentStep.ClearedLinesAmount,
                            info.PreviousStep.ClearedLinesAmount) + GetPowerScores(info.CurrentStep.PowerPhrases));
        }

        private static int GetMoveScore(Unit unit, bool isLocked, int currentStepClearedLinesAmount, int previousStepClearedLinesAmount)
        {
            if (!isLocked)
            {
                return 0;
            }

            var size = unit.cells.Length;
            var ls = currentStepClearedLinesAmount;
            var ls_old = previousStepClearedLinesAmount;

            var points = size + 100*(1 + ls)*ls/2;
            var lineBonus = ls_old > 1 ? (int) Math.Floor((ls_old - 1)*points/10.0) : 0;

            return points + lineBonus;
        }

        private static int GetPowerScores(IList<string> powerPhrases)
        {
            return powerPhrases
                .GroupBy(e => e)
                .Select(e => new {PowerPhrase = e.Key, Amount = e.Count()})
                .Aggregate(0, (sum, info) => GetPowerScore(info.PowerPhrase, info.Amount));
        }

        private static int GetPowerScore(string powerPhrase, int reps)
        {
            var len = powerPhrase.Length;

            var powerBonus = reps > 0 ? 300 : 0;

            return 2 * len * reps + powerBonus;
        }
    }
}