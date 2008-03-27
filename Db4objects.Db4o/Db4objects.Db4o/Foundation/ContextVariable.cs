/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
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

			public ContextVariable.ThreadSlot next;

			public ThreadSlot(object value_, ContextVariable.ThreadSlot next_)
			{
				thread = Thread.CurrentThread();
				value = value_;
				next = next_;
			}
		}

		private readonly Type _expectedType;

		private ContextVariable.ThreadSlot _values = null;

		public ContextVariable() : this(null)
		{
		}

		public ContextVariable(Type expectedType)
		{
			_expectedType = expectedType;
		}

		public virtual object Value()
		{
			Thread current = Thread.CurrentThread();
			lock (this)
			{
				ContextVariable.ThreadSlot slot = _values;
				while (null != slot)
				{
					if (slot.thread == current)
					{
						return slot.value;
					}
					slot = slot.next;
				}
			}
			return null;
		}

		public virtual object With(object value, IClosure4 block)
		{
			Validate(value);
			ContextVariable.ThreadSlot slot = PushValue(value);
			try
			{
				return block.Run();
			}
			finally
			{
				PopValue(slot);
			}
		}

		public virtual void With(object value, IRunnable block)
		{
			With(value, new _IClosure4_62(block));
		}

		private sealed class _IClosure4_62 : IClosure4
		{
			public _IClosure4_62(IRunnable block)
			{
				this.block = block;
			}

			public object Run()
			{
				block.Run();
				return null;
			}

			private readonly IRunnable block;
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
				if (slot == _values)
				{
					_values = _values.next;
					return;
				}
				ContextVariable.ThreadSlot previous = _values;
				ContextVariable.ThreadSlot current = _values.next;
				while (current != null)
				{
					if (current == slot)
					{
						previous.next = current.next;
						return;
					}
					previous = current;
					current = current.next;
				}
			}
		}

		private ContextVariable.ThreadSlot PushValue(object value)
		{
			lock (this)
			{
				ContextVariable.ThreadSlot slot = new ContextVariable.ThreadSlot(value, _values);
				_values = slot;
				return slot;
			}
		}
	}
}
