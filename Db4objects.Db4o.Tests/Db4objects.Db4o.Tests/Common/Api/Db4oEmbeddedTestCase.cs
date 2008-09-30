/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Common.Api;

namespace Db4objects.Db4o.Tests.Common.Api
{
	public class Db4oEmbeddedTestCase : TestWithTempFile
	{
		public virtual void TestOpenFile()
		{
			IObjectContainer container = Db4oEmbedded.OpenFile(Db4oEmbedded.NewConfiguration(
				), _tempFile);
			try
			{
				Assert.IsTrue(System.IO.File.Exists(_tempFile));
			}
			finally
			{
				container.Close();
			}
		}

		public virtual void TestOpenFileWithNullConfiguration()
		{
			Assert.Expect(typeof(ArgumentNullException), new _ICodeBlock_21(this));
		}

		private sealed class _ICodeBlock_21 : ICodeBlock
		{
			public _ICodeBlock_21(Db4oEmbeddedTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				Db4oEmbedded.OpenFile(null, this._enclosing._tempFile);
			}

			private readonly Db4oEmbeddedTestCase _enclosing;
		}
	}
}
