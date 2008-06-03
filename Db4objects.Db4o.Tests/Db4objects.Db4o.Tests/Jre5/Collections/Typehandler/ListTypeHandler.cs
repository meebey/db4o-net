/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Tests.Jre5.Collections.Typehandler
{
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
			return;
		}

		public virtual object Read(IReadContext context)
		{
			IList list = (IList)((UnmarshallingContext)context).PersistentObject();
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
			//       it is possible to use a more specific handler
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
		}

		// TODO Auto-generated method stub
		public void CascadeActivation(Transaction trans, object onObject, IActivationDepth
			 depth)
		{
			ObjectContainerBase container = trans.Container();
			IList list = (IList)onObject;
			IEnumerator all = list.GetEnumerator();
			while (all.MoveNext())
			{
				object current = all.Current;
				IActivationDepth elementDepth = Descend(container, depth, current);
				if (elementDepth.RequiresActivation())
				{
					if (depth.Mode().IsDeactivate())
					{
						container.StillToDeactivate(trans, current, elementDepth, false);
					}
					else
					{
						container.StillToActivate(trans, current, elementDepth);
					}
				}
			}
		}

		public virtual ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, ByteArrayBuffer[] a_bytes)
		{
			return this;
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual void ReadCandidates(QueryingReadContext context)
		{
			int elementCount = context.ReadInt();
			ITypeHandler4 elementHandler = context.Container().Handlers().UntypedObjectHandler
				();
			ReadSubCandidates(context, elementCount, elementHandler);
		}

		private void ReadSubCandidates(QueryingReadContext context, int count, ITypeHandler4
			 elementHandler)
		{
			QCandidates candidates = context.Candidates();
			for (int i = 0; i < count; i++)
			{
				QCandidate qc = candidates.ReadSubCandidate(context, elementHandler);
				if (qc != null)
				{
					candidates.AddByIdentity(qc);
				}
			}
		}

		private IActivationDepth Descend(ObjectContainerBase container, IActivationDepth 
			depth, object obj)
		{
			if (obj == null)
			{
				return new NonDescendingActivationDepth(depth.Mode());
			}
			ClassMetadata cm = container.ClassMetadataForObject(obj);
			if (cm.IsPrimitive())
			{
				return new NonDescendingActivationDepth(depth.Mode());
			}
			return depth.Descend(cm);
		}
	}
}
