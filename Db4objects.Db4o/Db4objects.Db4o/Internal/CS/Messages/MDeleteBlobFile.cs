/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MDeleteBlobFile : MsgBlob, IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			try
			{
				IBlob blob = this.ServerGetBlobImpl();
				if (blob != null)
				{
					blob.DeleteFile();
				}
			}
			catch (Exception)
			{
			}
			return true;
		}

		/// <exception cref="IOException"></exception>
		public override void ProcessClient(ISocket4 sock)
		{
		}
		// nothing to do here
	}
}
