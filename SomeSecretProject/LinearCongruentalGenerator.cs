using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeSecretProject
{
	public class LinearCongruentalGenerator
	{
		private const long Modulus = 1 << 32;
		private const long Multiplier = 1103515245;
		private const long Increment = 12345;

		private long CurrentState = 0;
		public int Seed { get; private set; }

		public LinearCongruentalGenerator(int seed)
		{
			Seed = seed;
		}

		public int GetNext()
		{
			CurrentState = (Multiplier*CurrentState + Increment)%Modulus; 
			return (int)CurrentState;
		}
	}
}
