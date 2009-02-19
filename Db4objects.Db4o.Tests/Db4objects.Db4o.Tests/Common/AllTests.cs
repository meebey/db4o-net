/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common
{
	public class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[] { typeof(Db4objects.Db4o.Tests.Common.Acid.AllTests), typeof(Db4objects.Db4o.Tests.Common.Activation.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Api.AllTests), typeof(Db4objects.Db4o.Tests.Common.Assorted.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Btree.AllTests), typeof(Db4objects.Db4o.Tests.Common.Classindex.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Config.AllTests), typeof(Db4objects.Db4o.Tests.Common.Constraints.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.CS.AllTests), typeof(Db4objects.Db4o.Tests.Common.Defragment.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Diagnostics.AllTests), typeof(Db4objects.Db4o.Tests.Common.Events.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Exceptions.AllTests), typeof(Db4objects.Db4o.Tests.Common.Ext.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Fatalerror.AllTests), typeof(Db4objects.Db4o.Tests.Common.Fieldindex.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Foundation.AllTests), typeof(Db4objects.Db4o.Tests.Common.Freespace.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Handlers.AllTests), typeof(Db4objects.Db4o.Tests.Common.Header.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Interfaces.AllTests), typeof(Db4objects.Db4o.Tests.Common.Internal.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.IO.AllTests), typeof(Db4objects.Db4o.Tests.Common.Refactor.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.References.AllTests), typeof(Db4objects.Db4o.Tests.Common.Reflect.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Regression.AllTests), typeof(Db4objects.Db4o.Tests.Common.Querying.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Set.AllTests), typeof(Db4objects.Db4o.Tests.Common.Soda.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Stored.AllTests), typeof(AllCommonTATests
				), typeof(Db4objects.Db4o.Tests.Common.TP.AllTests), typeof(Db4objects.Db4o.Tests.Common.Types.AllTests
				), typeof(Db4objects.Db4o.Tests.Common.Uuid.AllTests), typeof(Db4objects.Db4o.Tests.Util.Test.AllTests
				) };
		}
	}
}
