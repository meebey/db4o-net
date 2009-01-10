/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MQueryExecute : MsgQuery, IServerSideMessage
	{
		private QueryEvaluationMode _evaluationMode;

		public bool ProcessAtServer()
		{
			try
			{
				Unmarshall(_payLoad._offset);
				Stream().WithTransaction(Transaction(), new _IRunnable_18(this));
			}
			catch (Db4oException e)
			{
				WriteException(e);
			}
			return true;
		}

		private sealed class _IRunnable_18 : IRunnable
		{
			public _IRunnable_18(MQueryExecute _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.WriteQueryResult(this._enclosing.Execute(), this._enclosing._evaluationMode
					);
			}

			private readonly MQueryExecute _enclosing;
		}

		private AbstractQueryResult Execute()
		{
			// TODO: The following used to run outside of the
			// synchronisation block for better performance but
			// produced inconsistent results, cause unknown.
			QQuery query = (QQuery)ReadObjectFromPayLoad();
			query.Unmarshall(Transaction());
			_evaluationMode = query.EvaluationMode();
			return ExecuteFully(query);
		}

		private AbstractQueryResult ExecuteFully(QQuery query)
		{
			AbstractQueryResult qr = NewQueryResult(query.EvaluationMode());
			qr.LoadFromQuery(query);
			return qr;
		}
	}
}
