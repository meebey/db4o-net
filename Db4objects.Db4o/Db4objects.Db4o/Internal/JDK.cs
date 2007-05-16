/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Types;
using Sharpen.Lang;
using Sharpen.Util;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class JDK
	{
		internal virtual Thread AddShutdownHook(IRunnable runnable)
		{
			return null;
		}

		internal virtual IDb4oCollections Collections(ObjectContainerBase session)
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

		internal virtual object CreateYapRef(object queue, ObjectReference @ref, object obj
			)
		{
			return null;
		}

		internal virtual object Deserialize(byte[] bytes)
		{
			throw new Db4oException(Db4objects.Db4o.Internal.Messages.NOT_IMPLEMENTED);
		}

		public virtual Config4Class ExtendConfiguration(IReflectClass clazz, IConfiguration
			 config, Config4Class classConfig)
		{
			return classConfig;
		}

		internal virtual void ForEachCollectionElement(object obj, IVisitor4 visitor)
		{
		}

		internal virtual string Format(Date date, bool showTime)
		{
			return date.ToString();
		}

		internal virtual object GetContextClassLoader()
		{
			return null;
		}

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

		internal virtual void KillYapRef(object obj)
		{
		}

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
		internal virtual bool MethodIsAvailable(string className, string methodName, Type[]
			 @params)
		{
			return false;
		}

		internal virtual void PollReferenceQueue(ObjectContainerBase session, object referenceQueue
			)
		{
		}

		public virtual void RegisterCollections(GenericReflector reflector)
		{
		}

		internal virtual void RemoveShutdownHook(Thread thread)
		{
		}

		public virtual ConstructorInfo SerializableConstructor(Type clazz)
		{
			return null;
		}

		internal virtual byte[] Serialize(object obj)
		{
			throw new Db4oException(Db4objects.Db4o.Internal.Messages.NOT_IMPLEMENTED);
		}

		internal virtual void SetAccessible(object accessibleObject)
		{
		}

		internal virtual bool IsEnum(IReflector reflector, IReflectClass clazz)
		{
			return false;
		}

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

		public virtual IReflector CreateReflector(object classLoader)
		{
			return null;
		}
	}
}
