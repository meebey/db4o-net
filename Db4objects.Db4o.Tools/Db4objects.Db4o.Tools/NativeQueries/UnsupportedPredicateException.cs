/* Copyright (C) 2006   db4objects Inc.   http://www.db4o.com */

namespace Db4oTools.NativeQueries
{
	using System;

	public class UnsupportedPredicateException : Exception
	{
		public UnsupportedPredicateException(string reason)
			: base(reason)
		{
		}
	}
}
