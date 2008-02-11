/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Sharpen;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class TimeStampIdGenerator
	{
		private long _last;

		public static long IdToMilliseconds(long id)
		{
			return id >> 15;
		}

		public TimeStampIdGenerator() : this(0)
		{
		}

		public TimeStampIdGenerator(long minimumNext)
		{
			_last = minimumNext;
		}

		public virtual long Generate()
		{
			long t = Runtime.CurrentTimeMillis();
			t = t << 15;
			if (t <= _last)
			{
				_last++;
			}
			else
			{
				_last = t;
			}
			return _last;
		}

		public virtual long Last()
		{
			return _last;
		}

		public virtual bool SetMinimumNext(long newMinimum)
		{
			if (newMinimum <= _last)
			{
				return false;
			}
			_last = newMinimum;
			return true;
		}
	}
}
