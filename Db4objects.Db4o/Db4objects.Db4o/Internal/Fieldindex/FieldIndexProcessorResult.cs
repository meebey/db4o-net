/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Fieldindex;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Fieldindex
{
	public class FieldIndexProcessorResult
	{
		public static readonly Db4objects.Db4o.Internal.Fieldindex.FieldIndexProcessorResult
			 NoIndexFound = new Db4objects.Db4o.Internal.Fieldindex.FieldIndexProcessorResult
			(null);

		public static readonly Db4objects.Db4o.Internal.Fieldindex.FieldIndexProcessorResult
			 FoundIndexButNoMatch = new Db4objects.Db4o.Internal.Fieldindex.FieldIndexProcessorResult
			(null);

		private readonly IIndexedNode _indexedNode;

		public FieldIndexProcessorResult(IIndexedNode indexedNode)
		{
			_indexedNode = indexedNode;
		}

		public virtual Tree ToQCandidate(QCandidates candidates)
		{
			return TreeInt.ToQCandidate(ToTreeInt(), candidates);
		}

		public virtual TreeInt ToTreeInt()
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
			return this != NoIndexFound;
		}

		public virtual bool NoMatch()
		{
			return this == FoundIndexButNoMatch;
		}

		public virtual IEnumerator IterateIDs()
		{
			return new _MappingIterator_46(this, _indexedNode.GetEnumerator());
		}

		private sealed class _MappingIterator_46 : MappingIterator
		{
			public _MappingIterator_46(FieldIndexProcessorResult _enclosing, IEnumerator baseArg1
				) : base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override object Map(object current)
			{
				FieldIndexKey composite = (FieldIndexKey)current;
				return composite.ParentID();
			}

			private readonly FieldIndexProcessorResult _enclosing;
		}
	}
}
