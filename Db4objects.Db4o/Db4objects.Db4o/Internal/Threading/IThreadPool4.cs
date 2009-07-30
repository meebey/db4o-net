/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Threading
{
	public interface IThreadPool4
	{
		void Start(IRunnable task);

		void StartLowPriority(IRunnable task);

		event System.EventHandler<UncaughtExceptionEventArgs> UncaughtException;

		/// <exception cref="System.Exception"></exception>
		void Join(int timeoutMilliseconds);
	}
}
