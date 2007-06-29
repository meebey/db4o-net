/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Config
{
	/// <summary>base class for CustomClassHandler, to change some behaviour only</summary>
	public class VanillaClassHandler : ICustomClassHandler
	{
		public virtual bool CanNewInstance()
		{
			return false;
		}

		public virtual object NewInstance()
		{
			return null;
		}

		public virtual IReflectClass ClassSubstitute()
		{
			return null;
		}

		public virtual bool IgnoreAncestor()
		{
			return false;
		}
	}
}
