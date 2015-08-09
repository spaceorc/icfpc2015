using System;
using System.Collections.Generic;
using System.Linq;

namespace Emulator.Drawing
{
	public class FastConsole
	{
		private readonly List<FastConsoleRow> rows = new List<FastConsoleRow>();

		public int Height
		{
			get { return rows.Count; }
		}

		public void Refresh()
		{
			Console.Clear();
			var writer = new FastConsoleWriter(this, new List<FastConsoleRow>(rows));
			rows.Clear();
			writer.EndWrite();
		}

		public FastConsoleWriter BeginWrite()
		{
			return new FastConsoleWriter(this, null);
		}

		public class FastConsoleWriter
		{
			private readonly FastConsole fastConsole;
			private readonly List<FastConsoleRow> rows;

			public FastConsoleWriter(FastConsole fastConsole, List<FastConsoleRow> rows)
			{
				Line = 0;
				this.rows = rows ?? new List<FastConsoleRow>();
				ResetColor();
				this.fastConsole = fastConsole;
			}

			public void WriteLine(object value)
			{
				WriteLine(Convert.ToString(value));
			}

			public void WriteLine(string text)
			{
				Write(text);
				WriteLine();
			}

			public int Position
			{
				get
				{
					while (rows.Count <= Line)
						rows.Add(new FastConsoleRow());
					return rows[Line].Length;
				}
			}

			public ConsoleColor BackgroundColor { get; set; }
			public ConsoleColor ForegroundColor { get; set; }

			public void ResetColor()
			{
				BackgroundColor = ConsoleColor.Black;
				ForegroundColor = ConsoleColor.Gray;
			}

			public void Write(object value)
			{
				Write(Convert.ToString(value));
			}

			public void Write(string text)
			{
				if (string.IsNullOrEmpty(text))
					return;
				if (text.IndexOf('\t') >= 0)
				{
					Write(text.Replace("\t", "    "));
					return;
				}
				if (!string.IsNullOrEmpty(text) && text.IndexOf("\n") >= 0)
				{
					var texts = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
					for (int index = 0; index < texts.Length - 1; index++)
						WriteLine(texts[index]);
					Write(texts.Last());
					return;
				}
				while (rows.Count <= Line)
					rows.Add(new FastConsoleRow());
				if (rows[Line].Length + text.Length > Console.WindowWidth)
				{
					var breakIndex = Console.WindowWidth - rows[Line].Length;
					WriteLine(text.Substring(0, breakIndex));
					Write(text.Substring(breakIndex));
					return;
				}
				rows[Line].Segments.Add(new FastConsoleRowSegment(ForegroundColor, BackgroundColor, text));
			}

			public void WriteLine()
			{
				Line++;
			}

			public int Line { get; set; }

			public void EndWrite()
			{
				for (var i = Math.Min(Line + 1, fastConsole.rows.Count); i < fastConsole.rows.Count; i++)
					WriteLine();
				while (rows.Count <= Line)
					rows.Add(new FastConsoleRow());
				for (var i = 0; i < Math.Min(rows.Count, fastConsole.rows.Count); i++)
				{
					if (!rows[i].Equals(fastConsole.rows[i]))
						rows[i].WriteToConsole(i, fastConsole.rows[i].Length);
				}
				for (var i = Math.Min(rows.Count, fastConsole.rows.Count); i < rows.Count; i++)
					rows[i].WriteToConsole(i, 0);
				fastConsole.rows.Clear();
				fastConsole.rows.AddRange(rows);
			}
		}
	}
}