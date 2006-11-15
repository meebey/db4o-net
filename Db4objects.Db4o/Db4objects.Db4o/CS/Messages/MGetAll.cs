namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MGetAll : Db4objects.Db4o.CS.Messages.MsgQuery
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Db4objects.Db4o.Config.QueryEvaluationMode evaluationMode = Db4objects.Db4o.Config.QueryEvaluationMode
				.FromInt(ReadInt());
			WriteQueryResult(GetAll(evaluationMode), serverThread, evaluationMode);
			return true;
		}

		private Db4objects.Db4o.Inside.Query.AbstractQueryResult GetAll(Db4objects.Db4o.Config.QueryEvaluationMode
			 mode)
		{
			lock (StreamLock())
			{
				try
				{
					return File().GetAll(Transaction(), mode);
				}
				catch (System.Exception e)
				{
				}
				return NewQueryResult(mode);
			}
		}
	}
}
