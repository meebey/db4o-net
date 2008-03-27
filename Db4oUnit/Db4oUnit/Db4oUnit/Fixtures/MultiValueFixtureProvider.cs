/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Fixtures
{
	public class MultiValueFixtureProvider : IFixtureProvider
	{
		public static object[] Value()
		{
			return (object[])_variable.Value();
		}

		private static readonly ContextVariable _variable = new ContextVariable();

		private readonly object[][] _values;

		public MultiValueFixtureProvider(object[][] values)
		{
			_values = values;
		}

		public virtual ContextVariable Variable()
		{
			return _variable;
		}

		public virtual IEnumerator GetEnumerator()
		{
			return Iterators.Iterate(_values);
		}

		public virtual string Label()
		{
			return "data";
		}
	}
}
