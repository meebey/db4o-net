/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;

namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class DebugCS
	{
		public static ClientObjectContainer clientStream;

		public static LocalObjectContainer serverStream;

		public static IQueue4 clientMessageQueue;
	}
}
