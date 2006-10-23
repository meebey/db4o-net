namespace Db4objects.Db4o.Inside.IX
{
	/// <summary>An addition to a field index.</summary>
	/// <remarks>An addition to a field index.</remarks>
	public class IxAdd : Db4objects.Db4o.Inside.IX.IxPatch
	{
		internal bool _keepRemoved;

		public IxAdd(Db4objects.Db4o.Inside.IX.IndexTransaction a_ft, int a_parentID, object
			 a_value) : base(a_ft, a_parentID, a_value)
		{
		}

		internal override void BeginMerge()
		{
			base.BeginMerge();
			Handler().PrepareComparison(Handler().ComparableObject(Trans(), _value));
		}

		public override void Visit(object obj)
		{
			((Db4objects.Db4o.Foundation.IVisitor4)obj).Visit(_parentID);
		}

		public override void Visit(Db4objects.Db4o.Foundation.IVisitor4 visitor, int[] lowerAndUpperMatch
			)
		{
			visitor.Visit(_parentID);
		}

		public override void FreespaceVisit(Db4objects.Db4o.Inside.Freespace.FreespaceVisitor
			 visitor, int index)
		{
			visitor.Visit(_parentID, ((int)_value));
		}

		public override int Write(Db4objects.Db4o.Inside.IX.IIndexable4 a_handler, Db4objects.Db4o.YapWriter
			 a_writer)
		{
			a_handler.WriteIndexEntry(a_writer, _value);
			a_writer.WriteInt(_parentID);
			a_writer.WriteForward();
			return 1;
		}

		public override string ToString()
		{
			return base.ToString();
			string str = "IxAdd " + _parentID + "\n " + Handler().ComparableObject(Trans(), _value
				);
			return str;
		}

		public override void VisitAll(Db4objects.Db4o.Foundation.IIntObjectVisitor visitor
			)
		{
			visitor.Visit(_parentID, Handler().ComparableObject(Trans(), _value));
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Inside.IX.IxAdd add = new Db4objects.Db4o.Inside.IX.IxAdd(_fieldTransaction
				, _parentID, _value);
			base.ShallowCloneInternal(add);
			add._keepRemoved = _keepRemoved;
			return add;
		}
	}
}
