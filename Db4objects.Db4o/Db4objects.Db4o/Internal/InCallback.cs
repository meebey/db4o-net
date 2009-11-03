/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class InCallback
	{
		private sealed class _DynamicVariable_12 : DynamicVariable
		{
			public _DynamicVariable_12()
			{
			}

			protected override object DefaultValue()
			{
				return false;
			}
		}

		private static readonly DynamicVariable _inCallback = new _DynamicVariable_12();

		public static bool Value()
		{
			return (((bool)_inCallback.Value));
		}

		public static void Run(IRunnable runnable)
		{
			_inCallback.With(true, runnable);
		}
	}
}
