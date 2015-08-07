using System.Collections.Generic;

namespace SomeSecretProject.Logic.Score
{
    public class UnitStep
    {
        public static UnitStep Null = new UnitStep(null, false, 0, new string[0]);

        public UnitStep(Unit item, bool isLocked, int clearedLinesAmount, IList<string> powerPhrases)
        {
            Item = item;
            ClearedLinesAmount = clearedLinesAmount;
            PowerPhrases = powerPhrases;
            IsLocked = isLocked;
        }

        public Unit Item { get; private set; }
        public bool IsLocked { get; private set; }
        public int ClearedLinesAmount { get; private set; }
        public IList<string> PowerPhrases { get; private set; }
    }
}