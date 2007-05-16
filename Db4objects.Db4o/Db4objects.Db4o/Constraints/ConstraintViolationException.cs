/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Constraints
{
	/// <summary>Base class for all constraint exceptions.</summary>
	/// <remarks>Base class for all constraint exceptions.</remarks>
	[System.Serializable]
	public abstract class ConstraintViolationException : Db4oException
	{
		public ConstraintViolationException(string msg) : base(msg)
		{
		}
	}
}
