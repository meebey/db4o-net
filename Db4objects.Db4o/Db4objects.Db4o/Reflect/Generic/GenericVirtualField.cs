namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericVirtualField : Db4objects.Db4o.Reflect.Generic.GenericField
	{
		public GenericVirtualField(string name) : base(name, null, false, false, false)
		{
		}

		public override object DeepClone(object obj)
		{
			return new Db4objects.Db4o.Reflect.Generic.GenericVirtualField(GetName());
		}

		public override object Get(object onObject)
		{
			return null;
		}

		public override Db4objects.Db4o.Reflect.IReflectClass GetFieldType()
		{
			return null;
		}

		public override bool IsPublic()
		{
			return false;
		}

		public override bool IsStatic()
		{
			return true;
		}

		public override bool IsTransient()
		{
			return true;
		}

		public override void Set(object onObject, object value)
		{
		}
	}
}
