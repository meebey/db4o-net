using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4oUnit.Extensions
{
	public interface IDb4oFixture
	{
		string GetLabel();

		void Open();

		void Close();

		void Reopen();

		void Clean();

		LocalObjectContainer FileSession();

		IExtObjectContainer Db();

		IConfiguration Config();

		bool Accept(Type clazz);

		void Defragment();
	}
}
