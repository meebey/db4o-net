/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Util
{
	public class ThreadServices
	{
		/// <exception cref="System.Exception"></exception>
		public static void SpawnAndJoin(int threadCount, ICodeBlock codeBlock)
		{
			Thread[] threads = new Thread[threadCount];
			for (int i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(new _IRunnable_14(codeBlock));
				threads[i].Start();
			}
			for (int i = 0; i < threads.Length; i++)
			{
				threads[i].Join();
			}
		}

		private sealed class _IRunnable_14 : IRunnable
		{
			public _IRunnable_14(ICodeBlock codeBlock)
			{
				this.codeBlock = codeBlock;
			}

			public void Run()
			{
				try
				{
					codeBlock.Run();
				}
				catch (Exception t)
				{
					Sharpen.Runtime.PrintStackTrace(t);
				}
			}

			private readonly ICodeBlock codeBlock;
		}
	}
}
