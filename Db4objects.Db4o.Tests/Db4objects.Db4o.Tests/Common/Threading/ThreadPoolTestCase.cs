/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Threading;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Threading
{
	public class ThreadPoolTestCase : ITestCase
	{
		internal IThreadPool4 _subject = new ThreadPool4Impl();

		//	ThreadPool4 _subject = new ParkingThreadPool4Impl();
		/// <exception cref="System.Exception"></exception>
		public virtual void TestFailureEvent()
		{
			ByRef executed = ByRef.NewInstance(false);
			Exception exception = new Exception();
			_subject.UncaughtException += new System.EventHandler<UncaughtExceptionEventArgs>
				(new _IEventListener4_19(exception, executed).OnEvent);
			_subject.Start(new _IRunnable_26(exception));
			_subject.Join(1000);
			Assert.IsTrue((((bool)executed.value)));
		}

		private sealed class _IEventListener4_19
		{
			public _IEventListener4_19(Exception exception, ByRef executed)
			{
				this.exception = exception;
				this.executed = executed;
			}

			public void OnEvent(object sender, UncaughtExceptionEventArgs args)
			{
				Assert.AreSame(exception, ((UncaughtExceptionEventArgs)args).Exception);
				executed.value = true;
			}

			private readonly Exception exception;

			private readonly ByRef executed;
		}

		private sealed class _IRunnable_26 : IRunnable
		{
			public _IRunnable_26(Exception exception)
			{
				this.exception = exception;
			}

			public void Run()
			{
				throw exception;
			}

			private readonly Exception exception;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestDaemon()
		{
			ByRef isDaemon = ByRef.NewInstance();
			_subject.StartLowPriority(new _IRunnable_59(isDaemon));
			_subject.Join(1000);
			Assert.IsTrue((((bool)isDaemon.value)));
		}

		private sealed class _IRunnable_59 : IRunnable
		{
			public _IRunnable_59(ByRef isDaemon)
			{
				this.isDaemon = isDaemon;
			}

			public void Run()
			{
				isDaemon.value = Thread.CurrentThread().IsDaemon();
			}

			private readonly ByRef isDaemon;
		}
	}
}
