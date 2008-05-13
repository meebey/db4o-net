/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class QueryingReadContext : AbstractReadContext
	{
		private readonly QCandidates _candidates;

		private readonly int _collectionID;

		private readonly int _handlerVersion;

		public QueryingReadContext(Transaction transaction, QCandidates candidates, int handlerVersion
			, IReadBuffer buffer, int collectionID) : base(transaction, buffer)
		{
			_candidates = candidates;
			_activationDepth = new LegacyActivationDepth(0);
			_collectionID = collectionID;
			_handlerVersion = handlerVersion;
		}

		public QueryingReadContext(Transaction transaction, int handlerVersion, IReadBuffer
			 buffer) : this(transaction, null, handlerVersion, buffer, 0)
		{
		}

		public virtual int CollectionID()
		{
			return _collectionID;
		}

		public virtual QCandidates Candidates()
		{
			return _candidates;
		}

		public override int HandlerVersion()
		{
			return _handlerVersion;
		}
	}
}
