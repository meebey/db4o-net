/* Copyright (C) 2007   Versant Inc.   http://www.db4o.com */
using System;
using System.Reflection;
using Db4objects.Db4o.Reflect.Net;

namespace Db4oUnit.Extensions
{
	public class Db4oUnitPlatform
	{
		public static bool IsPascalCase()
		{
			return true;
		}

	    public static bool IsUserField(FieldInfo field)
	    {
	        if (field.IsStatic) return false;
            if (NetField.IsTransient(field)) return false;
	        if (field.Name.IndexOf("$") != -1) return false;
	        return true;
	    }
	}
}
