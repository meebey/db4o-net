/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class ExternalObjectContainer : ObjectContainerBase, IInternalObjectContainer
	{
		public ExternalObjectContainer(IConfiguration config, ObjectContainerBase parentContainer
			) : base(config, parentContainer)
		{
		}

		public void Activate(object obj, int depth)
		{
			Activate(null, obj, depth);
		}

		public void Bind(object obj, long id)
		{
			Bind(null, obj, id);
		}

		public virtual IDb4oCollections Collections()
		{
			return Collections(null);
		}

		public void Commit()
		{
			Commit(null);
		}

		public void Deactivate(object obj, int depth)
		{
			Deactivate(null, obj, depth);
		}

		public void Delete(object a_object)
		{
			Delete(null, a_object);
		}

		public virtual object Descend(object obj, string[] path)
		{
			return Descend(null, obj, path);
		}

		public virtual IExtObjectContainer Ext()
		{
			return this;
		}

		public IObjectSet Get(object template)
		{
			return Get(null, template);
		}

		public object GetByID(long id)
		{
			return GetByID(null, id);
		}

		public object GetByUUID(Db4oUUID uuid)
		{
			return GetByUUID(null, uuid);
		}

		public long GetID(object obj)
		{
			return GetID(null, obj);
		}

		public IObjectInfo GetObjectInfo(object obj)
		{
			return GetObjectInfo(null, obj);
		}

		public virtual bool IsActive(object obj)
		{
			return IsActive(null, obj);
		}

		public virtual bool IsCached(long id)
		{
			return IsCached(null, id);
		}

		public virtual bool IsStored(object obj)
		{
			return IsStored(null, obj);
		}

		public object PeekPersisted(object obj, int depth, bool committed)
		{
			return PeekPersisted(null, obj, depth, committed);
		}

		public void Purge(object obj)
		{
			Purge(null, obj);
		}

		public virtual IQuery Query()
		{
			return Query((Transaction)null);
		}

		public IObjectSet Query(Type clazz)
		{
			return Get(clazz);
		}

		public IObjectSet Query(Predicate predicate)
		{
			return Query(predicate, (IQueryComparator)null);
		}

		public IObjectSet Query(Predicate predicate, IQueryComparator comparator)
		{
			return Query(null, predicate, comparator);
		}

		public void Refresh(object obj, int depth)
		{
			Refresh(null, obj, depth);
		}

		public void Rollback()
		{
			Rollback(null);
		}

		public void Set(object obj)
		{
			Set(obj, Const4.UNSPECIFIED);
		}

		public void Set(object obj, int depth)
		{
			Set(null, obj, depth);
		}

		public IStoredClass StoredClass(object clazz)
		{
			return StoredClass(null, clazz);
		}

		public virtual IStoredClass[] StoredClasses()
		{
			return StoredClasses(null);
		}

		public abstract void Backup(string path);

		public abstract Db4oDatabase Identity();
	}
}
