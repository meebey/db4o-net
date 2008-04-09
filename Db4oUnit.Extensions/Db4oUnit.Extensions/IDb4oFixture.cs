/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4oUnit.Extensions
{
	public interface IDb4oFixture : ILabeled
	{
		/// <exception cref="Exception"></exception>
		void Open(Type testCaseClass);

		/// <exception cref="Exception"></exception>
		void Close();

		/// <exception cref="Exception"></exception>
		void Reopen(Type testCaseClass);

		void Clean();

		LocalObjectContainer FileSession();

		IExtObjectContainer Db();

		IConfiguration Config();

		bool Accept(Type clazz);

		/// <exception cref="Exception"></exception>
		void Defragment();

		void ConfigureAtRuntime(IRuntimeConfigureAction action);

		void FixtureConfiguration(IFixtureConfiguration configuration);
	}
}
