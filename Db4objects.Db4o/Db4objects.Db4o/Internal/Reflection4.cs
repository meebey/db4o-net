/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude>
	/// Use the methods in this class for system classes only, since they
	/// are not ClassLoader or Reflector-aware.
	/// TODO: this class should go to foundation.reflect, along with ReflectException and ReflectPlatform
	/// </exclude>
	public class Reflection4
	{
		/// <exception cref="Db4objects.Db4o.Internal.ReflectException"></exception>
		public static object Invoke(object obj, string methodName)
		{
			return Invoke(obj.GetType(), methodName, null, null, obj);
		}

		/// <exception cref="Db4objects.Db4o.Internal.ReflectException"></exception>
		public static object Invoke(object obj, string methodName, object[] @params)
		{
			Type[] paramClasses = new Type[@params.Length];
			for (int i = 0; i < @params.Length; i++)
			{
				paramClasses[i] = @params[i].GetType();
			}
			return Invoke(obj.GetType(), methodName, paramClasses, @params, obj);
		}

		/// <exception cref="Db4objects.Db4o.Internal.ReflectException"></exception>
		public static object Invoke(object obj, string methodName, Type[] paramClasses, object
			[] @params)
		{
			return Invoke(obj.GetType(), methodName, paramClasses, @params, obj);
		}

		/// <exception cref="Db4objects.Db4o.Internal.ReflectException"></exception>
		public static object Invoke(Type clazz, string methodName, Type[] paramClasses, object
			[] @params)
		{
			return Invoke(clazz, methodName, paramClasses, @params, null);
		}

		private static object Invoke(Type clazz, string methodName, Type[] paramClasses, 
			object[] @params, object onObject)
		{
			return Invoke(@params, onObject, GetMethod(clazz, methodName, paramClasses));
		}

		/// <exception cref="Db4objects.Db4o.Internal.ReflectException"></exception>
		public static object Invoke(string className, string methodName, Type[] paramClasses
			, object[] @params, object onObject)
		{
			MethodInfo method = GetMethod(className, methodName, paramClasses);
			return Invoke(@params, onObject, method);
		}

		/// <exception cref="Db4objects.Db4o.Internal.ReflectException"></exception>
		public static object Invoke(object[] @params, object onObject, MethodInfo method)
		{
			if (method == null)
			{
				return null;
			}
			Platform4.SetAccessible(method);
			try
			{
				return method.Invoke(onObject, @params);
			}
			catch (TargetInvocationException e)
			{
				throw new ReflectException(e.InnerException);
			}
			catch (ArgumentException e)
			{
				throw new ReflectException(e);
			}
			catch (MemberAccessException e)
			{
				throw new ReflectException(e);
			}
		}

		/// <summary>calling this method "method" will break C# conversion with the old converter
		/// 	</summary>
		public static MethodInfo GetMethod(string className, string methodName, Type[] paramClasses
			)
		{
			Type clazz = ReflectPlatform.ForName(className);
			if (clazz == null)
			{
				return null;
			}
			return GetMethod(clazz, methodName, paramClasses);
		}

		public static MethodInfo GetMethod(Type clazz, string methodName, Type[] paramClasses
			)
		{
			Type curclazz = clazz;
			while (curclazz != null)
			{
				try
				{
					return Sharpen.Runtime.GetDeclaredMethod(curclazz, methodName, paramClasses);
				}
				catch (Exception)
				{
				}
				curclazz = curclazz.BaseType;
			}
			return null;
		}

		/// <exception cref="Db4objects.Db4o.Internal.ReflectException"></exception>
		public static object Invoke(object obj, string methodName, Type signature, object
			 value)
		{
			return Invoke(obj, methodName, new Type[] { signature }, new object[] { value });
		}

		public static FieldInfo GetField(Type clazz, string name)
		{
			Type curclazz = clazz;
			while (curclazz != null)
			{
				try
				{
					FieldInfo field = Sharpen.Runtime.GetDeclaredField(curclazz, name);
					Platform4.SetAccessible(field);
					return field;
				}
				catch (Exception)
				{
				}
				curclazz = curclazz.BaseType;
			}
			return null;
		}

		/// <exception cref="System.MemberAccessException"></exception>
		public static object GetFieldValue(object obj, string fieldName)
		{
			return GetField(obj.GetType(), fieldName).GetValue(obj);
		}

		public static object NewInstance(object template)
		{
			try
			{
				return System.Activator.CreateInstance(template.GetType());
			}
			catch (Exception e)
			{
				throw new ReflectException(e);
			}
		}
	}
}
