using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.IX;

namespace Db4objects.Db4o.Internal.IX
{
	/// <summary>A node to represent an entry removed from an Index</summary>
	public class IxRemove : IxPatch
	{
		public IxRemove(IndexTransaction a_ft, int a_parentID, object a_value) : base(a_ft
			, a_parentID, a_value)
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

		public override void FreespaceVisit(FreespaceVisitor visitor, int index)
		{
		}

		public override void Visit(object obj)
		{
		}

		public override void Visit(IVisitor4 visitor, int[] lowerAndUpperMatch)
		{
		}

		public override int Write(IIndexable4 a_handler, StatefulBuffer a_writer)
		{
			return 0;
		}

		public override void VisitAll(IIntObjectVisitor visitor)
		{
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Internal.IX.IxRemove remove = new Db4objects.Db4o.Internal.IX.IxRemove
				(_fieldTransaction, _parentID, _value);
			base.ShallowCloneInternal(remove);
			return remove;
		}
	}
}
