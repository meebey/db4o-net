namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericObject
	{
		internal readonly Db4objects.Db4o.Reflect.Generic.GenericClass _class;

		private object[] _values;

		public GenericObject(Db4objects.Db4o.Reflect.Generic.GenericClass clazz)
		{
			_class = clazz;
		}

		private void EnsureValuesInitialized()
		{
			if (_values == null)
			{
				_values = new object[_class.GetFieldCount()];
			}
		}

		public virtual void Set(int index, object value)
		{
			EnsureValuesInitialized();
			_values[index] = value;
		}

		public virtual object Get(int index)
		{
			EnsureValuesInitialized();
			return _values[index];
		}

		public override string ToString()
		{
			if (_class == null)
			{
				return base.ToString();
			}
			return _class.ToString(this);
		}
	}
}
