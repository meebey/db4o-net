using Sharpen.Lang;

namespace Db4objects.Db4o.Reflect.Net
{
	public class NetReflector : Db4objects.Db4o.Reflect.IReflector
	{

		private Db4objects.Db4o.Reflect.IReflector _parent;

		private Db4objects.Db4o.Reflect.IReflectArray _array;


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

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForClass(System.Type clazz)
		{
			return new Db4objects.Db4o.Reflect.Net.NetClass(_parent, clazz);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForName(string className)
		{
			System.Type clazz = null;
			try
			{
				clazz = TypeReference.FromString(className).Resolve();
			}
			catch (System.Exception)
			{
			}
			return clazz == null
				? null
				: new Db4objects.Db4o.Reflect.Net.NetClass(_parent, clazz);
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
			if (typeof(System.Collections.ICollection).IsAssignableFrom(
				((Db4objects.Db4o.Reflect.Net.NetClass)candidate).GetNetType()))
			{
				return true;
			}
			return false;
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

		internal static System.Type[] ToNative(Db4objects.Db4o.Reflect.IReflectClass[] claxx)
		{
			System.Type[] clazz = null;
			if (claxx != null)
			{
				clazz = new System.Type[claxx.Length];
				for (int i = 0; i < claxx.Length; i++)
				{
					if (claxx[i] != null)
					{
						clazz[i] = ((Db4objects.Db4o.Reflect.Net.NetClass)claxx[i].GetDelegate()).GetNetType();
					}
				}
			}
			return clazz;
		}

		public virtual void SetParent(IReflector reflector)
		{
			_parent = reflector;
		}
	}
}
