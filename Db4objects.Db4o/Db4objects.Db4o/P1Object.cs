/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	/// <summary>Kept for 5.7 migration support (PBootRecord depends on it).</summary>
	/// <remarks>Kept for 5.7 migration support (PBootRecord depends on it).</remarks>
	/// <exclude></exclude>
	/// <persistent></persistent>
	[System.ObsoleteAttribute]
	public class P1Object : IDb4oTypeImpl
	{
		public P1Object()
		{
		}

		public virtual object CreateDefault(Transaction a_trans)
		{
			throw Exceptions4.VirtualException();
		}

		public virtual bool HasClassIndex()
		{
			return false;
		}

		public virtual void SetTrans(Transaction a_trans)
		{
		}

		public virtual void SetObjectReference(ObjectReference a_yapObject)
		{
		}
	}
}
