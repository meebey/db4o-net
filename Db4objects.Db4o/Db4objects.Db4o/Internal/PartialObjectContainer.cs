/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.Callbacks;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;
using Db4objects.Db4o.Internal.Replication;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Replication;
using Db4objects.Db4o.Types;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// NOTE: This is just a 'partial' base class to allow for variant implementations
	/// in db4oj and db4ojdk1.2.
	/// </summary>
	/// <remarks>
	/// NOTE: This is just a 'partial' base class to allow for variant implementations
	/// in db4oj and db4ojdk1.2. It assumes that itself is an instance of YapStream
	/// and should never be used explicitly.
	/// </remarks>
	/// <exclude></exclude>
	#if NET_2_0
	public abstract partial class PartialObjectContainer : ITransientClass, IInternal4
		, IObjectContainerSpec
	#else
	public abstract class PartialObjectContainer : ITransientClass, IInternal4, IObjectContainerSpec
	#endif
	{
		protected ClassMetadataRepository _classCollection;

		protected ClassInfoHelper _classMetaHelper = new ClassInfoHelper();

		protected Config4Impl _config;

		private int _stackDepth;

		private readonly Db4objects.Db4o.Internal.ReferenceSystemRegistry _referenceSystemRegistry
			 = new Db4objects.Db4o.Internal.ReferenceSystemRegistry();

		private Tree _justPeeked;

		public readonly object _lock;

		private List4 _pendingClassUpdates;

		internal readonly ObjectContainerBase _parent;

		internal bool _refreshInsteadOfActivate;

		internal int _showInternalClasses = 0;

		private List4 _stillToActivate;

		private List4 _stillToDeactivate;

		private List4 _stillToSet;

		private Db4objects.Db4o.Internal.Transaction _systemTransaction;

		protected Db4objects.Db4o.Internal.Transaction _transaction;

		private bool _instantiating;

		public HandlerRegistry _handlers;

		internal int _replicationCallState;

		internal WeakReferenceCollector _references;

		private NativeQueryHandler _nativeQueryHandler;

		private readonly ObjectContainerBase _this;

		private ICallbacks _callbacks = new NullCallbacks();

		protected readonly PersistentTimeStampIdGenerator _timeStampIdGenerator = new PersistentTimeStampIdGenerator
			();

		private int _topLevelCallId = 1;

		private IntIdGenerator _topLevelCallIdGenerator = new IntIdGenerator();

		private bool _topLevelCallCompleted;

		protected PartialObjectContainer(IConfiguration config, ObjectContainerBase parent
			)
		{
			_this = Cast(this);
			_parent = parent == null ? _this : parent;
			_lock = parent == null ? new object() : parent._lock;
			_config = (Config4Impl)config;
		}

		public void Open()
		{
			bool ok = false;
			lock (_lock)
			{
				try
				{
					InitializeTransactions();
					Initialize1(_config);
					OpenImpl();
					InitializePostOpen();
					Platform4.PostOpen(Cast(_this));
					ok = true;
				}
				finally
				{
					if (!ok)
					{
						ShutdownObjectContainer();
					}
				}
			}
		}

		protected abstract void OpenImpl();

		public void ActivateDefaultDepth(Db4objects.Db4o.Internal.Transaction trans, object
			 obj)
		{
			Activate(trans, obj, ConfigImpl().ActivationDepth());
		}

		public void Activate(Db4objects.Db4o.Internal.Transaction trans, object obj, int 
			depth)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				BeginTopLevelCall();
				try
				{
					StillToActivate(trans, obj, depth);
					Activate3CheckStill(trans);
					CompleteTopLevelCall();
				}
				catch (Db4oException e)
				{
					CompleteTopLevelCall(e);
				}
				finally
				{
					EndTopLevelCall();
				}
			}
		}

		internal void Activate3CheckStill(Db4objects.Db4o.Internal.Transaction ta)
		{
			while (_stillToActivate != null)
			{
				IEnumerator i = new Iterator4Impl(_stillToActivate);
				_stillToActivate = null;
				while (i.MoveNext())
				{
					ObjectReference yo = (ObjectReference)i.Current;
					i.MoveNext();
					int depth = ((int)i.Current);
					object obj = yo.GetObject();
					if (obj == null)
					{
						ta.RemoveReference(yo);
					}
					else
					{
						yo.Activate1(ta, obj, depth, _refreshInsteadOfActivate);
					}
				}
			}
		}

		public void Bind(Db4objects.Db4o.Internal.Transaction trans, object obj, long id)
		{
			lock (_lock)
			{
				if (obj == null)
				{
					throw new ArgumentNullException();
				}
				trans = CheckTransaction(trans);
				int intID = (int)id;
				object oldObject = GetByID(trans, id);
				if (oldObject == null)
				{
					throw new ArgumentException("id");
				}
				ObjectReference yo = trans.ReferenceForId(intID);
				if (yo == null)
				{
					throw new ArgumentException("obj");
				}
				if (trans.Reflector().ForObject(obj) == yo.ClassMetadata().ClassReflector())
				{
					ObjectReference newRef = Bind2(trans, yo, obj);
					newRef.VirtualAttributes(trans);
				}
				else
				{
					throw new Exception(Db4objects.Db4o.Internal.Messages.Get(57));
				}
			}
		}

		public ObjectReference Bind2(Db4objects.Db4o.Internal.Transaction trans, ObjectReference
			 oldRef, object obj)
		{
			int id = oldRef.GetID();
			trans.RemoveReference(oldRef);
			ObjectReference newRef = new ObjectReference(ClassMetadataForReflectClass(Reflector
				().ForObject(obj)), id);
			newRef.SetObjectWeak(_this, obj);
			newRef.SetStateDirty();
			trans.ReferenceSystem().AddExistingReference(newRef);
			return newRef;
		}

		public abstract byte BlockSize();

		public int BytesToBlocks(long bytes)
		{
			int blockLen = BlockSize();
			return (int)((bytes + blockLen - 1) / blockLen);
		}

		public int BlockAlignedBytes(int bytes)
		{
			return BytesToBlocks(bytes) * BlockSize();
		}

		public int BlocksToBytes(int blocks)
		{
			return blocks * BlockSize();
		}

		private bool BreakDeleteForEnum(ObjectReference reference, bool userCall)
		{
			return false;
			if (userCall)
			{
				return false;
			}
			if (reference == null)
			{
				return false;
			}
			return Platform4.Jdk().IsEnum(Reflector(), reference.ClassMetadata().ClassReflector
				());
		}

		internal virtual bool CanUpdate()
		{
			return true;
		}

		public void CheckClosed()
		{
			if (_classCollection == null)
			{
				throw new DatabaseClosedException();
			}
		}

		protected void CheckReadOnly()
		{
			if (_config.IsReadOnly())
			{
				throw new DatabaseReadOnlyException();
			}
		}

		internal void ProcessPendingClassUpdates()
		{
			if (_pendingClassUpdates == null)
			{
				return;
			}
			IEnumerator i = new Iterator4Impl(_pendingClassUpdates);
			while (i.MoveNext())
			{
				ClassMetadata yapClass = (ClassMetadata)i.Current;
				yapClass.SetStateDirty();
				yapClass.Write(_systemTransaction);
			}
			_pendingClassUpdates = null;
		}

		public Db4objects.Db4o.Internal.Transaction CheckTransaction()
		{
			return CheckTransaction(null);
		}

		public Db4objects.Db4o.Internal.Transaction CheckTransaction(Db4objects.Db4o.Internal.Transaction
			 ta)
		{
			CheckClosed();
			if (ta != null)
			{
				return ta;
			}
			return Transaction();
		}

		public bool Close()
		{
			lock (_lock)
			{
				Close1();
				return true;
			}
		}

		protected virtual void HandleExceptionOnClose(Exception exc)
		{
			FatalException(exc);
		}

		private void Close1()
		{
			if (_classCollection == null)
			{
				return;
			}
			Platform4.PreClose(Cast(_this));
			ProcessPendingClassUpdates();
			if (StateMessages())
			{
				LogMsg(2, ToString());
			}
			Close2();
		}

		protected abstract void Close2();

		public void ShutdownObjectContainer()
		{
			LogMsg(3, ToString());
			lock (_lock)
			{
				StopSession();
				ShutdownDataStorage();
			}
		}

		protected abstract void ShutdownDataStorage();

		public virtual IDb4oCollections Collections(Db4objects.Db4o.Internal.Transaction 
			trans)
		{
			lock (_lock)
			{
				return Platform4.Collections(CheckTransaction(trans));
			}
		}

		public void Commit(Db4objects.Db4o.Internal.Transaction trans)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				CheckReadOnly();
				BeginTopLevelCall();
				try
				{
					Commit1(trans);
					trans.CommitReferenceSystem();
					CompleteTopLevelCall();
				}
				catch (Db4oException e)
				{
					CompleteTopLevelCall(e);
				}
				finally
				{
					EndTopLevelCall();
				}
			}
		}

		public abstract void Commit1(Db4objects.Db4o.Internal.Transaction trans);

		public virtual IConfiguration Configure()
		{
			return ConfigImpl();
		}

		public virtual Config4Impl Config()
		{
			return ConfigImpl();
		}

		public abstract int ConverterVersion();

		public abstract AbstractQueryResult NewQueryResult(Db4objects.Db4o.Internal.Transaction
			 trans, QueryEvaluationMode mode);

		protected virtual void CreateStringIO(byte encoding)
		{
			StringIO(LatinStringIO.ForEncoding(encoding));
		}

		protected void InitializeTransactions()
		{
			_systemTransaction = NewTransaction(null, CreateReferenceSystem());
			_transaction = NewUserTransaction();
		}

		public abstract Db4objects.Db4o.Internal.Transaction NewTransaction(Db4objects.Db4o.Internal.Transaction
			 parentTransaction, TransactionalReferenceSystem referenceSystem);

		public virtual Db4objects.Db4o.Internal.Transaction NewUserTransaction()
		{
			return NewTransaction(SystemTransaction(), null);
		}

		public abstract long CurrentVersion();

		public virtual bool CreateClassMetadata(ClassMetadata classMeta, IReflectClass clazz
			, ClassMetadata superClassMeta)
		{
			return classMeta.Init(_this, superClassMeta, clazz);
		}

		/// <summary>allows special handling for all Db4oType objects.</summary>
		/// <remarks>
		/// allows special handling for all Db4oType objects.
		/// Redirected here from #set() so only instanceof check is necessary
		/// in the #set() method.
		/// </remarks>
		/// <returns>object if handled here and #set() should not continue processing</returns>
		public virtual IDb4oType Db4oTypeStored(Db4objects.Db4o.Internal.Transaction trans
			, object obj)
		{
			if (!(obj is Db4oDatabase))
			{
				return null;
			}
			Db4oDatabase database = (Db4oDatabase)obj;
			if (trans.ReferenceForObject(obj) != null)
			{
				return database;
			}
			ShowInternalClasses(true);
			try
			{
				return database.Query(trans);
			}
			finally
			{
				ShowInternalClasses(false);
			}
		}

		public void Deactivate(Db4objects.Db4o.Internal.Transaction trans, object obj, int
			 depth)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				BeginTopLevelCall();
				try
				{
					DeactivateInternal(trans, obj, depth);
					CompleteTopLevelCall();
				}
				catch (Db4oException e)
				{
					CompleteTopLevelCall(e);
				}
				finally
				{
					EndTopLevelCall();
				}
			}
		}

		private void DeactivateInternal(Db4objects.Db4o.Internal.Transaction trans, object
			 obj, int depth)
		{
			StillToDeactivate(trans, obj, depth, true);
			while (_stillToDeactivate != null)
			{
				IEnumerator i = new Iterator4Impl(_stillToDeactivate);
				_stillToDeactivate = null;
				while (i.MoveNext())
				{
					ObjectReference currentObject = (ObjectReference)i.Current;
					i.MoveNext();
					int currentInteger = ((int)i.Current);
					currentObject.Deactivate(trans, currentInteger);
				}
			}
		}

		public void Delete(Db4objects.Db4o.Internal.Transaction trans, object obj)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				CheckReadOnly();
				Delete1(trans, obj, true);
				trans.ProcessDeletes();
			}
		}

		public void Delete1(Db4objects.Db4o.Internal.Transaction trans, object obj, bool 
			userCall)
		{
			if (obj == null)
			{
				return;
			}
			ObjectReference @ref = trans.ReferenceForObject(obj);
			if (@ref == null)
			{
				return;
			}
			if (userCall)
			{
				GenerateCallIDOnTopLevel();
			}
			try
			{
				BeginTopLevelCall();
				Delete2(trans, @ref, obj, 0, userCall);
				CompleteTopLevelCall();
			}
			catch (Db4oException e)
			{
				CompleteTopLevelCall(e);
			}
			finally
			{
				EndTopLevelCall();
			}
		}

		public void Delete2(Db4objects.Db4o.Internal.Transaction trans, ObjectReference @ref
			, object obj, int cascade, bool userCall)
		{
			if (BreakDeleteForEnum(@ref, userCall))
			{
				return;
			}
			if (obj is ISecondClass)
			{
				if (!FlagForDelete(@ref))
				{
					return;
				}
				Delete3(trans, @ref, cascade, userCall);
				return;
			}
			trans.Delete(@ref, @ref.GetID(), cascade);
		}

		internal void Delete3(Db4objects.Db4o.Internal.Transaction trans, ObjectReference
			 @ref, int cascade, bool userCall)
		{
			if (@ref == null || !@ref.BeginProcessing())
			{
				return;
			}
			if (BreakDeleteForEnum(@ref, userCall))
			{
				@ref.EndProcessing();
				return;
			}
			if (!@ref.IsFlaggedForDelete())
			{
				@ref.EndProcessing();
				return;
			}
			ClassMetadata yc = @ref.ClassMetadata();
			object obj = @ref.GetObject();
			@ref.EndProcessing();
			ActivateForDeletionCallback(trans, yc, obj);
			if (!ObjectCanDelete(trans, yc, obj))
			{
				return;
			}
			@ref.BeginProcessing();
			if (Delete4(trans, @ref, cascade, userCall))
			{
				ObjectOnDelete(trans, yc, obj);
				if (ConfigImpl().MessageLevel() > Const4.STATE)
				{
					Message(string.Empty + @ref.GetID() + " delete " + @ref.ClassMetadata().GetName()
						);
				}
			}
			@ref.EndProcessing();
		}

		private void ActivateForDeletionCallback(Db4objects.Db4o.Internal.Transaction trans
			, ClassMetadata yc, object obj)
		{
			if (!IsActive(trans, obj) && (CaresAboutDeleting(yc) || CaresAboutDeleted(yc)))
			{
				Activate(trans, obj, 1);
			}
		}

		private bool CaresAboutDeleting(ClassMetadata yc)
		{
			return this._callbacks.CaresAboutDeleting() || yc.HasEventRegistered(_this, EventDispatcher
				.CAN_DELETE);
		}

		private bool CaresAboutDeleted(ClassMetadata yc)
		{
			return this._callbacks.CaresAboutDeleted() || yc.HasEventRegistered(_this, EventDispatcher
				.DELETE);
		}

		private bool ObjectCanDelete(Db4objects.Db4o.Internal.Transaction transaction, ClassMetadata
			 yc, object obj)
		{
			return _this.Callbacks().ObjectCanDelete(transaction, obj) && yc.DispatchEvent(_this
				, obj, EventDispatcher.CAN_DELETE);
		}

		private void ObjectOnDelete(Db4objects.Db4o.Internal.Transaction transaction, ClassMetadata
			 yc, object obj)
		{
			_this.Callbacks().ObjectOnDelete(transaction, obj);
			yc.DispatchEvent(_this, obj, EventDispatcher.DELETE);
		}

		public abstract bool Delete4(Db4objects.Db4o.Internal.Transaction ta, ObjectReference
			 yapObject, int a_cascade, bool userCall);

		internal virtual object Descend(Db4objects.Db4o.Internal.Transaction trans, object
			 obj, string[] path)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				ObjectReference @ref = trans.ReferenceForObject(obj);
				if (@ref == null)
				{
					return null;
				}
				string fieldName = path[0];
				if (fieldName == null)
				{
					return null;
				}
				ClassMetadata classMetadata = @ref.ClassMetadata();
				FieldMetadata[] field = new FieldMetadata[] { null };
				classMetadata.ForEachFieldMetadata(new _IVisitor4_580(this, fieldName, field));
				if (field[0] == null)
				{
					return null;
				}
				object child = @ref.IsActive() ? field[0].Get(trans, obj) : new UnmarshallingContext
					(trans, @ref, Const4.ADD_TO_ID_TREE, false).ReadFieldValue(@ref.GetID(), field[0
					]);
				if (path.Length == 1)
				{
					return child;
				}
				if (child == null)
				{
					return null;
				}
				string[] subPath = new string[path.Length - 1];
				System.Array.Copy(path, 1, subPath, 0, path.Length - 1);
				return Descend(trans, child, subPath);
			}
		}

		private sealed class _IVisitor4_580 : IVisitor4
		{
			public _IVisitor4_580(PartialObjectContainer _enclosing, string fieldName, FieldMetadata[]
				 field)
			{
				this._enclosing = _enclosing;
				this.fieldName = fieldName;
				this.field = field;
			}

			public void Visit(object yf)
			{
				FieldMetadata yapField = (FieldMetadata)yf;
				if (yapField.CanAddToQuery(fieldName))
				{
					field[0] = yapField;
				}
			}

			private readonly PartialObjectContainer _enclosing;

			private readonly string fieldName;

			private readonly FieldMetadata[] field;
		}

		public virtual bool DetectSchemaChanges()
		{
			return ConfigImpl().DetectSchemaChanges();
		}

		public virtual bool DispatchsEvents()
		{
			return true;
		}

		protected virtual bool DoFinalize()
		{
			return true;
		}

		internal void ShutdownHook()
		{
			if (IsClosed())
			{
				return;
			}
			if (AllOperationsCompleted())
			{
				Db4objects.Db4o.Internal.Messages.LogErr(ConfigImpl(), 50, ToString(), null);
				Close();
			}
			else
			{
				ShutdownObjectContainer();
				if (OperationIsProcessing())
				{
					Db4objects.Db4o.Internal.Messages.LogErr(ConfigImpl(), 24, null, null);
				}
			}
		}

		private bool OperationIsProcessing()
		{
			return _stackDepth > 0;
		}

		private bool AllOperationsCompleted()
		{
			return _stackDepth == 0;
		}

		internal virtual void FatalException(int msgID)
		{
			FatalException(null, msgID);
		}

		internal void FatalException(Exception t)
		{
			FatalException(t, Db4objects.Db4o.Internal.Messages.FATAL_MSG_ID);
		}

		internal void FatalException(Exception t, int msgID)
		{
			Db4objects.Db4o.Internal.Messages.LogErr(ConfigImpl(), (msgID == Db4objects.Db4o.Internal.Messages
				.FATAL_MSG_ID ? 18 : msgID), null, t);
			if (!IsClosed())
			{
				ShutdownObjectContainer();
			}
			throw new Exception(Db4objects.Db4o.Internal.Messages.Get(msgID));
		}

		private bool ConfiguredForAutomaticShutDown()
		{
			return (ConfigImpl() == null || ConfigImpl().AutomaticShutDown());
		}

		internal virtual void Gc()
		{
			_references.PollReferenceQueue();
		}

		public IObjectSet Get(Db4objects.Db4o.Internal.Transaction trans, object template
			)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				IQueryResult res = null;
				try
				{
					BeginTopLevelCall();
					res = GetInternal(trans, template);
					CompleteTopLevelCall();
				}
				catch (Db4oException e)
				{
					CompleteTopLevelCall(e);
				}
				finally
				{
					EndTopLevelCall();
				}
				return new ObjectSetFacade(res);
			}
		}

		private IQueryResult GetInternal(Db4objects.Db4o.Internal.Transaction trans, object
			 template)
		{
			if (template == null || template.GetType() == Const4.CLASS_OBJECT)
			{
				return GetAll(trans);
			}
			IQuery q = Query(trans);
			q.Constrain(template);
			return ExecuteQuery((QQuery)q);
		}

		public abstract AbstractQueryResult GetAll(Db4objects.Db4o.Internal.Transaction ta
			);

		public object GetByID(Db4objects.Db4o.Internal.Transaction ta, long id)
		{
			lock (_lock)
			{
				if (id <= 0)
				{
					throw new ArgumentException();
				}
				CheckClosed();
				ta = CheckTransaction(ta);
				BeginTopLevelCall();
				try
				{
					object obj = GetByID2(ta, (int)id);
					CompleteTopLevelCall();
					return obj;
				}
				catch (Db4oException e)
				{
					CompleteTopLevelCall(new InvalidIDException(e));
				}
				finally
				{
					EndTopLevelCall();
				}
				return null;
			}
		}

		public object GetByID2(Db4objects.Db4o.Internal.Transaction ta, int id)
		{
			object obj = ta.ObjectForIdFromCache(id);
			if (obj != null)
			{
				return obj;
			}
			return new ObjectReference(id).Read(ta, 0, Const4.ADD_TO_ID_TREE, true);
		}

		public object GetActivatedObjectFromCache(Db4objects.Db4o.Internal.Transaction ta
			, int id)
		{
			object obj = ta.ObjectForIdFromCache(id);
			if (obj == null)
			{
				return null;
			}
			Activate(ta, obj, ConfigImpl().ActivationDepth());
			return obj;
		}

		public object ReadActivatedObjectNotInCache(Db4objects.Db4o.Internal.Transaction 
			ta, int id)
		{
			object obj = null;
			BeginTopLevelCall();
			try
			{
				obj = new ObjectReference(id).Read(ta, ConfigImpl().ActivationDepth(), Const4.ADD_TO_ID_TREE
					, true);
				CompleteTopLevelCall();
			}
			catch (Db4oException e)
			{
				CompleteTopLevelCall(e);
			}
			finally
			{
				EndTopLevelCall();
			}
			Activate3CheckStill(ta);
			return obj;
		}

		public object GetByUUID(Db4objects.Db4o.Internal.Transaction trans, Db4oUUID uuid
			)
		{
			lock (_lock)
			{
				if (uuid == null)
				{
					return null;
				}
				trans = CheckTransaction(trans);
				HardObjectReference hardRef = trans.GetHardReferenceBySignature(uuid.GetLongPart(
					), uuid.GetSignaturePart());
				return hardRef._object;
			}
		}

		public int GetID(Db4objects.Db4o.Internal.Transaction trans, object obj)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				CheckClosed();
				if (obj == null)
				{
					return 0;
				}
				ObjectReference yo = trans.ReferenceForObject(obj);
				if (yo != null)
				{
					return yo.GetID();
				}
				return 0;
			}
		}

		public IObjectInfo GetObjectInfo(Db4objects.Db4o.Internal.Transaction trans, object
			 obj)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				return trans.ReferenceForObject(obj);
			}
		}

		public HardObjectReference GetHardObjectReferenceById(Db4objects.Db4o.Internal.Transaction
			 trans, int id)
		{
			if (id <= 0)
			{
				return HardObjectReference.INVALID;
			}
			ObjectReference @ref = trans.ReferenceForId(id);
			if (@ref != null)
			{
				object candidate = @ref.GetObject();
				if (candidate != null)
				{
					return new HardObjectReference(@ref, candidate);
				}
				trans.RemoveReference(@ref);
			}
			@ref = new ObjectReference(id);
			object readObject = @ref.Read(trans, 0, Const4.ADD_TO_ID_TREE, true);
			if (readObject == null)
			{
				return HardObjectReference.INVALID;
			}
			if (readObject != @ref.GetObject())
			{
				return GetHardObjectReferenceById(trans, id);
			}
			return new HardObjectReference(@ref, readObject);
		}

		public StatefulBuffer GetWriter(Db4objects.Db4o.Internal.Transaction a_trans, int
			 a_address, int a_length)
		{
			if (Debug.ExceedsMaximumBlockSize(a_length))
			{
				return null;
			}
			return new StatefulBuffer(a_trans, a_address, a_length);
		}

		public Db4objects.Db4o.Internal.Transaction SystemTransaction()
		{
			return _systemTransaction;
		}

		public Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return _transaction;
		}

		public ClassMetadata ClassMetadataForReflectClass(IReflectClass claxx)
		{
			if (CantGetClassMetadata(claxx))
			{
				return null;
			}
			ClassMetadata yc = _handlers.ClassMetadataForClass(claxx);
			if (yc != null)
			{
				return yc;
			}
			return _classCollection.ClassMetadataForReflectClass(claxx);
		}

		public virtual ClassMetadata ProduceClassMetadata(IReflectClass claxx)
		{
			if (CantGetClassMetadata(claxx))
			{
				return null;
			}
			ClassMetadata classMetadata = _handlers.ClassMetadataForClass(claxx);
			if (classMetadata != null)
			{
				return classMetadata;
			}
			return _classCollection.ProduceClassMetadata(claxx);
		}

		/// <summary>
		/// Differentiating getActiveYapClass from getYapClass is a tuning
		/// optimization: If we initialize a YapClass, #set3() has to check for
		/// the possibility that class initialization associates the currently
		/// stored object with a previously stored static object, causing the
		/// object to be known afterwards.
		/// </summary>
		/// <remarks>
		/// Differentiating getActiveYapClass from getYapClass is a tuning
		/// optimization: If we initialize a YapClass, #set3() has to check for
		/// the possibility that class initialization associates the currently
		/// stored object with a previously stored static object, causing the
		/// object to be known afterwards.
		/// In this call we only return active YapClasses, initialization
		/// is not done on purpose
		/// </remarks>
		internal ClassMetadata GetActiveClassMetadata(IReflectClass claxx)
		{
			if (CantGetClassMetadata(claxx))
			{
				return null;
			}
			ClassMetadata yc = _handlers.ClassMetadataForClass(claxx);
			if (yc != null)
			{
				return yc;
			}
			return _classCollection.GetActiveYapClass(claxx);
		}

		private bool CantGetClassMetadata(IReflectClass claxx)
		{
			if (claxx == null)
			{
				return true;
			}
			if ((!ShowInternalClasses()) && _handlers.ICLASS_INTERNAL.IsAssignableFrom(claxx)
				)
			{
				return true;
			}
			return false;
		}

		public virtual int ClassMetadataIdForName(string name)
		{
			return _classCollection.ClassMetadataIdForName(name);
		}

		public virtual ClassMetadata ClassMetadataForName(string name)
		{
			return ClassMetadataForId(ClassMetadataIdForName(name));
		}

		public virtual ClassMetadata ClassMetadataForId(int id)
		{
			if (id == 0)
			{
				return null;
			}
			ClassMetadata yc = _handlers.ClassMetadataForId(id);
			if (yc != null)
			{
				return yc;
			}
			return _classCollection.GetYapClass(id);
		}

		public virtual HandlerRegistry Handlers()
		{
			return _handlers;
		}

		public virtual bool NeedsLockFileThread()
		{
			if (!Platform4.HasLockFileThread())
			{
				return false;
			}
			if (Platform4.HasNio())
			{
				return false;
			}
			if (ConfigImpl().IsReadOnly())
			{
				return false;
			}
			return ConfigImpl().LockFile();
		}

		protected virtual bool HasShutDownHook()
		{
			return ConfigImpl().AutomaticShutDown();
		}

		protected virtual void Initialize1(IConfiguration config)
		{
			_config = InitializeConfig(config);
			_handlers = new HandlerRegistry(_this, ConfigImpl().Encoding(), ConfigImpl().Reflector
				());
			if (_references != null)
			{
				Gc();
				_references.StopTimer();
			}
			_references = new WeakReferenceCollector(_this);
			if (HasShutDownHook())
			{
				Platform4.AddShutDownHook(this);
			}
			_handlers.InitEncryption(ConfigImpl());
			Initialize2();
			_stillToSet = null;
		}

		private Config4Impl InitializeConfig(IConfiguration config)
		{
			Config4Impl impl = ((Config4Impl)config);
			impl.Stream(_this);
			impl.Reflector().SetTransaction(SystemTransaction());
			return impl;
		}

		/// <summary>before file is open</summary>
		internal virtual void Initialize2()
		{
			Initialize2NObjectCarrier();
		}

		public TransactionalReferenceSystem CreateReferenceSystem()
		{
			TransactionalReferenceSystem referenceSystem = new TransactionalReferenceSystem();
			_referenceSystemRegistry.AddReferenceSystem(referenceSystem);
			return referenceSystem;
		}

		/// <summary>overridden in YapObjectCarrier</summary>
		internal virtual void Initialize2NObjectCarrier()
		{
			_classCollection = new ClassMetadataRepository(_systemTransaction);
			_references.StartTimer();
		}

		private void InitializePostOpen()
		{
			_showInternalClasses = 100000;
			InitializePostOpenExcludingTransportObjectContainer();
			_showInternalClasses = 0;
		}

		protected virtual void InitializePostOpenExcludingTransportObjectContainer()
		{
			InitializeEssentialClasses();
			Rename(ConfigImpl());
			_classCollection.InitOnUp(_systemTransaction);
			if (ConfigImpl().DetectSchemaChanges())
			{
				_systemTransaction.Commit();
			}
			ConfigImpl().ApplyConfigurationItems(Cast(_this));
		}

		internal virtual void InitializeEssentialClasses()
		{
			for (int i = 0; i < Const4.ESSENTIAL_CLASSES.Length; i++)
			{
				ProduceClassMetadata(Reflector().ForClass(Const4.ESSENTIAL_CLASSES[i]));
			}
		}

		internal void Instantiating(bool flag)
		{
			_instantiating = flag;
		}

		internal bool IsActive(Db4objects.Db4o.Internal.Transaction trans, object obj)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				if (obj != null)
				{
					ObjectReference @ref = trans.ReferenceForObject(obj);
					if (@ref != null)
					{
						return @ref.IsActive();
					}
				}
				return false;
			}
		}

		public virtual bool IsCached(Db4objects.Db4o.Internal.Transaction trans, long id)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				return trans.ObjectForIdFromCache((int)id) != null;
			}
		}

		/// <summary>
		/// overridden in YapClient
		/// This method will make it easier to refactor than
		/// an "instanceof YapClient" check.
		/// </summary>
		/// <remarks>
		/// overridden in YapClient
		/// This method will make it easier to refactor than
		/// an "instanceof YapClient" check.
		/// </remarks>
		public virtual bool IsClient()
		{
			return false;
		}

		public bool IsClosed()
		{
			lock (_lock)
			{
				return _classCollection == null;
			}
		}

		internal bool IsInstantiating()
		{
			return _instantiating;
		}

		internal virtual bool IsServer()
		{
			return false;
		}

		public bool IsStored(Db4objects.Db4o.Internal.Transaction trans, object obj)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				if (obj == null)
				{
					return false;
				}
				ObjectReference @ref = trans.ReferenceForObject(obj);
				if (@ref == null)
				{
					return false;
				}
				return !trans.IsDeleted(@ref.GetID());
			}
		}

		public virtual IReflectClass[] KnownClasses()
		{
			lock (_lock)
			{
				CheckClosed();
				return Reflector().KnownClasses();
			}
		}

		public virtual ITypeHandler4 HandlerByID(int id)
		{
			if (id < 1)
			{
				return null;
			}
			if (_handlers.IsSystemHandler(id))
			{
				return _handlers.HandlerForID(id);
			}
			return ClassMetadataForId(id);
		}

		public virtual object Lock()
		{
			return _lock;
		}

		public void LogMsg(int code, string msg)
		{
			Db4objects.Db4o.Internal.Messages.LogMsg(ConfigImpl(), code, msg);
		}

		public virtual bool MaintainsIndices()
		{
			return true;
		}

		internal virtual void Message(string msg)
		{
			new Db4objects.Db4o.Internal.Message(_this, msg);
		}

		public virtual void MigrateFrom(IObjectContainer objectContainer)
		{
			if (objectContainer == null)
			{
				if (_replicationCallState == Const4.NONE)
				{
					return;
				}
				_replicationCallState = Const4.NONE;
				if (_handlers.i_migration != null)
				{
					_handlers.i_migration.Terminate();
				}
				_handlers.i_migration = null;
			}
			else
			{
				ObjectContainerBase peer = (ObjectContainerBase)objectContainer;
				_replicationCallState = Const4.OLD;
				peer._replicationCallState = Const4.OLD;
				_handlers.i_migration = new MigrationConnection(_this, (ObjectContainerBase)objectContainer
					);
				peer._handlers.i_migration = _handlers.i_migration;
			}
		}

		public void NeedsUpdate(ClassMetadata a_yapClass)
		{
			_pendingClassUpdates = new List4(_pendingClassUpdates, a_yapClass);
		}

		public virtual long GenerateTimeStampId()
		{
			return _timeStampIdGenerator.Next();
		}

		public abstract int NewUserObject();

		public object PeekPersisted(Db4objects.Db4o.Internal.Transaction trans, object obj
			, int depth, bool committed)
		{
			lock (_lock)
			{
				CheckClosed();
				BeginTopLevelCall();
				try
				{
					_justPeeked = null;
					trans = CheckTransaction(trans);
					ObjectReference @ref = trans.ReferenceForObject(obj);
					trans = committed ? _systemTransaction : trans;
					object cloned = null;
					if (@ref != null)
					{
						cloned = PeekPersisted(trans, @ref.GetID(), depth);
					}
					_justPeeked = null;
					CompleteTopLevelCall();
					return cloned;
				}
				catch (Db4oException e)
				{
					CompleteTopLevelCall(e);
					return null;
				}
				finally
				{
					EndTopLevelCall();
				}
			}
		}

		public object PeekPersisted(Db4objects.Db4o.Internal.Transaction trans, int id, int
			 depth)
		{
			if (depth < 0)
			{
				return null;
			}
			TreeInt ti = new TreeInt(id);
			TreeIntObject tio = (TreeIntObject)Tree.Find(_justPeeked, ti);
			if (tio == null)
			{
				return new ObjectReference(id).PeekPersisted(trans, depth);
			}
			return tio._object;
		}

		internal virtual void Peeked(int a_id, object a_object)
		{
			_justPeeked = Tree.Add(_justPeeked, new TreeIntObject(a_id, a_object));
		}

		public virtual void Purge()
		{
			lock (_lock)
			{
				CheckClosed();
				Runtime.Gc();
				Runtime.RunFinalization();
				Runtime.Gc();
				Gc();
				_classCollection.Purge();
			}
		}

		public void Purge(Db4objects.Db4o.Internal.Transaction trans, object obj)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				trans.RemoveObjectFromReferenceSystem(obj);
			}
		}

		internal void RemoveFromAllReferenceSystems(object obj)
		{
			if (obj == null)
			{
				return;
			}
			if (obj is ObjectReference)
			{
				_referenceSystemRegistry.RemoveReference((ObjectReference)obj);
				return;
			}
			_referenceSystemRegistry.RemoveObject(obj);
		}

		public NativeQueryHandler GetNativeQueryHandler()
		{
			lock (_lock)
			{
				if (null == _nativeQueryHandler)
				{
					_nativeQueryHandler = new NativeQueryHandler(Cast(_this));
				}
				return _nativeQueryHandler;
			}
		}

		public IObjectSet Query(Db4objects.Db4o.Internal.Transaction trans, Predicate predicate
			)
		{
			return Query(trans, predicate, (IQueryComparator)null);
		}

		public IObjectSet Query(Db4objects.Db4o.Internal.Transaction trans, Predicate predicate
			, IQueryComparator comparator)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				return GetNativeQueryHandler().Execute(Query(trans), predicate, comparator);
			}
		}

		public IObjectSet Query(Db4objects.Db4o.Internal.Transaction trans, Type clazz)
		{
			return Get(trans, clazz);
		}

		public IQuery Query(Db4objects.Db4o.Internal.Transaction ta)
		{
			return new QQuery(CheckTransaction(ta), null, null);
		}

		public abstract void RaiseVersion(long a_minimumVersion);

		public abstract void ReadBytes(byte[] a_bytes, int a_address, int a_length);

		public abstract void ReadBytes(byte[] bytes, int address, int addressOffset, int 
			length);

		public Db4objects.Db4o.Internal.Buffer BufferByAddress(int address, int length)
		{
			CheckAddress(address);
			Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(length
				);
			ReadBytes(reader._buffer, address, length);
			_handlers.Decrypt(reader);
			return reader;
		}

		private void CheckAddress(int address)
		{
			if (address <= 0)
			{
				throw new ArgumentException("Invalid address offset: " + address);
			}
		}

		public StatefulBuffer ReadWriterByAddress(Db4objects.Db4o.Internal.Transaction a_trans
			, int address, int length)
		{
			CheckAddress(address);
			StatefulBuffer reader = GetWriter(a_trans, address, length);
			reader.ReadEncrypt(_this, address);
			return reader;
		}

		public abstract StatefulBuffer ReadWriterByID(Db4objects.Db4o.Internal.Transaction
			 a_ta, int a_id);

		public abstract Db4objects.Db4o.Internal.Buffer ReadReaderByID(Db4objects.Db4o.Internal.Transaction
			 a_ta, int a_id);

		public abstract StatefulBuffer[] ReadWritersByIDs(Db4objects.Db4o.Internal.Transaction
			 a_ta, int[] ids);

		private void Reboot()
		{
			Commit(null);
			Close();
			Open();
		}

		public virtual GenericReflector Reflector()
		{
			return _handlers._reflector;
		}

		public void Refresh(Db4objects.Db4o.Internal.Transaction trans, object obj, int depth
			)
		{
			lock (_lock)
			{
				_refreshInsteadOfActivate = true;
				try
				{
					Activate(trans, obj, depth);
				}
				finally
				{
					_refreshInsteadOfActivate = false;
				}
			}
		}

		internal void RefreshClasses()
		{
			lock (_lock)
			{
				_classCollection.RefreshClasses();
			}
		}

		public abstract void ReleaseSemaphore(string name);

		public virtual void FlagAsHandled(ObjectReference @ref)
		{
			@ref.FlagAsHandled(_topLevelCallId);
		}

		internal virtual bool FlagForDelete(ObjectReference @ref)
		{
			if (@ref == null)
			{
				return false;
			}
			if (HandledInCurrentTopLevelCall(@ref))
			{
				return false;
			}
			@ref.FlagForDelete(_topLevelCallId);
			return true;
		}

		public abstract void ReleaseSemaphores(Db4objects.Db4o.Internal.Transaction ta);

		internal virtual void Rename(Config4Impl config)
		{
			bool renamedOne = false;
			if (config.Rename() != null)
			{
				renamedOne = Rename1(config);
			}
			_classCollection.CheckChanges();
			if (renamedOne)
			{
				Reboot();
			}
		}

		protected virtual bool Rename1(Config4Impl config)
		{
			bool renamedOne = false;
			IEnumerator i = config.Rename().GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Rename ren = (Db4objects.Db4o.Rename)i.Current;
				if (Get(SystemTransaction(), ren).Size() == 0)
				{
					bool renamed = false;
					bool isField = ren.rClass.Length > 0;
					ClassMetadata yapClass = _classCollection.GetYapClass(isField ? ren.rClass : ren.
						rFrom);
					if (yapClass != null)
					{
						if (isField)
						{
							renamed = yapClass.RenameField(ren.rFrom, ren.rTo);
						}
						else
						{
							ClassMetadata existing = _classCollection.GetYapClass(ren.rTo);
							if (existing == null)
							{
								yapClass.SetName(ren.rTo);
								renamed = true;
							}
							else
							{
								LogMsg(9, "class " + ren.rTo);
							}
						}
					}
					if (renamed)
					{
						renamedOne = true;
						SetDirtyInSystemTransaction(yapClass);
						LogMsg(8, ren.rFrom + " to " + ren.rTo);
						IObjectSet backren = Get(SystemTransaction(), new Db4objects.Db4o.Rename(ren.rClass
							, null, ren.rFrom));
						while (backren.HasNext())
						{
							Delete(SystemTransaction(), backren.Next());
						}
						Set(SystemTransaction(), ren);
					}
				}
			}
			return renamedOne;
		}

		[System.ObsoleteAttribute(@"see")]
		public virtual IReplicationProcess ReplicationBegin(IObjectContainer peerB, IReplicationConflictHandler
			 conflictHandler)
		{
			return new ReplicationImpl(_this, (ObjectContainerBase)peerB, conflictHandler);
		}

		[System.ObsoleteAttribute]
		public int OldReplicationHandles(Db4objects.Db4o.Internal.Transaction trans, object
			 obj)
		{
			if (_replicationCallState != Const4.OLD)
			{
				return 0;
			}
			if (_handlers.i_replication == null)
			{
				return 0;
			}
			if (obj is IInternal4)
			{
				return 0;
			}
			ObjectReference reference = trans.ReferenceForObject(obj);
			if (reference != null && HandledInCurrentTopLevelCall(reference))
			{
				return reference.GetID();
			}
			return _handlers.i_replication.TryToHandle(_this, obj);
		}

		public bool HandledInCurrentTopLevelCall(ObjectReference @ref)
		{
			return @ref.IsFlaggedAsHandled(_topLevelCallId);
		}

		public abstract void Reserve(int byteCount);

		public void Rollback(Db4objects.Db4o.Internal.Transaction trans)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				CheckReadOnly();
				Rollback1(trans);
				trans.RollbackReferenceSystem();
			}
		}

		public abstract void Rollback1(Db4objects.Db4o.Internal.Transaction trans);

		/// <param name="obj"></param>
		public virtual void Send(object obj)
		{
			throw new NotSupportedException();
		}

		public void Set(Db4objects.Db4o.Internal.Transaction trans, object obj)
		{
			Set(trans, obj, Const4.UNSPECIFIED);
		}

		public void Set(Db4objects.Db4o.Internal.Transaction trans, object obj, int depth
			)
		{
			lock (_lock)
			{
				SetInternal(trans, obj, depth, true);
			}
		}

		public int SetInternal(Db4objects.Db4o.Internal.Transaction trans, object obj, bool
			 checkJustSet)
		{
			return SetInternal(trans, obj, Const4.UNSPECIFIED, checkJustSet);
		}

		public int SetInternal(Db4objects.Db4o.Internal.Transaction trans, object obj, int
			 depth, bool checkJustSet)
		{
			trans = CheckTransaction(trans);
			CheckReadOnly();
			BeginTopLevelSet();
			try
			{
				int id = OldReplicationHandles(trans, obj);
				if (id != 0)
				{
					CompleteTopLevelSet();
					if (id < 0)
					{
						return 0;
					}
					return id;
				}
				id = SetAfterReplication(trans, obj, depth, checkJustSet);
				CompleteTopLevelSet();
				return id;
			}
			catch (Db4oException e)
			{
				CompleteTopLevelSet(e);
				return 0;
			}
			finally
			{
				EndTopLevelSet(trans);
			}
		}

		public int SetAfterReplication(Db4objects.Db4o.Internal.Transaction trans, object
			 obj, int depth, bool checkJust)
		{
			if (obj is IDb4oType)
			{
				IDb4oType db4oType = Db4oTypeStored(trans, obj);
				if (db4oType != null)
				{
					return GetID(trans, db4oType);
				}
			}
			try
			{
				return Set2(trans, obj, depth, checkJust);
			}
			catch (ObjectNotStorableException e)
			{
				throw;
			}
			catch (Db4oException exc)
			{
				throw;
			}
		}

		public void SetByNewReplication(IDb4oReplicationReferenceProvider referenceProvider
			, object obj)
		{
			lock (_lock)
			{
				_replicationCallState = Const4.NEW;
				_handlers._replicationReferenceProvider = referenceProvider;
				Set2(CheckTransaction(), obj, 1, false);
				_replicationCallState = Const4.NONE;
				_handlers._replicationReferenceProvider = null;
			}
		}

		private int Set2(Db4objects.Db4o.Internal.Transaction trans, object obj, int depth
			, bool checkJust)
		{
			int id = Set3(trans, obj, depth, checkJust);
			if (StackIsSmall())
			{
				CheckStillToSet();
			}
			return id;
		}

		public virtual void CheckStillToSet()
		{
			List4 postponedStillToSet = null;
			while (_stillToSet != null)
			{
				IEnumerator i = new Iterator4Impl(_stillToSet);
				_stillToSet = null;
				while (i.MoveNext())
				{
					int updateDepth = (int)i.Current;
					i.MoveNext();
					ObjectReference @ref = (ObjectReference)i.Current;
					i.MoveNext();
					Db4objects.Db4o.Internal.Transaction trans = (Db4objects.Db4o.Internal.Transaction
						)i.Current;
					if (!@ref.ContinueSet(trans, updateDepth))
					{
						postponedStillToSet = new List4(postponedStillToSet, trans);
						postponedStillToSet = new List4(postponedStillToSet, @ref);
						postponedStillToSet = new List4(postponedStillToSet, updateDepth);
					}
				}
			}
			_stillToSet = postponedStillToSet;
		}

		internal virtual void NotStorable(IReflectClass claxx, object obj)
		{
			if (!ConfigImpl().ExceptionsOnNotStorable())
			{
				return;
			}
			if (true)
			{
				return;
			}
			if (claxx != null)
			{
				throw new ObjectNotStorableException(claxx);
			}
			throw new ObjectNotStorableException(obj.ToString());
		}

		public int Set3(Db4objects.Db4o.Internal.Transaction trans, object obj, int updateDepth
			, bool checkJustSet)
		{
			if (obj == null || (obj is ITransientClass))
			{
				return 0;
			}
			if (obj is IDb4oTypeImpl)
			{
				((IDb4oTypeImpl)obj).StoredTo(trans);
			}
			ObjectAnalyzer analyzer = new ObjectAnalyzer(this, obj);
			analyzer.Analyze(trans);
			if (analyzer.NotStorable())
			{
				return 0;
			}
			ObjectReference @ref = analyzer.ObjectReference();
			if (@ref == null)
			{
				ClassMetadata classMetadata = analyzer.ClassMetadata();
				if (!ObjectCanNew(trans, classMetadata, obj))
				{
					return 0;
				}
				@ref = new ObjectReference();
				@ref.Store(trans, classMetadata, obj);
				trans.AddNewReference(@ref);
				if (obj is IDb4oTypeImpl)
				{
					((IDb4oTypeImpl)obj).SetTrans(trans);
				}
				if (ConfigImpl().MessageLevel() > Const4.STATE)
				{
					Message(string.Empty + @ref.GetID() + " new " + @ref.ClassMetadata().GetName());
				}
				FlagAsHandled(@ref);
				StillToSet(trans, @ref, updateDepth);
			}
			else
			{
				if (CanUpdate())
				{
					if (checkJustSet)
					{
						if ((!@ref.IsNew()) && HandledInCurrentTopLevelCall(@ref))
						{
							return @ref.GetID();
						}
					}
					if (UpdateDepthSufficient(updateDepth))
					{
						FlagAsHandled(@ref);
						@ref.WriteUpdate(trans, updateDepth);
					}
				}
			}
			ProcessPendingClassUpdates();
			return @ref.GetID();
		}

		private bool UpdateDepthSufficient(int updateDepth)
		{
			return (updateDepth == Const4.UNSPECIFIED) || (updateDepth > 0);
		}

		private bool ObjectCanNew(Db4objects.Db4o.Internal.Transaction transaction, ClassMetadata
			 yc, object obj)
		{
			return Callbacks().ObjectCanNew(transaction, obj) && yc.DispatchEvent(_this, obj, 
				EventDispatcher.CAN_NEW);
		}

		public abstract void SetDirtyInSystemTransaction(PersistentBase a_object);

		public abstract bool SetSemaphore(string name, int timeout);

		internal virtual void StringIO(LatinStringIO io)
		{
			_handlers.StringIO(io);
		}

		internal bool ShowInternalClasses()
		{
			return IsServer() || _showInternalClasses > 0;
		}

		/// <summary>
		/// Objects implementing the "Internal4" marker interface are
		/// not visible to queries, unless this flag is set to true.
		/// </summary>
		/// <remarks>
		/// Objects implementing the "Internal4" marker interface are
		/// not visible to queries, unless this flag is set to true.
		/// The caller should reset the flag after the call.
		/// </remarks>
		public virtual void ShowInternalClasses(bool show)
		{
			lock (this)
			{
				if (show)
				{
					_showInternalClasses++;
				}
				else
				{
					_showInternalClasses--;
				}
				if (_showInternalClasses < 0)
				{
					_showInternalClasses = 0;
				}
			}
		}

		private bool StackIsSmall()
		{
			return _stackDepth < Const4.MAX_STACK_DEPTH;
		}

		internal virtual bool StateMessages()
		{
			return true;
		}

		/// <summary>
		/// returns true in case an unknown single object is passed
		/// This allows deactivating objects before queries are called.
		/// </summary>
		/// <remarks>
		/// returns true in case an unknown single object is passed
		/// This allows deactivating objects before queries are called.
		/// </remarks>
		internal List4 StillTo1(Db4objects.Db4o.Internal.Transaction trans, List4 still, 
			object obj, int depth, bool forceUnknownDeactivate)
		{
			if (obj == null || depth <= 0)
			{
				return still;
			}
			ObjectReference @ref = trans.ReferenceForObject(obj);
			if (@ref != null)
			{
				if (HandledInCurrentTopLevelCall(@ref))
				{
					return still;
				}
				FlagAsHandled(@ref);
				return new List4(new List4(still, depth), @ref);
			}
			IReflectClass clazz = Reflector().ForObject(obj);
			if (clazz.IsArray())
			{
				if (!clazz.GetComponentType().IsPrimitive())
				{
					object[] arr = ArrayHandler.ToArray(_this, obj);
					for (int i = 0; i < arr.Length; i++)
					{
						still = StillTo1(trans, still, arr[i], depth, forceUnknownDeactivate);
					}
				}
			}
			else
			{
				if (obj is Entry)
				{
					still = StillTo1(trans, still, ((Entry)obj).key, depth, false);
					still = StillTo1(trans, still, ((Entry)obj).value, depth, false);
				}
				else
				{
					if (forceUnknownDeactivate)
					{
						ClassMetadata yc = ClassMetadataForReflectClass(Reflector().ForObject(obj));
						if (yc != null)
						{
							yc.Deactivate(trans, obj, depth);
						}
					}
				}
			}
			return still;
		}

		public void StillToActivate(Db4objects.Db4o.Internal.Transaction trans, object a_object
			, int a_depth)
		{
			_stillToActivate = StillTo1(trans, _stillToActivate, a_object, a_depth, false);
		}

		public void StillToDeactivate(Db4objects.Db4o.Internal.Transaction trans, object 
			a_object, int a_depth, bool a_forceUnknownDeactivate)
		{
			_stillToDeactivate = StillTo1(trans, _stillToDeactivate, a_object, a_depth, a_forceUnknownDeactivate
				);
		}

		internal virtual void StillToSet(Db4objects.Db4o.Internal.Transaction a_trans, ObjectReference
			 a_yapObject, int a_updateDepth)
		{
			if (StackIsSmall())
			{
				if (a_yapObject.ContinueSet(a_trans, a_updateDepth))
				{
					return;
				}
			}
			_stillToSet = new List4(_stillToSet, a_trans);
			_stillToSet = new List4(_stillToSet, a_yapObject);
			_stillToSet = new List4(_stillToSet, a_updateDepth);
		}

		protected void StopSession()
		{
			if (HasShutDownHook())
			{
				Platform4.RemoveShutDownHook(this);
			}
			_classCollection = null;
			if (_references != null)
			{
				_references.StopTimer();
			}
			_systemTransaction = null;
			_transaction = null;
		}

		public IStoredClass StoredClass(Db4objects.Db4o.Internal.Transaction trans, object
			 clazz)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				IReflectClass claxx = ReflectorUtils.ReflectClassFor(Reflector(), clazz);
				if (claxx == null)
				{
					return null;
				}
				ClassMetadata classMetadata = ClassMetadataForReflectClass(claxx);
				if (classMetadata == null)
				{
					return null;
				}
				return new StoredClassImpl(trans, classMetadata);
			}
		}

		public virtual IStoredClass[] StoredClasses(Db4objects.Db4o.Internal.Transaction 
			trans)
		{
			lock (_lock)
			{
				trans = CheckTransaction(trans);
				IStoredClass[] classMetadata = _classCollection.StoredClasses();
				IStoredClass[] storedClasses = new IStoredClass[classMetadata.Length];
				for (int i = 0; i < classMetadata.Length; i++)
				{
					storedClasses[i] = new StoredClassImpl(trans, (ClassMetadata)classMetadata[i]);
				}
				return storedClasses;
			}
		}

		public virtual LatinStringIO StringIO()
		{
			return _handlers.StringIO();
		}

		public abstract ISystemInfo SystemInfo();

		private void BeginTopLevelCall()
		{
			GenerateCallIDOnTopLevel();
			if (_stackDepth == 0)
			{
				_topLevelCallCompleted = false;
			}
			_stackDepth++;
		}

		public void BeginTopLevelSet()
		{
			BeginTopLevelCall();
		}

		private void CompleteTopLevelCall()
		{
			if (_stackDepth == 1)
			{
				_topLevelCallCompleted = true;
			}
		}

		private void CompleteTopLevelCall(Db4oException e)
		{
			CompleteTopLevelCall();
			throw e;
		}

		public void CompleteTopLevelSet()
		{
			CompleteTopLevelCall();
		}

		public void CompleteTopLevelSet(Db4oException e)
		{
			CompleteTopLevelCall();
			throw e;
		}

		private void EndTopLevelCall()
		{
			_stackDepth--;
			GenerateCallIDOnTopLevel();
			if (_stackDepth == 0)
			{
				if (!_topLevelCallCompleted)
				{
					ShutdownObjectContainer();
				}
			}
		}

		public void EndTopLevelSet(Db4objects.Db4o.Internal.Transaction trans)
		{
			EndTopLevelCall();
			if (_stackDepth == 0 && _topLevelCallCompleted)
			{
				trans.ProcessDeletes();
			}
		}

		private void GenerateCallIDOnTopLevel()
		{
			if (_stackDepth == 0)
			{
				_topLevelCallId = _topLevelCallIdGenerator.Next();
			}
		}

		public virtual int StackDepth()
		{
			return _stackDepth;
		}

		public virtual void StackDepth(int depth)
		{
			_stackDepth = depth;
		}

		public virtual int TopLevelCallId()
		{
			return _topLevelCallId;
		}

		public virtual void TopLevelCallId(int id)
		{
			_topLevelCallId = id;
		}

		public virtual long Version()
		{
			lock (_lock)
			{
				return CurrentVersion();
			}
		}

		public abstract void Shutdown();

		public abstract void WriteDirty();

		public abstract void WriteNew(Db4objects.Db4o.Internal.Transaction trans, Pointer4
			 pointer, ClassMetadata classMetadata, Db4objects.Db4o.Internal.Buffer buffer);

		public abstract void WriteTransactionPointer(int address);

		public abstract void WriteUpdate(Db4objects.Db4o.Internal.Transaction trans, Pointer4
			 pointer, ClassMetadata classMetadata, Db4objects.Db4o.Internal.Buffer buffer);

		private static ExternalObjectContainer Cast(Db4objects.Db4o.Internal.PartialObjectContainer
			 obj)
		{
			return (ExternalObjectContainer)obj;
		}

		public virtual ICallbacks Callbacks()
		{
			return _callbacks;
		}

		public virtual void Callbacks(ICallbacks cb)
		{
			if (cb == null)
			{
				throw new ArgumentException();
			}
			_callbacks = cb;
		}

		public virtual Config4Impl ConfigImpl()
		{
			return _config;
		}

		public virtual UUIDFieldMetadata UUIDIndex()
		{
			return _handlers.Indexes()._uUID;
		}

		public virtual VersionFieldMetadata VersionIndex()
		{
			return _handlers.Indexes()._version;
		}

		public virtual ClassMetadataRepository ClassCollection()
		{
			return _classCollection;
		}

		public virtual ClassInfoHelper GetClassMetaHelper()
		{
			return _classMetaHelper;
		}

		public abstract long[] GetIDsForClass(Db4objects.Db4o.Internal.Transaction trans, 
			ClassMetadata clazz);

		public abstract IQueryResult ClassOnlyQuery(Db4objects.Db4o.Internal.Transaction 
			trans, ClassMetadata clazz);

		public abstract IQueryResult ExecuteQuery(QQuery query);

		public virtual void ReplicationCallState(int state)
		{
			_replicationCallState = state;
		}

		public abstract void OnCommittedListener();

		public virtual Db4objects.Db4o.Internal.ReferenceSystemRegistry ReferenceSystemRegistry
			()
		{
			return _referenceSystemRegistry;
		}

		public virtual ObjectContainerBase Container()
		{
			return _this;
		}
	}
}
