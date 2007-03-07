namespace Db4objects.Db4o.Internal.CS
{
	/// <summary>Platform specific defaults.</summary>
	/// <remarks>Platform specific defaults.</remarks>
	public class ClientServerPlatform
	{
		/// <summary>
		/// The default
		/// <see cref="Db4objects.Db4o.Internal.CS.ClientQueryResultIterator">Db4objects.Db4o.Internal.CS.ClientQueryResultIterator
		/// 	</see>
		/// for this platform.
		/// </summary>
		/// <returns></returns>
		public static System.Collections.IEnumerator CreateClientQueryResultIterator(Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult
			 result)
		{
			Db4objects.Db4o.Internal.CS.IQueryResultIteratorFactory factory = result.Config()
				.QueryResultIteratorFactory();
			if (null != factory)
			{
				return factory.NewInstance(result);
			}
			return new Db4objects.Db4o.Internal.CS.ClientQueryResultIterator(result);
		}
	}
}
