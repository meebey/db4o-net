/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Events;

namespace Db4objects.Db4o.Events
{
	/// <summary>Argument for object related events which can be cancelled.</summary>
	/// <remarks>Argument for object related events which can be cancelled.</remarks>
	/// <seealso cref="IEventRegistry">IEventRegistry</seealso>
	/// <seealso cref="ICancellableEventArgs">ICancellableEventArgs</seealso>
	public class CancellableObjectEventArgs : ObjectEventArgs, ICancellableEventArgs
	{
		private bool _cancelled;

		public CancellableObjectEventArgs(object obj) : base(obj)
		{
		}

		/// <seealso cref="ICancellableEventArgs.Cancel">ICancellableEventArgs.Cancel</seealso>
		public virtual void Cancel()
		{
			_cancelled = true;
		}

		/// <seealso cref="ICancellableEventArgs.IsCancelled">ICancellableEventArgs.IsCancelled
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
