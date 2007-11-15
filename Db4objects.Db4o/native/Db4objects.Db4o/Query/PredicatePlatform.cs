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
	}
}