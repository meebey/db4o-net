/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	/// <persistent></persistent>
	[System.ObsoleteAttribute(@"since 7.0")]
	public class P1HashElement : P1ListElement
	{
		public object i_key;

		public int i_hashCode;

		public int i_position;

		public P1HashElement()
		{
		}

		public P1HashElement(Transaction a_trans, P1ListElement a_next, object a_key, int
			 a_hashCode, object a_object) : base(a_trans, a_next, a_object)
		{
			i_hashCode = a_hashCode;
			i_key = a_key;
		}

		internal virtual object ActivatedKey(int a_depth)
		{
			// TODO: It may be possible to optimise away the following call.
			CheckActive();
			// The pathologic case here:
			// No activation depth for the map.
			// Global activation depth of 0 during defragment
			// The key can't activate at all.
			// Let's make sure it has a depth of 1 at least, but of course that
			// may not be sufficient for more complex #hashCode calls.
			if (a_depth < 0)
			{
				Transaction trans = GetTrans();
				if (trans != null)
				{
					if (trans.Container().ConfigImpl().ActivationDepth() < 1)
					{
						a_depth = 1;
					}
				}
			}
			Activate(i_key, a_depth);
			return i_key;
		}

		internal override void Delete(bool a_deleteRemoved)
		{
			if (a_deleteRemoved)
			{
				Delete(i_key);
			}
			base.Delete(a_deleteRemoved);
		}
	}
}
