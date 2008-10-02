/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MGetAll : MsgQuery, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			QueryEvaluationMode evaluationMode = QueryEvaluationMode.FromInt(ReadInt());
			lock (StreamLock())
			{
				WriteQueryResult(GetAll(evaluationMode), evaluationMode);
			}
			return true;
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
