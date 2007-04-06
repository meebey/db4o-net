using System;
using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.Tests.CLI1.Aliases
{
    /// <summary>
	/// </summary>
	public class TypeAliasesTestCase : BaseAliasesTestCase
	{
		public void TestTypeAlias()
		{
		    Db().Set(new Person1("Homer Simpson"));
			Db().Set(new Person1("John Cleese"));

			Reopen();
			Db().Ext().Configure().AddAlias(
				// Person1 instances should be read as Person2 objects
				new TypeAlias(
				GetTypeName(typeof(Person1)),
				GetTypeName(typeof(Person2))));
			AssertAliasedData(Db());
		}

	    private string GetTypeName(Type type)
	    {
	        return type.FullName + ", " + CurrentAssemblyName;
	    }

        private string CurrentAssemblyName
        {
            get { return GetType().Assembly.GetName().Name; }
        }
	}
}
