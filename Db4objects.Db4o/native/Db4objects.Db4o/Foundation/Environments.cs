/* Copyright (C) 2009 Versant Inc.  http://www.db4o.com */
using System;

namespace Db4objects.Db4o.Foundation
{
	public partial class Environments
	{
		public static string DefaultImplementationFor(Type type)
		{
			string ns = type.Namespace;
			int lastDot = ns.LastIndexOf('.');
			string typeName = ns.Substring(0, lastDot) + ".Internal." + ns.Substring(lastDot + 1) + "." + type.Name.Substring(1) + "Impl";
			return typeName + ", " + AssemblyNameFor(type);
		}

		private static string AssemblyNameFor(Type type)
		{
#if SILVERLIGHT
			string fullyQualifiedTypeName = type.AssemblyQualifiedName;
			int assemblyNameSeparator = fullyQualifiedTypeName.IndexOf(',');
			return fullyQualifiedTypeName.Substring(assemblyNameSeparator + 1);
#else
			return type.Assembly.GetName().Name;
#endif
		}
	}
}
