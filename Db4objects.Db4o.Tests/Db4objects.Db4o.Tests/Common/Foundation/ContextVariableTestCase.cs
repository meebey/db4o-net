/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Foundation;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class ContextVariableTestCase : ITestCase
	{
		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(ContextVariableTestCase)).Run();
		}

		public virtual void TestSingleThread()
		{
			ContextVariable variable = new ContextVariable();
			CheckVariableBehavior(variable);
		}

		public virtual void TestMultiThread()
		{
			ContextVariable variable = new ContextVariable();
			Collection4 failures = new Collection4();
			variable.With("mine", new _IRunnable_23(this, variable, failures));
			Assert.IsNull(variable.Value());
			Assert.IsTrue(failures.IsEmpty(), failures.ToString());
		}

		private sealed class _IRunnable_23 : IRunnable
		{
			public _IRunnable_23(ContextVariableTestCase _enclosing, ContextVariable variable
				, Collection4 failures)
			{
				this._enclosing = _enclosing;
				this.variable = variable;
				this.failures = failures;
			}

			public void Run()
			{
				Thread[] threads = this._enclosing.CreateThreads(variable, failures);
				this._enclosing.StartAll(threads);
				for (int i = 0; i < 10; ++i)
				{
					Assert.AreEqual("mine", variable.Value());
				}
				this._enclosing.JoinAll(threads);
			}

			private readonly ContextVariableTestCase _enclosing;

			private readonly ContextVariable variable;

			private readonly Collection4 failures;
		}

		private void JoinAll(Thread[] threads)
		{
			for (int i = 0; i < threads.Length; i++)
			{
				try
				{
					threads[i].Join();
				}
				catch (Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
		}

		private void StartAll(Thread[] threads)
		{
			for (int i = 0; i < threads.Length; i++)
			{
				threads[i].Start();
			}
		}

		private Thread[] CreateThreads(ContextVariable variable, Collection4 failures)
		{
			Thread[] threads = new Thread[5];
			for (int i = 0; i < threads.Length; i++)
			{
				threads[i] = new Thread(new _IRunnable_56(this, variable, failures));
			}
			return threads;
		}

		private sealed class _IRunnable_56 : IRunnable
		{
			public _IRunnable_56(ContextVariableTestCase _enclosing, ContextVariable variable
				, Collection4 failures)
			{
				this._enclosing = _enclosing;
				this.variable = variable;
				this.failures = failures;
			}

			public void Run()
			{
				try
				{
					for (int i = 0; i < 10; ++i)
					{
						this._enclosing.CheckVariableBehavior(variable);
					}
				}
				catch (Exception failure)
				{
					lock (failures)
					{
						failures.Add(failure);
					}
				}
			}

			private readonly ContextVariableTestCase _enclosing;

			private readonly ContextVariable variable;

			private readonly Collection4 failures;
		}

		public virtual void TestTypeChecking()
		{
			IRunnable emptyBlock = new _IRunnable_75();
			ContextVariable stringVar = new ContextVariable(typeof(string));
			stringVar.With("foo", emptyBlock);
			Assert.Expect(typeof(ArgumentException), new _ICodeBlock_83(stringVar, emptyBlock
				));
		}

		private sealed class _IRunnable_75 : IRunnable
		{
			public _IRunnable_75()
			{
			}

			public void Run()
			{
			}
		}

		private sealed class _ICodeBlock_83 : ICodeBlock
		{
			public _ICodeBlock_83(ContextVariable stringVar, IRunnable emptyBlock)
			{
				this.stringVar = stringVar;
				this.emptyBlock = emptyBlock;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				stringVar.With(true, emptyBlock);
			}

			private readonly ContextVariable stringVar;

			private readonly IRunnable emptyBlock;
		}

		private void CheckVariableBehavior(ContextVariable variable)
		{
			Assert.IsNull(variable.Value());
			variable.With("foo", new _IRunnable_93(variable));
			Assert.IsNull(variable.Value());
		}

		private sealed class _IRunnable_93 : IRunnable
		{
			public _IRunnable_93(ContextVariable variable)
			{
				this.variable = variable;
			}

			public void Run()
			{
				Assert.AreEqual("foo", variable.Value());
				variable.With("bar", new _IRunnable_96(variable));
				Assert.AreEqual("foo", variable.Value());
			}

			private sealed class _IRunnable_96 : IRunnable
			{
				public _IRunnable_96(ContextVariable variable)
				{
					this.variable = variable;
				}

				public void Run()
				{
					Assert.AreEqual("bar", variable.Value());
				}

				private readonly ContextVariable variable;
			}

			private readonly ContextVariable variable;
		}
	}
}
