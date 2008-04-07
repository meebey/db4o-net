/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

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
using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Drs.Inside
{
	/// <summary>
	/// Platform dependent code goes here to minimize manually
	/// converted code.
	/// </summary>
	public class ReplicationPlatform
	{	
		interface ICollectionInitializer
		{
			void Clear();
			void Add(object o);
		}

        public static void CopyCollectionState(object original, object destination, Db4objects.Drs.Inside.ICounterpartFinder
			 counterpartFinder)
		{
			System.Collections.IEnumerable originalCollection = (System.Collections.IEnumerable
				)original;
        	ICollectionInitializer destinationCollection = CollectionInitializerFor(destination);
			destinationCollection.Clear();

			foreach (object element in originalCollection)
			{
				object counterpart = counterpartFinder.FindCounterpart(element);
				destinationCollection.Add(counterpart);
			}
		}

		private static ICollectionInitializer CollectionInitializerFor(object destination)
		{
			if (destination is IList)
			{
				return new ListCollectionProtocol((IList) destination);
			}
			Type collectionElementType = CollectionElementTypeFor(destination);
			if (collectionElementType != null)
			{
				Type genericProtocolType = typeof(GenericCollectionProtocol<>).MakeGenericType(collectionElementType);
				return (ICollectionInitializer) Activator.CreateInstance(genericProtocolType, destination);
			}
			throw new ArgumentException("Unknown collection: " + destination);
		}

		private static Type CollectionElementTypeFor(object destination)
		{
			foreach (Type interfaceType in destination.GetType().GetInterfaces())
			{
				if (IsGenericCollection(interfaceType))
				{
					return interfaceType.GetGenericArguments()[0];
				}
			}
			return null;
		}

		private static bool IsGenericCollection(Type type)
		{
			return type.GetGenericTypeDefinition() == typeof(ICollection<>);
		}

		private class GenericCollectionProtocol<T> : ICollectionInitializer
		{
			private ICollection<T> _collection;

			public GenericCollectionProtocol(ICollection<T> collection)
			{
				_collection = collection;
			}

			public void Clear()
			{
				_collection.Clear();
			}

			public void Add(object o)
			{
				_collection.Add((T)o);
			}
		}

		private class ListCollectionProtocol : ICollectionInitializer
		{
			private readonly IList _list;

			public ListCollectionProtocol(IList list)
			{
				_list = list;
			}

			public void Clear()
			{
				_list.Clear();
			}

			public void Add(object o)
			{
				_list.Add(o);
			}
		}

		public static System.Collections.ICollection EmptyCollectionClone(ICollectionSource source, System.Collections.ICollection
			 original)
		{	
			if (original is System.Collections.ArrayList)
			{
				return new System.Collections.ArrayList(original.Count);
			}
			try
			{
				return (System.Collections.ICollection) Activator.CreateInstance(original.GetType());
			}
			catch (MissingMethodException x)
			{
				throw new ArgumentException(string.Format("Parameterless ctor required for type '{0}'", original.GetType()), x);
			}
		}
		
		public static bool IsValueType(object o)
		{
			if (null == o) return false;
			return o.GetType().IsValueType;
		}
	}
}

