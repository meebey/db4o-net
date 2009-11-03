/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.CS;
using Sharpen;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class SetSemaphoreTestCase : Db4oClientServerTestCase, IOptOutSolo
	{
		private static readonly string SemaphoreName = "hi";

		public static void Main(string[] args)
		{
			new SetSemaphoreTestCase().RunNetworking();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test()
		{
			IExtObjectContainer[] clients = new IExtObjectContainer[5];
			clients[0] = Db();
			Assert.IsTrue(clients[0].SetSemaphore(SemaphoreName, 0));
			Assert.IsTrue(clients[0].SetSemaphore(SemaphoreName, 0));
			for (int i = 1; i < clients.Length; i++)
			{
				clients[i] = OpenNewSession();
			}
			Assert.IsFalse(clients[1].SetSemaphore(SemaphoreName, 0));
			clients[0].ReleaseSemaphore(SemaphoreName);
			Assert.IsTrue(clients[1].SetSemaphore(SemaphoreName, 50));
			Assert.IsFalse(clients[0].SetSemaphore(SemaphoreName, 0));
			Assert.IsFalse(clients[2].SetSemaphore(SemaphoreName, 0));
			Thread[] threads = new Thread[clients.Length];
			for (int i = 0; i < clients.Length; i++)
			{
				threads[i] = StartGetAndReleaseThread(clients[i]);
			}
			for (int i = 0; i < threads.Length; i++)
			{
				threads[i].Join();
			}
			EnsureMessageProcessed(clients[0]);
			Assert.IsTrue(clients[0].SetSemaphore(SemaphoreName, 0));
			clients[0].Close();
			threads[2] = StartGetAndReleaseThread(clients[2]);
			threads[1] = StartGetAndReleaseThread(clients[1]);
			threads[1].Join();
			threads[2].Join();
			for (int i = 1; i < 4; i++)
			{
				clients[i].Close();
			}
			clients[4].SetSemaphore(SemaphoreName, 1000);
			clients[4].Close();
		}

		private Thread StartGetAndReleaseThread(IExtObjectContainer client)
		{
			Thread t = new Thread(new SetSemaphoreTestCase.GetAndRelease(client));
			t.Start();
			return t;
		}

		private static void EnsureMessageProcessed(IExtObjectContainer client)
		{
			client.Commit();
			Cool.SleepIgnoringInterruption(50);
		}

		internal class GetAndRelease : IRunnable
		{
			internal IExtObjectContainer _client;

			public GetAndRelease(IExtObjectContainer client)
			{
				this._client = client;
			}

			public virtual void Run()
			{
				long time = Runtime.CurrentTimeMillis();
				Assert.IsTrue(_client.SetSemaphore(SemaphoreName, 50000));
				time = Runtime.CurrentTimeMillis() - time;
				// System.out.println("Time to get semaphore: " + time);
				EnsureMessageProcessed(_client);
				// System.out.println("About to release semaphore.");
				_client.ReleaseSemaphore(SemaphoreName);
			}
		}
	}
}
#endif // !SILVERLIGHT
