/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Foundation;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class EnvironmentsTestCase : ITestCase
	{
		public interface IWhatever
		{
		}

		public virtual void TestNoEnvironment()
		{
			Assert.Expect(typeof(InvalidOperationException), new _ICodeBlock_13());
		}

		private sealed class _ICodeBlock_13 : ICodeBlock
		{
			public _ICodeBlock_13()
			{
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				Environments.My(typeof(EnvironmentsTestCase.IWhatever));
			}
		}

		public virtual void TestRunWith()
		{
			EnvironmentsTestCase.IWhatever whatever = new _IWhatever_21();
			IEnvironment environment = new _IEnvironment_22(whatever);
			ByRef ran = ByRef.NewInstance();
			Environments.RunWith(environment, new _IRunnable_28(ran, whatever));
			Assert.IsTrue((((bool)ran.value)));
		}

		private sealed class _IWhatever_21 : EnvironmentsTestCase.IWhatever
		{
			public _IWhatever_21()
			{
			}
		}

		private sealed class _IEnvironment_22 : IEnvironment
		{
			public _IEnvironment_22(EnvironmentsTestCase.IWhatever whatever)
			{
				this.whatever = whatever;
			}

			public object Provide(Type service)
			{
				return (object)whatever;
			}

			private readonly EnvironmentsTestCase.IWhatever whatever;
		}

		private sealed class _IRunnable_28 : IRunnable
		{
			public _IRunnable_28(ByRef ran, EnvironmentsTestCase.IWhatever whatever)
			{
				this.ran = ran;
				this.whatever = whatever;
			}

			public void Run()
			{
				ran.value = true;
				Assert.AreSame(whatever, ((EnvironmentsTestCase.IWhatever)Environments.My(typeof(
					EnvironmentsTestCase.IWhatever))));
			}

			private readonly ByRef ran;

			private readonly EnvironmentsTestCase.IWhatever whatever;
		}

		public virtual void TestNestedEnvironments()
		{
			EnvironmentsTestCase.IWhatever whatever = new _IWhatever_38();
			IEnvironment environment1 = new _IEnvironment_40(whatever);
			IEnvironment environment2 = new _IEnvironment_46();
			Environments.RunWith(environment1, new _IRunnable_52(whatever, environment2));
		}

		private sealed class _IWhatever_38 : EnvironmentsTestCase.IWhatever
		{
			public _IWhatever_38()
			{
			}
		}

		private sealed class _IEnvironment_40 : IEnvironment
		{
			public _IEnvironment_40(EnvironmentsTestCase.IWhatever whatever)
			{
				this.whatever = whatever;
			}

			public object Provide(Type service)
			{
				return (object)whatever;
			}

			private readonly EnvironmentsTestCase.IWhatever whatever;
		}

		private sealed class _IEnvironment_46 : IEnvironment
		{
			public _IEnvironment_46()
			{
			}

			public object Provide(Type service)
			{
				return null;
			}
		}

		private sealed class _IRunnable_52 : IRunnable
		{
			public _IRunnable_52(EnvironmentsTestCase.IWhatever whatever, IEnvironment environment2
				)
			{
				this.whatever = whatever;
				this.environment2 = environment2;
			}

			public void Run()
			{
				Assert.AreSame(whatever, ((EnvironmentsTestCase.IWhatever)Environments.My(typeof(
					EnvironmentsTestCase.IWhatever))));
				Environments.RunWith(environment2, new _IRunnable_55());
				Assert.AreSame(whatever, ((EnvironmentsTestCase.IWhatever)Environments.My(typeof(
					EnvironmentsTestCase.IWhatever))));
			}

			private sealed class _IRunnable_55 : IRunnable
			{
				public _IRunnable_55()
				{
				}

				public void Run()
				{
					Assert.IsNull(((EnvironmentsTestCase.IWhatever)Environments.My(typeof(EnvironmentsTestCase.IWhatever
						))));
				}
			}

			private readonly EnvironmentsTestCase.IWhatever whatever;

			private readonly IEnvironment environment2;
		}
	}
}
