namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class CrashSimulatingTestCase : Db4oUnit.ITestCase, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		public string _name;

		public Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase _next;

		private static readonly string PATH = System.IO.Path.Combine(System.IO.Path.GetTempPath
			(), "crashSimulate");

		private static readonly string FILE = System.IO.Path.Combine(PATH, "cs");

		internal const bool LOG = false;

		public CrashSimulatingTestCase()
		{
		}

		public CrashSimulatingTestCase(Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase
			 next_, string name)
		{
			_next = next_;
			_name = name;
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Foundation.IO.File4.Delete(FILE);
			System.IO.Directory.CreateDirectory(PATH);
			CreateFile();
			Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingIoAdapter adapterFactory = new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingIoAdapter
				(new Db4objects.Db4o.IO.RandomAccessFileAdapter());
			Db4objects.Db4o.Db4oFactory.Configure().Io(adapterFactory);
			Db4objects.Db4o.IObjectContainer oc = Db4objects.Db4o.Db4oFactory.OpenFile(FILE);
			Db4objects.Db4o.IObjectSet objectSet = oc.Get(new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase
				(null, "three"));
			oc.Delete(objectSet.Next());
			oc.Set(new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase(null, "four"
				));
			oc.Set(new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase(null, "five"
				));
			oc.Set(new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase(null, "six")
				);
			oc.Set(new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase(null, "seven"
				));
			oc.Set(new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase(null, "eight"
				));
			oc.Set(new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase(null, "nine"
				));
			oc.Commit();
			oc.Close();
			Db4objects.Db4o.Db4oFactory.Configure().Io(new Db4objects.Db4o.IO.RandomAccessFileAdapter
				());
			int count = adapterFactory.batch.WriteVersions(FILE);
			CheckFiles("R", adapterFactory.batch.NumSyncs());
			CheckFiles("W", count);
			Sharpen.Runtime.Out.WriteLine("Total versions: " + count);
		}

		private void CheckFiles(string infix, int count)
		{
			for (int i = 1; i <= count; i++)
			{
				string fileName = FILE + infix + i;
				Db4objects.Db4o.IObjectContainer oc = Db4objects.Db4o.Db4oFactory.OpenFile(fileName
					);
				if (!StateBeforeCommit(oc))
				{
					Db4oUnit.Assert.IsTrue(StateAfterCommit(oc));
				}
				oc.Close();
			}
		}

		private bool StateBeforeCommit(Db4objects.Db4o.IObjectContainer oc)
		{
			return Expect(oc, new string[] { "one", "two", "three" });
		}

		private bool StateAfterCommit(Db4objects.Db4o.IObjectContainer oc)
		{
			return Expect(oc, new string[] { "one", "two", "four", "five", "six", "seven", "eight"
				, "nine" });
		}

		private bool Expect(Db4objects.Db4o.IObjectContainer oc, string[] names)
		{
			Db4objects.Db4o.IObjectSet objectSet = oc.Query(typeof(Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase)
				);
			if (objectSet.Size() != names.Length)
			{
				return false;
			}
			while (objectSet.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase cst = (Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase
					)objectSet.Next();
				bool found = false;
				for (int i = 0; i < names.Length; i++)
				{
					if (cst._name.Equals(names[i]))
					{
						names[i] = null;
						found = true;
						break;
					}
				}
				if (!found)
				{
					return false;
				}
			}
			for (int i = 0; i < names.Length; i++)
			{
				if (names[i] != null)
				{
					return false;
				}
			}
			return true;
		}

		private void CreateFile()
		{
			Db4objects.Db4o.IObjectContainer oc = Db4objects.Db4o.Db4oFactory.OpenFile(FILE);
			for (int i = 0; i < 10; i++)
			{
				oc.Set(new Db4objects.Db4o.Tests.Common.Assorted.SimplestPossibleItem("delme"));
			}
			Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase one = new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase
				(null, "one");
			Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase two = new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase
				(one, "two");
			Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase three = new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingTestCase
				(one, "three");
			oc.Set(one);
			oc.Set(two);
			oc.Set(three);
			oc.Commit();
			Db4objects.Db4o.IObjectSet objectSet = oc.Query(typeof(Db4objects.Db4o.Tests.Common.Assorted.SimplestPossibleItem)
				);
			while (objectSet.HasNext())
			{
				oc.Delete(objectSet.Next());
			}
			oc.Close();
			Db4objects.Db4o.Foundation.IO.File4.Copy(FILE, FILE + "0");
		}

		public override string ToString()
		{
			return _name + " -> " + _next;
		}
	}
}
