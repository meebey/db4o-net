/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Null : IIndexable4, IPreparedComparison
	{
		public static readonly Null INSTANCE = new Null();

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

		public virtual object ReadIndexEntry(BufferImpl a_reader)
		{
			return null;
		}

		public virtual void WriteIndexEntry(BufferImpl a_writer, object a_object)
		{
		}

		public virtual void DefragIndexEntry(DefragmentContextImpl context)
		{
		}

		public virtual IPreparedComparison NewPrepareCompare(object obj)
		{
			return new _IPreparedComparison_44(this);
		}

		private sealed class _IPreparedComparison_44 : IPreparedComparison
		{
			public _IPreparedComparison_44(Null _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public int CompareTo(object obj)
			{
				if (obj == null)
				{
					return 0;
				}
				if (obj is Null)
				{
					return 0;
				}
				return -1;
			}

			private readonly Null _enclosing;
		}
	}
}
