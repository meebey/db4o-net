namespace Db4objects.Db4o.Tests.Common.Header
{
	public class IdentityTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Header.IdentityTestCase().RunSolo();
		}

		public virtual void TestIdentityPreserved()
		{
			Db4objects.Db4o.Ext.Db4oDatabase ident = Db().Identity();
			Reopen();
			Db4objects.Db4o.Ext.Db4oDatabase ident2 = Db().Identity();
			Db4oUnit.Assert.IsNotNull(ident);
			Db4oUnit.Assert.AreEqual(ident, ident2);
		}

		public virtual void TestGenerateIdentity()
		{
			byte[] oldSignature = Db().Identity().GetSignature();
			GenerateNewIdentity();
			Reopen();
			Db4oUnit.ArrayAssert.AreNotEqual(oldSignature, Db().Identity().GetSignature());
		}

		private void GenerateNewIdentity()
		{
			((Db4objects.Db4o.Internal.LocalObjectContainer)Db()).GenerateNewIdentity();
		}
	}
}
