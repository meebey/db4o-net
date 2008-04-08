using System.Collections;

namespace Db4objects.Drs.Tests
{
	class ReplicationTestPlatform
	{
		public static IEnumerator Adapt(IEnumerator enumerator)
		{
			return enumerator;
		}
	}
}
