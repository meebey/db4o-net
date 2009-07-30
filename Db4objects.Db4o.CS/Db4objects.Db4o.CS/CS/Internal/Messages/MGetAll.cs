/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.CS.Internal.Objectexchange;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MGetAll : MsgQuery, IMessageWithResponse
	{
		public Msg ReplyFromServer()
		{
			QueryEvaluationMode evaluationMode = QueryEvaluationMode.FromInt(ReadInt());
			int prefetchDepth = ReadInt();
			int prefetchCount = ReadInt();
			lock (StreamLock())
			{
				return WriteQueryResult(GetAll(evaluationMode), evaluationMode, new ObjectExchangeConfiguration
					(prefetchDepth, prefetchCount));
			}
		}

		private AbstractQueryResult GetAll(QueryEvaluationMode mode)
		{
			try
			{
				return File().GetAll(Transaction(), mode);
			}
			catch (Exception e)
			{
			}
			return NewQueryResult(mode);
		}
	}
}
