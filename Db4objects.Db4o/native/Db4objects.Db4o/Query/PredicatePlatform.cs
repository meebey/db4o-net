using System;

namespace Db4objects.Db4o.Query
{
	using System.Reflection;
	
	public sealed class PredicatePlatform
	{
		public static readonly string PREDICATEMETHOD_NAME = "Match";
		
		public static bool IsFilterMethod(MethodInfo method)
		{
			if (method.GetParameters().Length != 1) return false;
			return method.Name == PREDICATEMETHOD_NAME;
		}

        public static T GetField<T>(Object obj, string fieldName)
        {
            return (T) obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }
	}
}