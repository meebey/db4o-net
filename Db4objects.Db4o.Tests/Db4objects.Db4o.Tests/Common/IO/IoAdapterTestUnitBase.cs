/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class IoAdapterTestUnitBase : ITestLifeCycle
	{
		private readonly string _filename = Path.GetTempFileName();

		protected IoAdapter _adapter;

		public IoAdapterTestUnitBase() : base()
		{
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetUp()
		{
			DeleteTestFile();
			Open(false);
		}

		protected virtual void Open(bool readOnly)
		{
			if (null != _adapter)
			{
				throw new InvalidOperationException();
			}
			_adapter = Factory().Open(_filename, false, 0, readOnly);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TearDown()
		{
			Close();
			DeleteTestFile();
		}

		protected virtual void Close()
		{
			if (null != _adapter)
			{
				_adapter.Close();
				_adapter = null;
			}
		}

		private IoAdapter Factory()
		{
			return ((IoAdapter)SubjectFixtureProvider.Value());
		}

		/// <exception cref="System.Exception"></exception>
		private void DeleteTestFile()
		{
			new Sharpen.IO.File(_filename).Delete();
		}
	}
}
