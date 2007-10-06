/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
		/// <exception cref="ReflectException"></exception>
		public static object Invoke(object obj, string methodName)
		{
			return Invoke(obj.GetType().FullName, methodName, null, null, obj);
		}

		/// <exception cref="ReflectException"></exception>
		public static object Invoke(object obj, string methodName, object[] @params)
		{
			Type[] paramClasses = new Type[@params.Length];
			for (int i = 0; i < @params.Length; i++)
			{
				paramClasses[i] = @params[i].GetType();
			}
			return Invoke(obj.GetType().FullName, methodName, paramClasses, @params, obj);
		}

		/// <exception cref="ReflectException"></exception>
		public static object Invoke(object obj, string methodName, Type[] paramClasses, object[]
			 @params)
		{
			return Invoke(obj.GetType().FullName, methodName, paramClasses, @params, obj);
		}

		/// <exception cref="ReflectException"></exception>
		public static object Invoke(Type clazz, string methodName, Type[] paramClasses, object[]
			 @params)
		{
			return Invoke(clazz.FullName, methodName, paramClasses, @params, null);
		}

		/// <exception cref="ReflectException"></exception>
		public static object Invoke(string className, string methodName, Type[] paramClasses
			, object[] @params, object onObject)
		{
			MethodInfo method = GetMethod(className, methodName, paramClasses);
			return Invoke(@params, onObject, method);
		}

		/// <exception cref="ReflectException"></exception>
		public static object Invoke(object[] @params, object onObject, MethodInfo method)
		{
			if (method == null)
			{
				return null;
			}
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
			try
			{
				return clazz.GetMethod(methodName, paramClasses);
			}
			catch (Exception)
			{
			}
			return null;
		}
	}
}
