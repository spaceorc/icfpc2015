using System;
using Emulator.ConsoleUtils;
using JetBrains.Annotations;
using SomeSecretProject.Logic;

namespace Emulator
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var console = new FastConsole();
			using (var drawer = new Drawer(console))
			{
				drawer.DrawMap(new Map(5, 5));
            }
		}
	}

	public class Drawer : IDisposable
	{
		private readonly FastConsole.FastConsoleWriter console;

		public Drawer([NotNull] FastConsole console)
		{
			this.console = console.BeginWrite();
		}

		public void Dispose()
		{
			console.EndWrite();
		}

		public void DrawMap(Map map)
		{
		}
	}
}