using System;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace Emulator
{
    class ConsoleGame : GameBase
    {
        public ConsoleGame(Problem problem, int seed)
            :base(problem,seed)
        {}

        protected override bool TryGetNextMove(out MoveType? moveType)
        {
            moveType = null;
            while (true)
            {
                var key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.A) moveType = MoveType.W;
                else if (key.Key == ConsoleKey.D) moveType = MoveType.E;
                else if (key.Key == ConsoleKey.Z) moveType = MoveType.SW;
                else if (key.Key == ConsoleKey.X) moveType = MoveType.SE;
                else if (key.Key == ConsoleKey.Q) moveType = MoveType.RotateCCW;
                else if (key.Key == ConsoleKey.E) moveType = MoveType.RotateCW;
                else if (key.Key == ConsoleKey.Escape) return false;
                if (moveType != null) return true;
            }
        }
    }
}
