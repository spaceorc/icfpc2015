using System;
using System.Collections.Generic;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace Emulator
{
    class ConsoleGame : GameBase
    {
        public ConsoleGame(Problem problem, int seed)
            :base(problem,seed)
        {}

        public string Solution { get{return new string(solutionChars.ToArray());} }
        private List<char> solutionChars = new List<char>();

        public override void ReStart()
        {
            base.ReStart();
            solutionChars = new List<char>();
        }

        private bool ins = false;
        protected override bool TryGetNextMove(out char move)
        {
            move = (char) 0;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Insert) { ins = !ins; continue;}
                else if (key.Key == ConsoleKey.Escape) return false;
                if (!ins)
                {
                    if (key.Key == ConsoleKey.A) move = 'p';
                    else if (key.Key == ConsoleKey.D) move = 'b';
                    else if (key.Key == ConsoleKey.Z) move = 'a';
                    else if (key.Key == ConsoleKey.X) move = 'l';
                    else if (key.Key == ConsoleKey.Q) move = 'd';
                    else if (key.Key == ConsoleKey.E) move = 'k';
                }
                else
                {
                    var c = char.ToLower(key.KeyChar);
                    if (MoveTypeExt.IsIgnored(c) || MoveTypeExt.Convert(c) == null) continue;
                    move = c;
                }
                if (move != 0)
                {
                    solutionChars.Add(move);
                    return true;

                }
            }
        }
    }
}
