namespace Db4objects.Db4o.Inside.Fieldindex
{
	public class FieldIndexProcessorResult
	{
		public static readonly Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult
			 NO_INDEX_FOUND = new Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult
			(null);

		public static readonly Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult
			 FOUND_INDEX_BUT_NO_MATCH = new Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult
			(null);

		private readonly Db4objects.Db4o.Inside.Fieldindex.IIndexedNode _indexedNode;

		public FieldIndexProcessorResult(Db4objects.Db4o.Inside.Fieldindex.IIndexedNode indexedNode
			)
		{
			_indexedNode = indexedNode;
		}

		public virtual Db4objects.Db4o.Foundation.Tree ToQCandidate(Db4objects.Db4o.QCandidates
			 candidates)
		{
			return Db4objects.Db4o.TreeInt.ToQCandidate(ToTreeInt(), candidates);
		}

		public virtual Db4objects.Db4o.TreeInt ToTreeInt()
		{
			if (FoundMatch())
			{
				return _indexedNode.ToTreeInt();
			}
			return null;
		}

		public virtual bool FoundMatch()
		{
			return FoundIndex() && !NoMatch();
		}

		public virtual bool FoundIndex()
		{
			return this != NO_INDEX_FOUND;
		}

		public virtual bool NoMatch()
		{
			return this == FOUND_INDEX_BUT_NO_MATCH;
		}

		public virtual System.Collections.IEnumerator IterateIDs()
		{
			return new _AnonymousInnerClass45(this, _indexedNode.GetEnumerator());
		}

		private sealed class _AnonymousInnerClass45 : Db4objects.Db4o.Foundation.MappingIterator
		{
			public _AnonymousInnerClass45(FieldIndexProcessorResult _enclosing, System.Collections.IEnumerator
				 baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override object Map(object current)
			{
				Db4objects.Db4o.Inside.Btree.FieldIndexKey composite = (Db4objects.Db4o.Inside.Btree.FieldIndexKey
					)current;
				return composite.ParentID();
			}

			private readonly FieldIndexProcessorResult _enclosing;
		}
	}
}
