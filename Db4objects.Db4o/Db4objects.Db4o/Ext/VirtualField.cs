namespace Db4objects.Db4o.Ext
{
	/// <summary>intended for future virtual fields on classes.</summary>
	/// <remarks>
	/// intended for future virtual fields on classes. Currently only
	/// the constant for the virtual version field is found here.
	/// </remarks>
	/// <exclude></exclude>
	public class VirtualField
	{
		/// <summary>
		/// the field name of the virtual version field, to be used
		/// for querying.
		/// </summary>
		/// <remarks>
		/// the field name of the virtual version field, to be used
		/// for querying.
		/// </remarks>
		public static readonly string VERSION = Db4objects.Db4o.YapConst.VIRTUAL_FIELD_PREFIX
			 + "version";
	}
}
