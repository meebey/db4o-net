/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Types;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class JDK
	{
		/// <param name="runnable"></param>
		internal virtual Thread AddShutdownHook(IRunnable runnable)
		{
			return null;
		}

		/// <param name="transaction">TODO</param>
		internal virtual IDb4oCollections Collections(Transaction transaction)
		{
			return null;
		}

		internal virtual Type ConstructorClass()
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

		/// <param name="queue"></param>
		/// <param name="@ref"></param>
		/// <param name="obj"></param>
		internal virtual object CreateActivateObjectReference(object queue, ObjectReference
			 @ref, object obj)
		{
			return null;
		}

		/// <param name="bytes"></param>
		internal virtual object Deserialize(byte[] bytes)
		{
			throw new Db4oException(Db4objects.Db4o.Internal.Messages.NOT_IMPLEMENTED);
		}

		/// <param name="clazz"></param>
		/// <param name="config"></param>
		public virtual Config4Class ExtendConfiguration(IReflectClass clazz, IConfiguration
			 config, Config4Class classConfig)
		{
			return classConfig;
		}

		internal virtual void ForEachCollectionElement(object obj, IVisitor4 visitor)
		{
		}

		/// <param name="showTime"></param>
		internal virtual string Format(DateTime date, bool showTime)
		{
			return date.ToString();
		}

		internal virtual object GetContextClassLoader()
		{
			return null;
		}

		/// <param name="obj"></param>
		internal virtual object GetYapRefObject(object obj)
		{
			return null;
		}

		internal virtual bool IsCollectionTranslator(Config4Class config)
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

		/// <param name="obj"></param>
		internal virtual void KillYapRef(object obj)
		{
		}

		/// <param name="path"></param>
		/// <param name="file"></param>
		internal virtual void LockFile(string path, object file)
		{
			lock (this)
			{
			}
		}

		/// <summary>
		/// use for system classes only, since not ClassLoader
		/// or Reflector-aware
		/// </summary>
		/// <param name="className"></param>
		/// <param name="methodName"></param>
		/// <param name="@params"></param>
		internal virtual bool MethodIsAvailable(string className, string methodName, Type[]
			 @params)
		{
			return false;
		}

		/// <param name="session"></param>
		/// <param name="referenceQueue"></param>
		internal virtual void PollReferenceQueue(ObjectContainerBase session, object referenceQueue
			)
		{
		}

		/// <param name="reflector"></param>
		public virtual void RegisterCollections(GenericReflector reflector)
		{
		}

		/// <param name="thread"></param>
		internal virtual void RemoveShutdownHook(Thread thread)
		{
		}

		/// <param name="clazz"></param>
		public virtual ConstructorInfo SerializableConstructor(Type clazz)
		{
			return null;
		}

		/// <param name="obj"></param>
		internal virtual byte[] Serialize(object obj)
		{
			throw new Db4oException(Db4objects.Db4o.Internal.Messages.NOT_IMPLEMENTED);
		}

		/// <param name="accessibleObject"></param>
		internal virtual void SetAccessible(object accessibleObject)
		{
		}

		/// <param name="reflector"></param>
		/// <param name="clazz"></param>
		internal virtual bool IsEnum(IReflector reflector, IReflectClass clazz)
		{
			return false;
		}

		/// <param name="path"></param>
		/// <param name="file"></param>
		internal virtual void UnlockFile(string path, object file)
		{
			lock (this)
			{
			}
		}

		public virtual object WeakReferenceTarget(object weakRef)
		{
			return weakRef;
		}

		/// <param name="classLoader"></param>
		public virtual IReflector CreateReflector(object classLoader)
		{
			return null;
		}

		/// <param name="clazz"></param>
		public virtual IReflector ReflectorForType(Type clazz)
		{
			return null;
		}

		/// <param name="container"></param>
		public virtual NetTypeHandler[] Types(ObjectContainerBase container)
		{
			return new NetTypeHandler[] {  };
		}
	}
}
