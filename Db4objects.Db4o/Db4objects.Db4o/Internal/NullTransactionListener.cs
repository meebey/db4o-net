/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	public class NullTransactionListener : ITransactionListener
	{
		public static readonly ITransactionListener Instance = new NullTransactionListener
			();

		private NullTransactionListener()
		{
		}

		public virtual void PostRollback()
		{
		}

		public virtual void PreCommit()
		{
		}
	}
}
