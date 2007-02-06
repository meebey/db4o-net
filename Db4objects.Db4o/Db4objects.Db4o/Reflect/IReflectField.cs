namespace Db4objects.Db4o.Reflect
{
	/// <summary>representation for java.lang.reflect.Field.</summary>
	/// <remarks>
	/// representation for java.lang.reflect.Field.
	/// <br /><br />See the respective documentation in the JDK API.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Reflect.IReflector">Db4objects.Db4o.Reflect.IReflector
	/// 	</seealso>
	public interface IReflectField
	{
		object Get(object onObject);

		string GetName();

		Db4objects.Db4o.Reflect.IReflectClass GetFieldType();

		bool IsPublic();

		bool IsStatic();

		bool IsTransient();

		void Set(object onObject, object value);

		void SetAccessible();

		Db4objects.Db4o.Reflect.IReflectClass IndexType();

		object IndexEntry(object orig);
	}
}
