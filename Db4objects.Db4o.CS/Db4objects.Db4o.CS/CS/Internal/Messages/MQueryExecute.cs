/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.CS.Internal.Objectexchange;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;
using Sharpen.Lang;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MQueryExecute : MsgQuery, IMessageWithResponse
	{
		public Msg ReplyFromServer()
		{
			Unmarshall(_payLoad._offset);
			ObjectByRef result = new ObjectByRef();
			Stream().WithTransaction(Transaction(), new _IRunnable_15(this, result));
			return ((Msg)result.value);
		}

		private sealed class _IRunnable_15 : IRunnable
		{
			public _IRunnable_15(MQueryExecute _enclosing, ObjectByRef result)
			{
				this._enclosing = _enclosing;
				this.result = result;
			}

			public void Run()
			{
				QQuery query = this._enclosing.UnmarshallQuery();
				result.value = this._enclosing.WriteQueryResult(this._enclosing.ExecuteFully(query
					), query.EvaluationMode(), new ObjectExchangeConfiguration(query.PrefetchDepth()
					, query.PrefetchCount()));
			}

			private readonly MQueryExecute _enclosing;

			private readonly ObjectByRef result;
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
