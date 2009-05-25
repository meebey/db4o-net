/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.CS.Objectexchange;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MQueryExecute : MsgQuery, IMessageWithResponse
	{
		public bool ProcessAtServer()
		{
			Unmarshall(_payLoad._offset);
			Stream().WithTransaction(Transaction(), new _IRunnable_14(this));
			return true;
		}

		private sealed class _IRunnable_14 : IRunnable
		{
			public _IRunnable_14(MQueryExecute _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				QQuery query = this._enclosing.UnmarshallQuery();
				this._enclosing.WriteQueryResult(this._enclosing.ExecuteFully(query), query.EvaluationMode
					(), new ObjectExchangeConfiguration(query.PrefetchDepth(), query.PrefetchCount()
					));
			}

			private readonly MQueryExecute _enclosing;
		}

		private QQuery UnmarshallQuery()
		{
			// TODO: The following used to run outside of the
			// Synchronization block for better performance but
			// produced inconsistent results, cause unknown.
			QQuery query = (QQuery)ReadObjectFromPayLoad();
			query.Unmarshall(Transaction());
			return query;
		}

		private AbstractQueryResult ExecuteFully(QQuery query)
		{
			AbstractQueryResult qr = NewQueryResult(query.EvaluationMode());
			qr.LoadFromQuery(query);
			return qr;
		}
	}
}
