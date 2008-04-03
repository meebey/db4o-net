/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Reflect;
using Db4objects.Drs.Inside.Traversal;

namespace Db4objects.Drs.Inside.Traversal
{
	public class GenericTraverser : ITraverser
	{
		protected readonly IReflector _reflector;

		protected readonly Db4objects.Drs.Inside.Traversal.ICollectionHandler _collectionHandler;

		protected readonly IQueue4 _queue = new NonblockingQueue();

		public GenericTraverser(IReflector reflector, Db4objects.Drs.Inside.Traversal.ICollectionHandler
			 collectionHandler)
		{
			_reflector = reflector;
			_collectionHandler = collectionHandler;
		}

		public virtual void TraverseGraph(object @object, IVisitor visitor)
		{
			QueueUpForTraversing(@object);
			while (true)
			{
				object next = _queue.Next();
				if (next == null)
				{
					return;
				}
				TraverseObject(next, visitor);
			}
		}

		protected virtual void TraverseObject(object @object, IVisitor visitor)
		{
			if (!visitor.Visit(@object))
			{
				return;
			}
			IReflectClass claxx = _reflector.ForObject(@object);
			TraverseAllFields(@object, claxx);
		}

		protected virtual void TraverseAllFields(object @object, IReflectClass claxx)
		{
			TraverseFields(@object, claxx);
			TraverseSuperclass(@object, claxx);
		}

		private void TraverseSuperclass(object @object, IReflectClass claxx)
		{
			IReflectClass superclass = claxx.GetSuperclass();
			if (superclass == null)
			{
				return;
			}
			TraverseAllFields(@object, superclass);
		}

		private void TraverseFields(object @object, IReflectClass claxx)
		{
			IReflectField[] fields = claxx.GetDeclaredFields();
			for (int i = 0; i < fields.Length; i++)
			{
				IReflectField field = fields[i];
				if (field.IsStatic())
				{
					continue;
				}
				if (field.IsTransient())
				{
					continue;
				}
				field.SetAccessible();
				//TODO Optimize: Change the reflector so I dont have to call setAcessible all the time.
				object value = field.Get(@object);
				QueueUpForTraversing(value);
			}
		}

		protected virtual void TraverseCollection(object collection)
		{
			IEnumerator elements = _collectionHandler.IteratorFor(collection);
			//TODO Optimization: visit instead of flattening.
			while (elements.MoveNext())
			{
				object element = elements.Current;
				if (element == null)
				{
					continue;
				}
				QueueUpForTraversing(element);
			}
		}

		protected virtual void TraverseArray(IReflectClass claxx, object array)
		{
			IEnumerator contents = ArrayHandler.Iterator(claxx, array);
			while (contents.MoveNext())
			{
				QueueUpForTraversing(contents.Current);
			}
		}

		protected virtual void QueueUpForTraversing(object @object)
		{
			if (@object == null)
			{
				return;
			}
			IReflectClass claxx = _reflector.ForObject(@object);
			if (IsSecondClass(claxx))
			{
				return;
			}
			if (_collectionHandler.CanHandle(claxx))
			{
				TraverseCollection(@object);
				return;
			}
			if (claxx.IsArray())
			{
				TraverseArray(claxx, @object);
				return;
			}
			QueueAdd(@object);
		}

		protected virtual void QueueAdd(object @object)
		{
			_queue.Add(@object);
		}

		protected virtual bool IsSecondClass(IReflectClass claxx)
		{
			//      TODO Optimization: Compute this lazily in ReflectClass;
			if (claxx.IsSecondClass())
			{
				return true;
			}
			return claxx.IsArray() && claxx.GetComponentType().IsSecondClass();
		}

		public virtual void ExtendTraversalTo(object disconnected)
		{
			QueueUpForTraversing(disconnected);
		}
	}
}
