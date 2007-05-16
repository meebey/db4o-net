/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class IncompatibleFileFormatExceptionTestCase : IDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new TestRunner(typeof(IncompatibleFileFormatExceptionTestCase)).Run();
		}

		private static readonly string INCOMPATIBLE_FILE_FORMAT = "IncompatibleFileFormatExceptionTestCase";

		public virtual void SetUp()
		{
			File4.Delete(INCOMPATIBLE_FILE_FORMAT);
			IoAdapter adapter = new RandomAccessFileAdapter();
			adapter = adapter.Open(INCOMPATIBLE_FILE_FORMAT, false, 0);
			adapter.Write(new byte[] { 1, 2, 3 }, 3);
			adapter.Close();
		}

		public virtual void TearDown()
		{
			File4.Delete(INCOMPATIBLE_FILE_FORMAT);
		}

		public virtual void Test()
		{
			Assert.Expect(typeof(IncompatibleFileFormatException), new _AnonymousInnerClass32
				(this));
		}

		private sealed class _AnonymousInnerClass32 : ICodeBlock
		{
			public _AnonymousInnerClass32(IncompatibleFileFormatExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4oFactory.OpenFile(IncompatibleFileFormatExceptionTestCase.INCOMPATIBLE_FILE_FORMAT
					);
			}

			private readonly IncompatibleFileFormatExceptionTestCase _enclosing;
		}
	}
}
