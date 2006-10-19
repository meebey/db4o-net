namespace Db4objects.Db4o
{
	/// <summary>element of linked lists</summary>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class P1ListElement : Db4objects.Db4o.P1Object
	{
		public Db4objects.Db4o.P1ListElement i_next;

		public object i_object;

		public P1ListElement()
		{
		}

		public P1ListElement(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.P1ListElement
			 a_next, object a_object) : base(a_trans)
		{
			i_next = a_next;
			i_object = a_object;
		}

		public override int AdjustReadDepth(int a_depth)
		{
			if (a_depth >= 1)
			{
				return 1;
			}
			return 0;
		}

		internal virtual object ActivatedObject(int a_depth)
		{
			CheckActive();
			Activate(i_object, a_depth);
			return i_object;
		}

		public override object CreateDefault(Db4objects.Db4o.Transaction a_trans)
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
