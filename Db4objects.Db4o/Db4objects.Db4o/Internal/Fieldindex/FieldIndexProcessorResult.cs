namespace Db4objects.Db4o.Internal.Fieldindex
{
	public class FieldIndexProcessorResult
	{
		public static readonly Db4objects.Db4o.Internal.Fieldindex.FieldIndexProcessorResult
			 NO_INDEX_FOUND = new Db4objects.Db4o.Internal.Fieldindex.FieldIndexProcessorResult
			(null);

		public static readonly Db4objects.Db4o.Internal.Fieldindex.FieldIndexProcessorResult
			 FOUND_INDEX_BUT_NO_MATCH = new Db4objects.Db4o.Internal.Fieldindex.FieldIndexProcessorResult
			(null);

		private readonly Db4objects.Db4o.Internal.Fieldindex.IIndexedNode _indexedNode;

		public FieldIndexProcessorResult(Db4objects.Db4o.Internal.Fieldindex.IIndexedNode
			 indexedNode)
		{
			_indexedNode = indexedNode;
		}

		public virtual Db4objects.Db4o.Foundation.Tree ToQCandidate(Db4objects.Db4o.Internal.Query.Processor.QCandidates
			 candidates)
		{
			return Db4objects.Db4o.Internal.TreeInt.ToQCandidate(ToTreeInt(), candidates);
		}

		public virtual Db4objects.Db4o.Internal.TreeInt ToTreeInt()
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
			return new _AnonymousInnerClass46(this, _indexedNode.GetEnumerator());
		}

		private sealed class _AnonymousInnerClass46 : Db4objects.Db4o.Foundation.MappingIterator
		{
			public _AnonymousInnerClass46(FieldIndexProcessorResult _enclosing, System.Collections.IEnumerator
				 baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override object Map(object current)
			{
				Db4objects.Db4o.Internal.Btree.FieldIndexKey composite = (Db4objects.Db4o.Internal.Btree.FieldIndexKey
					)current;
				return composite.ParentID();
			}

			private readonly FieldIndexProcessorResult _enclosing;
		}
	}
}
