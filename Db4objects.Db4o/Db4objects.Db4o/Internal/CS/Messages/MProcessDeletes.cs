using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MProcessDeletes : Msg, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			lock (StreamLock())
			{
				try
				{
					Transaction().ProcessDeletes();
				}
				catch (Db4oException e)
				{
				}
			}
			return true;
		}
	}
}
