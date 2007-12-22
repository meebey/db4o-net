/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
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
			return _valueHandler.LinkLength() + Const4.INT_LENGTH;
		}

		public virtual object ReadIndexEntry(BufferImpl a_reader)
		{
			int parentID = ReadParentID(a_reader);
			object objPart = _valueHandler.ReadIndexEntry(a_reader);
			if (parentID < 0)
			{
				objPart = null;
				parentID = -parentID;
			}
			return new FieldIndexKey(parentID, objPart);
		}

		private int ReadParentID(BufferImpl a_reader)
		{
			return ((int)_parentIdHandler.ReadIndexEntry(a_reader));
		}

		public virtual void WriteIndexEntry(BufferImpl writer, object obj)
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

		public virtual IComparable4 PrepareComparison(object obj)
		{
			FieldIndexKey composite = (FieldIndexKey)obj;
			_valueHandler.PrepareComparison(composite.Value());
			_parentIdHandler.PrepareComparison(composite.ParentID());
			return this;
		}

		public virtual int CompareTo(object obj)
		{
			if (null == obj)
			{
				throw new ArgumentNullException();
			}
			FieldIndexKey composite = (FieldIndexKey)obj;
			try
			{
				int delegateResult = _valueHandler.CompareTo(composite.Value());
				if (delegateResult != 0)
				{
					return delegateResult;
				}
			}
			catch (IllegalComparisonException)
			{
			}
			return _parentIdHandler.CompareTo(composite.ParentID());
		}

		public virtual void DefragIndexEntry(DefragmentContextImpl context)
		{
			_parentIdHandler.DefragIndexEntry(context);
			_valueHandler.DefragIndexEntry(context);
		}

		public virtual IPreparedComparison NewPrepareCompare(object obj)
		{
			FieldIndexKey source = (FieldIndexKey)obj;
			IPreparedComparison preparedValueComparison = _valueHandler.NewPrepareCompare(source
				.Value());
			IPreparedComparison preparedParentIdComparison = _parentIdHandler.NewPrepareCompare
				(source.ParentID());
			return new _IPreparedComparison_90(this, preparedValueComparison, preparedParentIdComparison
				);
		}

		private sealed class _IPreparedComparison_90 : IPreparedComparison
		{
			public _IPreparedComparison_90(FieldIndexKeyHandler _enclosing, IPreparedComparison
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
				return preparedParentIdComparison.CompareTo(target.ParentID());
			}

			private readonly FieldIndexKeyHandler _enclosing;

			private readonly IPreparedComparison preparedValueComparison;

			private readonly IPreparedComparison preparedParentIdComparison;
		}
	}
}
