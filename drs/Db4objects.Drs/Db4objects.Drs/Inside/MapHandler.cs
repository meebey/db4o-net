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
	public class MapHandler : Db4objects.Drs.Inside.ICollectionHandler
	{
		private readonly Db4objects.Db4o.Reflect.IReflectClass _reflectMapClass;

		private readonly Db4objects.Db4o.Reflect.IReflector _reflector;

		public MapHandler(Db4objects.Db4o.Reflect.IReflector reflector)
		{
			_reflector = reflector;
			_reflectMapClass = reflector.ForClass(typeof(System.Collections.IDictionary));
		}

		public virtual bool CanHandle(Db4objects.Db4o.Reflect.IReflectClass claxx)
		{
			return _reflectMapClass.IsAssignableFrom(claxx);
		}

		public virtual bool CanHandle(object obj)
		{
			return CanHandle(_reflector.ForObject(obj));
		}

		public virtual bool CanHandle(System.Type c)
		{
			return CanHandle(_reflector.ForClass(c));
		}

		public virtual System.Collections.IEnumerator IteratorFor(object collection)
		{
			System.Collections.IDictionary map = (System.Collections.IDictionary)collection;
			Db4objects.Db4o.Foundation.Collection4 result = new Db4objects.Db4o.Foundation.Collection4
				();
			System.Collections.IEnumerator it = map.GetEnumerator();
			while (it.MoveNext())
			{
				System.Collections.DictionaryEntry entry = (System.Collections.DictionaryEntry)it
					.Current;
				result.Add(entry.Key);
				result.Add(entry.Value);
			}
			return result.GetEnumerator();
		}

		public virtual object EmptyClone(object original, Db4objects.Db4o.Reflect.IReflectClass
			 originalCollectionClass)
		{
			return new System.Collections.Hashtable(((System.Collections.IDictionary)original
				).Count);
		}

		public virtual void CopyState(object original, object destination, Db4objects.Drs.Inside.ICounterpartFinder
			 counterpartFinder)
		{
			System.Collections.IDictionary originalMap = (System.Collections.IDictionary)original;
			System.Collections.IDictionary destinationMap = (System.Collections.IDictionary)destination;
			destinationMap.Clear();
			System.Collections.IEnumerator it = originalMap.GetEnumerator();
			while (it.MoveNext())
			{
				System.Collections.DictionaryEntry entry = (System.Collections.DictionaryEntry)it
					.Current;
				object keyClone = counterpartFinder.FindCounterpart(entry.Key);
				object valueClone = counterpartFinder.FindCounterpart(entry.Value);
				destinationMap.Add(keyClone, valueClone);
			}
		}

		public virtual object CloneWithCounterparts(object originalMap, Db4objects.Db4o.Reflect.IReflectClass
			 claxx, Db4objects.Drs.Inside.ICounterpartFinder elementCloner)
		{
			System.Collections.IDictionary original = (System.Collections.IDictionary)originalMap;
			System.Collections.IDictionary result = (System.Collections.IDictionary)EmptyClone
				(original, claxx);
			CopyState(original, result, elementCloner);
			return result;
		}
	}
}
