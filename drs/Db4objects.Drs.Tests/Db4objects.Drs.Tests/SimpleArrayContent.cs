/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class SimpleArrayContent
	{
		private string name;

		public SimpleArrayContent()
		{
		}

		public SimpleArrayContent(string name)
		{
			this.name = name;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}
	}
}
