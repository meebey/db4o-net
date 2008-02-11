/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class InvalidSlotExceptionTestCase : AbstractDb4oTestCase
	{
		private const int Max = 100000;

		public static void Main(string[] args)
		{
			new InvalidSlotExceptionTestCase().RunAll();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestInvalidSlotException()
		{
			Assert.Expect(typeof(InvalidIDException), typeof(InvalidSlotException), new _ICodeBlock_19
				(this));
		}

		private sealed class _ICodeBlock_19 : ICodeBlock
		{
			public _ICodeBlock_19(InvalidSlotExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				object byID = this._enclosing.Db().GetByID(1);
			}

			private readonly InvalidSlotExceptionTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void _testTimes()
		{
			long[] ids = new long[Max];
			for (int i = 0; i < Max; i++)
			{
				object o = ComplexObject();
				Db().Store(o);
				ids[i] = Db().Ext().GetID(o);
			}
			Reopen();
			for (int i = 0; i < Max; i++)
			{
				Db().Ext().GetByID(ids[i]);
			}
		}

		public class A
		{
			internal InvalidSlotExceptionTestCase.A _a;

			public A(InvalidSlotExceptionTestCase.A a)
			{
				this._a = a;
			}
		}

		private object ComplexObject()
		{
			return new InvalidSlotExceptionTestCase.A(new InvalidSlotExceptionTestCase.A(new 
				InvalidSlotExceptionTestCase.A(null)));
		}
	}
}
