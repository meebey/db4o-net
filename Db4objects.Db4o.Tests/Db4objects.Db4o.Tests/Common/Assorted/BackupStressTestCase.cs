using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Assorted;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class BackupStressTestCase : ITestLifeCycle
	{
		private static bool verbose = false;

		private static bool runOnOldJDK = false;

		private static readonly string FILE = "backupstress.yap";

		private const int ITERATIONS = 5;

		private const int OBJECTS = 50;

		private const int COMMITS = 10;

		private IObjectContainer _objectContainer;

		private volatile bool _inBackup;

		private volatile bool _noMoreBackups;

		private int _backups;

		private int _commitCounter;

		public static void Main(string[] args)
		{
			verbose = true;
			runOnOldJDK = true;
			BackupStressTestCase stressTest = new BackupStressTestCase();
			stressTest.SetUp();
			stressTest.Test();
		}

		public virtual void SetUp()
		{
			Db4oFactory.Configure().ObjectClass(typeof(BackupStressItem)).ObjectField("_iteration"
				).Indexed(true);
		}

		public virtual void TearDown()
		{
		}

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

		private void RunTestIterations()
		{
			if (!runOnOldJDK && IsOldJDK())
			{
				Sharpen.Runtime.Out.WriteLine("BackupStressTest is too slow for regression testing on Java JDKs < 1.4"
					);
				return;
			}
			BackupStressIteration iteration = new BackupStressIteration();
			_objectContainer.Set(iteration);
			_objectContainer.Commit();
			StartBackupThread();
			for (int i = 1; i <= ITERATIONS; i++)
			{
				for (int obj = 0; obj < OBJECTS; obj++)
				{
					_objectContainer.Set(new BackupStressItem("i" + obj, i));
					_commitCounter++;
					if (_commitCounter >= COMMITS)
					{
						_objectContainer.Commit();
						_commitCounter = 0;
					}
				}
				iteration.SetCount(i);
				_objectContainer.Set(iteration);
				_objectContainer.Commit();
			}
		}

		private void StartBackupThread()
		{
			new Thread(new _AnonymousInnerClass92(this)).Start();
		}

		private sealed class _AnonymousInnerClass92 : IRunnable
		{
			public _AnonymousInnerClass92(BackupStressTestCase _enclosing)
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
					try
					{
						this._enclosing._inBackup = true;
						this._enclosing._objectContainer.Ext().Backup(fileName);
						this._enclosing._inBackup = false;
					}
					catch (IOException e)
					{
						Sharpen.Runtime.PrintStackTrace(e);
					}
				}
			}

			private readonly BackupStressTestCase _enclosing;
		}

		private void OpenDatabase()
		{
			DeleteFile(FILE);
			_objectContainer = Db4oFactory.OpenFile(FILE);
		}

		private void CloseDatabase()
		{
			_noMoreBackups = true;
			while (_inBackup)
			{
				Thread.Sleep(1000);
			}
			_objectContainer.Close();
		}

		private void CheckBackups()
		{
			Stdout("BackupStressTest");
			Stdout("Backups created: " + _backups);
			for (int i = 1; i < _backups; i++)
			{
				Stdout("Backup " + i);
				IObjectContainer container = Db4oFactory.OpenFile(BackupFile(i));
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
						Assert.AreEqual(OBJECTS, items.Size());
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
			Sharpen.Runtime.Out.WriteLine("BackupStressTest " + _backups + " files OK.");
			for (int i = 1; i <= _backups; i++)
			{
				DeleteFile(BackupFile(i));
			}
			DeleteFile(FILE);
		}

		private bool DeleteFile(string fname)
		{
			return new Sharpen.IO.File(fname).Delete();
		}

		private bool IsOldJDK()
		{
			ObjectContainerBase stream = (ObjectContainerBase)_objectContainer;
			return stream.NeedsLockFileThread();
		}

		private string BackupFile(int count)
		{
			return string.Empty + count + FILE;
		}

		private void Stdout(string @string)
		{
			if (verbose)
			{
				Sharpen.Runtime.Out.WriteLine(@string);
			}
		}
	}
}
