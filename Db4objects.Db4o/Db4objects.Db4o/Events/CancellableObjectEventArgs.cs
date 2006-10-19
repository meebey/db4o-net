namespace Db4objects.Db4o.Events
{
	/// <summary>Argument for object related events which can be cancelled.</summary>
	/// <remarks>Argument for object related events which can be cancelled.</remarks>
	/// <seealso cref="Db4objects.Db4o.Events.IEventRegistry">Db4objects.Db4o.Events.IEventRegistry
	/// 	</seealso>
	/// <seealso cref="Db4objects.Db4o.Events.ICancellableEventArgs">Db4objects.Db4o.Events.ICancellableEventArgs
	/// 	</seealso>
	public class CancellableObjectEventArgs : Db4objects.Db4o.Events.ObjectEventArgs, 
		Db4objects.Db4o.Events.ICancellableEventArgs
	{
		private bool _cancelled;

		public CancellableObjectEventArgs(object obj) : base(obj)
		{
		}

		/// <seealso cref="Db4objects.Db4o.Events.ICancellableEventArgs.Cancel">Db4objects.Db4o.Events.ICancellableEventArgs.Cancel
		/// 	</seealso>
		public virtual void Cancel()
		{
			_cancelled = true;
		}

		/// <seealso cref="Db4objects.Db4o.Events.ICancellableEventArgs.IsCancelled">Db4objects.Db4o.Events.ICancellableEventArgs.IsCancelled
		/// 	</seealso>
		public virtual bool IsCancelled
		{
			get
			{
				return _cancelled;
			}
		}
	}
}
