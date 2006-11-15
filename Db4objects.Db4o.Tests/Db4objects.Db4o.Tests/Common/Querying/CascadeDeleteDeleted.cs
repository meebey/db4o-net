namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadeDeleteDeleted : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class CddMember
		{
			public string name;
		}

		public string name;

		public object untypedMember;

		public Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted.CddMember typedMember;

		public CascadeDeleteDeleted()
		{
		}

		public CascadeDeleteDeleted(string name)
		{
			this.name = name;
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(this).CascadeOnDelete(true);
		}

		protected override void Store()
		{
			MembersFirst("membersFirst commit");
			MembersFirst("membersFirst");
			TwoRef("twoRef");
			TwoRef("twoRef commit");
			TwoRef("twoRef delete");
			TwoRef("twoRef delete commit");
		}

		private void MembersFirst(string name)
		{
			Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted cdd = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted
				(name);
			cdd.untypedMember = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted.CddMember
				();
			cdd.typedMember = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted.CddMember
				();
			Db().Set(cdd);
		}

		private void TwoRef(string name)
		{
			Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted cdd = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted
				(name);
			cdd.untypedMember = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted.CddMember
				();
			cdd.typedMember = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted.CddMember
				();
			Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted cdd2 = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted
				(name);
			cdd2.untypedMember = cdd.untypedMember;
			cdd2.typedMember = cdd.typedMember;
			Db().Set(cdd);
			Db().Set(cdd2);
		}

		public virtual void Test()
		{
			TMembersFirst("membersFirst commit");
			TMembersFirst("membersFirst");
			TTwoRef("twoRef");
			TTwoRef("twoRef commit");
			TTwoRef("twoRef delete");
			TTwoRef("twoRef delete commit");
			Db4oUnit.Assert.AreEqual(0, CountOccurences(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted.CddMember)
				));
		}

		private void TMembersFirst(string name)
		{
			bool commit = name.IndexOf("commit") > 1;
			Db4objects.Db4o.Query.IQuery q = NewQuery(this.GetType());
			q.Descend("name").Constrain(name);
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted cdd = (Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted
				)objectSet.Next();
			Db().Delete(cdd.untypedMember);
			Db().Delete(cdd.typedMember);
			if (commit)
			{
				Db().Commit();
			}
			Db().Delete(cdd);
			if (!commit)
			{
				Db().Commit();
			}
		}

		private void TTwoRef(string name)
		{
			bool commit = name.IndexOf("commit") > 1;
			bool delete = name.IndexOf("delete") > 1;
			Db4objects.Db4o.Query.IQuery q = NewQuery(this.GetType());
			q.Descend("name").Constrain(name);
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted cdd = (Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted
				)objectSet.Next();
			Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted cdd2 = (Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteDeleted
				)objectSet.Next();
			if (delete)
			{
				Db().Delete(cdd.untypedMember);
				Db().Delete(cdd.typedMember);
			}
			Db().Delete(cdd);
			if (commit)
			{
				Db().Commit();
			}
			Db().Delete(cdd2);
			if (!commit)
			{
				Db().Commit();
			}
		}
	}
}
