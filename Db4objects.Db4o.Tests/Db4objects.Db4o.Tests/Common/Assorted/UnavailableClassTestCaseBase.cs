/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class UnavailableClassTestCaseBase : AbstractDb4oTestCase
	{
		public UnavailableClassTestCaseBase() : base()
		{
		}

		/// <exception cref="System.Exception"></exception>
		protected virtual void ReopenHidingClasses(Type[] classes)
		{
			CloseAndResetConfig();
			Fixture().Config().ReflectWith(new ExcludingReflector(classes));
			Open();
		}

		/// <exception cref="System.Exception"></exception>
		private void Open()
		{
			Fixture().Open(GetType());
		}

		/// <exception cref="System.Exception"></exception>
		private void CloseAndResetConfig()
		{
			Fixture().Close();
			Fixture().ResetConfig();
		}
	}
}
