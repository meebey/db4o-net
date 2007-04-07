namespace Db4oUnit.Util
{
	public class ExceptionUtil
	{
		public static System.Exception GetExceptionCause(System.Exception e)
		{
			return e.InnerException;
		}
	}
}
