namespace Db4objects.Db4o.Nativequery.Optimization
{
	public class ReflectUtil
	{
		public static System.Reflection.MethodInfo MethodFor(System.Type clazz, string methodName
			, System.Type[] paramTypes)
		{
			System.Type curclazz = clazz;
			while (curclazz != null)
			{
				try
				{
					System.Reflection.MethodInfo method = Sharpen.Runtime.GetDeclaredMethod(curclazz, 
						methodName, paramTypes);
					Db4objects.Db4o.Platform4.SetAccessible(method);
					return method;
				}
				catch
				{
				}
				curclazz = curclazz.BaseType;
			}
			return null;
		}

		public static System.Reflection.FieldInfo FieldFor(System.Type clazz, string name
			)
		{
			System.Type curclazz = clazz;
			while (curclazz != null)
			{
				try
				{
					System.Reflection.FieldInfo field = Sharpen.Runtime.GetDeclaredField(curclazz, name
						);
					Db4objects.Db4o.Platform4.SetAccessible(field);
					return field;
				}
				catch
				{
				}
				curclazz = curclazz.BaseType;
			}
			return null;
		}
	}
}
