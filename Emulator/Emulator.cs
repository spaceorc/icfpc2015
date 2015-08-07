using System;
using System.Threading;
using Emulator.Drawing;
using SomeSecretProject.Logic;

namespace Emulator
{
    class Emulator
    {
        private GameBase game;
        private int delay;
        public Emulator(GameBase game, int delay = -1)
        {
            this.game = game;
            this.delay = delay;
        }

        public void Run()
        {
            var console = new FastConsole();
            while (game.state == GameBase.State.UnitInGame || game.state == GameBase.State.WaitUnit)
            {
                using (var drawer = new Drawer(console))
                {
                    Console.SetWindowPosition(0, 0);
                    drawer.console.WriteLine("Score=" + game.currentScore);
                    drawer.DrawMap(game.map, game.currentUnit);
                    //Console.WriteLine("Score=" + game.currentScore);
                }
                Console.SetWindowPosition(0, 0);
                if (delay > 0) Thread.Sleep(delay);
                else if (delay < 0)
                {
                    var key = Console.ReadKey(false);
                    if (key.Key == ConsoleKey.Z && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                        if (key.Modifiers.HasFlag(ConsoleModifiers.Shift)) game.StepBack(-10);
                        else game.StepBack(-2);
                    else if (key.Key == ConsoleKey.Escape) break;
                }
                game.Step();
            }
            Console.Write("GAME OVER");
            Console.WriteLine(game.state);
            Console.Write("Score="+game.currentScore);
            Console.ReadKey();
        }
    }
}
