using Sharpen.IO;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	class PathProvider
	{
		public static File TestCasePath()
		{
			return new File(typeof(PathProvider).Module.FullyQualifiedName);
		}
	}
}
