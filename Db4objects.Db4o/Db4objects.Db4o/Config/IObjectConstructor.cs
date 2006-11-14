namespace Db4objects.Db4o.Config
{
	/// <summary>interface to allow instantiating objects by calling specific constructors.
	/// 	</summary>
	/// <remarks>
	/// interface to allow instantiating objects by calling specific constructors.
	/// <br /><br /><b>Examples: ../com/db4o/samples/translators.</b><br /><br />
	/// By writing classes that implement this interface, it is possible to
	/// define which constructor is to be used during the instantiation of a stored object.
	/// <br /><br />
	/// Before starting a db4o session, translator classes that implement the
	/// <code>ObjectConstructor</code> or
	/// <see cref="Db4objects.Db4o.Config.IObjectTranslator">ObjectTranslator</see>
	/// need to be registered.<br /><br />
	/// Example:<br />
	/// <code>
	/// Configuration config = Db4o.configure();<br />
	/// ObjectClass oc = config.objectClass("package.className");<br />
	/// oc.translate(new FooTranslator());</code><br /><br />
	/// </remarks>
	public interface IObjectConstructor : Db4objects.Db4o.Config.IObjectTranslator
	{
		/// <summary>db4o calls this method when a stored object needs to be instantiated.</summary>
		/// <remarks>
		/// db4o calls this method when a stored object needs to be instantiated.
		/// <br /><br />
		/// </remarks>
		/// <param name="container">the ObjectContainer used</param>
		/// <param name="storedObject">
		/// the object stored with
		/// <see cref="Db4objects.Db4o.Config.IObjectTranslator.OnStore">ObjectTranslator.onStore
		/// 	</see>
		/// .
		/// </param>
		/// <returns>the instantiated object.</returns>
		object OnInstantiate(Db4objects.Db4o.IObjectContainer container, object storedObject
			);
	}
}
