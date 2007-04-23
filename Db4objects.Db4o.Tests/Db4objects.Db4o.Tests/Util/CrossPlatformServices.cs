using System;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Util
{
	public class CrossPlatformServices
	{
		public static string SimpleName(string typeName)
		{
			int index = typeName.IndexOf(',');
			if (index < 0)
			{
				return typeName;
			}
			return Sharpen.Runtime.Substring(typeName, 0, index);
		}

		public static string FullyQualifiedName(Type klass)
		{
			return ReflectPlatform.FullyQualifiedName(klass);
		}
	}
}
