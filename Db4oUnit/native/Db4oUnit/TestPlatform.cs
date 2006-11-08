namespace Db4oUnit
{
	using System;
	using System.IO;
	using System.Reflection;

	public class TestPlatform
	{
		// will be assigned from the outside on CF
		public static TextWriter Out;

#if !CF_1_0
		static TestPlatform()
		{
			Out = Console.Out;
		}
#endif

		public static void PrintStackTrace(TextWriter writer, Exception e)
		{
			writer.Write(e);
		}

		public static TextWriter GetStdOut()
		{
			return Out;
		}
		
		public static void EmitWarning(string warning)
		{
			Out.WriteLine(warning);
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
