/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Config
{
	/// <summary>configuration interface for classes.</summary>
	/// <remarks>
	/// configuration interface for classes.
	/// <br/><br/>
	/// Use the global Configuration object to configure db4o before opening an
	/// <see cref="IObjectContainer">IObjectContainer</see>
	/// .<br/><br/>
	/// <b>Example:</b><br/>
	/// <code>
	/// IConfiguration config = Db4oFactory.Configure();<br/>
	/// IObjectClass oc = config.ObjectClass("Namespace.ClassName");<br/>
	/// oc.UpdateDepth(3);<br/>
	/// oc.MinimumActivationDepth(3);<br/>
	/// </code>
	/// </remarks>
	public interface IObjectClass
	{
		/// <summary>
		/// advises db4o to try instantiating objects of this class with/without
		/// calling constructors.
		/// </summary>
		/// <remarks>
		/// advises db4o to try instantiating objects of this class with/without
		/// calling constructors.
		/// &lt;br&gt;&lt;br&gt;
		/// Not all JDKs / .NET-environments support this feature. db4o will
		/// attempt, to follow the setting as good as the enviroment supports.
		/// In doing so, it may call implementation-specific features like
		/// sun.reflect.ReflectionFactory#newConstructorForSerialization on the
		/// Sun Java 1.4.x/5 VM (not available on other VMs) and
		/// FormatterServices.GetUninitializedObject() on
		/// the .NET framework (not available on CompactFramework).&lt;br&gt;&lt;br&gt;
		/// This setting may also be set globally for all classes in
		/// <see cref="IConfiguration.CallConstructors">IConfiguration.CallConstructors</see>
		/// .&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">
		/// - specify true, to request calling constructors, specify
		/// false to request &lt;b&gt;not&lt;/b&gt; calling constructors.
		/// </param>
		/// <seealso cref="IConfiguration.CallConstructors">IConfiguration.CallConstructors</seealso>
		void CallConstructor(bool flag);

		/// <summary>sets cascaded activation behaviour.</summary>
		/// <remarks>
		/// sets cascaded activation behaviour.
		/// &lt;br&gt;&lt;br&gt;
		/// Setting cascadeOnActivate to true will result in the activation
		/// of all member objects if an instance of this class is activated.
		/// &lt;br&gt;&lt;br&gt;
		/// The default setting is &lt;b&gt;false&lt;/b&gt;.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// Can be applied to an open ObjectContainer.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">whether activation is to be cascaded to member objects.</param>
		/// <seealso cref="IObjectField.CascadeOnActivate">IObjectField.CascadeOnActivate</seealso>
		/// <seealso cref="IObjectContainer.Activate">IObjectContainer.Activate</seealso>
		/// <seealso cref="IObjectCallbacks">Using callbacks</seealso>
		/// <seealso cref="IConfiguration.ActivationDepth">Why activation?</seealso>
		void CascadeOnActivate(bool flag);

		/// <summary>sets cascaded delete behaviour.</summary>
		/// <remarks>
		/// sets cascaded delete behaviour.
		/// &lt;br&gt;&lt;br&gt;
		/// Setting cascadeOnDelete to true will result in the deletion of
		/// all member objects of instances of this class, if they are
		/// passed to
		/// <see cref="IObjectContainer.Delete">IObjectContainer.Delete</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;
		/// &lt;b&gt;Caution !&lt;/b&gt;&lt;br&gt;
		/// This setting will also trigger deletion of old member objects, on
		/// calls to
		/// <see cref="IObjectContainer.Store">IObjectContainer.Store</see>
		/// .&lt;br&gt;&lt;br&gt;
		/// An example of the behaviour:&lt;br&gt;
		/// &lt;code&gt;
		/// ObjectContainer con;&lt;br&gt;
		/// Bar bar1 = new Bar();&lt;br&gt;
		/// Bar bar2 = new Bar();&lt;br&gt;
		/// foo.bar = bar1;&lt;br&gt;
		/// con.set(foo);  // bar1 is stored as a member of foo&lt;br&gt;
		/// foo.bar = bar2;&lt;br&gt;
		/// con.set(foo);  // bar2 is stored as a member of foo
		/// &lt;/code&gt;&lt;br&gt;The last statement will &lt;b&gt;also&lt;/b&gt; delete bar1 from the
		/// ObjectContainer, no matter how many other stored objects hold references
		/// to bar1.
		/// &lt;br&gt;&lt;br&gt;
		/// The default setting is &lt;b&gt;false&lt;/b&gt;.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">whether deletes are to be cascaded to member objects.</param>
		/// <seealso cref="IObjectField.CascadeOnDelete">IObjectField.CascadeOnDelete</seealso>
		/// <seealso cref="IObjectContainer.Delete">IObjectContainer.Delete</seealso>
		/// <seealso cref="IObjectCallbacks">Using callbacks</seealso>
		void CascadeOnDelete(bool flag);

		/// <summary>sets cascaded update behaviour.</summary>
		/// <remarks>
		/// sets cascaded update behaviour.
		/// &lt;br&gt;&lt;br&gt;
		/// Setting cascadeOnUpdate to true will result in the update
		/// of all member objects if a stored instance of this class is passed
		/// to
		/// <see cref="IObjectContainer.Store">IObjectContainer.Store</see>
		/// .&lt;br&gt;&lt;br&gt;
		/// The default setting is &lt;b&gt;false&lt;/b&gt;.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">whether updates are to be cascaded to member objects.</param>
		/// <seealso cref="IObjectField.CascadeOnUpdate">IObjectField.CascadeOnUpdate</seealso>
		/// <seealso cref="IObjectContainer.Set">IObjectContainer.Set</seealso>
		/// <seealso cref="IObjectCallbacks">Using callbacks</seealso>
		void CascadeOnUpdate(bool flag);

		/// <summary>registers an attribute provider for special query behavior.</summary>
		/// <remarks>
		/// registers an attribute provider for special query behavior.
		/// &lt;br&gt;&lt;br&gt;The query processor will compare the object returned by the
		/// attribute provider instead of the actual object, both for the constraint
		/// and the candidate persistent object.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="attributeProvider">the attribute provider to be used</param>
		[System.ObsoleteAttribute(@"since version 7.0")]
		void Compare(IObjectAttribute attributeProvider);

		/// <summary>
		/// Must be called before databases are created or opened
		/// so that db4o will control versions and generate UUIDs
		/// for objects of this class, which is required for using replication.
		/// </summary>
		/// <remarks>
		/// Must be called before databases are created or opened
		/// so that db4o will control versions and generate UUIDs
		/// for objects of this class, which is required for using replication.
		/// </remarks>
		/// <param name="setting"></param>
		void EnableReplication(bool setting);

		/// <summary>generate UUIDs for stored objects of this class.</summary>
		/// <remarks>
		/// generate UUIDs for stored objects of this class.
		/// This setting should be used before the database is first created.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="setting"></param>
		void GenerateUUIDs(bool setting);

		/// <summary>generate version numbers for stored objects of this class.</summary>
		/// <remarks>
		/// generate version numbers for stored objects of this class.
		/// This setting should be used before the database is first created.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="setting"></param>
		void GenerateVersionNumbers(bool setting);

		/// <summary>turns the class index on or off.</summary>
		/// <remarks>
		/// turns the class index on or off.
		/// &lt;br&gt;&lt;br&gt;db4o maintains an index for each class to be able to
		/// deliver all instances of a class in a query. If the class
		/// index is never needed, it can be turned off with this method
		/// to improve the performance to create and delete objects of
		/// a class.
		/// &lt;br&gt;&lt;br&gt;Common cases where a class index is not needed:&lt;br&gt;
		/// - The application always works with subclasses or superclasses.&lt;br&gt;
		/// - There are convenient field indexes that will always find instances
		/// of a class.&lt;br&gt;
		/// - The application always works with IDs.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		void Indexed(bool flag);

		/// <summary>sets the maximum activation depth to the desired value.</summary>
		/// <remarks>
		/// sets the maximum activation depth to the desired value.
		/// &lt;br&gt;&lt;br&gt;A class specific setting overrides the
		/// <see cref="IConfiguration.ActivationDepth">global setting</see>
		/// &lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="depth">the desired maximum activation depth</param>
		/// <seealso cref="IConfiguration.ActivationDepth">Why activation?</seealso>
		/// <seealso cref="IObjectClass.CascadeOnActivate">IObjectClass.CascadeOnActivate</seealso>
		void MaximumActivationDepth(int depth);

		/// <summary>sets the minimum activation depth to the desired value.</summary>
		/// <remarks>
		/// sets the minimum activation depth to the desired value.
		/// &lt;br&gt;&lt;br&gt;A class specific setting overrides the
		/// <see cref="IConfiguration.ActivationDepth">global setting</see>
		/// &lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="depth">the desired minimum activation depth</param>
		/// <seealso cref="IConfiguration.ActivationDepth">Why activation?</seealso>
		/// <seealso cref="IObjectClass.CascadeOnActivate">IObjectClass.CascadeOnActivate</seealso>
		void MinimumActivationDepth(int depth);

		/// <summary>gets the configured minimum activation depth.</summary>
		/// <remarks>
		/// gets the configured minimum activation depth.
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <returns>the configured minimum activation depth.</returns>
		int MinimumActivationDepth();

		/// <summary>
		/// returns an
		/// <see cref="IObjectField">IObjectField</see>
		/// object
		/// to configure the specified field.
		/// &lt;br&gt;&lt;br&gt;
		/// </summary>
		/// <param name="fieldName">the fieldname of the field to be configured.&lt;br&gt;&lt;br&gt;
		/// 	</param>
		/// <returns>
		/// an instance of an
		/// <see cref="IObjectField">IObjectField</see>
		/// object for configuration.
		/// </returns>
		IObjectField ObjectField(string fieldName);

		/// <summary>turns on storing static field values for this class.</summary>
		/// <remarks>
		/// turns on storing static field values for this class.
		/// &lt;br&gt;&lt;br&gt;By default, static field values of classes are not stored
		/// to the database file. By turning the setting on for a specific class
		/// with this switch, all &lt;b&gt;non-simple-typed&lt;/b&gt; static field values of this
		/// class are stored the first time an object of the class is stored, and
		/// restored, every time a database file is opened afterwards, &lt;b&gt;after
		/// class meta information is loaded for this class&lt;/b&gt; (which can happen
		/// by querying for a class or by loading an instance of a class).&lt;br&gt;&lt;br&gt;
		/// To update a static field value, once it is stored, you have to the following
		/// in this order:&lt;br&gt;
		/// (1) open the database file you are working agains&lt;br&gt;
		/// (2) make sure the class metadata is loaded&lt;br&gt;
		/// &lt;code&gt;objectContainer.query().constrain(Foo.class); // Java&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;objectContainer.Query().Constrain(typeof(Foo)); // C#&lt;/code&gt;&lt;br&gt;
		/// (3) change the static member&lt;br&gt;
		/// (4) store the static member explicitely&lt;br&gt;
		/// &lt;code&gt;objectContainer.set(Foo.staticMember); // C#&lt;/code&gt;
		/// &lt;br&gt;&lt;br&gt;The setting will be ignored for simple types.
		/// &lt;br&gt;&lt;br&gt;Use this setting for constant static object members.
		/// &lt;br&gt;&lt;br&gt;This option will slow down the process of opening database
		/// files and the stored objects will occupy space in the database file.
		/// &lt;br&gt;&lt;br&gt;In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can NOT be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		void PersistStaticFieldValues();

		/// <summary>creates a temporary mapping of a persistent class to a different class.</summary>
		/// <remarks>
		/// creates a temporary mapping of a persistent class to a different class.
		/// &lt;br&gt;&lt;br&gt;If meta information for this ObjectClass has been stored to
		/// the database file, it will be read from the database file as if it
		/// was representing the class specified by the clazz parameter passed to
		/// this method.
		/// The clazz parameter can be any of the following:&lt;br&gt;
		/// - a fully qualified classname as a String.&lt;br&gt;
		/// - a Class object.&lt;br&gt;
		/// - any other object to be used as a template.&lt;br&gt;&lt;br&gt;
		/// This method will be ignored if the database file already contains meta
		/// information for clazz.
		/// </remarks>
		/// <param name="clazz">class name, Class object, or example object.&lt;br&gt;&lt;br&gt;
		/// 	</param>
		[System.ObsoleteAttribute(@"use")]
		void ReadAs(object clazz);

		/// <summary>renames a stored class.</summary>
		/// <remarks>
		/// renames a stored class.
		/// &lt;br&gt;&lt;br&gt;Use this method to refactor classes.
		/// &lt;br&gt;&lt;br&gt;In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can NOT be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="newName">the new fully qualified classname.</param>
		void Rename(string newName);

		/// <summary>allows to specify if transient fields are to be stored.</summary>
		/// <remarks>
		/// allows to specify if transient fields are to be stored.
		/// &lt;br&gt;The default for every class is &lt;code&gt;false&lt;/code&gt;.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">whether or not transient fields are to be stored.</param>
		void StoreTransientFields(bool flag);

		/// <summary>registers a translator for this class.</summary>
		/// <remarks>
		/// registers a translator for this class.
		/// &lt;br&gt;&lt;br&gt;
		/// &lt;br&gt;&lt;br&gt;The use of an
		/// <see cref="IObjectTranslator">IObjectTranslator</see>
		/// is not
		/// compatible with the use of an
		/// internal class ObjectMarshaller.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="translator">
		/// this may be an
		/// <see cref="IObjectTranslator">IObjectTranslator</see>
		/// or an
		/// <see cref="IObjectConstructor">IObjectConstructor</see>
		/// </param>
		/// <seealso cref="IObjectTranslator">IObjectTranslator</seealso>
		/// <seealso cref="IObjectConstructor">IObjectConstructor</seealso>
		void Translate(IObjectTranslator translator);

		/// <summary>specifies the updateDepth for this class.</summary>
		/// <remarks>
		/// specifies the updateDepth for this class.
		/// &lt;br&gt;&lt;br&gt;see the documentation of
		/// <see cref="IObjectContainer.Store">IObjectContainer.Store</see>
		/// for further details.&lt;br&gt;&lt;br&gt;
		/// The default setting is 0: Only the object passed to
		/// <see cref="IObjectContainer.Store">IObjectContainer.Store</see>
		/// will be updated.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="depth">the depth of the desired update for this class.</param>
		/// <seealso cref="IConfiguration.UpdateDepth">IConfiguration.UpdateDepth</seealso>
		/// <seealso cref="IObjectClass.CascadeOnUpdate">IObjectClass.CascadeOnUpdate</seealso>
		/// <seealso cref="IObjectField.CascadeOnUpdate">IObjectField.CascadeOnUpdate</seealso>
		/// <seealso cref="IObjectCallbacks">Using callbacks</seealso>
		void UpdateDepth(int depth);
	}
}
