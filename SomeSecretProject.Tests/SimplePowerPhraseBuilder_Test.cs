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
	public class SimplePowerPhraseBuilder_Test
	{
		[Test]
		public void Test1()
		{
			var builder = new SimplePowerPhraseBuilder(null);
			Assert.AreEqual("", builder.Build(new MoveType[0]));
		}

		[Test]
		public void Test2()
		{
			var builder = new SimplePowerPhraseBuilder(null);
			Assert.AreEqual(3, builder.Build(new[] {MoveType.E, MoveType.SW, MoveType.W}).Length);
		}

		[Test]
		public void Test3()
		{
			var builder = new SimplePowerPhraseBuilder(new[] {"Ei!"});
			Assert.AreEqual("ei!", builder.Build(new[] {MoveType.E, MoveType.SW, MoveType.W}));
		}

		[Test]
		public void Test4()
		{
			var builder = new SimplePowerPhraseBuilder(new[] {"Ei!"});
			Assert.AreEqual("ei!ei!", builder.Build(new[] {MoveType.E, MoveType.SW, MoveType.W, MoveType.E, MoveType.SW, MoveType.W}));
		}

		[Test]
		public void Test5()
		{
			var builder = new SimplePowerPhraseBuilder(new[] {"Ei!", "hi", "hi hi"});
			var result = builder.Build(new[] {MoveType.E, MoveType.SW, MoveType.SW, MoveType.SW, MoveType.SE, MoveType.SW, MoveType.SW, MoveType.SE, MoveType.SW, MoveType.SW});
			Assert.IsTrue(Regex.IsMatch(result, @".hi..hi hi", RegexOptions.IgnoreCase));
		}

		[Test]
		public void Test6()
		{
			var builder = new SimplePowerPhraseBuilder(new[] {"hi", "hihi", "hihihi", "hihihihi", "hihihihihi", "hihihihihihi", "hihihihihihihi", "hihihihihihihihi", "hihihihihihihihihi", "hihihihihihihihihihi" });
			var moves = Enumerable.Range(0, 10000).Select(i => MoveType.SW).ToArray();
			var sw = Stopwatch.StartNew();
			var result = builder.Build(moves);
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
			Assert.IsTrue(Regex.IsMatch(result, @"(hi)+", RegexOptions.IgnoreCase));
		}
	}
}