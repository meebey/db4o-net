namespace Db4objects.Db4o.Internal.Fieldindex
{
	public class IndexedPath : Db4objects.Db4o.Internal.Fieldindex.IndexedNodeBase
	{
		public static Db4objects.Db4o.Internal.Fieldindex.IIndexedNode NewParentPath(Db4objects.Db4o.Internal.Fieldindex.IIndexedNode
			 next, Db4objects.Db4o.Internal.Query.Processor.QCon constraint)
		{
			if (!CanFollowParent(constraint))
			{
				return null;
			}
			return new Db4objects.Db4o.Internal.Fieldindex.IndexedPath((Db4objects.Db4o.Internal.Query.Processor.QConObject
				)constraint.Parent(), next);
		}

		private static bool CanFollowParent(Db4objects.Db4o.Internal.Query.Processor.QCon
			 con)
		{
			Db4objects.Db4o.Internal.Query.Processor.QCon parent = con.Parent();
			Db4objects.Db4o.Internal.FieldMetadata parentField = GetYapField(parent);
			if (null == parentField)
			{
				return false;
			}
			Db4objects.Db4o.Internal.FieldMetadata conField = GetYapField(con);
			if (null == conField)
			{
				return false;
			}
			return parentField.HasIndex() && parentField.GetParentYapClass().IsAssignableFrom
				(conField.GetParentYapClass());
		}

		private static Db4objects.Db4o.Internal.FieldMetadata GetYapField(Db4objects.Db4o.Internal.Query.Processor.QCon
			 con)
		{
			Db4objects.Db4o.Internal.Query.Processor.QField field = con.GetField();
			if (null == field)
			{
				return null;
			}
			return field.GetYapField();
		}

		private Db4objects.Db4o.Internal.Fieldindex.IIndexedNode _next;

		public IndexedPath(Db4objects.Db4o.Internal.Query.Processor.QConObject parent, Db4objects.Db4o.Internal.Fieldindex.IIndexedNode
			 next) : base(parent)
		{
			_next = next;
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return new Db4objects.Db4o.Internal.Fieldindex.IndexedPathIterator(this, _next.GetEnumerator
				());
		}

		public override int ResultSize()
		{
			throw new System.NotSupportedException();
		}
	}
}
