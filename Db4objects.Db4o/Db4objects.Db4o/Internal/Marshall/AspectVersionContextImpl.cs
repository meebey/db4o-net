/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class AspectVersionContextImpl : IAspectVersionContext
	{
		private readonly int _aspectCount;

		private AspectVersionContextImpl(int count)
		{
			_aspectCount = count;
		}

		public virtual int AspectCount()
		{
			return _aspectCount;
		}

		public virtual void AspectCount(int count)
		{
			throw new InvalidOperationException();
		}

		public static readonly Db4objects.Db4o.Internal.Marshall.AspectVersionContextImpl
			 AlwaysEnabled = new Db4objects.Db4o.Internal.Marshall.AspectVersionContextImpl(
			int.MaxValue);

		public static readonly Db4objects.Db4o.Internal.Marshall.AspectVersionContextImpl
			 CheckAlwaysEnabled = new Db4objects.Db4o.Internal.Marshall.AspectVersionContextImpl
			(int.MaxValue - 1);
	}
}
