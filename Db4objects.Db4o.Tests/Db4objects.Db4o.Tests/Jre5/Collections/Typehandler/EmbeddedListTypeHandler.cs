/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Tests.Jre5.Collections.Typehandler
{
	/// <exclude></exclude>
	public class EmbeddedListTypeHandler : ITypeHandler4, IFirstClassHandler, ICanHoldAnythingHandler
		, IVariableLengthTypeHandler, IEmbeddedTypeHandler
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
			WriteClass(context, list);
			WriteElementCount(context, list);
			WriteElements(context, list);
			return;
		}

		public virtual object Read(IReadContext context)
		{
			ClassMetadata classMetadata = ReadClass(context);
			IList list = (IList)classMetadata.InstantiateFromReflector(Container(context));
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

		private void WriteClass(IWriteContext context, IList list)
		{
			int classID = ClassID(context, list);
			context.WriteInt(classID);
		}

		private int ClassID(IWriteContext context, object obj)
		{
			ObjectContainerBase container = Container(context);
			GenericReflector reflector = container.Reflector();
			IReflectClass claxx = reflector.ForObject(obj);
			ClassMetadata classMetadata = container.ProduceClassMetadata(claxx);
			return classMetadata.GetID();
		}

		private ObjectContainerBase Container(IContext context)
		{
			return ((IInternalObjectContainer)context.ObjectContainer()).Container();
		}

		private ITypeHandler4 ElementTypeHandler(IContext context, IList list)
		{
			// TODO: If all elements in the list are of one type,
			//       it is possible to use a more specific handler
			return Container(context).Handlers().UntypedObjectHandler();
		}

		private ClassMetadata ReadClass(IReadContext context)
		{
			int classID = context.ReadInt();
			ClassMetadata classMetadata = Container(context).ClassMetadataForId(classID);
			return classMetadata;
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			if (!context.CascadeDelete())
			{
				return;
			}
			ITypeHandler4 handler = ElementTypeHandler(context, null);
			SkipClass(context);
			int elementCount = context.ReadInt();
			for (int i = elementCount; i > 0; i--)
			{
				handler.Delete(context);
			}
		}

		private void SkipClass(IReadBuffer context)
		{
			context.ReadInt();
		}

		// class ID
		public virtual void Defragment(IDefragmentContext context)
		{
			context.CopyID();
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

		public virtual void CollectIDs(QueryingReadContext context)
		{
			SkipClass(context);
			int elementCount = context.ReadInt();
			ITypeHandler4 elementHandler = context.Container().Handlers().UntypedObjectHandler
				();
			for (int i = 0; i < elementCount; i++)
			{
				context.ReadId(elementHandler);
			}
		}
	}
}
