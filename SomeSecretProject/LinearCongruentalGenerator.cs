namespace SomeSecretProject
{
	public class LinearCongruentalGenerator
	{
		private const long Modulus = 1 << 31;
		private const long Multiplier = 1103515245;
		private const long Increment = 12345;
		private const long mask = 0x000000007FFF0000; // bits 30..16

		private long state;

		public LinearCongruentalGenerator(int seed)
		{
			state = seed;
		}

		public int GetNext()
		{
			var currentState = state;
			state = (Multiplier * state + Increment) % Modulus;
			return (int)((currentState & mask) >> 16);
		}
	}
}
