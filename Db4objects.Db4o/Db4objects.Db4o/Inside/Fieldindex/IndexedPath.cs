namespace Db4objects.Db4o.Inside.Fieldindex
{
	public class IndexedPath : Db4objects.Db4o.Inside.Fieldindex.IndexedNodeBase
	{
		public static Db4objects.Db4o.Inside.Fieldindex.IIndexedNode NewParentPath(Db4objects.Db4o.Inside.Fieldindex.IIndexedNode
			 next, Db4objects.Db4o.QCon constraint)
		{
			if (!CanFollowParent(constraint))
			{
				return null;
			}
			return new Db4objects.Db4o.Inside.Fieldindex.IndexedPath((Db4objects.Db4o.QConObject
				)constraint.Parent(), next);
		}

		private static bool CanFollowParent(Db4objects.Db4o.QCon con)
		{
			Db4objects.Db4o.QCon parent = con.Parent();
			Db4objects.Db4o.YapField parentField = GetYapField(parent);
			if (null == parentField)
			{
				return false;
			}
			Db4objects.Db4o.YapField conField = GetYapField(con);
			if (null == conField)
			{
				return false;
			}
			return parentField.HasIndex() && parentField.GetParentYapClass().IsAssignableFrom
				(conField.GetParentYapClass());
		}

		private static Db4objects.Db4o.YapField GetYapField(Db4objects.Db4o.QCon con)
		{
			Db4objects.Db4o.QField field = con.GetField();
			if (null == field)
			{
				return null;
			}
			return field.GetYapField();
		}

		private Db4objects.Db4o.Inside.Fieldindex.IIndexedNode _next;

		public IndexedPath(Db4objects.Db4o.QConObject parent, Db4objects.Db4o.Inside.Fieldindex.IIndexedNode
			 next) : base(parent)
		{
			_next = next;
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return new Db4objects.Db4o.Inside.Fieldindex.IndexedPathIterator(this, _next.GetEnumerator
				());
		}

		public override int ResultSize()
		{
			throw new System.NotSupportedException();
		}
	}
}
