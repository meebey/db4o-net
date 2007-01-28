/* Copyright (C) 2005   db4objects Inc.   http://www.db4o.com */

using System;
using System.Collections;
using System.Reflection;
using Db4objects.Db4o.Query;
using Sharpen.Lang.Reflect;

namespace Sharpen.Lang
{
	public class Class
	{
		private static readonly IDictionary _typeToClassMap = new Hashtable();

		private static readonly IDictionary _typeNameToClassMap = new Hashtable();

		private static readonly IList PrimitiveTypes = new Type[] {
				typeof (DateTime), typeof (Decimal)
		};

		private Type _type;
		private String _name;
		private bool _primitive;

		public Class(Type type)
		{
			_type = type;
			_primitive = type.IsPrimitive || PrimitiveTypes.Contains(type);
		}

		public override bool Equals(object obj)
		{
			Class clazz = obj as Class;
			return clazz != null && clazz._type == _type;
		}

		public static Class ForName(String name)
		{
			if (null == name)
			{
				return null;
			}

			lock (_typeNameToClassMap.SyncRoot)
			{
				Class returnValue = (Class) _typeNameToClassMap[name];
				if (null != returnValue)
				{
					return returnValue;
				}

				try
				{
					Type t = TypeReference.FromString(name).Resolve();
					returnValue = GetClassForType(t);
					_typeNameToClassMap[name] = returnValue;
				}
				catch (Exception ex)
				{
					throw new TypeLoadException(name, ex);
				}
				return returnValue;
			}
		}

		public static Class GetClassForObject(object obj)
		{
			return GetClassForType(obj.GetType());
		}

		public static Class GetClassForType(Type forType)
		{
			if (forType == null)
			{
				return null;
            }

#if NET_2_0 || CF_2_0
            Type underlyingType = Nullable.GetUnderlyingType(forType);
            if (underlyingType != null)
            {
                forType = underlyingType;
            }
#endif
			
			lock (_typeToClassMap.SyncRoot)
			{
				Class clazz = (Class) _typeToClassMap[forType];
				if (clazz == null)
				{
					clazz = new Class(forType);
					_typeToClassMap[forType] = clazz;
				}
				return clazz;
			}
		}

		public Class GetComponentType()
		{
			return GetClassForType(_type.GetElementType());
		}
	    
		public String GetName()
		{
			if (_name == null)
			{
				_name = TypeReference.FromType(_type).GetUnversionedName();
			}
			return _name;
		}

		public Type GetNetType()
		{
			return _type;
		}

		public Class GetSuperclass()
		{
			return GetClassForType(_type.BaseType);
		}

		public static Type[] GetTypes(Class[] classes)
		{
			if (classes == null)
			{
				return new Type[] {};
			}
			Type[] types = new Type[classes.Length];
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = classes[i].GetNetType();
			}
			return types;
		}

		public bool IsArray()
		{
			return _type.IsArray;
		}

		public bool IsAssignableFrom(Class clazz)
		{
			return _type.IsAssignableFrom(clazz._type);
		}

		public bool IsInstance(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (_type.IsInterface)
			{
				return _type.IsAssignableFrom(obj.GetType());
			}
			return obj.GetType() == _type;
		}

		public bool IsInterface()
		{
			return _type.IsInterface;
		}

		public bool IsPrimitive()
		{
			return _primitive;
		}

		public Object NewInstance()
		{
			return Activator.CreateInstance(_type);
		}
	}
}
