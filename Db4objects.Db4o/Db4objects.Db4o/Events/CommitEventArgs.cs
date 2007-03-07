namespace Db4objects.Db4o.Events
{
	/// <summary>Arguments for commit time related events.</summary>
	/// <remarks>Arguments for commit time related events.</remarks>
	/// <seealso cref="Db4objects.Db4o.Events.IEventRegistry">Db4objects.Db4o.Events.IEventRegistry
	/// 	</seealso>
	public class CommitEventArgs : System.EventArgs
	{
		private readonly Db4objects.Db4o.Ext.IObjectInfoCollection _added;

		private readonly Db4objects.Db4o.Ext.IObjectInfoCollection _deleted;

		private readonly Db4objects.Db4o.Ext.IObjectInfoCollection _updated;

		public CommitEventArgs(Db4objects.Db4o.Ext.IObjectInfoCollection added, Db4objects.Db4o.Ext.IObjectInfoCollection
			 deleted, Db4objects.Db4o.Ext.IObjectInfoCollection updated)
		{
			_added = added;
			_deleted = deleted;
			_updated = updated;
		}

		/// <summary>Returns a iteration</summary>
		public virtual Db4objects.Db4o.Ext.IObjectInfoCollection Added
		{
			get
			{
				return _added;
			}
		}

		public virtual Db4objects.Db4o.Ext.IObjectInfoCollection Deleted
		{
			get
			{
				return _deleted;
			}
		}

		public virtual Db4objects.Db4o.Ext.IObjectInfoCollection Updated
		{
			get
			{
				return _updated;
			}
		}
	}
}
