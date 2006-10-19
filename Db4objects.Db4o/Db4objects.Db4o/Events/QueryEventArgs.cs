namespace Db4objects.Db4o.Events
{
	/// <summary>
	/// Arguments for
	/// <see cref="Db4objects.Db4o.Query.IQuery">Db4objects.Db4o.Query.IQuery</see>
	/// related events.
	/// </summary>
	/// <seealso cref="Db4objects.Db4o.Events.IEventRegistry">Db4objects.Db4o.Events.IEventRegistry
	/// 	</seealso>
	public class QueryEventArgs : Db4objects.Db4o.Events.ObjectEventArgs
	{
		public QueryEventArgs(Db4objects.Db4o.Query.IQuery q) : base(q)
		{
		}

		/// <summary>
		/// The
		/// <see cref="Db4objects.Db4o.Query.IQuery">Db4objects.Db4o.Query.IQuery</see>
		/// which triggered the event.
		/// </summary>
		public virtual Db4objects.Db4o.Query.IQuery Query
		{
			get
			{
				return (Db4objects.Db4o.Query.IQuery)Object;
			}
		}
	}
}
