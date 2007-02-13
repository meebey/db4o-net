namespace Db4objects.Db4o.Config
{
	/// <summary>interface for custom marshallers.</summary>
	/// <remarks>
	/// interface for custom marshallers.
	/// Custom marshallers can be used for tuning the performance to store
	/// and read objects. Instead of letting db4o do all the marshalling
	/// by detecting the fields on a class and by using reflection, a
	/// custom
	/// <see cref="Db4objects.Db4o.Config.IObjectMarshaller">ObjectMarshaller</see>
	/// allows the
	/// application developer to write the logic how the fields of an
	/// object are converted to a byte[] and back.
	/// <br /><br />To implement a custom marshaller, write a class that
	/// implements the methods of the
	/// <see cref="Db4objects.Db4o.Config.IObjectMarshaller">ObjectMarshaller</see>
	/// interface and register it for your persistent class:<br />
	/// <code>Db4o.configure().objectClass(YourClass.class).marshallWith(yourMarshaller);</code>
	/// </remarks>
	public interface IObjectMarshaller
	{
		/// <summary>
		/// implement to write the values of fields to a byte[] when
		/// an object gets stored.
		/// </summary>
		/// <remarks>
		/// implement to write the values of fields to a byte[] when
		/// an object gets stored.
		/// </remarks>
		/// <param name="obj">the object that is stored</param>
		/// <param name="slot">the byte[] where the fields are to be written</param>
		/// <param name="offset">
		/// the offset position in the byte[] where the first
		/// field value can be written
		/// </param>
		void WriteFields(object obj, byte[] slot, int offset);

		/// <summary>
		/// implement to read the values of fields from a byte[] and
		/// to set them on an object when the object gets instantiated
		/// </summary>
		/// <param name="obj">the object that is instantiated</param>
		/// <param name="slot">the byte[] where the fields are to be read from</param>
		/// <param name="offset">
		/// the offset position in the byte[] where the first
		/// field value is to be read from
		/// </param>
		void ReadFields(object obj, byte[] slot, int offset);

		/// <summary>
		/// return the length the marshalled fields will occupy in the
		/// slot byte[].
		/// </summary>
		/// <remarks>
		/// return the length the marshalled fields will occupy in the
		/// slot byte[]. You may not write beyond this offset when you
		/// store fields.
		/// </remarks>
		/// <returns>the marshalled length of the fields.</returns>
		int MarshalledFieldLength();
	}
}
