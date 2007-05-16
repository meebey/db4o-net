/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.Common.Reflect
{
	public class ReflectArrayTestCase : AbstractDb4oTestCase
	{
		public virtual void TestNewInstance()
		{
			string[][] a23 = (string[][])NewInstance(typeof(string), new int[] { 2, 3 });
			Assert.AreEqual(2, a23.Length);
			for (int i = 0; i < a23.Length; ++i)
			{
				Assert.AreEqual(3, a23[i].Length);
			}
		}

		private object NewInstance(Type elementType, int[] dimensions)
		{
			return Reflector().Array().NewInstance(Reflector().ForClass(elementType), dimensions
				);
		}
	}
}
