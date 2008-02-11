/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class FirstClassObjectHandler : ITypeHandler4
	{
		private readonly ClassMetadata _classMetadata;

		public FirstClassObjectHandler(ClassMetadata classMetadata)
		{
			_classMetadata = classMetadata;
		}

		public virtual void Defragment(IDefragmentContext context)
		{
			if (_classMetadata.HasClassIndex())
			{
				context.CopyID();
			}
			else
			{
				context.CopyUnindexedID();
			}
			int restLength = (_classMetadata.LinkLength() - Const4.IntLength);
			context.IncrementOffset(restLength);
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			((ObjectContainerBase)context.ObjectContainer()).DeleteByID(context.Transaction()
				, context.ReadInt(), context.CascadeDeleteDepth());
		}

		public virtual object Read(IReadContext context)
		{
			// FIXME: .NET value types should get their own TypeHandler and it 
			//        should do the following:
			if (_classMetadata.IsValueType())
			{
				IActivationDepth activationDepth = ((UnmarshallingContext)context).ActivationDepth
					();
				return _classMetadata.ReadValueType(context.Transaction(), context.ReadInt(), activationDepth
					.Descend(_classMetadata));
			}
			return context.ReadObject();
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			context.WriteObject(obj);
		}

		public virtual IPreparedComparison PrepareComparison(object source)
		{
			if (source == null)
			{
				return new _IPreparedComparison_59(this);
			}
			int id = 0;
			IReflectClass claxx = null;
			if (source is int)
			{
				id = ((int)source);
			}
			else
			{
				if (source is TransactionContext)
				{
					TransactionContext tc = (TransactionContext)source;
					object obj = tc._object;
					id = _classMetadata.Stream().GetID(tc._transaction, obj);
					claxx = _classMetadata.Reflector().ForObject(obj);
				}
				else
				{
					throw new IllegalComparisonException();
				}
			}
			return new ClassMetadata.PreparedComparisonImpl(id, claxx);
		}

		private sealed class _IPreparedComparison_59 : IPreparedComparison
		{
			public _IPreparedComparison_59(FirstClassObjectHandler _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public int CompareTo(object obj)
			{
				if (obj == null)
				{
					return 0;
				}
				return -1;
			}

			private readonly FirstClassObjectHandler _enclosing;
		}
	}
}
