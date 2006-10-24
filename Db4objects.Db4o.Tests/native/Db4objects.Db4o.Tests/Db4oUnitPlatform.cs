using System;
using System.Reflection;

namespace Db4objects.Db4o.Tests
{
	class Db4oUnitPlatform
	{
	    public static bool IsStoreableField(FieldInfo field)
	    {
	        if (field.IsStatic) return false;
            if (Sharpen.Lang.Reflect.Field.IsTransient(field)) return false;
	        if (field.Name.Contains("$")) return false;
	        return true;
	    }
	}
}
