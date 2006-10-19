namespace Db4objects.Db4o.Reflect
{
	/// <summary>representation for java.lang.reflect.Array.</summary>
	/// <remarks>
	/// representation for java.lang.reflect.Array.
	/// <br /><br />See the respective documentation in the JDK API.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Reflect.IReflector">Db4objects.Db4o.Reflect.IReflector
	/// 	</seealso>
	public interface IReflectArray
	{
		int[] Dimensions(object arr);

		int Flatten(object a_shaped, int[] a_dimensions, int a_currentDimension, object[]
			 a_flat, int a_flatElement);

		object Get(object onArray, int index);

		Db4objects.Db4o.Reflect.IReflectClass GetComponentType(Db4objects.Db4o.Reflect.IReflectClass
			 a_class);

		int GetLength(object array);

		bool IsNDimensional(Db4objects.Db4o.Reflect.IReflectClass a_class);

		object NewInstance(Db4objects.Db4o.Reflect.IReflectClass componentType, int length
			);

		object NewInstance(Db4objects.Db4o.Reflect.IReflectClass componentType, int[] dimensions
			);

		void Set(object onArray, int index, object element);

		int Shape(object[] a_flat, int a_flatElement, object a_shaped, int[] a_dimensions
			, int a_currentDimension);
	}
}
