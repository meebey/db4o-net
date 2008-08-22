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
using System.Reflection;

namespace Db4objects.Db4o.Foundation.Collections
{
	public interface ICollectionInitializer
	{
		void Clear();
		void Add(object o);
	}

	public sealed class CollectionInitializer
	{
		public static ICollectionInitializer For(object destination)
		{
			if (IsNonGenericList(destination))
			{
			    return new ListInitializer((IList)destination);
			}
			
			Type collectionElementType = CollectionElementTypeFor(destination);
			if (collectionElementType != null)
			{
			    Type genericProtocolType = typeof(CollectionInitializerImpl<>).MakeGenericType(collectionElementType);
			    return InstantiateInitializer(destination, genericProtocolType);
			}
		    throw new ArgumentException("Unknown collection: " + destination);
		}

		private static bool IsNonGenericList(object destination)
		{
			return !destination.GetType().IsGenericType && destination is IList;
		}

		private static ICollectionInitializer InstantiateInitializer(object destination, Type genericProtocolType)
	    {
#if !CF
            return (ICollectionInitializer) Activator.CreateInstance(genericProtocolType, destination);
#else
	        ConstructorInfo constructor = genericProtocolType.GetConstructors()[0];
	        return (ICollectionInitializer) constructor.Invoke(new object[] {destination});
#endif
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
			return type.IsGenericType &&  type.GetGenericTypeDefinition() == typeof(ICollection<>);
		}

		private sealed class ListInitializer : ICollectionInitializer
		{
			private readonly IList _list;

			public ListInitializer(IList list)
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

		private sealed class CollectionInitializerImpl<T> : ICollectionInitializer
		{
			private readonly ICollection<T> _collection;

			public CollectionInitializerImpl(ICollection<T> collection)
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
	}
}
