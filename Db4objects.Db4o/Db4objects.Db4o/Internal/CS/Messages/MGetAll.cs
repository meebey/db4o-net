namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MGetAll : Db4objects.Db4o.Internal.CS.Messages.MsgQuery, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			Db4objects.Db4o.Config.QueryEvaluationMode evaluationMode = Db4objects.Db4o.Config.QueryEvaluationMode
				.FromInt(ReadInt());
			WriteQueryResult(GetAll(evaluationMode), evaluationMode);
			return true;
		}

		private Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult GetAll(Db4objects.Db4o.Config.QueryEvaluationMode
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
