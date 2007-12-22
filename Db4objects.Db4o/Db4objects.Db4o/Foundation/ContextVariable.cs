/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Sharpen.Lang;

namespace Db4objects.Db4o.Foundation
{
	/// <summary>A context variable is a value associated to a specific thread and scope.
	/// 	</summary>
	/// <remarks>
	/// A context variable is a value associated to a specific thread and scope.
	/// The value is brought into scope with the
	/// <see cref="Db4objects.Db4o.Foundation.ContextVariable.With">Db4objects.Db4o.Foundation.ContextVariable.With
	/// 	</see>
	/// method.
	/// </remarks>
	public class ContextVariable
	{
		private class ThreadSlot
		{
			public readonly Thread thread;

			public readonly object value;

			public ThreadSlot(object value_)
			{
				thread = Thread.CurrentThread();
				value = value_;
			}
		}

		private readonly Type _expectedType;

		private readonly Collection4 _values = new Collection4();

		public ContextVariable() : this(null)
		{
		}

		public ContextVariable(Type expectedType)
		{
			_expectedType = expectedType;
		}

		public virtual object Value
		{
			get
			{
				Thread current = Thread.CurrentThread();
				lock (this)
				{
					IEnumerator iterator = _values.GetEnumerator();
					while (iterator.MoveNext())
					{
						ContextVariable.ThreadSlot slot = (ContextVariable.ThreadSlot)iterator.Current;
						if (slot.thread == current)
						{
							return slot.value;
						}
					}
				}
				return null;
			}
		}

		public virtual void With(object value, IRunnable block)
		{
			Validate(value);
			ContextVariable.ThreadSlot slot = PushValue(value);
			try
			{
				block.Run();
			}
			finally
			{
				PopValue(slot);
			}
		}

		private void Validate(object value)
		{
			if (value == null || _expectedType == null)
			{
				return;
			}
			if (_expectedType.IsInstanceOfType(value))
			{
				return;
			}
			throw new ArgumentException("Expecting instance of '" + _expectedType + "' but got '"
				 + value + "'");
		}

		private void PopValue(ContextVariable.ThreadSlot slot)
		{
			lock (this)
			{
				_values.Remove(slot);
			}
		}

		private ContextVariable.ThreadSlot PushValue(object value)
		{
			ContextVariable.ThreadSlot slot = new ContextVariable.ThreadSlot(value);
			lock (this)
			{
				_values.Prepend(slot);
			}
			return slot;
		}
	}
}
