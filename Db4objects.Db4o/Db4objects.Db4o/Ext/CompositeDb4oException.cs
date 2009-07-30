/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Ext
{
	[System.Serializable]
	public class CompositeDb4oException : Exception
	{
		public readonly Exception[] _exceptions;

		public CompositeDb4oException(Exception[] exceptions)
		{
			_exceptions = exceptions;
		}
	}
}
