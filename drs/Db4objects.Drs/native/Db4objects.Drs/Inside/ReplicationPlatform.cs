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
using Db4objects.Db4o.Foundation.Collections;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Net;

namespace Db4objects.Drs.Inside
{
	/// <summary>
	/// Platform dependent code goes here to minimize manually
	/// converted code.
	/// </summary>
	public class ReplicationPlatform
	{
		public static void CopyCollectionState(object original, object destination, ICounterpartFinder counterpartFinder)
		{
			IEnumerable originalCollection = (IEnumerable) original;
			ICollectionInitializer destinationCollection = CollectionInitializer.For(destination);
			destinationCollection.Clear();

			foreach (object element in originalCollection)
			{
				object counterpart = counterpartFinder.FindCounterpart(element);
				destinationCollection.Add(counterpart);
			}
		}

		public static ICollection EmptyCollectionClone(ICollectionSource source, ICollection original)
		{
			if (original is ArrayList)
			{
				return new ArrayList(original.Count);
			}
			try
			{
				return (ICollection) Activator.CreateInstance(original.GetType());
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

		private static readonly Type[] _nonGeneric = new Type[]
		                                             	{
		                                             		typeof (ArrayList),
		                                             		typeof (SortedList),
		                                             		typeof (Queue),
		                                             		typeof (Stack),
		                                             	};

		private static readonly Type[] _generic = new Type[]
		                                          	{
		                                          		typeof (List<>),
		                                          		typeof (LinkedList<>),
		                                          		typeof (Stack<>),
		                                          	};

		internal static bool IsBuiltinCollectionClass(ReplicationReflector reflector, IReflectClass claxx)
		{
			Type type = NetReflector.ToNative(claxx);

			if (Contains(_nonGeneric, type)) return true;

			if (!type.IsGenericType) return false;

			if (Contains(_generic, type.GetGenericTypeDefinition())) return true;

			return false;
		}

		private static bool Contains(Type[] array, Type type)
		{
			return ((IList) array).Contains(type);
		}
	}
}