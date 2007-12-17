/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Marshall
{
	public abstract class StringMarshaller
	{
		public abstract bool InlinedStrings();

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="Db4oIOException"></exception>
		public abstract BufferImpl ReadIndexEntry(StatefulBuffer parentSlot);
	}
}
