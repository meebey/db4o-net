/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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
			// Don't send the exception to the user because delete is asynchronous
			return true;
		}
	}
}
