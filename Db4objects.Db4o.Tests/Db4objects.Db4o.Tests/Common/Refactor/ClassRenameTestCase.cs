/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Extensions.Util;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Refactor;

namespace Db4objects.Db4o.Tests.Common.Refactor
{
	public class ClassRenameTestCase : AbstractDb4oTestCase, IOptOutDefragSolo
	{
		public static void Main(string[] args)
		{
			new ClassRenameTestCase().RunClientServer();
		}

		public class Original
		{
			public string originalName;

			public Original()
			{
			}

			public Original(string name)
			{
				originalName = name;
			}
		}

		public class Changed
		{
			public string changedName;
		}

		public virtual void Test()
		{
			Store(new ClassRenameTestCase.Original("original"));
			Db().Commit();
			Assert.AreEqual(1, CountOccurences(typeof(ClassRenameTestCase.Original)));
			IObjectClass oc = Fixture().Config().ObjectClass(typeof(ClassRenameTestCase.Original)
				);
			oc.ObjectField("originalName").Rename("changedName");
			oc.Rename(CrossPlatformServices.FullyQualifiedName(typeof(ClassRenameTestCase.Changed)
				));
			Reopen();
			Assert.AreEqual(0, CountOccurences(typeof(ClassRenameTestCase.Original)));
			Assert.AreEqual(1, CountOccurences(typeof(ClassRenameTestCase.Changed)));
			ClassRenameTestCase.Changed changed = (ClassRenameTestCase.Changed)RetrieveOnlyInstance
				(typeof(ClassRenameTestCase.Changed));
			Assert.AreEqual("original", changed.changedName);
		}
	}
}
