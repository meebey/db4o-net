/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	/// <summary>element of linked lists</summary>
	/// <exclude></exclude>
	/// <persistent></persistent>
	[System.ObsoleteAttribute(@"since 7.0")]
	public class P1ListElement : P1Object
	{
		public Db4objects.Db4o.P1ListElement i_next;

		public object i_object;

		public P1ListElement()
		{
		}

		public P1ListElement(Transaction a_trans, Db4objects.Db4o.P1ListElement a_next, object
			 a_object) : base(a_trans)
		{
			i_next = a_next;
			i_object = a_object;
		}

		internal virtual object ActivatedObject(int a_depth)
		{
			// TODO: It may be possible to optimise away the following call
			CheckActive();
			if (null == i_object)
			{
				return null;
			}
			Activate(i_object, a_depth);
			return i_object;
		}

		public override object CreateDefault(Transaction a_trans)
		{
			Db4objects.Db4o.P1ListElement elem4 = new Db4objects.Db4o.P1ListElement();
			elem4.SetTrans(a_trans);
			return elem4;
		}

		internal virtual void Delete(bool a_deleteRemoved)
		{
			if (a_deleteRemoved)
			{
				Delete(i_object);
			}
			Delete();
		}
	}
}
