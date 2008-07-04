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
	public class MapTypeHandler : ITypeHandler4, IFirstClassHandler, ICanHoldAnythingHandler
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
			IDictionary map = (IDictionary)obj;
			WriteElementCount(context, map);
			WriteElements(context, map);
		}

		public virtual object Read(IReadContext context)
		{
			IDictionary map = (IDictionary)((UnmarshallingContext)context).PersistentObject();
			int elementCount = context.ReadInt();
			ITypeHandler4 elementHandler = ElementTypeHandler(context, map);
			for (int i = 0; i < elementCount; i++)
			{
				object key = context.ReadObject(elementHandler);
				object value = context.ReadObject(elementHandler);
				map.Add(key, value);
			}
			return map;
		}

		private void WriteElementCount(IWriteContext context, IDictionary map)
		{
			context.WriteInt(map.Count);
		}

		private void WriteElements(IWriteContext context, IDictionary map)
		{
			ITypeHandler4 elementHandler = ElementTypeHandler(context, map);
			IEnumerator elements = map.Keys.GetEnumerator();
			while (elements.MoveNext())
			{
				object key = elements.Current;
				context.WriteObject(elementHandler, key);
				context.WriteObject(elementHandler, map[key]);
			}
		}

		private ObjectContainerBase Container(IContext context)
		{
			return ((IInternalObjectContainer)context.ObjectContainer()).Container();
		}

		private ITypeHandler4 ElementTypeHandler(IContext context, IDictionary map)
		{
			// TODO: If all elements in the map are of one type,
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
				handler.Delete(context);
			}
		}

		public virtual void Defragment(IDefragmentContext context)
		{
			ITypeHandler4 handler = ElementTypeHandler(context, null);
			int elementCount = context.ReadInt();
			for (int i = elementCount; i > 0; i--)
			{
				context.Defragment(handler);
				context.Defragment(handler);
			}
		}

		public void CascadeActivation(ActivationContext4 context)
		{
			IDictionary map = (IDictionary)context.TargetObject();
			IEnumerator keys = (map).Keys.GetEnumerator();
			while (keys.MoveNext())
			{
				object key = keys.Current;
				context.CascadeActivationToChild(key);
				context.CascadeActivationToChild(map[key]);
			}
		}

		public virtual ITypeHandler4 ReadCandidateHandler(QueryingReadContext context)
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
				// Read the value too, but ignore.
				// TODO: Optimize for just doing a seek here.
				candidates.ReadSubCandidate(context, elementHandler);
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
