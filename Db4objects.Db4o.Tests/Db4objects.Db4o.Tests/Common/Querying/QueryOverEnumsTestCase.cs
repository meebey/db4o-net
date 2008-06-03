/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Querying;

namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class QueryOverEnumsTestCase : AbstractDb4oTestCase
	{
		private static ArrayList _items = new ArrayList();

		static QueryOverEnumsTestCase()
		{
		}

		//_items.add(new this.Holder(EnumValue.Second, "bar"));
		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store();
			Store(new QueryOverEnumsTestCase.EnumHolder(this, QueryOverEnumsTestCase.EnumValue
				.First, "foo"));
		}

		public static void Main(string[] args)
		{
			new QueryOverEnumsTestCase().RunSolo();
		}

		public virtual void TestSimpleOrderBy()
		{
			IQuery query = NewQuery(typeof(QueryOverEnumsTestCase.EnumHolder));
			query.Descend("_value").OrderAscending();
		}

		public class EnumHolder
		{
			public QueryOverEnumsTestCase.EnumValue _value;

			public string _name;

			public EnumHolder(QueryOverEnumsTestCase _enclosing, QueryOverEnumsTestCase.EnumValue
				 value, string name)
			{
				this._enclosing = _enclosing;
				//Iterator4Assert.areEqual(, actual)
				this._value = value;
				this._name = name;
			}

			private readonly QueryOverEnumsTestCase _enclosing;
		}

		public class EnumValue : IComparable
		{
			public static QueryOverEnumsTestCase.EnumValue First = new QueryOverEnumsTestCase.EnumValue
				(1);

			public static QueryOverEnumsTestCase.EnumValue Second = new QueryOverEnumsTestCase.EnumValue
				(2);

			public static QueryOverEnumsTestCase.EnumValue Third = new QueryOverEnumsTestCase.EnumValue
				(3);

			public override string ToString()
			{
				return "EnumValue (" + _value + ")";
			}

			public virtual int CompareTo(object obj)
			{
				if (obj.GetType() != GetType())
				{
					return 1;
				}
				QueryOverEnumsTestCase.EnumValue rhs = (QueryOverEnumsTestCase.EnumValue)obj;
				return _value - rhs._value;
			}

			private int _value;

			private EnumValue(int value)
			{
				_value = value;
			}
		}
	}
}
