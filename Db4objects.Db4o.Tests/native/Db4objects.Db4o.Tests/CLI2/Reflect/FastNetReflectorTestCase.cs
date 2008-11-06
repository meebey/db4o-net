/* Copyright (C) 2004 - 2008 db4objects Inc.   http://www.db4o.com */

#if !CF
using Db4objects.Db4o.Internal.Reflect;
#endif

using System.Collections.Generic;
using Db4objects.Db4o.Reflect;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI2.Reflector
{
	class FastNetReflectorTestCase : ITestCase
	{
#if !CF
		public void TestNullAssignmentToValueTypeField()
		{
			FastNetReflector reflector = new FastNetReflector();
			IReflectField field = reflector.ForClass(typeof (ValueTypeContainer)).GetDeclaredField("_value");
			ValueTypeContainer subject = new ValueTypeContainer(0xDb40);
			
			field.Set(subject, null);
			Assert.AreEqual(0, subject.Value);

			field.Set(subject, 42);
			Assert.AreEqual(42, subject.Value);
		}

		public void TestNonAccessibleGenericTypeParamenterBugInReflectionEmit()
		{
			FastNetReflector reflector = new FastNetReflector();
			IReflectField sizeField = reflector.ForClass(typeof (List<NotAccessible>)).GetDeclaredField("_size");
			
			List<NotAccessible> list = new List<NotAccessible>();
			sizeField.Set(list, 42);
			Assert.AreEqual(42, sizeField.Get(list));
		}

		internal class ValueTypeContainer
		{
			private int _value;

			public ValueTypeContainer(int initialValue)
			{
				_value = initialValue;
			}

			public int Value
			{
				get { return _value;}
			}
		}

		private class NotAccessible
		{
		}
#endif
	}
}
