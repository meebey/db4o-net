namespace Db4objects.Db4o.Reflect.Net
{

	public class NetField : Db4objects.Db4o.Reflect.IReflectField
	{
		private readonly Db4objects.Db4o.Reflect.IReflector reflector;

		private readonly System.Reflection.FieldInfo field;

		public NetField(Db4objects.Db4o.Reflect.IReflector reflector, System.Reflection.FieldInfo field
			)
		{
			this.reflector = reflector;
			this.field = field;
		}

		public virtual string GetName()
		{
			return field.Name;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass GetFieldType()
		{
			return reflector.ForClass(field.FieldType);
		}

		public virtual bool IsPublic()
		{
			return field.IsPublic;
		}

		public virtual bool IsStatic()
		{
			return field.IsStatic;
		}

		public virtual bool IsTransient()
		{
			//return field.I
			return false;
		}

		public virtual void SetAccessible()
		{
			Db4objects.Db4o.Platform4.SetAccessible(field);
		}

		public virtual object Get(object onObject)
		{
			try
			{
				return field.GetValue(onObject);
			}
			catch (System.Exception e)
			{
				return null;
			}
		}

		public virtual void Set(object onObject, object attribute)
		{
			try
			{
				field.SetValue(onObject, attribute);
			}
			catch (System.Exception e)
			{
			}
		}
	}
}
