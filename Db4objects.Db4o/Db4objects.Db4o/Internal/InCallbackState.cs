/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class InCallbackState
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

		public static readonly DynamicVariable _inCallback = new _DynamicVariable_12();
	}
}
