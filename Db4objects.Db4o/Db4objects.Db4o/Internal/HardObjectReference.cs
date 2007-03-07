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
	}
}
