/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Tests.Fixtures
{
	public class SimpleFixtureProvider : IFixtureProvider
	{
		private readonly ContextVariable _variable;

		private readonly object[] _values;

		private readonly string _label;

		public SimpleFixtureProvider(ContextVariable variable, object[] values) : this(string.Empty
			, variable, values)
		{
		}

		public SimpleFixtureProvider(string label, ContextVariable variable, object[] values
			)
		{
			_label = label;
			_variable = variable;
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
			return _label;
		}
	}
}
