/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class HardObjectReference
	{
		public static readonly Db4objects.Db4o.Internal.HardObjectReference INVALID = new 
			Db4objects.Db4o.Internal.HardObjectReference(null, null);

		public readonly ObjectReference _reference;

		public readonly object _object;

		public HardObjectReference(ObjectReference @ref, object obj)
		{
			_reference = @ref;
			_object = obj;
		}

		public static Db4objects.Db4o.Internal.HardObjectReference PeekPersisted(Transaction
			 trans, int id, int depth)
		{
			ObjectReference @ref = new ObjectReference(id);
			object obj = @ref.PeekPersisted(trans, depth);
			return new Db4objects.Db4o.Internal.HardObjectReference(@ref, obj);
		}
	}
}
