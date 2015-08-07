using System;

namespace Emulator.ConsoleUtils
{
	public class FastConsoleRowSegment : IEquatable<FastConsoleRowSegment>
	{
		public bool Equals(FastConsoleRowSegment other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return ForegroundColor == other.ForegroundColor && BackgroundColor == other.BackgroundColor && string.Equals(Text, other.Text);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != GetType())
				return false;
			return Equals((FastConsoleRowSegment)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int)ForegroundColor;
				hashCode = (hashCode * 397) ^ (int)BackgroundColor;
				hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
				return hashCode;
			}
		}

		public FastConsoleRowSegment(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string text)
		{
			ForegroundColor = foregroundColor;
			BackgroundColor = backgroundColor;
			Text = text;
		}

		public ConsoleColor ForegroundColor { get; }
		public ConsoleColor BackgroundColor { get; }
		public string Text { get; }

		public override string ToString()
		{
			return string.Format("ForegroundColor: {0}, BackgroundColor: {1}, Text: {2}", ForegroundColor, BackgroundColor, Text);
		}
	}
}