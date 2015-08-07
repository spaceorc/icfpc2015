using System;
using System.Runtime.InteropServices;

namespace Emulator.ConsoleUtils
{
	public static class ConsoleApi
	{
		public const int STD_OUTPUT_HANDLE = -11;
		public static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

		[StructLayout(LayoutKind.Sequential)]
		public struct CHAR_INFO
		{
			public ushort charData; // Union between WCHAR and ASCII char 
			public short attributes;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct COORD
		{
			public short X;
			public short Y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SMALL_RECT
		{
			public short Left;
			public short Top;
			public short Right;
			public short Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CONSOLE_SCREEN_BUFFER_INFO
		{
			public COORD dwSize;
			public COORD dwCursorPosition;
			public short wAttributes;
			public SMALL_RECT srWindow;
			public COORD dwMaximumWindowSize;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool WriteConsoleOutput(
			IntPtr hConsoleOutput,
			/* This pointer is treated as the origin of a two-dimensional array of CHAR_INFO structures
			whose size is specified by the dwBufferSize parameter.*/
			[MarshalAs(UnmanagedType.LPArray), In] CHAR_INFO[] lpBuffer,
			COORD dwBufferSize,
			COORD dwBufferCoord,
			ref SMALL_RECT lpWriteRegion);
	}
}