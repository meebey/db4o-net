/* Copyright (C) 2009  Versant Inc.   http://www.db4o.com */
#if SILVERLIGHT
using Db4objects.Db4o.Config;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.Tests.Common.Api
{
	public class Db4oTestWithTempFile : TestWithTempFile
	{
		protected IEmbeddedConfiguration NewConfiguration()
		{
			IEmbeddedConfiguration config = Db4oEmbedded.NewConfiguration();
			config.File.Storage = new IsolatedStorageStorage();
			return config;
		}
	}
}
#endif