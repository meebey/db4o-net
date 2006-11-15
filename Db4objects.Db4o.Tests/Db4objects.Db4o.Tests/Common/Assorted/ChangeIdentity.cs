namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ChangeIdentity : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		public virtual void Test()
		{
			byte[] oldSignature = Db().Identity().GetSignature();
			((Db4objects.Db4o.YapFile)Db()).GenerateNewIdentity();
			Reopen();
			Db4oUnit.ArrayAssert.AreNotEqual(oldSignature, Db().Identity().GetSignature());
		}
	}
}
