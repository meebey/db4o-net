namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// interface to the internal reference that an ObjectContainer
	/// holds for a stored object.
	/// </summary>
	/// <remarks>
	/// interface to the internal reference that an ObjectContainer
	/// holds for a stored object.
	/// </remarks>
	public interface IObjectInfo
	{
		/// <summary>returns the object that is referenced.</summary>
		/// <remarks>
		/// returns the object that is referenced.
		/// <br /><br />This method may return null, if the object has
		/// been garbage collected.
		/// </remarks>
		/// <returns>
		/// the referenced object or null, if the object has
		/// been garbage collected.
		/// </returns>
		object GetObject();

		/// <summary>returns a UUID representation of the referenced object.</summary>
		/// <remarks>
		/// returns a UUID representation of the referenced object.
		/// UUID generation has to be turned on, in order to be able
		/// to use this feature:
		/// <see cref="Db4objects.Db4o.Config.IConfiguration.GenerateUUIDs">Db4objects.Db4o.Config.IConfiguration.GenerateUUIDs
		/// 	</see>
		/// </remarks>
		/// <returns>the UUID of the referenced object.</returns>
		Db4objects.Db4o.Ext.Db4oUUID GetUUID();

		/// <summary>
		/// returns the transaction serial number ("version") the
		/// referenced object was stored with last.
		/// </summary>
		/// <remarks>
		/// returns the transaction serial number ("version") the
		/// referenced object was stored with last.
		/// Version number generation has to be turned on, in order to
		/// be able to use this feature:
		/// <see cref="Db4objects.Db4o.Config.IConfiguration.GenerateVersionNumbers">Db4objects.Db4o.Config.IConfiguration.GenerateVersionNumbers
		/// 	</see>
		/// </remarks>
		/// <returns>the version number.</returns>
		long GetVersion();
	}
}
