namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class P1HashElement : Db4objects.Db4o.P1ListElement
	{
		public object i_key;

		public int i_hashCode;

		public int i_position;

		public P1HashElement()
		{
		}

		public P1HashElement(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.P1ListElement
			 a_next, object a_key, int a_hashCode, object a_object) : base(a_trans, a_next, 
			a_object)
		{
			i_hashCode = a_hashCode;
			i_key = a_key;
		}

		public override int AdjustReadDepth(int a_depth)
		{
			return 1;
		}

		internal virtual object ActivatedKey(int a_depth)
		{
			CheckActive();
			if (a_depth < 0)
			{
				Db4objects.Db4o.Transaction trans = GetTrans();
				if (trans != null)
				{
					if (trans.Stream().ConfigImpl().ActivationDepth() < 1)
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
