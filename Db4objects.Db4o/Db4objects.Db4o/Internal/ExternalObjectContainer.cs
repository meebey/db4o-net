/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Replication;
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

		public void Activate(object obj)
		{
			Activate(null, obj);
		}

		/// <exception cref="DatabaseClosedException"></exception>
		public void Activate(object obj, int depth)
		{
			Activate(null, obj, ActivationDepthProvider().ActivationDepth(depth, ActivationMode
				.Activate));
		}

		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public void Bind(object obj, long id)
		{
			Bind(null, obj, id);
		}

		[System.ObsoleteAttribute]
		public virtual IDb4oCollections Collections()
		{
			return Collections(null);
		}

		/// <exception cref="DatabaseReadOnlyException"></exception>
		/// <exception cref="DatabaseClosedException"></exception>
		public void Commit()
		{
			Commit(null);
		}

		/// <exception cref="DatabaseClosedException"></exception>
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

		/// <exception cref="DatabaseClosedException"></exception>
		[System.ObsoleteAttribute(@"Use")]
		public IObjectSet Get(object template)
		{
			return QueryByExample(template);
		}

		/// <exception cref="DatabaseClosedException"></exception>
		public IObjectSet QueryByExample(object template)
		{
			return Get(null, template);
		}

		/// <exception cref="DatabaseClosedException"></exception>
		/// <exception cref="InvalidIDException"></exception>
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

		/// <exception cref="DatabaseClosedException"></exception>
		public object PeekPersisted(object obj, int depth, bool committed)
		{
			return PeekPersisted(null, obj, ActivationDepthProvider().ActivationDepth(depth, 
				ActivationMode.Peek), committed);
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
			return QueryByExample(clazz);
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

		/// <exception cref="DatabaseClosedException"></exception>
		/// <exception cref="DatabaseReadOnlyException"></exception>
		[System.ObsoleteAttribute(@"Use")]
		public void Set(object obj)
		{
			Store(obj);
		}

		/// <exception cref="DatabaseClosedException"></exception>
		/// <exception cref="DatabaseReadOnlyException"></exception>
		public void Store(object obj)
		{
			Store(obj, Const4.Unspecified);
		}

		/// <exception cref="DatabaseClosedException"></exception>
		/// <exception cref="DatabaseReadOnlyException"></exception>
		[System.ObsoleteAttribute(@"Use")]
		public void Set(object obj, int depth)
		{
			Store(obj, depth);
		}

		/// <exception cref="DatabaseClosedException"></exception>
		/// <exception cref="DatabaseReadOnlyException"></exception>
		public void Store(object obj, int depth)
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

		/// <exception cref="Db4oIOException"></exception>
		/// <exception cref="DatabaseClosedException"></exception>
		/// <exception cref="NotSupportedException"></exception>
		public abstract void Backup(string path);

		public abstract Db4oDatabase Identity();

		/// <param name="peerB"></param>
		/// <param name="conflictHandler"></param>
		[System.ObsoleteAttribute]
		public virtual IReplicationProcess ReplicationBegin(IObjectContainer peerB, IReplicationConflictHandler
			 conflictHandler)
		{
			throw new NotSupportedException();
		}
	}
}
