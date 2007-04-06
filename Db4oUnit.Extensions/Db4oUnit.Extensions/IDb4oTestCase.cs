using Db4oUnit;
using Db4objects.Db4o.Ext;

namespace Db4oUnit.Extensions
{
	public interface IDb4oTestCase : ITestCase, ITestLifeCycle
	{
		/// <summary>returns an ExtObjectContainer as a parameter for test method.</summary>
		/// <remarks>returns an ExtObjectContainer as a parameter for test method.</remarks>
		/// <returns>ExtObjectContainer</returns>
		IExtObjectContainer Db();
	}
}
