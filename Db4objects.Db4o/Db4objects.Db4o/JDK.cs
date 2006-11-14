namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class JDK
	{
		internal virtual Sharpen.Lang.Thread AddShutdownHook(Sharpen.Lang.IRunnable a_runnable
			)
		{
			return null;
		}

		internal virtual Db4objects.Db4o.Types.IDb4oCollections Collections(Db4objects.Db4o.YapStream
			 a_stream)
		{
			return null;
		}

		internal virtual System.Type ConstructorClass()
		{
			return null;
		}

		internal virtual object CreateReferenceQueue()
		{
			return null;
		}

		public virtual object CreateWeakReference(object obj)
		{
			return obj;
		}

		internal virtual object CreateYapRef(object a_queue, Db4objects.Db4o.YapObject a_yapObject
			, object a_object)
		{
			return null;
		}

		internal virtual object Deserialize(byte[] bytes)
		{
			throw new Db4objects.Db4o.Ext.Db4oException(Db4objects.Db4o.Messages.NOT_IMPLEMENTED
				);
		}

		public virtual Db4objects.Db4o.Config4Class ExtendConfiguration(Db4objects.Db4o.Reflect.IReflectClass
			 clazz, Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.Config4Class
			 classConfig)
		{
			return classConfig;
		}

		internal virtual void ForEachCollectionElement(object a_object, Db4objects.Db4o.Foundation.IVisitor4
			 a_visitor)
		{
		}

		internal virtual string Format(Sharpen.Util.Date date, bool showTime)
		{
			return date.ToString();
		}

		internal virtual object GetContextClassLoader()
		{
			return null;
		}

		internal virtual object GetYapRefObject(object a_object)
		{
			return null;
		}

		internal virtual bool IsCollectionTranslator(Db4objects.Db4o.Config4Class a_config
			)
		{
			return false;
		}

		public virtual bool IsConnected(Sharpen.Net.Socket socket)
		{
			return socket != null;
		}

		public virtual int Ver()
		{
			return 1;
		}

		internal virtual void KillYapRef(object obj)
		{
		}

		internal virtual void LockFile(object file)
		{
			lock (this)
			{
			}
		}

		/// <summary>
		/// use for system classes only, since not ClassLoader
		/// or Reflector-aware
		/// </summary>
		internal virtual bool MethodIsAvailable(string className, string methodName, System.Type[]
			 @params)
		{
			return false;
		}

		internal virtual void PollReferenceQueue(Db4objects.Db4o.YapStream a_stream, object
			 a_referenceQueue)
		{
		}

		public virtual void RegisterCollections(Db4objects.Db4o.Reflect.Generic.GenericReflector
			 reflector)
		{
		}

		internal virtual void RemoveShutdownHook(Sharpen.Lang.Thread a_thread)
		{
		}

		public virtual System.Reflection.ConstructorInfo SerializableConstructor(System.Type
			 clazz)
		{
			return null;
		}

		internal virtual byte[] Serialize(object obj)
		{
			throw new Db4objects.Db4o.Ext.Db4oException(Db4objects.Db4o.Messages.NOT_IMPLEMENTED
				);
		}

		internal virtual void SetAccessible(object a_accessible)
		{
		}

		internal virtual bool IsEnum(Db4objects.Db4o.Reflect.IReflector reflector, Db4objects.Db4o.Reflect.IReflectClass
			 clazz)
		{
			return false;
		}

		internal virtual void UnlockFile(object file)
		{
			lock (this)
			{
			}
		}

		public virtual object WeakReferenceTarget(object weakRef)
		{
			return weakRef;
		}

		public virtual Db4objects.Db4o.Reflect.IReflector CreateReflector(object classLoader
			)
		{
			return null;
		}
	}
}
