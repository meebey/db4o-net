/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4oUnit
{
	[System.Serializable]
	public class AssertionException : Exception
	{
		private const long serialVersionUID = 900088031151055525L;

		public AssertionException(string s) : base(s)
		{
		}
	}
}
