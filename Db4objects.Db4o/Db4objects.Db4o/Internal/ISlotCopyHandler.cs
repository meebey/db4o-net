/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface ISlotCopyHandler
	{
		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		void ProcessCopy(DefragmentContextImpl context);
	}
}
