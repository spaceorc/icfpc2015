using NUnit.Framework;

namespace SomeSecretProject.Tests
{
	[TestFixture]
	public class LinearCongruentalGenerator_Test
	{
		/// <summary>
		/// Original test form http://icfpcontest.org/spec.html#the_source
		/// </summary>
		[Test]
		public void TestRandomSequence()
		{
			var generator = new LinearCongruentalGenerator(17);
			var answers = new[] { 0, 24107, 16552, 12125, 9427, 13152, 21440, 3383, 6873, 16117 };
			foreach (var expected in answers)
			{
				var actual = generator.GetNext();
				Assert.AreEqual(expected, actual);
			}
		}
	}
}