using System.Collections.Generic;
using SomeSecretProject.Logic;

namespace SomeSecretProject.Algorithm
{
	public interface IPowerPhraseBuilder
	{
		string Build(IList<MoveType> moveTypes);
	}
}