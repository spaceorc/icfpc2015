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
                    drawer.DrawMap(game.map, game.currentUnit);

                }
                Console.SetWindowPosition(0, 0);
                if (delay > 0) Thread.Sleep(delay);
                else if (delay < 0) Console.ReadKey();
                game.Step();
            }
            Console.Write("GAME OVER");
            Console.WriteLine(game.state);
        }
    }
}
