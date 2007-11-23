/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Extensions.Util;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Refactor;

namespace Db4objects.Db4o.Tests.Common.Refactor
{
	public class ClassRenameByConfigTestCase : AbstractDb4oTestCase, IOptOutDefragSolo
	{
		public static void Main(string[] args)
		{
			new ClassRenameByConfigTestCase().RunClientServer();
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

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			Store(new ClassRenameByConfigTestCase.Original("original"));
			Db().Commit();
			Assert.AreEqual(1, CountOccurences(typeof(ClassRenameByConfigTestCase.Original)));
			IObjectClass oc = Fixture().Config().ObjectClass(typeof(ClassRenameByConfigTestCase.Original
				));
			oc.ObjectField("originalName").Rename("changedName");
			oc.Rename(CrossPlatformServices.FullyQualifiedName(typeof(ClassRenameByConfigTestCase.Changed
				)));
			Reopen();
			Assert.AreEqual(0, CountOccurences(typeof(ClassRenameByConfigTestCase.Original)));
			Assert.AreEqual(1, CountOccurences(typeof(ClassRenameByConfigTestCase.Changed)));
			ClassRenameByConfigTestCase.Changed changed = (ClassRenameByConfigTestCase.Changed
				)RetrieveOnlyInstance(typeof(ClassRenameByConfigTestCase.Changed));
			Assert.AreEqual("original", changed.changedName);
		}
	}
}
