/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4objects.Db4o.Config;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.Aliases
{
    /// <summary>
	/// </summary>
	public class TypeAliasesTestCase : BaseAliasesTestCase
	{
		public void TestTypeAlias()
		{
		    Db().Store(new Person1("Homer Simpson"));
			Db().Store(new Person1("John Cleese"));

			Reopen();
			Fixture().ConfigureAtRuntime(new AddAliasAction());
            Reopen();
			AssertAliasedData(Db());
		}

		private sealed class AddAliasAction : IRuntimeConfigureAction
		{
			public void Apply(IConfiguration config)
			{
				config.AddAlias(
					// Person1 instances should be read as Person2 objects
					new TypeAlias(
					GetTypeName(typeof(Person1)),
					GetTypeName(typeof(Person2))));
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
}
