/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Typehandlers
{
	/// <summary>TypeHandler for classes that implement java.util.List.<br /><br /></summary>
	/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
	public class ListTypeHandler : ITypeHandler4, IFirstClassHandler, ICanHoldAnythingHandler
		, IVariableLengthTypeHandler
	{
		public virtual IPreparedComparison PrepareComparison(IContext context, object obj
			)
		{
			// TODO Auto-generated method stub
			return null;
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			IList list = (IList)obj;
			WriteElementCount(context, list);
			WriteElements(context, list);
		}

		public virtual object Read(IReadContext context)
		{
			IList list = (IList)((UnmarshallingContext)context).PersistentObject();
			list.Clear();
			int elementCount = context.ReadInt();
			ITypeHandler4 elementHandler = ElementTypeHandler(context, list);
			for (int i = 0; i < elementCount; i++)
			{
				list.Add(context.ReadObject(elementHandler));
			}
			return list;
		}

		private void WriteElementCount(IWriteContext context, IList list)
		{
			context.WriteInt(list.Count);
		}

		private void WriteElements(IWriteContext context, IList list)
		{
			ITypeHandler4 elementHandler = ElementTypeHandler(context, list);
			IEnumerator elements = list.GetEnumerator();
			while (elements.MoveNext())
			{
				context.WriteObject(elementHandler, elements.Current);
			}
		}

		private ObjectContainerBase Container(IContext context)
		{
			return ((IInternalObjectContainer)context.ObjectContainer()).Container();
		}

		private ITypeHandler4 ElementTypeHandler(IContext context, IList list)
		{
			// TODO: If all elements in the list are of one type,
			// it is possible to use a more specific handler
			return Container(context).Handlers().UntypedObjectHandler();
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			if (!context.CascadeDelete())
			{
				return;
			}
			ITypeHandler4 handler = ElementTypeHandler(context, null);
			int elementCount = context.ReadInt();
			for (int i = elementCount; i > 0; i--)
			{
				handler.Delete(context);
			}
		}

		public virtual void Defragment(IDefragmentContext context)
		{
			ITypeHandler4 handler = ElementTypeHandler(context, null);
			int elementCount = context.ReadInt();
			for (int i = 0; i < elementCount; i++)
			{
				handler.Defragment(context);
			}
		}

		public void CascadeActivation(ActivationContext4 context)
		{
			IEnumerator all = ((IList)context.TargetObject()).GetEnumerator();
			while (all.MoveNext())
			{
				context.CascadeActivationToChild(all.Current);
			}
		}

		public virtual ITypeHandler4 ReadCandidateHandler(QueryingReadContext context)
		{
			return this;
		}

		private ITypeHandler4 UntypedObjectHandlerFrom(IContext context)
		{
			return context.Transaction().Container().Handlers().UntypedObjectHandler();
		}

		public virtual void CollectIDs(QueryingReadContext context)
		{
			int elementCount = context.ReadInt();
			ITypeHandler4 elementHandler = UntypedObjectHandlerFrom(context);
			for (int i = 0; i < elementCount; i++)
			{
				context.ReadId(elementHandler);
			}
		}
	}
}
