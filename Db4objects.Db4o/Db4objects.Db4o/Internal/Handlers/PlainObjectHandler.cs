/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <summary>Tyehandler for naked plain objects (java.lang.Object).</summary>
	/// <remarks>Tyehandler for naked plain objects (java.lang.Object).</remarks>
	public class PlainObjectHandler : ITypeHandler4, IReadsObjectIds, IEmbeddedTypeHandler
	{
		public virtual void Defragment(IDefragmentContext context)
		{
			context.CopySlotlessID();
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
		}

		// do nothing
		public virtual object Read(IReadContext context)
		{
			int id = context.ReadInt();
			Transaction transaction = context.Transaction();
			object obj = transaction.ObjectForIdFromCache(id);
			if (obj != null)
			{
				return obj;
			}
			obj = new object();
			AddReference(context, obj, id);
			return obj;
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			Transaction transaction = context.Transaction();
			ObjectContainerBase container = transaction.Container();
			int id = container.GetID(transaction, obj);
			if (id <= 0)
			{
				id = container.NewUserObject();
				// TODO: Free on rollback
				AddReference(context, obj, id);
			}
			context.WriteInt(id);
		}

		private void AddReference(IContext context, object obj, int id)
		{
			Transaction transaction = context.Transaction();
			ObjectReference @ref = new ObjectReference(id);
			@ref.SetObjectWeak(transaction.Container(), obj);
			transaction.AddNewReference(@ref);
		}

		public virtual IPreparedComparison PrepareComparison(IContext context, object obj
			)
		{
			throw new NotImplementedException();
		}

		public virtual ObjectID ReadObjectID(IInternalReadContext context)
		{
			return ObjectID.Read(context);
		}
	}
}
