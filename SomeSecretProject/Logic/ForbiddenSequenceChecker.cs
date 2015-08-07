using JetBrains.Annotations;

namespace SomeSecretProject.Logic
{
	public class ForbiddenSequenceChecker
	{
		public ForbiddenSequenceChecker([NotNull] Unit unit)
		{
		}

		public bool Move(MoveType move)
		{
			return true;
		}
	}
}