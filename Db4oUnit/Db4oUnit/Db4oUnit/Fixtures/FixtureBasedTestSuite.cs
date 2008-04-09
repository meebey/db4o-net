/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Fixtures
{
	/// <summary>
	/// TODO: experiment with ParallelTestRunner that uses a thread pool to run tests in parallel
	/// TODO: FixtureProviders must accept the index of a specific fixture to run with (to make it easy to reproduce a failure)
	/// </summary>
	public abstract class FixtureBasedTestSuite : ITestSuiteBuilder
	{
		public abstract Type[] TestUnits();

		public abstract IFixtureProvider[] FixtureProviders();

		public virtual IEnumerator GetEnumerator()
		{
			IFixtureProvider[] providers = FixtureProviders();
			IEnumerable decorators = Iterators.Map(Iterators.Iterable(providers), new _IFunction4_24
				());
			IEnumerable testsXdecorators = Iterators.CrossProduct(new IEnumerable[] { Tests()
				, Iterators.CrossProduct(decorators) });
			return Iterators.Map(testsXdecorators, new _IFunction4_39(this)).GetEnumerator();
		}

		private sealed class _IFunction4_24 : IFunction4
		{
			public _IFunction4_24()
			{
			}

			public object Apply(object arg)
			{
				IFixtureProvider provider = (IFixtureProvider)arg;
				return Iterators.Map(Iterators.Enumerate(provider), new _IFunction4_27(provider));
			}

			private sealed class _IFunction4_27 : IFunction4
			{
				public _IFunction4_27(IFixtureProvider provider)
				{
					this.provider = provider;
				}

				public object Apply(object arg)
				{
					EnumerateIterator.Tuple tuple = (EnumerateIterator.Tuple)arg;
					return new FixtureDecorator(provider.Variable(), tuple.value, tuple.index);
				}

				private readonly IFixtureProvider provider;
			}
		}

		private sealed class _IFunction4_39 : IFunction4
		{
			public _IFunction4_39(FixtureBasedTestSuite _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object arg)
			{
				IEnumerator tuple = ((IEnumerable)arg).GetEnumerator();
				ITest test = (ITest)Iterators.Next(tuple);
				IEnumerable decorators = (IEnumerable)Iterators.Next(tuple);
				return this._enclosing.Decorate(test, decorators.GetEnumerator());
			}

			private readonly FixtureBasedTestSuite _enclosing;
		}

		private IEnumerable Tests()
		{
			return new ReflectionTestSuiteBuilder(TestUnits());
		}

		private ITest Decorate(ITest test, IEnumerator decorators)
		{
			while (decorators.MoveNext())
			{
				test = ((ITestDecorator)decorators.Current).Decorate(test);
			}
			return test;
		}
	}
}
