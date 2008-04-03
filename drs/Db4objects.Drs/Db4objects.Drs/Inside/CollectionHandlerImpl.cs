/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Reflect;
using Db4objects.Drs.Inside;

namespace Db4objects.Drs.Inside
{
	public class CollectionHandlerImpl : Db4objects.Drs.Inside.ICollectionHandler
	{
		private readonly Db4objects.Drs.Inside.ICollectionHandler _mapHandler;

		private readonly IReflectClass _reflectCollectionClass;

		private readonly IReflector _reflector;

		public CollectionHandlerImpl() : this(ReplicationReflector.GetInstance().Reflector
			())
		{
		}

		public CollectionHandlerImpl(IReflector reflector)
		{
			_mapHandler = new MapHandler(reflector);
			_reflector = reflector;
			_reflectCollectionClass = reflector.ForClass(typeof(ICollection));
		}

		public virtual bool CanHandle(IReflectClass claxx)
		{
			if (_mapHandler.CanHandle(claxx))
			{
				return true;
			}
			return _reflectCollectionClass.IsAssignableFrom(claxx);
		}

		public virtual bool CanHandle(object obj)
		{
			return CanHandle(_reflector.ForObject(obj));
		}

		public virtual bool CanHandle(Type c)
		{
			return CanHandle(_reflector.ForClass(c));
		}

		public virtual object EmptyClone(object originalCollection, IReflectClass originalCollectionClass
			)
		{
			if (_mapHandler.CanHandle(originalCollectionClass))
			{
				return _mapHandler.EmptyClone(originalCollection, originalCollectionClass);
			}
			ICollection original = (ICollection)originalCollection;
			ICollection clone = ReplicationPlatform.EmptyCollectionClone(original);
			if (null != clone)
			{
				return clone;
			}
			return _reflector.ForClass(original.GetType()).NewInstance();
		}

		public virtual IEnumerator IteratorFor(object collection)
		{
			if (_mapHandler.CanHandle(_reflector.ForObject(collection)))
			{
				return _mapHandler.IteratorFor(collection);
			}
			ICollection subject = (ICollection)collection;
			Collection4 result = new Collection4();
			IEnumerator it = subject.GetEnumerator();
			while (it.MoveNext())
			{
				result.Add(it.Current);
			}
			return result.GetEnumerator();
		}

		public virtual void CopyState(object original, object destination, ICounterpartFinder
			 counterpartFinder)
		{
			if (_mapHandler.CanHandle(original))
			{
				_mapHandler.CopyState(original, destination, counterpartFinder);
			}
			else
			{
				ReplicationPlatform.CopyCollectionState(original, destination, counterpartFinder);
			}
		}

		public virtual object CloneWithCounterparts(object originalCollection, IReflectClass
			 claxx, ICounterpartFinder counterpartFinder)
		{
			if (_mapHandler.CanHandle(claxx))
			{
				return _mapHandler.CloneWithCounterparts(originalCollection, claxx, counterpartFinder
					);
			}
			ICollection original = (ICollection)originalCollection;
			ICollection result = (ICollection)EmptyClone(originalCollection, claxx);
			CopyState(original, result, counterpartFinder);
			return result;
		}
	}
}
