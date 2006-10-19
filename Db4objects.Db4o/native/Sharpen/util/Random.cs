/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System.Collections;

namespace Sharpen.Util {
	public class Random {
		public Random() {
		}
		
		public long NextLong() {
			return Sharpen.Runtime.CurrentTimeMillis();
		}
	}
}