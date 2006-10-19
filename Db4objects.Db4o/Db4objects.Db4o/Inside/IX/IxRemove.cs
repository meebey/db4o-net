namespace Db4objects.Db4o.Inside.IX
{
	/// <summary>A node to represent an entry removed from an Index</summary>
	public class IxRemove : Db4objects.Db4o.Inside.IX.IxPatch
	{
		public IxRemove(Db4objects.Db4o.Inside.IX.IndexTransaction a_ft, int a_parentID, 
			object a_value) : base(a_ft, a_parentID, a_value)
		{
			_size = 0;
		}

		public override int OwnSize()
		{
			return 0;
		}

		public override string ToString()
		{
			return base.ToString();
			string str = "IxRemove " + _parentID + "\n " + Handler().ComparableObject(Trans()
				, _value);
			return str;
		}

		public override void FreespaceVisit(Db4objects.Db4o.Inside.Freespace.FreespaceVisitor
			 visitor, int index)
		{
		}

		public override void Visit(object obj)
		{
		}

		public override void Visit(Db4objects.Db4o.Foundation.IVisitor4 visitor, int[] lowerAndUpperMatch
			)
		{
		}

		public override int Write(Db4objects.Db4o.Inside.IX.IIndexable4 a_handler, Db4objects.Db4o.YapWriter
			 a_writer)
		{
			return 0;
		}

		public override void VisitAll(Db4objects.Db4o.Foundation.IIntObjectVisitor visitor
			)
		{
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Inside.IX.IxRemove remove = new Db4objects.Db4o.Inside.IX.IxRemove
				(_fieldTransaction, _parentID, _value);
			base.ShallowCloneInternal(remove);
			return remove;
		}
	}
}
