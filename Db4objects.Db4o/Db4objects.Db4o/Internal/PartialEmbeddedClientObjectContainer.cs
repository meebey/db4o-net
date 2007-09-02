/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Callbacks;
using Db4objects.Db4o.Internal.Query;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Replication;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class PartialEmbeddedClientObjectContainer : ITransientClass, IObjectContainerSpec
	{
		protected readonly LocalObjectContainer _server;

		protected readonly Db4objects.Db4o.Internal.Transaction _transaction;

		private bool _closed = false;

		public PartialEmbeddedClientObjectContainer(LocalObjectContainer server, Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			_server = server;
			_transaction = trans;
			_transaction.SetOutSideRepresentation(Cast(this));
		}

		public PartialEmbeddedClientObjectContainer(LocalObjectContainer server) : this(server
			, server.NewTransaction(server.SystemTransaction(), server.CreateReferenceSystem
			()))
		{
		}

		/// <param name="path"></param>
		public virtual void Backup(string path)
		{
			throw new NotSupportedException();
		}

		public virtual void Bind(object obj, long id)
		{
			_server.Bind(_transaction, obj, id);
		}

		public virtual IDb4oCollections Collections()
		{
			return _server.Collections(_transaction);
		}

		public virtual IConfiguration Configure()
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.Configure();
			}
		}

		public virtual object Descend(object obj, string[] path)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.Descend(_transaction, obj, path);
			}
		}

		private void CheckClosed()
		{
			if (IsClosed())
			{
				throw new DatabaseClosedException();
			}
		}

		public virtual object GetByID(long id)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.GetByID(_transaction, id);
			}
		}

		public virtual object GetByUUID(Db4oUUID uuid)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.GetByUUID(_transaction, uuid);
			}
		}

		public virtual long GetID(object obj)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.GetID(_transaction, obj);
			}
		}

		public virtual IObjectInfo GetObjectInfo(object obj)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.GetObjectInfo(_transaction, obj);
			}
		}

		public virtual Db4oDatabase Identity()
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.Identity();
			}
		}

		public virtual bool IsActive(object obj)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.IsActive(_transaction, obj);
			}
		}

		public virtual bool IsCached(long id)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.IsCached(_transaction, id);
			}
		}

		public virtual bool IsClosed()
		{
			lock (Lock())
			{
				return _closed == true;
			}
		}

		public virtual bool IsStored(object obj)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.IsStored(_transaction, obj);
			}
		}

		public virtual IReflectClass[] KnownClasses()
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.KnownClasses();
			}
		}

		public virtual object Lock()
		{
			return _server.Lock();
		}

		/// <param name="objectContainer"></param>
		public virtual void MigrateFrom(IObjectContainer objectContainer)
		{
			throw new NotSupportedException();
		}

		public virtual object PeekPersisted(object @object, int depth, bool committed)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.PeekPersisted(_transaction, @object, depth, committed);
			}
		}

		public virtual void Purge()
		{
			lock (Lock())
			{
				CheckClosed();
				_server.Purge();
			}
		}

		public virtual void Purge(object obj)
		{
			lock (Lock())
			{
				CheckClosed();
				_server.Purge(_transaction, obj);
			}
		}

		public virtual GenericReflector Reflector()
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.Reflector();
			}
		}

		public virtual void Refresh(object obj, int depth)
		{
			lock (Lock())
			{
				CheckClosed();
				_server.Refresh(_transaction, obj, depth);
			}
		}

		public virtual void ReleaseSemaphore(string name)
		{
			lock (Lock())
			{
				CheckClosed();
				_server.ReleaseSemaphore(_transaction, name);
			}
		}

		/// <param name="peerB"></param>
		/// <param name="conflictHandler"></param>
		[System.ObsoleteAttribute]
		public virtual IReplicationProcess ReplicationBegin(IObjectContainer peerB, IReplicationConflictHandler
			 conflictHandler)
		{
			throw new NotSupportedException();
		}

		public virtual void Set(object obj, int depth)
		{
			lock (Lock())
			{
				CheckClosed();
				_server.Set(_transaction, obj, depth);
			}
		}

		public virtual bool SetSemaphore(string name, int waitForAvailability)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.SetSemaphore(_transaction, name, waitForAvailability);
			}
		}

		public virtual IStoredClass StoredClass(object clazz)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.StoredClass(_transaction, clazz);
			}
		}

		public virtual IStoredClass[] StoredClasses()
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.StoredClasses(_transaction);
			}
		}

		public virtual ISystemInfo SystemInfo()
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.SystemInfo();
			}
		}

		public virtual long Version()
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.Version();
			}
		}

		public virtual void Activate(object obj, int depth)
		{
			lock (Lock())
			{
				CheckClosed();
				_server.Activate(_transaction, obj, depth);
			}
		}

		public virtual bool Close()
		{
			lock (Lock())
			{
				if (IsClosed())
				{
					return false;
				}
				if (!_server.IsClosed())
				{
					if (!_server.ConfigImpl().IsReadOnly())
					{
						Commit();
					}
				}
				_transaction.Close(false);
				_closed = true;
				return true;
			}
		}

		public virtual void Commit()
		{
			lock (Lock())
			{
				CheckClosed();
				_server.Commit(_transaction);
			}
		}

		public virtual void Deactivate(object obj, int depth)
		{
			lock (Lock())
			{
				CheckClosed();
				_server.Deactivate(_transaction, obj, depth);
			}
		}

		public virtual void Delete(object obj)
		{
			lock (Lock())
			{
				CheckClosed();
				_server.Delete(_transaction, obj);
			}
		}

		public virtual IExtObjectContainer Ext()
		{
			return (IExtObjectContainer)this;
		}

		public virtual IObjectSet Get(object template)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.Get(_transaction, template);
			}
		}

		public virtual IQuery Query()
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.Query(_transaction);
			}
		}

		public virtual IObjectSet Query(Type clazz)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.Query(_transaction, clazz);
			}
		}

		public virtual IObjectSet Query(Predicate predicate)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.Query(_transaction, predicate);
			}
		}

		public virtual IObjectSet Query(Predicate predicate, IQueryComparator comparator)
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.Query(_transaction, predicate, comparator);
			}
		}

		public virtual void Rollback()
		{
			lock (Lock())
			{
				CheckClosed();
				_server.Rollback(_transaction);
			}
		}

		public virtual void Set(object obj)
		{
			lock (Lock())
			{
				CheckClosed();
				_server.Set(_transaction, obj);
			}
		}

		public virtual ObjectContainerBase Container()
		{
			return _server;
		}

		public virtual Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return _transaction;
		}

		public virtual void Callbacks(ICallbacks cb)
		{
			lock (Lock())
			{
				CheckClosed();
				_server.Callbacks(cb);
			}
		}

		public virtual ICallbacks Callbacks()
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.Callbacks();
			}
		}

		public NativeQueryHandler GetNativeQueryHandler()
		{
			lock (Lock())
			{
				CheckClosed();
				return _server.GetNativeQueryHandler();
			}
		}

		public virtual void OnCommittedListener()
		{
		}

		private static IObjectContainer Cast(Db4objects.Db4o.Internal.PartialEmbeddedClientObjectContainer
			 container)
		{
			return (IObjectContainer)container;
		}

		public virtual ClassMetadata ClassMetadataForReflectClass(IReflectClass reflectClass
			)
		{
			return _server.ClassMetadataForReflectClass(reflectClass);
		}

		public virtual ClassMetadata ClassMetadataForName(string name)
		{
			return _server.ClassMetadataForName(name);
		}

		public virtual ClassMetadata ClassMetadataForId(int id)
		{
			return _server.ClassMetadataForId(id);
		}

		public virtual HandlerRegistry Handlers()
		{
			return _server.Handlers();
		}
	}
}
