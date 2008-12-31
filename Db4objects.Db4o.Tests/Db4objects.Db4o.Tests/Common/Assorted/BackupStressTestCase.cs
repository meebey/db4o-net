/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Assorted;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class BackupStressTestCase : IDb4oTestCase, ITestLifeCycle
	{
		private static bool verbose = false;

		private static bool runOnOldJDK = false;

		private static readonly string File = Path.GetTempFileName();

		private const int Iterations = 5;

		private const int Objects = 50;

		private const int Commits = 10;

		private IObjectContainer _objectContainer;

		private volatile bool _inBackup;

		private volatile bool _noMoreBackups;

		private int _backups;

		private int _commitCounter;

		/// <exception cref="System.Exception"></exception>
		public static void Main(string[] args)
		{
			verbose = true;
			runOnOldJDK = true;
			BackupStressTestCase stressTest = new BackupStressTestCase();
			try
			{
				stressTest.SetUp();
				stressTest.Test();
			}
			finally
			{
				stressTest.TearDown();
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetUp()
		{
			DeleteFile(File);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TearDown()
		{
			DeleteFile(File);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test()
		{
			OpenDatabase();
			try
			{
				RunTestIterations();
			}
			finally
			{
				CloseDatabase();
			}
			CheckBackups();
		}

		/// <exception cref="System.Exception"></exception>
		private void RunTestIterations()
		{
			if (!runOnOldJDK && IsOldJDK())
			{
				Sharpen.Runtime.Out.WriteLine("BackupStressTest is too slow for regression testing on Java JDKs < 1.4"
					);
				return;
			}
			BackupStressIteration iteration = new BackupStressIteration();
			_objectContainer.Store(iteration);
			_objectContainer.Commit();
			Thread backupThread = StartBackupThread();
			for (int i = 1; i <= Iterations; i++)
			{
				for (int obj = 0; obj < Objects; obj++)
				{
					_objectContainer.Store(new BackupStressItem("i" + obj, i));
					_commitCounter++;
					if (_commitCounter >= Commits)
					{
						_objectContainer.Commit();
						_commitCounter = 0;
					}
				}
				iteration.SetCount(i);
				_objectContainer.Store(iteration);
				_objectContainer.Commit();
			}
			_noMoreBackups = true;
			backupThread.Join();
		}

		private Thread StartBackupThread()
		{
			Thread thread = new Thread(new _IRunnable_102(this));
			thread.Start();
			return thread;
		}

		private sealed class _IRunnable_102 : IRunnable
		{
			public _IRunnable_102(BackupStressTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				while (!this._enclosing._noMoreBackups)
				{
					this._enclosing._backups++;
					string fileName = this._enclosing.BackupFile(this._enclosing._backups);
					this._enclosing.DeleteFile(fileName);
					this._enclosing._inBackup = true;
					this._enclosing._objectContainer.Ext().Backup(fileName);
					this._enclosing._inBackup = false;
				}
			}

			private readonly BackupStressTestCase _enclosing;
		}

		private void OpenDatabase()
		{
			DeleteFile(File);
			_objectContainer = Db4oFactory.OpenFile(Config(), File);
		}

		/// <exception cref="System.Exception"></exception>
		private void CloseDatabase()
		{
			while (_inBackup)
			{
				Thread.Sleep(1000);
			}
			_objectContainer.Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void CheckBackups()
		{
			Stdout("BackupStressTest");
			Stdout("Backups created: " + _backups);
			for (int i = 1; i < _backups; i++)
			{
				Stdout("Backup " + i);
				IObjectContainer container = Db4oFactory.OpenFile(Config(), BackupFile(i));
				try
				{
					Stdout("Open successful");
					IQuery q = container.Query();
					q.Constrain(typeof(BackupStressIteration));
					BackupStressIteration iteration = (BackupStressIteration)q.Execute().Next();
					int iterations = iteration.GetCount();
					Stdout("Iterations in backup: " + iterations);
					if (iterations > 0)
					{
						q = container.Query();
						q.Constrain(typeof(BackupStressItem));
						q.Descend("_iteration").Constrain(iteration.GetCount());
						IObjectSet items = q.Execute();
						Assert.AreEqual(Objects, items.Count);
						while (items.HasNext())
						{
							BackupStressItem item = (BackupStressItem)items.Next();
							Assert.AreEqual(iterations, item._iteration);
						}
					}
				}
				finally
				{
					container.Close();
				}
				Stdout("Backup OK");
			}
			Stdout("BackupStressTest " + _backups + " files OK.");
			for (int i = 1; i <= _backups; i++)
			{
				DeleteFile(BackupFile(i));
			}
		}

		private void DeleteFile(string fname)
		{
			File4.Delete(fname);
		}

		private bool IsOldJDK()
		{
			ObjectContainerBase stream = (ObjectContainerBase)_objectContainer;
			return stream.NeedsLockFileThread();
		}

		private string BackupFile(int count)
		{
			return File + count;
		}

		private void Stdout(string @string)
		{
			if (verbose)
			{
				Sharpen.Runtime.Out.WriteLine(@string);
			}
		}

		private IConfiguration Config()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			config.ObjectClass(typeof(BackupStressItem)).ObjectField("_iteration").Indexed(true
				);
			config.ReflectWith(Platform4.ReflectorForType(typeof(BackupStressItem)));
			return config;
		}
	}
}
