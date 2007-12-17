/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class QueryingReadContext : AbstractReadContext
	{
		private readonly int _handlerVersion;

		public QueryingReadContext(Transaction transaction, int handlerVersion, BufferImpl
			 buffer) : base(transaction, buffer)
		{
			_handlerVersion = handlerVersion;
			_activationDepth = new LegacyActivationDepth(0);
		}

		public override int HandlerVersion()
		{
			return _handlerVersion;
		}
	}
}
