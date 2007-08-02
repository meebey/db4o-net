/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4objects.Db4o.Tests.Common.Reflect.Custom;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests
{
	public class AllTests : Db4oTestSuite
	{
		public static int Main(string[] args)
		{
#if CF_1_0 || CF_2_0
			return new AllTests().RunSolo();
//            return new AllTests().RunClientServer();
#else
		    return new AllTests().RunAll();
#endif
		}
		
		protected override Type[] TestCases()
		{
			return new Type[]
				{	
#if CF_1_0 || CF_2_0
                    typeof(Db4objects.Db4o.Tests.Common.Acid.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Assorted.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Btree.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Classindex.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Config.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Constraints.AllTests), 
                    //typeof(Db4objects.Db4o.Tests.Common.CS.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Defragment.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Events.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Exceptions.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Ext.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Fatalerror.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Fieldindex.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Foundation.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Freespace.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Handlers.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Header.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Interfaces.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Internal.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.IO.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Reflect.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Regression.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Querying.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Set.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Soda.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Stored.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Common.Types.AllTests), 
                    typeof(Db4objects.Db4o.Tests.Util.Test.AllTests),
#else
                    typeof(Db4objects.Db4o.Tests.Common.AllTests),
#endif
                    typeof(Db4objects.Db4o.Tests.CLI1.AllTests),
					typeof(Db4objects.Db4o.Tests.CLI2.AllTests),
                    typeof(Db4objects.Db4o.Tests.SharpenLang.AllTests),
				};
		}
	}
}
