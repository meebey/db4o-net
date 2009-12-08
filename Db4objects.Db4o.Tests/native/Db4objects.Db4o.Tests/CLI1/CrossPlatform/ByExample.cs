/* Copyright (C) 2009 Versant Inc.   http://www.db4o.com */
namespace Db4objects.Db4o.Tests.CLI1.CrossPlatform
{
	internal class ByExample
	{
		public string Name;
		public ByExample Child;

		public ByExample(string name)
		{
			Name = name;
		}

		public ByExample(string name, ByExample child) : this(name)
		{
			Child = child;
		}
	}
}
