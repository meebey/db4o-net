using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ClassRenameTestCase : AbstractDb4oTestCase
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
			oc.Rename(typeof(ClassRenameTestCase.Changed).FullName);
			Reopen();
			Assert.AreEqual(0, CountOccurences(typeof(ClassRenameTestCase.Original)));
			Assert.AreEqual(1, CountOccurences(typeof(ClassRenameTestCase.Changed)));
			ClassRenameTestCase.Changed changed = (ClassRenameTestCase.Changed)RetrieveOnlyInstance
				(typeof(ClassRenameTestCase.Changed));
			Assert.AreEqual("original", changed.changedName);
		}
	}
}
