/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System;
using System.Reflection;

using Sharpen;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal
{
	public class ReflectPlatform
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

	    public static string FullyQualifiedName(Type type)
	    {
	        return TypeReference.FromType(type).GetUnversionedName();
	    }
	    
		/// <exception cref="Exception"></exception>
		public static object GetField(object parent, string fieldName) 
		{
			FieldInfo field = Sharpen.Runtime.GetDeclaredField(parent.GetType(),fieldName);
			return field.GetValue(parent);
		}
	}
}
