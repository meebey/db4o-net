/* Copyright (C) 2004 - 2006  db4objects Inc.  http://www.db4o.com */

using System.Collections;

namespace Sharpen.Util {
	public class Random {

		private long _lastValue;

		public long NextLong() {
			long newValue = Sharpen.Runtime.CurrentTimeMillis();
			if(newValue <= _lastValue)
			{
				newValue = _lastValue +1;
			}
			_lastValue = newValue;
			return _lastValue;
		}
	}
}