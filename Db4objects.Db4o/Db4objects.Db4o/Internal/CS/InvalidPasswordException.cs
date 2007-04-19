using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Internal.CS
{
	[System.Serializable]
	public class InvalidPasswordException : Db4oException
	{
		public InvalidPasswordException() : base((string)null)
		{
		}
	}
}
