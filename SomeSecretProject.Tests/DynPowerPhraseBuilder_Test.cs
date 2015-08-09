using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using SomeSecretProject.Algorithm;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Tests
{
	[TestFixture]
	public class DynPowerPhraseBuilder_Test
	{
		[Test]
		public void Test1()
		{
			var builder = new DynPowerPhraseBuilder(null);
			Assert.AreEqual("", builder.Build(new MoveType[0]));
		}

		[Test]
		public void Test2()
		{
			var builder = new DynPowerPhraseBuilder(null);
			Assert.AreEqual(3, builder.Build(new[] {MoveType.E, MoveType.SW, MoveType.W}).Length);
		}

		[Test]
		public void Test3()
		{
			var builder = new DynPowerPhraseBuilder(new[] {"Ei!"});
			Assert.AreEqual("ei!", builder.Build(new[] {MoveType.E, MoveType.SW, MoveType.W}));
		}

		[Test]
		public void Test4()
		{
			var builder = new DynPowerPhraseBuilder(new[] {"Ei!"});
			Assert.AreEqual("ei!ei!", builder.Build(new[] {MoveType.E, MoveType.SW, MoveType.W, MoveType.E, MoveType.SW, MoveType.W}));
		}

		[Test]
		public void Test5()
		{
			var builder = new DynPowerPhraseBuilder(new[] {"Ei!", "hi", "hi hi"});
			var result = builder.Build(new[] {MoveType.E, MoveType.SW, MoveType.SW, MoveType.SW, MoveType.SE, MoveType.SW, MoveType.SW, MoveType.SE, MoveType.SW, MoveType.SW});
			Console.WriteLine(result);
			Assert.IsTrue(Regex.IsMatch(result, @"..hi.hi hi", RegexOptions.IgnoreCase));
		}

		[Test]
		public void Test6()
		{
			var builder = new DynPowerPhraseBuilder(new[] {"hi", "hihi", "hihihi", "hihihihi", "hihihihihi", "hihihihihihi", "hihihihihihihi", "hihihihihihihihi", "hihihihihihihihihi", "hihihihihihihihihihi" });
			var moves = Enumerable.Range(0, 10000).Select(i => MoveType.SW).ToArray();
			var sw = Stopwatch.StartNew();
			var result = builder.Build(moves);
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
			Assert.IsTrue(Regex.IsMatch(result, @"(hi)+", RegexOptions.IgnoreCase));
		}

		[Test]
		public void Test7()
		{
			var builder = new DynPowerPhraseBuilder(new[] {"hi", "ie!"});

			int score;
			var result = builder.Build(ToMoves("hie!"), out score);
			Console.WriteLine(result);

			Assert.IsTrue(Regex.IsMatch(result, @".ie!"));
			Assert.AreEqual(306, score);
		}

		[Test]
		public void Test8()
		{
			var phrases = new[] {"Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn.", "Cthulhu R'lyeh wgah'nagl", "nglui mglw'nafh Cthulhu", "Cthulhu", "fhtagn."};
			var builder = new DynPowerPhraseBuilder(phrases);

			int score;
			var result = builder.Build(ToMoves("Ph'nglui mglw'nafh Cthulhu R'lyeh wgah'nagl fhtagn."), out score);
			Console.WriteLine(result);

			Assert.IsTrue(Regex.IsMatch(result, @"...................cthulhu r'lyeh wgah'nagl.fhtagn\."));
			Assert.AreEqual(662, score);
		}

		private static List<MoveType> ToMoves(string s)
		{
			return s.Select(c => MoveTypeExt.Convert(c).Value).ToList();
		}
	}
}