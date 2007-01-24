namespace Db4objects.Db4o.Reflect
{
	/// <summary>root of the reflection implementation API.</summary>
	/// <remarks>
	/// root of the reflection implementation API.
	/// <br /><br />The open reflection interface is supplied to allow to implement
	/// reflection functionality on JDKs that do not come with the
	/// java.lang.reflect.* package.<br /><br />
	/// See the code in com.db4o.samples.reflect for a reference implementation
	/// that uses java.lang.reflect.*.
	/// <br /><br />
	/// Use
	/// <see cref="Db4objects.Db4o.Config.IConfiguration.ReflectWith">Db4o.configure().reflectWith(IReflect reflector)
	/// 	</see>
	/// to register the use of your implementation before opening database
	/// files.
	/// </remarks>
	public interface IReflector : Db4objects.Db4o.Foundation.IDeepClone
	{
		/// <summary>returns an IArray object, the equivalent to java.lang.reflect.Array.</summary>
		/// <remarks>returns an IArray object, the equivalent to java.lang.reflect.Array.</remarks>
		Db4objects.Db4o.Reflect.IReflectArray Array();

		/// <summary>specifiy whether parameterized Constructors are supported.</summary>
		/// <remarks>
		/// specifiy whether parameterized Constructors are supported.
		/// <br /><br />The support of Constructors is optional. If Constructors
		/// are not supported, every persistent class needs a public default
		/// constructor with zero parameters.
		/// </remarks>
		bool ConstructorCallsSupported();

		/// <summary>returns an IClass for a Class</summary>
		Db4objects.Db4o.Reflect.IReflectClass ForClass(System.Type clazz);

		/// <summary>
		/// returns an IClass class reflector for a class name or null
		/// if no such class is found
		/// </summary>
		Db4objects.Db4o.Reflect.IReflectClass ForName(string className);

		/// <summary>returns an IClass for an object or null if the passed object is null.</summary>
		/// <remarks>returns an IClass for an object or null if the passed object is null.</remarks>
		Db4objects.Db4o.Reflect.IReflectClass ForObject(object a_object);

		bool IsCollection(Db4objects.Db4o.Reflect.IReflectClass claxx);

		void SetParent(Db4objects.Db4o.Reflect.IReflector reflector);
	}
}
