using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Internal
{
	[System.Serializable]
	public class ReflectException : Db4oException
	{
		public ReflectException(Exception cause) : base(cause)
		{
		}

		public ReflectException(string msg, Exception cause) : base(msg, cause)
		{
		}

		public virtual Exception GetTarget()
		{
			return InnerException;
		}
	}
}
