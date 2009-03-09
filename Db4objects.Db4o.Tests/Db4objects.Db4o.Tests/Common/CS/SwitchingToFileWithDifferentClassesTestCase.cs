/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class SwitchingToFileWithDifferentClassesTestCase : StandaloneCSTestCaseBase
	{
		public class Data1
		{
			public int _id;

			public Data1(int id)
			{
				this._id = id;
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				if (obj == null)
				{
					return false;
				}
				if (GetType() != obj.GetType())
				{
					return false;
				}
				SwitchingToFileWithDifferentClassesTestCase.Data1 other = (SwitchingToFileWithDifferentClassesTestCase.Data1
					)obj;
				if (_id != other._id)
				{
					return false;
				}
				return true;
			}
		}

		public class Data2 : SwitchingToFileWithDifferentClassesTestCase.Data1
		{
			public Data2(int id) : base(id)
			{
			}
		}

		protected override void Configure(IConfiguration config)
		{
		}

		/// <exception cref="System.Exception"></exception>
		protected override void RunTest()
		{
			ClientObjectContainer clientA = OpenClient();
			clientA.Store(new SwitchingToFileWithDifferentClassesTestCase.Data1(1));
			ClientObjectContainer clientB = OpenClient();
			clientB.Store(new SwitchingToFileWithDifferentClassesTestCase.Data1(2));
			clientB.Commit();
			clientB.SwitchToFile(SwitchingFilesFromClientUtil.FilenameB);
			SwitchingToFileWithDifferentClassesTestCase.Data2 data2 = new SwitchingToFileWithDifferentClassesTestCase.Data2
				(3);
			clientA.Store(data2);
			clientA.Commit();
			clientB.SwitchToMainFile();
			ObjectSetAssert.SameContent(clientB.Query(typeof(SwitchingToFileWithDifferentClassesTestCase.Data2
				)), new object[] { data2 });
		}
	}
}
