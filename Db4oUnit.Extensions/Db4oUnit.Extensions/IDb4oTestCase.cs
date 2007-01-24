namespace Db4oUnit.Extensions
{
	public interface IDb4oTestCase : Db4oUnit.ITestCase, Db4oUnit.ITestLifeCycle
	{
		/// <summary>returns an ExtObjectContainer as a parameter for test method.</summary>
		/// <remarks>returns an ExtObjectContainer as a parameter for test method.</remarks>
		/// <returns>ExtObjectContainer</returns>
		Db4objects.Db4o.Ext.IExtObjectContainer Db();
	}
}
