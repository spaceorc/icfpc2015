using System;
using NUnit.Framework;
using SomeSecretProject.IO;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Tests
{
	[TestFixture]
	public class ForbiddenSequenceChecker_Test
	{
		[Test]
		public void SinglePointUnit_NorthForbidden()
		{
			var checker = new ForbiddenSequenceChecker("{pivot: {x: 0, y: 0}, members: [{x: 0, y: 0}]}".ParseAsJson<Unit>());
			Assert.IsFalse(checker.CheckLastMove(new MoveType[0], MoveType.NE));
			Assert.IsFalse(checker.CheckLastMove(new MoveType[0], MoveType.NW));
		}

		[Test]
		public void SinglePointUnit_RotateForbidden()
		{
			var checker = new ForbiddenSequenceChecker("{pivot: {x: 0, y: 0}, members: [{x: 0, y: 0}]}".ParseAsJson<Unit>());
			Assert.IsFalse(checker.CheckLastMove(new MoveType[0], MoveType.RotateCW));
			Assert.IsFalse(checker.CheckLastMove(new MoveType[0], MoveType.RotateCCW));
		}

		[Test]
		public void Symmetric2Unit_RotateCW_Twice_Forbidden()
		{
			var checker = new ForbiddenSequenceChecker("{pivot: {x: 1, y: 1}, members: [{x: 1, y: 0}, {x: 2, y: 1}, {x: 1, y: 2}]}".ParseAsJson<Unit>());
			Assert.IsTrue(checker.CheckLastMove(new MoveType[0], MoveType.RotateCW));
			Assert.IsFalse(checker.CheckLastMove(new[] { MoveType.RotateCW }, MoveType.RotateCW));
		}

		[Test]
		public void Symmetric2Unit_RotateCCW_Twice_Forbidden()
		{
			var checker = new ForbiddenSequenceChecker("{pivot: {x: 1, y: 1}, members: [{x: 1, y: 0}, {x: 2, y: 1}, {x: 1, y: 2}]}".ParseAsJson<Unit>());
			Assert.IsTrue(checker.CheckLastMove(new MoveType[0], MoveType.RotateCCW));
			Assert.IsFalse(checker.CheckLastMove(new[] { MoveType.RotateCCW }, MoveType.RotateCCW));
		}

		[Test]
		public void Symmetric3Unit_RotateCW_ThreeTimes_Forbidden()
		{
			var checker = new ForbiddenSequenceChecker("{pivot: {x: 1, y: 1}, members: [{x: 0, y: 1}, {x: 2, y: 1}]}".ParseAsJson<Unit>());
			Assert.IsTrue(checker.CheckLastMove(new MoveType[0], MoveType.RotateCW));
			Assert.IsTrue(checker.CheckLastMove(new[] { MoveType.RotateCW }, MoveType.RotateCW));
			Assert.IsFalse(checker.CheckLastMove(new[] { MoveType.RotateCW, MoveType.RotateCW }, MoveType.RotateCW));
		}

		[Test]
		public void Symmetric3Unit_RotateCCW_ThreeTimes_Forbidden()
		{
			var checker = new ForbiddenSequenceChecker("{pivot: {x: 1, y: 1}, members: [{x: 0, y: 1}, {x: 2, y: 1}]}".ParseAsJson<Unit>());
			Assert.IsTrue(checker.CheckLastMove(new MoveType[0], MoveType.RotateCCW));
			Assert.IsTrue(checker.CheckLastMove(new[] { MoveType.RotateCCW }, MoveType.RotateCCW));
			Assert.IsFalse(checker.CheckLastMove(new[] { MoveType.RotateCCW, MoveType.RotateCCW }, MoveType.RotateCCW));
		}

		[Test]
		public void AssymmetricUnit_RotateCW_SixTimes_Forbidden()
		{
			var checker = new ForbiddenSequenceChecker("{pivot: {x: 1, y: 1}, members: [{x: 0, y: 1}]}".ParseAsJson<Unit>());
			Assert.IsTrue(checker.CheckLastMove(new MoveType[0], MoveType.RotateCW));
			Assert.IsTrue(checker.CheckLastMove(new[] { MoveType.RotateCW }, MoveType.RotateCW));
			Assert.IsTrue(checker.CheckLastMove(new[] { MoveType.RotateCW, MoveType.RotateCW }, MoveType.RotateCW));
			Assert.IsTrue(checker.CheckLastMove(new[] { MoveType.RotateCW, MoveType.RotateCW, MoveType.RotateCW }, MoveType.RotateCW));
			Assert.IsTrue(checker.CheckLastMove(new[] { MoveType.RotateCW, MoveType.RotateCW, MoveType.RotateCW, MoveType.RotateCW }, MoveType.RotateCW));
			Assert.IsFalse(checker.CheckLastMove(new[] { MoveType.RotateCW, MoveType.RotateCW, MoveType.RotateCW, MoveType.RotateCW, MoveType.RotateCW }, MoveType.RotateCW));
		}

		[Test]
		public void AssymmetricUnit_RotateCCW_SixTimes_Forbidden()
		{
			var checker = new ForbiddenSequenceChecker("{pivot: {x: 1, y: 1}, members: [{x: 0, y: 1}]}".ParseAsJson<Unit>());
			Assert.IsTrue(checker.CheckLastMove(new MoveType[0], MoveType.RotateCCW));
			Assert.IsTrue(checker.CheckLastMove(new[] { MoveType.RotateCCW }, MoveType.RotateCCW));
			Assert.IsTrue(checker.CheckLastMove(new[] { MoveType.RotateCCW, MoveType.RotateCCW }, MoveType.RotateCCW));
			Assert.IsTrue(checker.CheckLastMove(new[] { MoveType.RotateCCW, MoveType.RotateCCW, MoveType.RotateCCW }, MoveType.RotateCCW));
			Assert.IsTrue(checker.CheckLastMove(new[] { MoveType.RotateCCW, MoveType.RotateCCW, MoveType.RotateCCW, MoveType.RotateCCW }, MoveType.RotateCCW));
			Assert.IsFalse(checker.CheckLastMove(new[] { MoveType.RotateCCW, MoveType.RotateCCW, MoveType.RotateCCW, MoveType.RotateCCW, MoveType.RotateCCW }, MoveType.RotateCCW));
		}

		[Test]
		public void SinglePointUnit_EW_Forbidden()
		{
			var checker = new ForbiddenSequenceChecker("{pivot: {x: 0, y: 0}, members: [{x: 0, y: 0}]}".ParseAsJson<Unit>());
			Assert.IsTrue(checker.CheckLastMove(new MoveType[0], MoveType.E));
			Assert.IsFalse(checker.CheckLastMove(new[] { MoveType.E }, MoveType.W));
		}

		[Test]
		public void SinglePointUnit_WE_Forbidden()
		{
			var checker = new ForbiddenSequenceChecker("{pivot: {x: 0, y: 0}, members: [{x: 0, y: 0}]}".ParseAsJson<Unit>());
			Assert.IsTrue(checker.CheckLastMove(new MoveType[0], MoveType.W));
			Assert.IsFalse(checker.CheckLastMove(new[] { MoveType.W }, MoveType.E));
		}
	}
}