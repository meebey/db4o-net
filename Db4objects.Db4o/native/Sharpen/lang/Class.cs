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

		private const BindingFlags AllDeclaredMembers = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

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
					throw new ClassNotFoundException(name, ex);
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

		public Constructor[] GetDeclaredConstructors()
		{
			ConstructorInfo[] constructorInfos = _type.GetConstructors(AllDeclaredMembers);
			Constructor[] constructors = new Constructor[constructorInfos.Length];
			for (int i = 0; i < constructorInfos.Length; i++)
			{
				constructors[i] = new Constructor(constructorInfos[i]);
			}
			return constructors;
		}

		public Field GetDeclaredField(String name)
		{
			return GetField(_type.GetField(name, AllDeclaredMembers | BindingFlags.Static));
		}

		public Field[] GetDeclaredFields()
		{
			FieldInfo[] fieldInfos = _type.GetFields(AllDeclaredMembers | BindingFlags.Static);
			Field[] fields = new Field[fieldInfos.Length];
			for (int i = 0; i < fieldInfos.Length; i++)
			{
				fields[i] = GetField(fieldInfos[i]);
			}
			return fields;
		}

		public Method GetDeclaredMethod(String name, Class[] parameterTypes)
		{
			return GetMethod(_type.GetMethod(name, AllDeclaredMembers, null, GetTypes(parameterTypes), null));
		}

		public Method[] GetDeclaredMethods()
		{
			MethodInfo[] methodInfos = _type.GetMethods(AllDeclaredMembers);
			Method[] methods = new Method[methodInfos.Length];
			for (int i = 0; i < methodInfos.Length; i++)
			{
				methods[i] = new Method(methodInfos[i]);
			}
			return methods;
		}

		private Field GetField(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				return null;
			}
			return new Field(fieldInfo, _type.GetEvent(fieldInfo.Name, AllDeclaredMembers));
		}

		public Field GetField(String name)
		{
			return GetField(_type.GetField(name));
		}

		public Method GetMethod(String name, Class[] parameterTypes)
		{
			return GetMethod(_type.GetMethod(name, GetTypes(parameterTypes)));
		}

		public Method[] GetMethods()
		{
			MethodInfo[] methods = _type.GetMethods();
			Method[] result = new Method[methods.Length];
			for (int i = 0; i < methods.Length; ++i)
			{
				result[i] = GetMethod(methods[i]);
			}
			return result;
		}

		private Method GetMethod(MethodInfo methodInfo)
		{
			if (methodInfo == null)
			{
				return null;
			}
			return new Method(methodInfo);
		}

		public int GetModifiers()
		{
			int modifiers = 0;
			if (_type.IsAbstract)
			{
				modifiers |= Modifier.ABSTRACT;
			}
			if (_type.IsPublic || _type.IsNestedPublic)
			{
				modifiers |= Modifier.PUBLIC;
			}
			if (_type.IsNestedPrivate)
			{
				modifiers |= Modifier.PRIVATE;
			}
			if (_type.IsInterface)
			{
				modifiers |= Modifier.INTERFACE;
			}
			return modifiers;
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
