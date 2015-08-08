using System;
using System.Threading;
using Emulator.Drawing;
using SomeSecretProject;
using SomeSecretProject.IO;
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
                    drawer.console.WriteLine("Score=" + game.CurrentScore);
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
//                Console.SetCursorPosition(0, 40);
//                Console.Write(new string(Enumerable.Range(0, 500).Select(_ => ' ').ToArray()));
//                Console.SetCursorPosition(0, 40);
//                Console.Write(game.enteredString.ToString());
            }
			Console.SetCursorPosition(0, game.map.Height * 3 + 5);
            Console.Write("GAME OVER");
            Console.WriteLine(game.state);
            Console.Write("Score="+game.CurrentScore);
            Console.ReadKey();
        }
    }

    public class EmulatorProblemSolver : IProblemSolver
    {
		public string Solve(Problem problem, int seed, string[] magicSpells)
        {
            var game = new ConsoleGame(problem, seed, magicSpells);
            var emulator = new Emulator(game, -1);
            emulator.Run();
            var solution = game.Solution;
            return solution;
        }
    }

    
}
