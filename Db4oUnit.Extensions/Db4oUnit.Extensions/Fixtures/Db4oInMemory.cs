/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oInMemory : AbstractSoloDb4oFixture
	{
		public Db4oInMemory() : base()
		{
		}

		public Db4oInMemory(IFixtureConfiguration fc) : this()
		{
			FixtureConfiguration(fc);
		}

		public override bool Accept(Type clazz)
		{
			if (!base.Accept(clazz))
			{
				return false;
			}
			if (typeof(IOptOutInMemory).IsAssignableFrom(clazz))
			{
				return false;
			}
			return true;
		}

		private MemoryFile _memoryFile;

		protected override IObjectContainer CreateDatabase(IConfiguration config)
		{
			if (null == _memoryFile)
			{
				_memoryFile = new MemoryFile();
			}
			return ExtDb4oFactory.OpenMemoryFile(config, _memoryFile);
		}

		protected override void DoClean()
		{
			_memoryFile = null;
		}

		public override string Label()
		{
			return BuildLabel("IN-MEMORY");
		}

		/// <exception cref="System.Exception"></exception>
		public override void Defragment()
		{
		}
		// do nothing
		// defragment is file-based for now
	}
}
