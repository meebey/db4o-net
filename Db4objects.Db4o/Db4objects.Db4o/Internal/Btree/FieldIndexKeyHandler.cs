/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class FieldIndexKeyHandler : IIndexable4
	{
		private readonly IIndexable4 _valueHandler;

		private readonly IntHandler _parentIdHandler;

		public FieldIndexKeyHandler(ObjectContainerBase stream, IIndexable4 delegate_)
		{
			_parentIdHandler = new IDHandler(stream);
			_valueHandler = delegate_;
		}

		public virtual int LinkLength()
		{
			return _valueHandler.LinkLength() + Const4.IntLength;
		}

		public virtual object ReadIndexEntry(ByteArrayBuffer a_reader)
		{
			// TODO: could read int directly here with a_reader.readInt()
			int parentID = ReadParentID(a_reader);
			object objPart = _valueHandler.ReadIndexEntry(a_reader);
			if (parentID < 0)
			{
				objPart = null;
				parentID = -parentID;
			}
			return new FieldIndexKey(parentID, objPart);
		}

		private int ReadParentID(ByteArrayBuffer a_reader)
		{
			return ((int)_parentIdHandler.ReadIndexEntry(a_reader));
		}

		public virtual void WriteIndexEntry(ByteArrayBuffer writer, object obj)
		{
			FieldIndexKey composite = (FieldIndexKey)obj;
			int parentID = composite.ParentID();
			object value = composite.Value();
			if (value == null)
			{
				parentID = -parentID;
			}
			_parentIdHandler.Write(parentID, writer);
			_valueHandler.WriteIndexEntry(writer, composite.Value());
		}

		public virtual IIndexable4 ValueHandler()
		{
			return _valueHandler;
		}

		public virtual void DefragIndexEntry(DefragmentContextImpl context)
		{
			_parentIdHandler.DefragIndexEntry(context);
			_valueHandler.DefragIndexEntry(context);
		}

		public virtual IPreparedComparison PrepareComparison(object fieldIndexKey)
		{
			FieldIndexKey source = (FieldIndexKey)fieldIndexKey;
			IPreparedComparison preparedValueComparison = _valueHandler.PrepareComparison(source
				.Value());
			IPreparedComparison preparedParentIdComparison = _parentIdHandler.NewPrepareCompare
				(source.ParentID());
			return new _IPreparedComparison_67(this, preparedValueComparison, preparedParentIdComparison
				);
		}

		private sealed class _IPreparedComparison_67 : IPreparedComparison
		{
			public _IPreparedComparison_67(FieldIndexKeyHandler _enclosing, IPreparedComparison
				 preparedValueComparison, IPreparedComparison preparedParentIdComparison)
			{
				this._enclosing = _enclosing;
				this.preparedValueComparison = preparedValueComparison;
				this.preparedParentIdComparison = preparedParentIdComparison;
			}

			public int CompareTo(object obj)
			{
				FieldIndexKey target = (FieldIndexKey)obj;
				try
				{
					int delegateResult = preparedValueComparison.CompareTo(target.Value());
					if (delegateResult != 0)
					{
						return delegateResult;
					}
				}
				catch (IllegalComparisonException)
				{
				}
				// can happen, is expected
				return preparedParentIdComparison.CompareTo(target.ParentID());
			}

			private readonly FieldIndexKeyHandler _enclosing;

			private readonly IPreparedComparison preparedValueComparison;

			private readonly IPreparedComparison preparedParentIdComparison;
		}
	}
}
