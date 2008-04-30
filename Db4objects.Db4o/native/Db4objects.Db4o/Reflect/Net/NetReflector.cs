/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Reflect.Net
{
	public class NetReflector : Db4objects.Db4o.Reflect.IReflector
	{
		protected Db4objects.Db4o.Reflect.IReflector _parent;

		private Db4objects.Db4o.Reflect.IReflectArray _array;
		
		private Db4objects.Db4o.Reflect.IReflectorConfiguration _config;
	    
		public virtual Db4objects.Db4o.Reflect.IReflectArray Array()
		{
			if (_array == null)
			{
				_array = new Db4objects.Db4o.Reflect.Net.NetArray(_parent);
			}
			return _array;
		}

		public virtual bool ConstructorCallsSupported()
		{
			return true;
		}

		public virtual object DeepClone(object obj)
		{
			return new NetReflector();
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForClass(System.Type forType)
		{
            if (Db4objects.Db4o.Internal.NullableArrayHandling.UseOldNetHandling())
            {
                return CreateClass(GetUnderlyingType(forType));
            }
            return CreateClass(forType);
		}

		protected virtual Db4objects.Db4o.Reflect.IReflectClass CreateClass(Type type)
		{
			return new Db4objects.Db4o.Reflect.Net.NetClass(_parent, type, _config);
		}

		private static Type GetUnderlyingType(Type type)
        {	
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return underlyingType;
            }
            return type;
        }

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForName(string className)
		{
			try
			{
				Type type = ReflectPlatform.ForName(className);
				if (type == null) return null;
				return ForClass(type);
			}
			catch
			{
			}
			return null;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForObject(object a_object)
		{
			if (a_object == null)
			{
				return null;
			}
			return _parent.ForClass(a_object.GetType());
		}

		public virtual bool IsCollection(Db4objects.Db4o.Reflect.IReflectClass candidate)
		{
			if (candidate.IsArray())
			{
				return false;
			}
		    NetClass netClass = candidate as NetClass;
            if (null == netClass)
            {
                return false;
            }
		    return typeof(System.Collections.ICollection).IsAssignableFrom(netClass.GetNetType());
		}

		public virtual bool MethodCallsSupported()
		{
			return true;
		}

		public static Db4objects.Db4o.Reflect.IReflectClass[] ToMeta(
			Db4objects.Db4o.Reflect.IReflector reflector,
			System.Type[] clazz)
		{
			Db4objects.Db4o.Reflect.IReflectClass[] claxx = null;
			if (clazz != null)
			{
				claxx = new Db4objects.Db4o.Reflect.IReflectClass[clazz.Length];
				for (int i = 0; i < clazz.Length; i++)
				{
					if (clazz[i] != null)
					{
						claxx[i] = reflector.ForClass(clazz[i]);
					}
				}
			}
			return claxx;
		}

		public static System.Type[] ToNative(Db4objects.Db4o.Reflect.IReflectClass[] claxx)
		{
			System.Type[] clazz = null;
			if (claxx != null)
			{
				clazz = new System.Type[claxx.Length];
				for (int i = 0; i < claxx.Length; i++)
				{
					if (claxx[i] != null)
					{
						IReflectClass reflectClass = claxx[i];
						clazz[i] = ToNative(reflectClass);
					}
				}
			}
			return clazz;
		}

		public static Type ToNative(IReflectClass reflectClass)
		{
			return ((Db4objects.Db4o.Reflect.Net.NetClass)reflectClass.GetDelegate()).GetNetType();
		}

		public virtual void SetParent(IReflector reflector)
		{
			_parent = reflector;
		}

        public virtual void Configuration(Db4objects.Db4o.Reflect.IReflectorConfiguration config)
		{
			_config = config;
		}
	}
}
