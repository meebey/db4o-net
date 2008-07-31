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
				_array = new Db4objects.Db4o.Reflect.Net.NetArray(Parent());
			}
			return _array;
		}

		public virtual object DeepClone(object obj)
		{
			return new NetReflector();
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForClass(System.Type forType)
		{
            System.Type underlyingType = GetUnderlyingType(forType);
            if (underlyingType.IsPrimitive && ! Db4objects.Db4o.Internal.NullableArrayHandling.UseOldNetHandling())
            {
                return CreateClass(forType);
            }
            return CreateClass(underlyingType);
		}

		protected virtual Db4objects.Db4o.Reflect.IReflectClass CreateClass(Type type)
		{
			if(type == null)
			{
				return null;
			}
			return new Db4objects.Db4o.Reflect.Net.NetClass(Parent(), this, type);
		}

		private static Type GetUnderlyingType(Type type)
        {	
        	if(type == null)
        	{
        		return null;
        	}
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
			return Parent().ForClass(a_object.GetType());
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
            NetClass netClass = reflectClass.GetDelegate() as NetClass;
            if(netClass == null)
            {
                return null;
            }
			return netClass.GetNetType();
		}

		public virtual void SetParent(IReflector reflector)
		{
			_parent = reflector;
		}

        public virtual void Configuration(Db4objects.Db4o.Reflect.IReflectorConfiguration config)
		{
			_config = config;
		}
		
        public virtual Db4objects.Db4o.Reflect.IReflectorConfiguration Configuration()
		{
			return _config;
		}

		public virtual object NullValue(IReflectClass clazz) 
		{
			return Platform4.NullValue(ToNative(clazz));
		}
		
		private IReflector Parent()
		{
			if(_parent == null)
			{
				return this;
			}
			
			return _parent;
		}
		
	}
}
