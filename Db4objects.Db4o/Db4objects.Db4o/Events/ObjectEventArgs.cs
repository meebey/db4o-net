using System;
using Db4objects.Db4o.Events;

namespace Db4objects.Db4o.Events
{
	/// <summary>Arguments for object related events.</summary>
	/// <remarks>Arguments for object related events.</remarks>
	/// <seealso cref="IEventRegistry">IEventRegistry</seealso>
	public class ObjectEventArgs : EventArgs
	{
		private object _obj;

		public ObjectEventArgs(object obj)
		{
			_obj = obj;
		}

		/// <summary>The object that triggered this event.</summary>
		/// <remarks>The object that triggered this event.</remarks>
		public virtual object Object
		{
			get
			{
				return _obj;
			}
		}
	}
}
