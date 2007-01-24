namespace Db4objects.Db4o.Types
{
	/// <summary>factory and other methods for database-aware collections.</summary>
	/// <remarks>factory and other methods for database-aware collections.</remarks>
	public interface IDb4oCollections
	{
		/// <summary>creates a new database-aware linked list.</summary>
		/// <remarks>
		/// creates a new database-aware linked list.
		/// <br /><br />Usage:<br />
		/// - declare a <code>java.util.List</code> variable in your persistent class.<br />
		/// - fill this variable with this method.<br /><br />
		/// <b>Example:</b><br /><br />
		/// <code><pre>
		/// class MyClass{
		/// List myList;
		/// }
		/// MyClass myObject = new MyClass();
		/// myObject.myList = objectContainer.ext().collections().newLinkedList();</pre></code><br /><br />
		/// </remarks>
		/// <returns>
		/// 
		/// <see cref="Db4objects.Db4o.Types.IDb4oList">Db4objects.Db4o.Types.IDb4oList</see>
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.Types.IDb4oList">Db4objects.Db4o.Types.IDb4oList</seealso>
		Db4objects.Db4o.Types.IDb4oList NewLinkedList();

		/// <summary>creates a new database-aware HashMap.</summary>
		/// <remarks>
		/// creates a new database-aware HashMap.
		/// <br /><br />
		/// This map will call the hashCode() method on the key objects to calculate the
		/// hash value. Since the hash value is stored to the ObjectContainer, key objects
		/// will have to return the same hashCode() value in every VM session.
		/// <br /><br />
		/// Usage:<br />
		/// - declare a <code>java.util.Map</code> variable in your persistent class.<br />
		/// - fill the variable with this method.<br /><br />
		/// <b>Example:</b><br /><br />
		/// <code><pre>
		/// class MyClass{
		/// Map myMap;
		/// }
		/// MyClass myObject = new MyClass();
		/// myObject.myMap = objectContainer.ext().collections().newHashMap(0);</pre></code><br /><br />
		/// </remarks>
		/// <param name="initialSize">the initial size of the HashMap</param>
		/// <returns>
		/// 
		/// <see cref="Db4objects.Db4o.Types.IDb4oMap">Db4objects.Db4o.Types.IDb4oMap</see>
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.Types.IDb4oMap">Db4objects.Db4o.Types.IDb4oMap</seealso>
		Db4objects.Db4o.Types.IDb4oMap NewHashMap(int initialSize);

		/// <summary>creates a new database-aware IdentityHashMap.</summary>
		/// <remarks>
		/// creates a new database-aware IdentityHashMap.
		/// <br /><br />
		/// Only first class objects already stored to the ObjectContainer (Objects with a db4o ID)
		/// can be used as keys for this type of Map. The internal db4o ID will be used as
		/// the hash value.
		/// <br /><br />
		/// Usage:<br />
		/// - declare a <code>java.util.Map</code> variable in your persistent class.<br />
		/// - fill the variable with this method.<br /><br />
		/// <b>Example:</b><br /><br />
		/// <code><pre>
		/// class MyClass{
		/// Map myMap;
		/// }
		/// MyClass myObject = new MyClass();
		/// myObject.myMap = objectContainer.ext().collections().newIdentityMap(0);</pre></code><br /><br />
		/// </remarks>
		/// <param name="initialSize">the initial size of the HashMap</param>
		/// <returns>
		/// 
		/// <see cref="Db4objects.Db4o.Types.IDb4oMap">Db4objects.Db4o.Types.IDb4oMap</see>
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.Types.IDb4oMap">Db4objects.Db4o.Types.IDb4oMap</seealso>
		Db4objects.Db4o.Types.IDb4oMap NewIdentityHashMap(int initialSize);
	}
}
