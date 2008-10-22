/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Internal.CS
{
	[System.Serializable]
	public class InternalServerError : Db4oException
	{
		public InternalServerError(Exception cause) : base(cause)
		{
		}
	}
}
