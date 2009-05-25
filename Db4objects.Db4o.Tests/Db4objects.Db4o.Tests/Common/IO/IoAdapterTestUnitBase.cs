/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Api;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class IoAdapterTestUnitBase : TestWithTempFile
	{
		protected IoAdapter _adapter;

		public IoAdapterTestUnitBase() : base()
		{
		}

		/// <exception cref="System.Exception"></exception>
		public override void SetUp()
		{
			Open(false);
		}

		protected virtual void Open(bool readOnly)
		{
			if (null != _adapter)
			{
				throw new InvalidOperationException();
			}
			_adapter = Factory().Open(TempFile(), false, 0, readOnly);
		}

		/// <exception cref="System.Exception"></exception>
		public override void TearDown()
		{
			Close();
			base.TearDown();
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
	}
}
