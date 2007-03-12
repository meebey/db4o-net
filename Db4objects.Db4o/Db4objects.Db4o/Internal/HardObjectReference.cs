namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class HardObjectReference
	{
		public static readonly Db4objects.Db4o.Internal.HardObjectReference INVALID = new 
			Db4objects.Db4o.Internal.HardObjectReference(null, null);

		public readonly Db4objects.Db4o.Internal.ObjectReference _reference;

		public readonly object _object;

		public HardObjectReference(Db4objects.Db4o.Internal.ObjectReference @ref, object 
			obj)
		{
			_reference = @ref;
			_object = obj;
		}

		public static Db4objects.Db4o.Internal.HardObjectReference PeekPersisted(Db4objects.Db4o.Internal.Transaction
			 trans, int id, int depth)
		{
			Db4objects.Db4o.Internal.ObjectReference @ref = new Db4objects.Db4o.Internal.ObjectReference
				(id);
			object obj = @ref.PeekPersisted(trans, depth);
			return new Db4objects.Db4o.Internal.HardObjectReference(@ref, obj);
		}
	}
}
