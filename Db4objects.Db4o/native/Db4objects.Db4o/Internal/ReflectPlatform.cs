/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System;

using Sharpen.Lang;

namespace Db4objects.Db4o.Internal
{
	internal class ReflectPlatform
	{
		public static Type ForName(string typeName)
		{
			try
			{
				return TypeReference.FromString(typeName).Resolve();
			}
			catch
			{
				return null;
			}
		}

		public static object CreateInstance(string typeName)
		{
			try
			{
				return Activator.CreateInstance(ForName(typeName));	
			}
			catch
			{
				return null;
			}
		}
	}
}
