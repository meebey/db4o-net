/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class ContextVariableTestCase : ITestCase
	{
		public virtual void _testSingleThread()
		{
			ContextVariable variable = new ContextVariable();
			Assert.IsNull(variable.Value);
			variable.With("foo", new _IRunnable_15(this, variable));
			Assert.IsNull(variable.Value);
		}

		private sealed class _IRunnable_15 : IRunnable
		{
			public _IRunnable_15(ContextVariableTestCase _enclosing, ContextVariable variable
				)
			{
				this._enclosing = _enclosing;
				this.variable = variable;
			}

			public void Run()
			{
				Assert.AreEqual("foo", variable.Value);
				variable.With("bar", new _IRunnable_18(this, variable));
				Assert.AreEqual("foo", variable.Value);
			}

			private sealed class _IRunnable_18 : IRunnable
			{
				public _IRunnable_18(_IRunnable_15 _enclosing, ContextVariable variable)
				{
					this._enclosing = _enclosing;
					this.variable = variable;
				}

				public void Run()
				{
					Assert.AreEqual("bar", variable.Value);
				}

				private readonly _IRunnable_15 _enclosing;

				private readonly ContextVariable variable;
			}

			private readonly ContextVariableTestCase _enclosing;

			private readonly ContextVariable variable;
		}

		public virtual void TestMultipleThreads()
		{
		}

		public virtual void TestTypeChecking()
		{
			IRunnable emptyBlock = new _IRunnable_35(this);
			ContextVariable stringVar = new ContextVariable(typeof(string));
			stringVar.With("foo", emptyBlock);
			Assert.Expect(typeof(ArgumentException), new _ICodeBlock_43(this, stringVar, emptyBlock
				));
		}

		private sealed class _IRunnable_35 : IRunnable
		{
			public _IRunnable_35(ContextVariableTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
			}

			private readonly ContextVariableTestCase _enclosing;
		}

		private sealed class _ICodeBlock_43 : ICodeBlock
		{
			public _ICodeBlock_43(ContextVariableTestCase _enclosing, ContextVariable stringVar
				, IRunnable emptyBlock)
			{
				this._enclosing = _enclosing;
				this.stringVar = stringVar;
				this.emptyBlock = emptyBlock;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				stringVar.With(true, emptyBlock);
			}

			private readonly ContextVariableTestCase _enclosing;

			private readonly ContextVariable stringVar;

			private readonly IRunnable emptyBlock;
		}
	}
}
