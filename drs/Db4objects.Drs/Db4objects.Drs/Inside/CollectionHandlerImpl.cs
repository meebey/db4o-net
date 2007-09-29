/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
namespace Db4objects.Drs.Inside
{
	public class CollectionHandlerImpl : Db4objects.Drs.Inside.ICollectionHandler
	{
		private readonly Db4objects.Drs.Inside.ICollectionHandler _mapHandler;

		private readonly Db4objects.Db4o.Reflect.IReflectClass _reflectCollectionClass;

		private readonly Db4objects.Db4o.Reflect.IReflector _reflector;

		public CollectionHandlerImpl() : this(Db4objects.Drs.Inside.ReplicationReflector.
			GetInstance().Reflector())
		{
		}

		public CollectionHandlerImpl(Db4objects.Db4o.Reflect.IReflector reflector)
		{
			_mapHandler = new Db4objects.Drs.Inside.MapHandler(reflector);
			_reflector = reflector;
			_reflectCollectionClass = reflector.ForClass(typeof(System.Collections.ICollection)
				);
		}

		public virtual bool CanHandle(Db4objects.Db4o.Reflect.IReflectClass claxx)
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

		public virtual bool CanHandle(System.Type c)
		{
			return CanHandle(_reflector.ForClass(c));
		}

		public virtual object EmptyClone(object originalCollection, Db4objects.Db4o.Reflect.IReflectClass
			 originalCollectionClass)
		{
			if (_mapHandler.CanHandle(originalCollectionClass))
			{
				return _mapHandler.EmptyClone(originalCollection, originalCollectionClass);
			}
			System.Collections.ICollection original = (System.Collections.ICollection)originalCollection;
			System.Collections.ICollection clone = Db4objects.Drs.Inside.ReplicationPlatform.
				EmptyCollectionClone(original);
			if (null != clone)
			{
				return clone;
			}
			return _reflector.ForClass(original.GetType()).NewInstance();
		}

		public virtual System.Collections.IEnumerator IteratorFor(object collection)
		{
			if (_mapHandler.CanHandle(_reflector.ForObject(collection)))
			{
				return _mapHandler.IteratorFor(collection);
			}
			System.Collections.ICollection subject = (System.Collections.ICollection)collection;
			Db4objects.Db4o.Foundation.Collection4 result = new Db4objects.Db4o.Foundation.Collection4
				();
			System.Collections.IEnumerator it = subject.GetEnumerator();
			while (it.MoveNext())
			{
				result.Add(it.Current);
			}
			return result.GetEnumerator();
		}

		public virtual void CopyState(object original, object destination, Db4objects.Drs.Inside.ICounterpartFinder
			 counterpartFinder)
		{
			if (_mapHandler.CanHandle(original))
			{
				_mapHandler.CopyState(original, destination, counterpartFinder);
			}
			else
			{
				Db4objects.Drs.Inside.ReplicationPlatform.CopyCollectionState(original, destination
					, counterpartFinder);
			}
		}

		public virtual object CloneWithCounterparts(object originalCollection, Db4objects.Db4o.Reflect.IReflectClass
			 claxx, Db4objects.Drs.Inside.ICounterpartFinder counterpartFinder)
		{
			if (_mapHandler.CanHandle(claxx))
			{
				return _mapHandler.CloneWithCounterparts(originalCollection, claxx, counterpartFinder
					);
			}
			System.Collections.ICollection original = (System.Collections.ICollection)originalCollection;
			System.Collections.ICollection result = (System.Collections.ICollection)EmptyClone
				(originalCollection, claxx);
			CopyState(original, result, counterpartFinder);
			return result;
		}
	}
}
