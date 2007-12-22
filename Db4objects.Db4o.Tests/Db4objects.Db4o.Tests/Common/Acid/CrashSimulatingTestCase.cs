/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Acid;

namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class CrashSimulatingTestCase : ITestCase, IOptOutCS
	{
		public class CrashData
		{
			public string _name;

			public CrashSimulatingTestCase.CrashData _next;

			public CrashData(CrashSimulatingTestCase.CrashData next_, string name)
			{
				_next = next_;
				_name = name;
			}

			public override string ToString()
			{
				return _name + " -> " + _next;
			}
		}

		internal const bool LOG = false;

		private bool HasLockFileThread()
		{
			if (!Platform4.HasLockFileThread())
			{
				return false;
			}
			return !Platform4.HasNio();
		}

		/// <exception cref="IOException"></exception>
		public virtual void Test()
		{
			if (HasLockFileThread())
			{
				Sharpen.Runtime.Out.WriteLine("CrashSimulatingTestCase is ignored on platforms with lock file thread."
					);
				return;
			}
			string path = Path.Combine(Path.GetTempPath(), "crashSimulate");
			string fileName = Path.Combine(path, "cs");
			File4.Delete(fileName);
			System.IO.Directory.CreateDirectory(path);
			CreateFile(BaseConfig(), fileName);
			CrashSimulatingIoAdapter adapterFactory = new CrashSimulatingIoAdapter(new RandomAccessFileAdapter
				());
			IConfiguration recordConfig = BaseConfig();
			recordConfig.Io(adapterFactory);
			IObjectContainer oc = Db4oFactory.OpenFile(recordConfig, fileName);
			IObjectSet objectSet = oc.Get(new CrashSimulatingTestCase.CrashData(null, "three"
				));
			oc.Delete(objectSet.Next());
			oc.Set(new CrashSimulatingTestCase.CrashData(null, "four"));
			oc.Set(new CrashSimulatingTestCase.CrashData(null, "five"));
			oc.Set(new CrashSimulatingTestCase.CrashData(null, "six"));
			oc.Set(new CrashSimulatingTestCase.CrashData(null, "seven"));
			oc.Set(new CrashSimulatingTestCase.CrashData(null, "eight"));
			oc.Set(new CrashSimulatingTestCase.CrashData(null, "nine"));
			oc.Set(new CrashSimulatingTestCase.CrashData(null, "10"));
			oc.Set(new CrashSimulatingTestCase.CrashData(null, "11"));
			oc.Set(new CrashSimulatingTestCase.CrashData(null, "12"));
			oc.Set(new CrashSimulatingTestCase.CrashData(null, "13"));
			oc.Set(new CrashSimulatingTestCase.CrashData(null, "14"));
			oc.Commit();
			IQuery q = oc.Query();
			q.Constrain(typeof(CrashSimulatingTestCase.CrashData));
			objectSet = q.Execute();
			while (objectSet.HasNext())
			{
				CrashSimulatingTestCase.CrashData cData = (CrashSimulatingTestCase.CrashData)objectSet
					.Next();
				if (!(cData._name.Equals("10") || cData._name.Equals("13")))
				{
					oc.Delete(cData);
				}
			}
			oc.Commit();
			oc.Close();
			int count = adapterFactory.batch.WriteVersions(fileName);
			CheckFiles(fileName, "R", adapterFactory.batch.NumSyncs());
			CheckFiles(fileName, "W", count);
		}

		private IConfiguration BaseConfig()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			config.ObjectClass(typeof(CrashSimulatingTestCase.CrashData)).ObjectField("_name"
				).Indexed(true);
			config.ReflectWith(Platform4.ReflectorForType(typeof(CrashSimulatingTestCase)));
			config.BTreeNodeSize(4);
			return config;
		}

		private void CheckFiles(string fileName, string infix, int count)
		{
			for (int i = 1; i <= count; i++)
			{
				string versionedFileName = fileName + infix + i;
				IObjectContainer oc = Db4oFactory.OpenFile(BaseConfig(), versionedFileName);
				try
				{
					if (!StateBeforeCommit(oc))
					{
						if (!StateAfterFirstCommit(oc))
						{
							Assert.IsTrue(StateAfterSecondCommit(oc));
						}
					}
				}
				finally
				{
					oc.Close();
				}
			}
		}

		private bool StateBeforeCommit(IObjectContainer oc)
		{
			return Expect(oc, new string[] { "one", "two", "three" });
		}

		private bool StateAfterFirstCommit(IObjectContainer oc)
		{
			return Expect(oc, new string[] { "one", "two", "four", "five", "six", "seven", "eight"
				, "nine", "10", "11", "12", "13", "14" });
		}

		private bool StateAfterSecondCommit(IObjectContainer oc)
		{
			return Expect(oc, new string[] { "10", "13" });
		}

		private bool Expect(IObjectContainer container, string[] names)
		{
			Collection4 expected = new Collection4(names);
			IObjectSet actual = container.Query(typeof(CrashSimulatingTestCase.CrashData));
			while (actual.HasNext())
			{
				CrashSimulatingTestCase.CrashData current = (CrashSimulatingTestCase.CrashData)actual
					.Next();
				if (null == expected.Remove(current._name))
				{
					return false;
				}
			}
			return expected.IsEmpty();
		}

		/// <exception cref="IOException"></exception>
		private void CreateFile(IConfiguration config, string fileName)
		{
			IObjectContainer oc = Db4oFactory.OpenFile(config, fileName);
			try
			{
				Populate(oc);
			}
			finally
			{
				oc.Close();
			}
			File4.Copy(fileName, fileName + "0");
		}

		private void Populate(IObjectContainer container)
		{
			for (int i = 0; i < 10; i++)
			{
				container.Set(new CrashSimulatingTestCase.Item("delme"));
			}
			CrashSimulatingTestCase.CrashData one = new CrashSimulatingTestCase.CrashData(null
				, "one");
			CrashSimulatingTestCase.CrashData two = new CrashSimulatingTestCase.CrashData(one
				, "two");
			CrashSimulatingTestCase.CrashData three = new CrashSimulatingTestCase.CrashData(one
				, "three");
			container.Set(one);
			container.Set(two);
			container.Set(three);
			container.Commit();
			IObjectSet objectSet = container.Query(typeof(CrashSimulatingTestCase.Item));
			while (objectSet.HasNext())
			{
				container.Delete(objectSet.Next());
			}
		}

		public class Item
		{
			public string name;

			public Item()
			{
			}

			public Item(string name_)
			{
				this.name = name_;
			}

			public virtual string GetName()
			{
				return name;
			}

			public virtual void SetName(string name_)
			{
				name = name_;
			}
		}
	}
}
