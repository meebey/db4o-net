using System;
using Sharpen.Lang;

namespace Db4objects.Db4o.Reflect.Net
{
	/// <summary>Reflection implementation for Class to map to .NET reflection.</summary>
	/// <remarks>Reflection implementation for Class to map to .NET reflection.</remarks>
	public class NetClass : Db4objects.Db4o.Reflect.IReflectClass
	{
		private readonly Db4objects.Db4o.Reflect.IReflector _reflector;

		private readonly System.Type _type;

		private Db4objects.Db4o.Reflect.IReflectConstructor _constructor;

		private object[] constructorParams;
		
	    private string _name;
	    
	    private Db4objects.Db4o.Reflect.IReflectField[] _fields;

	    public NetClass(Db4objects.Db4o.Reflect.IReflector reflector, System.Type clazz)
		{
			_reflector = reflector;
			_type = clazz;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass GetComponentType()
		{
			return _reflector.ForClass(_type.GetElementType());
		}

		public virtual Db4objects.Db4o.Reflect.IReflectConstructor[] GetDeclaredConstructors()
		{
			System.Reflection.ConstructorInfo[] constructors = _type.GetConstructors();
			Db4objects.Db4o.Reflect.IReflectConstructor[] reflectors = new Db4objects.Db4o.Reflect.IReflectConstructor
				[constructors.Length];
			for (int i = 0; i < constructors.Length; i++)
			{
				reflectors[i] = new Db4objects.Db4o.Reflect.Net.NetConstructor(_reflector, constructors[i]);
			}
			return reflectors;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectField GetDeclaredField(string name)
		{
			foreach (IReflectField field in GetDeclaredFields())
			{
				if (field.GetName() == name) return field;
			}
			return null;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectField[] GetDeclaredFields()
		{
			if (_fields == null)
			{
				_fields = CreateDeclaredFieldsArray();
			}
			return _fields;
		}
		
		private Db4objects.Db4o.Reflect.IReflectField[] CreateDeclaredFieldsArray()
		{	
			System.Reflection.FieldInfo[] fields = Sharpen.Runtime.GetDeclaredFields(_type);
			Db4objects.Db4o.Reflect.IReflectField[] reflectors = new Db4objects.Db4o.Reflect.IReflectField[fields.Length];
			for (int i = 0; i < reflectors.Length; i++)
			{
				reflectors[i] = new Db4objects.Db4o.Reflect.Net.NetField(_reflector, fields[i]);
			}
			return reflectors;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass GetDelegate()
		{
			return this;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectMethod GetMethod(
			string methodName,
			Db4objects.Db4o.Reflect.IReflectClass[] paramClasses)
		{
			try
			{
				System.Reflection.MethodInfo method = Sharpen.Runtime.GetDeclaredMethod(_type, methodName, Db4objects.Db4o.Reflect.Net.NetReflector
					.ToNative(paramClasses));
				if (method == null)
				{
					return null;
				}
				return new Db4objects.Db4o.Reflect.Net.NetMethod(_reflector, method);
			}
			catch
			{
				return null;
			}
		}

		public virtual string GetName()
		{
            if (_name == null)
            {
                _name = TypeReference.FromType(_type).GetUnversionedName();
            }
            return _name;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass GetSuperclass()
		{
			return _reflector.ForClass(_type.BaseType);
		}

		public virtual bool IsAbstract()
		{
			return _type.IsAbstract;
		}

		public virtual bool IsArray()
		{
			return _type.IsArray;
		}

		public virtual bool IsAssignableFrom(Db4objects.Db4o.Reflect.IReflectClass type)
		{
			if (!(type is Db4objects.Db4o.Reflect.Net.NetClass))
			{
				return false;
			}
			return _type.IsAssignableFrom(((Db4objects.Db4o.Reflect.Net.NetClass)type).GetNetType());
		}

		public virtual bool IsInstance(object obj)
		{
			return _type.IsInstanceOfType(obj);
		}

		public virtual bool IsInterface()
		{
			return _type.IsInterface;
		}

		public virtual bool IsCollection()
		{
			return _reflector.IsCollection(this);
		}

		public virtual bool IsPrimitive()
		{
			return _type.IsPrimitive
			       || _type == typeof(System.DateTime)
			       || _type == typeof(decimal);
		}

		public virtual bool IsSecondClass()
		{
			return IsPrimitive() || _type.IsValueType;
		}

		public virtual object NewInstance()
		{
			try
			{
				if (_constructor == null)
				{
					if (CanCreate(_type)) return System.Activator.CreateInstance(_type);
					return null;
				}
				return _constructor.NewInstance(constructorParams);
			}
			catch
			{
			}
			return null;
		}

		private static bool CanCreate(Type type)
		{
			return !type.IsAbstract && !type.IsInterface;
		}

		public virtual System.Type GetNetType()
		{
			return _type;
		}

		public virtual Db4objects.Db4o.Reflect.IReflector Reflector()
		{
			return _reflector;
		}

		public virtual bool SkipConstructor(bool flag)
		{
#if !CF_1_0 && !CF_2_0
			if (flag)
			{
				IReflectConstructor constructor = new SerializationConstructor(GetNetType());
				if (constructor != null)
				{
					try
					{
						object o = constructor.NewInstance(null);
						if (o != null)
						{
							UseConstructor(constructor, null);
							return true;
						}
					}
					catch
					{
					}
				}
			}
#endif
			UseConstructor(null, null);
			return false;
		}

		public override string ToString()
		{
			return "NetClass(" + _type + ")";
		}

		public virtual void UseConstructor(
			Db4objects.Db4o.Reflect.IReflectConstructor constructor,
			object[] _params)
		{
			_constructor = constructor;
			constructorParams = _params;
		}

		public virtual object[] ToArray(object obj)
		{
			// handled in GenericClass
			return null;
		}
	}
}
