namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericArray
	{
		internal Db4objects.Db4o.Reflect.Generic.GenericClass _clazz;

		internal object[] _data;

		public GenericArray(Db4objects.Db4o.Reflect.Generic.GenericClass clazz, int size)
		{
			_clazz = clazz;
			_data = new object[size];
		}

		internal virtual int GetLength()
		{
			return _data.Length;
		}
	}
}
