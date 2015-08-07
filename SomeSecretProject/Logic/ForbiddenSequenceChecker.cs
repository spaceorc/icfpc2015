using System.Collections.Generic;
using JetBrains.Annotations;

namespace SomeSecretProject.Logic
{
	public class ForbiddenSequenceChecker
	{
		public ForbiddenSequenceChecker([NotNull] Unit unit)
		{
		}

		public bool CheckLastMove(IList<MoveType> move)
		{
			return true;
		}
	}
}