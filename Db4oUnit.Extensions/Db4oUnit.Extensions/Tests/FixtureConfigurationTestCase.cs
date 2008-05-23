/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Extensions.Tests;
using Db4oUnit.Mocking;
using Db4objects.Db4o.Config;

namespace Db4oUnit.Extensions.Tests
{
	public class FixtureConfigurationTestCase : ITestCase
	{
		internal sealed class MockFixtureConfiguration : MethodCallRecorder, IFixtureConfiguration
		{
			public void Configure(Type clazz, IConfiguration config)
			{
				Record(new MethodCall("configure", clazz, config));
			}

			public string GetLabel()
			{
				return "MOCK";
			}
		}

		public sealed class TestCase1 : AbstractDb4oTestCase
		{
			public void Test()
			{
			}
		}

		public sealed class TestCase2 : AbstractDb4oTestCase
		{
			public void Test()
			{
			}
		}

		public virtual void TestSolo()
		{
			AssertFixtureConfiguration(new Db4oSolo());
		}

		public virtual void TestClientServer()
		{
			AssertFixtureConfiguration(new Db4oClientServer(new CachingConfigurationSource(new 
				IndependentConfigurationSource()), false, "C/S"));
		}

		public virtual void TestInMemory()
		{
			AssertFixtureConfiguration(new Db4oInMemory());
		}

		private void AssertFixtureConfiguration(IDb4oFixture fixture)
		{
			FixtureConfigurationTestCase.MockFixtureConfiguration configuration = new FixtureConfigurationTestCase.MockFixtureConfiguration
				();
			fixture.FixtureConfiguration(configuration);
			Assert.IsTrue(fixture.Label().EndsWith(" - " + configuration.GetLabel()), "FixtureConfiguration label must be part of Fixture label."
				);
			new TestRunner(new Db4oTestSuiteBuilder(fixture, new Type[] { typeof(FixtureConfigurationTestCase.TestCase1
				), typeof(FixtureConfigurationTestCase.TestCase2) })).Run(new TestResult());
			configuration.Verify(new MethodCall[] { new MethodCall("configure", typeof(FixtureConfigurationTestCase.TestCase1
				), MethodCall.IgnoredArgument), new MethodCall("configure", typeof(FixtureConfigurationTestCase.TestCase1
				), MethodCall.IgnoredArgument), new MethodCall("configure", typeof(FixtureConfigurationTestCase.TestCase2
				), MethodCall.IgnoredArgument), new MethodCall("configure", typeof(FixtureConfigurationTestCase.TestCase2
				), MethodCall.IgnoredArgument) });
		}
	}
}
