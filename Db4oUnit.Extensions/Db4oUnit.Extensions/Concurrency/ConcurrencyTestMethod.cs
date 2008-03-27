/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Concurrency;
using Db4objects.Db4o.Ext;
using Sharpen.Lang;

namespace Db4oUnit.Extensions.Concurrency
{
	public class ConcurrencyTestMethod : TestMethod
	{
		private Thread[] threads;

		private Exception[] failures;

		public ConcurrencyTestMethod(object instance, MethodInfo method) : base(instance, 
			method)
		{
		}

		/// <exception cref="Exception"></exception>
		protected override void Invoke()
		{
			AbstractDb4oTestCase toTest = (AbstractDb4oTestCase)GetSubject();
			MethodInfo method = GetMethod();
			InvokeConcurrencyMethod(toTest, method);
		}

		/// <exception cref="Exception"></exception>
		private void InvokeConcurrencyMethod(AbstractDb4oTestCase toTest, MethodInfo method
			)
		{
			Type[] parameters = Sharpen.Runtime.GetParameterTypes(method);
			bool hasSequenceParameter = false;
			if (parameters.Length == 2)
			{
				// ExtObjectContainer, seq
				hasSequenceParameter = true;
			}
			int threadCount = toTest.ThreadCount();
			threads = new Thread[threadCount];
			failures = new Exception[threadCount];
			for (int i = 0; i < threadCount; ++i)
			{
				threads[i] = new Thread(new ConcurrencyTestMethod.RunnableTestMethod(this, toTest
					, method, i, hasSequenceParameter));
			}
			// start threads simultaneously
			for (int i = 0; i < threadCount; ++i)
			{
				threads[i].Start();
			}
			// wait for the threads to end
			for (int i = 0; i < threadCount; ++i)
			{
				threads[i].Join();
			}
			// check if any of the threads ended abnormally
			for (int i = 0; i < threadCount; ++i)
			{
				if (failures[i] != null)
				{
					// TODO: show all failures by throwing another kind of exception.
					throw failures[i];
				}
			}
			// check test result
			CheckConcurrencyMethod(toTest, method.Name);
		}

		/// <exception cref="Exception"></exception>
		private void CheckConcurrencyMethod(AbstractDb4oTestCase toTest, string testMethodName
			)
		{
			MethodInfo checkMethod = CheckMethodFor(toTest.GetType(), testMethodName);
			if (null == checkMethod)
			{
				return;
			}
			// pass ExtObjectContainer as a param to check method
			IExtObjectContainer oc = toTest.Fixture().Db();
			try
			{
				checkMethod.Invoke(toTest, new object[] { oc });
			}
			finally
			{
				oc.Close();
			}
		}

		private MethodInfo CheckMethodFor(Type testClass, string testMethodName)
		{
			try
			{
				Type[] types = new Type[] { typeof(IExtObjectContainer) };
				return Sharpen.Runtime.GetDeclaredMethod(testClass, ConcurrencyConventions.CheckMethodNameFor
					(testMethodName), types);
			}
			catch (Exception)
			{
				// if checkMethod is not availble, return as success
				return null;
			}
		}

		internal class RunnableTestMethod : IRunnable
		{
			private AbstractDb4oTestCase toTest;

			private MethodInfo method;

			private int seq;

			private bool showSeq;

			private readonly IDb4oClientServerFixture fixture;

			internal RunnableTestMethod(ConcurrencyTestMethod _enclosing, AbstractDb4oTestCase
				 toTest, MethodInfo method, int seq, bool showSeq)
			{
				this._enclosing = _enclosing;
				this.toTest = toTest;
				this.fixture = this._enclosing.Fixture(toTest);
				this.method = method;
				this.seq = seq;
				this.showSeq = showSeq;
			}

			public virtual void Run()
			{
				AbstractDb4oTestCase.FixtureVariable.With(this.fixture, new _IRunnable_112(this));
			}

			private sealed class _IRunnable_112 : IRunnable
			{
				public _IRunnable_112(RunnableTestMethod _enclosing)
				{
					this._enclosing = _enclosing;
				}

				public void Run()
				{
					this._enclosing.RunMethod();
				}

				private readonly RunnableTestMethod _enclosing;
			}

			private void RunMethod()
			{
				IExtObjectContainer oc = null;
				try
				{
					oc = this.fixture.OpenNewClient();
					object[] args;
					if (this.showSeq)
					{
						args = new object[2];
						args[0] = oc;
						args[1] = this.seq;
					}
					else
					{
						args = new object[1];
						args[0] = oc;
					}
					this.method.Invoke(this.toTest, (object[])args);
				}
				catch (Exception e)
				{
					this._enclosing.failures[this.seq] = e;
				}
				finally
				{
					if (oc != null)
					{
						oc.Close();
					}
				}
			}

			private readonly ConcurrencyTestMethod _enclosing;
		}

		private IDb4oClientServerFixture Fixture(AbstractDb4oTestCase toTest)
		{
			return ((IDb4oClientServerFixture)toTest.Fixture());
		}
	}
}
