using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;

namespace Emulator.Drawing
{
	public class FastConsoleRow : IEquatable<FastConsoleRow>
	{
		private readonly List<FastConsoleRowSegment> segments = new List<FastConsoleRowSegment>();

		[NotNull]
		public List<FastConsoleRowSegment> Segments
		{
			get { return segments; }
		}

		public int Length
		{
			get { return segments.Sum(x => x.Text == null ? 0 : x.Text.Length); }
		}

		public bool Equals(FastConsoleRow other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			if (segments.Count != other.segments.Count)
				return false;
			for (var i = 0; i < segments.Count; i++)
				if (!segments[i].Equals(other.segments[i]))
					return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != GetType())
				return false;
			return Equals((FastConsoleRow)obj);
		}

		public override int GetHashCode()
		{
			return (segments != null ? segments.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return string.Join("", segments.Select(x => x.Text));
		}

		public void WriteToConsole(int line, int minLength)
		{
			var stdHandle = ConsoleApi.GetStdHandle(ConsoleApi.STD_OUTPUT_HANDLE);
			if (stdHandle == ConsoleApi.INVALID_HANDLE_VALUE)
				throw new Win32Exception();

			var charInfos = new ConsoleApi.CHAR_INFO[Math.Max(minLength, Length)];
			var i = 0;
			foreach (var segment in segments)
			{
				if (segment.Text != null)
					for (var j = 0; j < segment.Text.Length; j++)
					{
						charInfos[i].charData = segment.Text[j];
						charInfos[i].attributes = (short)((short)segment.ForegroundColor | (((short)segment.BackgroundColor) << 4));
						i++;
					}
			}
			for(; i < charInfos.Length; ++i)
			{
				charInfos[i].charData = ' ';
				charInfos[i].attributes = 0;
			}
			var bufSize = new ConsoleApi.COORD { X = (short)charInfos.Length, Y = 1 };
			var bufferCoord = new ConsoleApi.COORD { X = 0, Y = 0 };

			var writeRegion = new ConsoleApi.SMALL_RECT
			{
				Top = (short)line,
				Bottom = (short)line,
				Left = 0,
				Right = (short)(charInfos.Length - 1)
			};
			ConsoleApi.WriteConsoleOutput(stdHandle, charInfos, bufSize, bufferCoord, ref writeRegion);
		}
	}
}