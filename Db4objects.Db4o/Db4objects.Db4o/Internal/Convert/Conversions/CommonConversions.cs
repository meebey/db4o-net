/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Convert;
using Db4objects.Db4o.Internal.Convert.Conversions;

namespace Db4objects.Db4o.Internal.Convert.Conversions
{
	/// <exclude></exclude>
	public class CommonConversions
	{
		public static void Register(Converter converter)
		{
			converter.Register(ClassIndexesToBTrees_5_5.Version, new ClassIndexesToBTrees_5_5
				());
			converter.Register(FieldIndexesToBTrees_5_7.Version, new FieldIndexesToBTrees_5_7
				());
		}
	}
}
