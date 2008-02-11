/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public class SnapShotQueryResult : AbstractLateQueryResult
	{
		public SnapShotQueryResult(Transaction transaction) : base(transaction)
		{
		}

		public override void LoadFromClassIndex(ClassMetadata clazz)
		{
			CreateSnapshot(ClassIndexIterable(clazz));
		}

		public override void LoadFromClassIndexes(ClassMetadataIterator classCollectionIterator
			)
		{
			CreateSnapshot(ClassIndexesIterable(classCollectionIterator));
		}

		public override void LoadFromQuery(QQuery query)
		{
			IEnumerator _iterator = query.ExecuteSnapshot();
			_iterable = new _IEnumerable_29(this, _iterator);
		}

		private sealed class _IEnumerable_29 : IEnumerable
		{
			public _IEnumerable_29(SnapShotQueryResult _enclosing, IEnumerator _iterator)
			{
				this._enclosing = _enclosing;
				this._iterator = _iterator;
			}

			public IEnumerator GetEnumerator()
			{
				_iterator.Reset();
				return _iterator;
			}

			private readonly SnapShotQueryResult _enclosing;

			private readonly IEnumerator _iterator;
		}

		private void CreateSnapshot(IEnumerable iterable)
		{
			Tree ids = TreeInt.AddAll(null, new IntIterator4Adaptor(iterable));
			_iterable = new _IEnumerable_39(this, ids);
		}

		private sealed class _IEnumerable_39 : IEnumerable
		{
			public _IEnumerable_39(SnapShotQueryResult _enclosing, Tree ids)
			{
				this._enclosing = _enclosing;
				this.ids = ids;
			}

			public IEnumerator GetEnumerator()
			{
				return new TreeKeyIterator(ids);
			}

			private readonly SnapShotQueryResult _enclosing;

			private readonly Tree ids;
		}
	}
}
