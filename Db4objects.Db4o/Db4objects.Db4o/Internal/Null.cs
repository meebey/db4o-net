/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Null : IIndexable4
	{
		public static readonly IIndexable4 INSTANCE = new Null();

		public virtual int CompareTo(object a_obj)
		{
			if (a_obj == null)
			{
				return 0;
			}
			return -1;
		}

		public virtual int LinkLength()
		{
			return 0;
		}

		public virtual IComparable4 PrepareComparison(object obj)
		{
			return this;
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer a_reader)
		{
			return null;
		}

		public virtual void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer a_writer, object
			 a_object)
		{
		}

		public virtual void DefragIndexEntry(ReaderPair readers)
		{
		}
	}
}
