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

        protected override bool TryGetNextMove(out char move)
        {
            move = (char) 0;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.A) move = 'p'; //todo all keys may be?
                else if (key.Key == ConsoleKey.D) move = 'b';
                else if (key.Key == ConsoleKey.Z) move = 'a';
                else if (key.Key == ConsoleKey.X) move = 'l';
                else if (key.Key == ConsoleKey.Q) move = 'd';
                else if (key.Key == ConsoleKey.E) move = 'k';
                else if (key.Key == ConsoleKey.Escape) return false;
                if (move != 0)
                    return true;
            }
        }
    }
}
