namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class BackupStressTestCase : Db4oUnit.ITestLifeCycle
	{
		private static bool verbose = false;

		private static bool runOnOldJDK = false;

		private static readonly string FILE = "backupstress.yap";

		private const int ITERATIONS = 5;

		private const int OBJECTS = 50;

		private const int COMMITS = 10;

		private Db4objects.Db4o.IObjectContainer _objectContainer;

		private volatile bool _inBackup;

		private volatile bool _noMoreBackups;

		private int _backups;

		private int _commitCounter;

		public static void Main(string[] args)
		{
			verbose = true;
			runOnOldJDK = true;
			Db4objects.Db4o.Tests.Common.Assorted.BackupStressTestCase stressTest = new Db4objects.Db4o.Tests.Common.Assorted.BackupStressTestCase
				();
			stressTest.SetUp();
			stressTest.Test();
		}

		public virtual void SetUp()
		{
			Db4objects.Db4o.Db4o.Configure().ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.BackupStressItem)
				).ObjectField("_iteration").Indexed(true);
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
			Db4objects.Db4o.Tests.Common.Assorted.BackupStressIteration iteration = new Db4objects.Db4o.Tests.Common.Assorted.BackupStressIteration
				();
			_objectContainer.Set(iteration);
			_objectContainer.Commit();
			StartBackupThread();
			for (int i = 1; i <= ITERATIONS; i++)
			{
				for (int obj = 0; obj < OBJECTS; obj++)
				{
					_objectContainer.Set(new Db4objects.Db4o.Tests.Common.Assorted.BackupStressItem("i"
						 + obj, i));
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
			new Sharpen.Lang.Thread(new _AnonymousInnerClass91(this)).Start();
		}

		private sealed class _AnonymousInnerClass91 : Sharpen.Lang.IRunnable
		{
			public _AnonymousInnerClass91(BackupStressTestCase _enclosing)
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
					catch (System.IO.IOException e)
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
			_objectContainer = Db4objects.Db4o.Db4o.OpenFile(FILE);
		}

		private void CloseDatabase()
		{
			_noMoreBackups = true;
			while (_inBackup)
			{
				Sharpen.Lang.Thread.Sleep(1000);
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
				Db4objects.Db4o.IObjectContainer container = Db4objects.Db4o.Db4o.OpenFile(BackupFile
					(i));
				try
				{
					Stdout("Open successful");
					Db4objects.Db4o.Query.IQuery q = container.Query();
					q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Assorted.BackupStressIteration));
					Db4objects.Db4o.Tests.Common.Assorted.BackupStressIteration iteration = (Db4objects.Db4o.Tests.Common.Assorted.BackupStressIteration
						)q.Execute().Next();
					int iterations = iteration.GetCount();
					Stdout("Iterations in backup: " + iterations);
					if (iterations > 0)
					{
						q = container.Query();
						q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Assorted.BackupStressItem));
						q.Descend("_iteration").Constrain(iteration.GetCount());
						Db4objects.Db4o.IObjectSet items = q.Execute();
						Db4oUnit.Assert.AreEqual(OBJECTS, items.Size());
						while (items.HasNext())
						{
							Db4objects.Db4o.Tests.Common.Assorted.BackupStressItem item = (Db4objects.Db4o.Tests.Common.Assorted.BackupStressItem
								)items.Next();
							Db4oUnit.Assert.AreEqual(iterations, item._iteration);
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
			Db4objects.Db4o.YapStream stream = (Db4objects.Db4o.YapStream)_objectContainer;
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
