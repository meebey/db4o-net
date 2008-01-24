/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class MultiDimensionalArrayHandlerTestCase : TypeHandlerTestCaseBase
	{
		public static void Main(string[] args)
		{
			new MultiDimensionalArrayHandlerTestCase().RunSolo();
		}

		public class Item
		{
			public int[][] _int;

			public Item(int[][] int_)
			{
				_int = int_;
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
				{
					return true;
				}
				if (!(obj is MultiDimensionalArrayHandlerTestCase.Item))
				{
					return false;
				}
				MultiDimensionalArrayHandlerTestCase.Item other = (MultiDimensionalArrayHandlerTestCase.Item
					)obj;
				if (_int.Length != other._int.Length)
				{
					return false;
				}
				for (int i = 0; i < _int.Length; i++)
				{
					if (_int[i].Length != other._int[i].Length)
					{
						return false;
					}
					for (int j = 0; j < _int[i].Length; j++)
					{
						if (_int[i][j] != other._int[i][j])
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		private ArrayHandler IntArrayHandler()
		{
			return ArrayHandler(typeof(int), true);
		}

		private ArrayHandler ArrayHandler(Type clazz, bool isPrimitive)
		{
			//    private ArrayHandler stringArrayHandler(){
			//        return arrayHandler(String.class, false);
			//    }
			ITypeHandler4 typeHandler = (ITypeHandler4)Stream().FieldHandlerForClass(Reflector
				().ForClass(clazz));
			return new MultidimensionalArrayHandler(Stream(), typeHandler, isPrimitive);
		}

		public virtual void TestReadWrite()
		{
			MockWriteContext writeContext = new MockWriteContext(Db());
			MultiDimensionalArrayHandlerTestCase.Item expected = new MultiDimensionalArrayHandlerTestCase.Item
				(new int[][] { new int[] { 1, 2, 3 }, new int[] { 6, 5, 4 } });
			IntArrayHandler().Write(writeContext, expected._int);
			MockReadContext readContext = new MockReadContext(writeContext);
			int[][] arr = (int[][])IntArrayHandler().Read(readContext);
			MultiDimensionalArrayHandlerTestCase.Item actualValue = new MultiDimensionalArrayHandlerTestCase.Item
				(arr);
			Assert.AreEqual(expected, actualValue);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestStoreObject()
		{
			MultiDimensionalArrayHandlerTestCase.Item storedItem = new MultiDimensionalArrayHandlerTestCase.Item
				(new int[][] { new int[] { 1, 2, 3 }, new int[] { 6, 5, 4 } });
			DoTestStoreObject(storedItem);
		}
	}
}
