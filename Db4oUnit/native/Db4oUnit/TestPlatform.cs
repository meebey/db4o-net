namespace Db4oUnit
{
	using System.Reflection;

	public class TestPlatform
	{
		public static void PrintStackTrace(System.IO.TextWriter writer, System.Exception 
			t)
		{
			writer.Write(t);
		}

		public static System.IO.TextWriter GetStdOut()
		{
			return System.Console.Error;
		}
		
		public static void EmitWarning(string warning)
		{
			System.Console.Error.WriteLine(warning);
		}		

		public static bool IsStatic(MethodInfo method)
		{
			return method.IsStatic;
		}

		public static bool IsPublic(MethodInfo method)
		{
			return method.IsPublic;
		}

		public static bool HasParameters(MethodInfo method)
		{
			return method.GetParameters().Length > 0;
		}
	}
}
