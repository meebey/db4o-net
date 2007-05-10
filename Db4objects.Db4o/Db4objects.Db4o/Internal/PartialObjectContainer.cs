using System;
using System.Collections;
using System.IO;
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

		protected Config4Impl i_config;

		private int _stackDepth;

		private TransactionalReferenceSystem _referenceSystem;

		private Tree i_justPeeked;

		public readonly object i_lock;

		private List4 _pendingClassUpdates;

		internal readonly ObjectContainerBase i_parent;

		internal bool i_refreshInsteadOfActivate;

		internal int i_showInternalClasses = 0;

		private List4 i_stillToActivate;

		private List4 i_stillToDeactivate;

		private List4 i_stillToSet;

		private Transaction i_systemTrans;

		protected Transaction i_trans;

		private bool i_instantiating;

		public HandlerRegistry i_handlers;

		internal int _replicationCallState;

		internal WeakReferenceCollector i_references;

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
			i_parent = parent == null ? _this : parent;
			i_lock = parent == null ? new object() : parent.i_lock;
			i_config = (Config4Impl)config;
		}

		public void Open()
		{
			bool ok = false;
			lock (i_lock)
			{
				try
				{
					InitializeTransactions();
					Initialize1(i_config);
					OpenImpl();
					InitializePostOpen();
					Platform4.PostOpen(_this);
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

		public virtual void Activate(object a_activate, int a_depth)
		{
			lock (i_lock)
			{
				CheckClosed();
				Activate1(null, a_activate, a_depth);
			}
		}

		public void Activate1(Transaction ta, object a_activate)
		{
			Activate1(ta, a_activate, ConfigImpl().ActivationDepth());
		}

		public void Activate1(Transaction ta, object a_activate, int a_depth)
		{
			Activate2(CheckTransaction(ta), a_activate, a_depth);
		}

		internal void Activate2(Transaction ta, object a_activate, int a_depth)
		{
			BeginTopLevelCall();
			try
			{
				StillToActivate(a_activate, a_depth);
				Activate3CheckStill(ta);
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

		internal void Activate3CheckStill(Transaction ta)
		{
			while (i_stillToActivate != null)
			{
				IEnumerator i = new Iterator4Impl(i_stillToActivate);
				i_stillToActivate = null;
				while (i.MoveNext())
				{
					ObjectReference yo = (ObjectReference)i.Current;
					i.MoveNext();
					int depth = ((int)i.Current);
					object obj = yo.GetObject();
					if (obj == null)
					{
						RemoveReference(yo);
					}
					else
					{
						yo.Activate1(ta, obj, depth, i_refreshInsteadOfActivate);
					}
				}
			}
		}

		public virtual void Bind(object obj, long id)
		{
			lock (i_lock)
			{
				Bind1(null, obj, id);
			}
		}

		/// <summary>TODO: This is not transactional yet.</summary>
		/// <remarks>TODO: This is not transactional yet.</remarks>
		public void Bind1(Transaction ta, object obj, long id)
		{
			ta = CheckTransaction(ta);
			int intID = (int)id;
			if (obj != null)
			{
				object oldObject = GetByID(id);
				if (oldObject != null)
				{
					ObjectReference yo = ReferenceForId(intID);
					if (yo != null)
					{
						if (ta.Reflector().ForObject(obj) == yo.GetYapClass().ClassReflector())
						{
							Bind2(yo, obj);
						}
						else
						{
							throw new Exception(Db4objects.Db4o.Internal.Messages.Get(57));
						}
					}
				}
			}
		}

		public void Bind2(ObjectReference @ref, object obj)
		{
			int id = @ref.GetID();
			RemoveReference(@ref);
			@ref = new ObjectReference(ClassMetadataForReflectClass(Reflector().ForObject(obj
				)), id);
			@ref.SetObjectWeak(_this, obj);
			@ref.SetStateDirty();
			_referenceSystem.AddExistingReference(@ref);
		}

		public abstract byte BlockSize();

		public virtual int BlocksToBytes(long bytes)
		{
			int blockLen = BlockSize();
			return (int)((bytes + blockLen - 1) / blockLen);
		}

		public int BlockAlignedBytes(int bytes)
		{
			return BlocksToBytes(bytes) * BlockSize();
		}

		public int BytesToBlocks(int blocks)
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
			return Platform4.Jdk().IsEnum(Reflector(), reference.GetYapClass().ClassReflector
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
			if (i_config.IsReadOnly())
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
				yapClass.Write(i_systemTrans);
			}
			_pendingClassUpdates = null;
		}

		public Transaction CheckTransaction(Transaction ta)
		{
			CheckClosed();
			if (ta != null)
			{
				return ta;
			}
			return GetTransaction();
		}

		public bool Close()
		{
			lock (i_lock)
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
			Platform4.PreClose(_this);
			ProcessPendingClassUpdates();
			if (StateMessages())
			{
				LogMsg(2, ToString());
			}
			Close2();
		}

		protected abstract void Close2();

		protected void ShutdownObjectContainer()
		{
			LogMsg(3, ToString());
			lock (i_lock)
			{
				StopSession();
				ShutdownDataStorage();
			}
		}

		protected abstract void ShutdownDataStorage();

		public virtual IDb4oCollections Collections()
		{
			lock (i_lock)
			{
				if (i_handlers.i_collections == null)
				{
					i_handlers.i_collections = Platform4.Collections(this);
				}
				return i_handlers.i_collections;
			}
		}

		public virtual void Commit()
		{
			lock (i_lock)
			{
				CheckClosed();
				CheckReadOnly();
				BeginTopLevelCall();
				try
				{
					Commit1();
					_referenceSystem.Commit();
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

		public abstract void Commit1();

		public virtual IConfiguration Configure()
		{
			return ConfigImpl();
		}

		public virtual Config4Impl Config()
		{
			return ConfigImpl();
		}

		public abstract int ConverterVersion();

		public abstract AbstractQueryResult NewQueryResult(Transaction trans, QueryEvaluationMode
			 mode);

		protected virtual void CreateStringIO(byte encoding)
		{
			SetStringIo(LatinStringIO.ForEncoding(encoding));
		}

		protected void InitializeTransactions()
		{
			i_systemTrans = NewTransaction(null);
			i_trans = NewTransaction();
		}

		public abstract Transaction NewTransaction(Transaction parentTransaction);

		public virtual Transaction NewTransaction()
		{
			return NewTransaction(i_systemTrans);
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
		public virtual IDb4oType Db4oTypeStored(Transaction trans, object obj)
		{
			if (!(obj is Db4oDatabase))
			{
				return null;
			}
			Db4oDatabase database = (Db4oDatabase)obj;
			if (ReferenceForObject(obj) != null)
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

		public virtual void Deactivate(object a_deactivate, int a_depth)
		{
			lock (i_lock)
			{
				CheckClosed();
				BeginTopLevelCall();
				try
				{
					Deactivate1(a_deactivate, a_depth);
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

		private void Deactivate1(object a_activate, int a_depth)
		{
			StillToDeactivate(a_activate, a_depth, true);
			while (i_stillToDeactivate != null)
			{
				IEnumerator i = new Iterator4Impl(i_stillToDeactivate);
				i_stillToDeactivate = null;
				while (i.MoveNext())
				{
					ObjectReference currentObject = (ObjectReference)i.Current;
					i.MoveNext();
					int currentInteger = ((int)i.Current);
					currentObject.Deactivate(i_trans, currentInteger);
				}
			}
		}

		public virtual void Delete(object a_object)
		{
			Delete(null, a_object);
		}

		public virtual void Delete(Transaction trans, object obj)
		{
			lock (i_lock)
			{
				CheckClosed();
				CheckReadOnly();
				trans = CheckTransaction(trans);
				Delete1(trans, obj, true);
				trans.ProcessDeletes();
			}
		}

		public void Delete1(Transaction trans, object obj, bool userCall)
		{
			if (obj == null)
			{
				return;
			}
			ObjectReference @ref = ReferenceForObject(obj);
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

		public void Delete2(Transaction trans, ObjectReference @ref, object obj, int cascade
			, bool userCall)
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

		internal void Delete3(Transaction trans, ObjectReference @ref, int cascade, bool 
			userCall)
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
			ClassMetadata yc = @ref.GetYapClass();
			object obj = @ref.GetObject();
			@ref.EndProcessing();
			if (!ObjectCanDelete(yc, obj))
			{
				return;
			}
			@ref.BeginProcessing();
			if (Delete4(trans, @ref, cascade, userCall))
			{
				ObjectOnDelete(yc, obj);
				if (ConfigImpl().MessageLevel() > Const4.STATE)
				{
					Message(string.Empty + @ref.GetID() + " delete " + @ref.GetYapClass().GetName());
				}
			}
			@ref.EndProcessing();
		}

		private bool ObjectCanDelete(ClassMetadata yc, object obj)
		{
			return _this.Callbacks().ObjectCanDelete(obj) && yc.DispatchEvent(_this, obj, EventDispatcher
				.CAN_DELETE);
		}

		private void ObjectOnDelete(ClassMetadata yc, object obj)
		{
			_this.Callbacks().ObjectOnDelete(obj);
			yc.DispatchEvent(_this, obj, EventDispatcher.DELETE);
		}

		public abstract bool Delete4(Transaction ta, ObjectReference yapObject, int a_cascade
			, bool userCall);

		public virtual object Descend(object obj, string[] path)
		{
			lock (i_lock)
			{
				return Descend1(CheckTransaction(null), obj, path);
			}
		}

		private object Descend1(Transaction trans, object obj, string[] path)
		{
			ObjectReference yo = ReferenceForObject(obj);
			if (yo == null)
			{
				return null;
			}
			object child = null;
			string fieldName = path[0];
			if (fieldName == null)
			{
				return null;
			}
			ClassMetadata yc = yo.GetYapClass();
			FieldMetadata[] field = new FieldMetadata[] { null };
			yc.ForEachFieldMetadata(new _AnonymousInnerClass584(this, fieldName, field));
			if (field[0] == null)
			{
				return null;
			}
			if (yo.IsActive())
			{
				child = field[0].Get(obj);
			}
			else
			{
				Db4objects.Db4o.Internal.Buffer reader = ReadReaderByID(trans, yo.GetID());
				if (reader == null)
				{
					return null;
				}
				MarshallerFamily mf = yc.FindOffset(reader, field[0]);
				if (mf == null)
				{
					return null;
				}
				try
				{
					child = field[0].ReadQuery(trans, mf, reader);
				}
				catch (CorruptionException)
				{
				}
			}
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
			return Descend1(trans, child, subPath);
		}

		private sealed class _AnonymousInnerClass584 : IVisitor4
		{
			public _AnonymousInnerClass584(PartialObjectContainer _enclosing, string fieldName
				, FieldMetadata[] field)
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

		public virtual IExtObjectContainer Ext()
		{
			return _this;
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
			i_references.PollReferenceQueue();
		}

		public virtual IObjectSet Get(object template)
		{
			lock (i_lock)
			{
				CheckClosed();
				return Get1(null, template);
			}
		}

		private ObjectSetFacade Get1(Transaction ta, object template)
		{
			ta = CheckTransaction(ta);
			IQueryResult res = null;
			try
			{
				BeginTopLevelCall();
				res = Get2(ta, template);
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

		private IQueryResult Get2(Transaction ta, object template)
		{
			if (template == null || template.GetType() == Const4.CLASS_OBJECT)
			{
				return GetAll(ta);
			}
			IQuery q = Query(ta);
			q.Constrain(template);
			return ExecuteQuery((QQuery)q);
		}

		public abstract AbstractQueryResult GetAll(Transaction ta);

		public virtual object GetByID(long id)
		{
			if (id <= 0)
			{
				throw new ArgumentException();
			}
			lock (i_lock)
			{
				CheckClosed();
				return GetByID1(null, id);
			}
		}

		public object GetByID1(Transaction ta, long id)
		{
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

		internal object GetByID2(Transaction ta, int id)
		{
			object obj = ObjectForIdFromCache(id);
			if (obj != null)
			{
				return obj;
			}
			return new ObjectReference(id).Read(ta, 0, Const4.ADD_TO_ID_TREE, true);
		}

		public object GetActivatedObjectFromCache(Transaction ta, int id)
		{
			object obj = ObjectForIdFromCache(id);
			if (obj == null)
			{
				return null;
			}
			Activate1(ta, obj, ConfigImpl().ActivationDepth());
			return obj;
		}

		public object ReadActivatedObjectNotInCache(Transaction ta, int id)
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

		public object GetByUUID(Db4oUUID uuid)
		{
			lock (i_lock)
			{
				if (uuid == null)
				{
					return null;
				}
				Transaction ta = CheckTransaction(null);
				HardObjectReference hardRef = ta.GetHardReferenceBySignature(uuid.GetLongPart(), 
					uuid.GetSignaturePart());
				return hardRef._object;
			}
		}

		public virtual long GetID(object obj)
		{
			lock (i_lock)
			{
				return GetID1(obj);
			}
		}

		public int GetID1(object obj)
		{
			CheckClosed();
			if (obj == null)
			{
				return 0;
			}
			ObjectReference yo = ReferenceForObject(obj);
			if (yo != null)
			{
				return yo.GetID();
			}
			return 0;
		}

		public virtual IObjectInfo GetObjectInfo(object obj)
		{
			lock (i_lock)
			{
				return ReferenceForObject(obj);
			}
		}

		public HardObjectReference GetHardObjectReferenceById(int id)
		{
			return GetHardObjectReferenceById(GetTransaction(), id);
		}

		public HardObjectReference GetHardObjectReferenceById(Transaction trans, int id)
		{
			if (id <= 0)
			{
				return HardObjectReference.INVALID;
			}
			ObjectReference @ref = ReferenceForId(id);
			if (@ref != null)
			{
				object candidate = @ref.GetObject();
				if (candidate != null)
				{
					return new HardObjectReference(@ref, candidate);
				}
				RemoveReference(@ref);
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

		public StatefulBuffer GetWriter(Transaction a_trans, int a_address, int a_length)
		{
			if (Debug.ExceedsMaximumBlockSize(a_length))
			{
				return null;
			}
			return new StatefulBuffer(a_trans, a_address, a_length);
		}

		public Transaction SystemTransaction()
		{
			return i_systemTrans;
		}

		public Transaction GetTransaction()
		{
			return i_trans;
		}

		public ClassMetadata ClassMetadataForReflectClass(IReflectClass claxx)
		{
			if (CantGetClassMetadata(claxx))
			{
				return null;
			}
			ClassMetadata yc = i_handlers.GetYapClassStatic(claxx);
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
			ClassMetadata yc = i_handlers.GetYapClassStatic(claxx);
			if (yc != null)
			{
				return yc;
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
			ClassMetadata yc = i_handlers.GetYapClassStatic(claxx);
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
			if ((!ShowInternalClasses()) && i_handlers.ICLASS_INTERNAL.IsAssignableFrom(claxx
				))
			{
				return true;
			}
			return false;
		}

		public virtual ClassMetadata ClassMetadataForId(int id)
		{
			if (id == 0)
			{
				return null;
			}
			ClassMetadata yc = i_handlers.GetYapClassStatic(id);
			if (yc != null)
			{
				return yc;
			}
			return _classCollection.GetYapClass(id);
		}

		public virtual object ObjectForIdFromCache(int id)
		{
			ObjectReference @ref = ReferenceForId(id);
			if (@ref == null)
			{
				return null;
			}
			object candidate = @ref.GetObject();
			if (candidate == null)
			{
				RemoveReference(@ref);
			}
			return candidate;
		}

		public ObjectReference ReferenceForId(int id)
		{
			return _referenceSystem.ReferenceForId(id);
		}

		public ObjectReference ReferenceForObject(object obj)
		{
			return _referenceSystem.ReferenceForObject(obj);
		}

		public virtual HandlerRegistry Handlers()
		{
			return i_handlers;
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
			InitializeConfig(i_config);
			i_handlers = new HandlerRegistry(_this, ConfigImpl().Encoding(), ConfigImpl().Reflector
				());
			if (i_references != null)
			{
				Gc();
				i_references.StopTimer();
			}
			i_references = new WeakReferenceCollector(_this);
			if (HasShutDownHook())
			{
				Platform4.AddShutDownHook(this);
			}
			i_handlers.InitEncryption(ConfigImpl());
			Initialize2();
			i_stillToSet = null;
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
			_referenceSystem = new TransactionalReferenceSystem();
		}

		/// <summary>overridden in YapObjectCarrier</summary>
		internal virtual void Initialize2NObjectCarrier()
		{
			_classCollection = new ClassMetadataRepository(i_systemTrans);
			i_references.StartTimer();
		}

		private void InitializePostOpen()
		{
			i_showInternalClasses = 100000;
			InitializePostOpenExcludingTransportObjectContainer();
			i_showInternalClasses = 0;
		}

		protected virtual void InitializePostOpenExcludingTransportObjectContainer()
		{
			InitializeEssentialClasses();
			try
			{
				Rename(ConfigImpl());
				_classCollection.InitOnUp(i_systemTrans);
				if (ConfigImpl().DetectSchemaChanges())
				{
					i_systemTrans.Commit();
				}
				ConfigImpl().ApplyConfigurationItems(_this);
			}
			catch (IOException e)
			{
				throw new Db4oIOException(e);
			}
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
			i_instantiating = flag;
		}

		public virtual bool IsActive(object obj)
		{
			lock (i_lock)
			{
				return IsActive1(obj);
			}
		}

		internal bool IsActive1(object obj)
		{
			CheckClosed();
			if (obj != null)
			{
				ObjectReference yo = ReferenceForObject(obj);
				if (yo != null)
				{
					return yo.IsActive();
				}
			}
			return false;
		}

		public virtual bool IsCached(long a_id)
		{
			lock (i_lock)
			{
				return ObjectForIdFromCache((int)a_id) != null;
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
			lock (i_lock)
			{
				return _classCollection == null;
			}
		}

		internal bool IsInstantiating()
		{
			return i_instantiating;
		}

		internal virtual bool IsServer()
		{
			return false;
		}

		public virtual bool IsStored(object obj)
		{
			lock (i_lock)
			{
				return IsStored1(obj);
			}
		}

		internal bool IsStored1(object obj)
		{
			Transaction ta = CheckTransaction(null);
			if (obj == null)
			{
				return false;
			}
			ObjectReference yo = ReferenceForObject(obj);
			if (yo == null)
			{
				return false;
			}
			return !ta.IsDeleted(yo.GetID());
		}

		public virtual IReflectClass[] KnownClasses()
		{
			lock (i_lock)
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
			if (i_handlers.IsSystemHandler(id))
			{
				return i_handlers.GetHandler(id);
			}
			return ClassMetadataForId(id);
		}

		public virtual object Lock()
		{
			return i_lock;
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
				if (i_handlers.i_migration != null)
				{
					i_handlers.i_migration.Terminate();
				}
				i_handlers.i_migration = null;
			}
			else
			{
				ObjectContainerBase peer = (ObjectContainerBase)objectContainer;
				_replicationCallState = Const4.OLD;
				peer._replicationCallState = Const4.OLD;
				i_handlers.i_migration = new MigrationConnection(_this, (ObjectContainerBase)objectContainer
					);
				peer.i_handlers.i_migration = i_handlers.i_migration;
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

		public virtual object PeekPersisted(object obj, int depth, bool committed)
		{
			lock (i_lock)
			{
				CheckClosed();
				BeginTopLevelCall();
				try
				{
					i_justPeeked = null;
					Transaction ta = committed ? i_systemTrans : CheckTransaction(null);
					object cloned = null;
					ObjectReference yo = ReferenceForObject(obj);
					if (yo != null)
					{
						cloned = PeekPersisted(ta, yo.GetID(), depth);
					}
					i_justPeeked = null;
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

		public object PeekPersisted(Transaction trans, int id, int depth)
		{
			if (depth < 0)
			{
				return null;
			}
			TreeInt ti = new TreeInt(id);
			TreeIntObject tio = (TreeIntObject)Tree.Find(i_justPeeked, ti);
			if (tio == null)
			{
				return new ObjectReference(id).PeekPersisted(trans, depth);
			}
			return tio._object;
		}

		internal virtual void Peeked(int a_id, object a_object)
		{
			i_justPeeked = Tree.Add(i_justPeeked, new TreeIntObject(a_id, a_object));
		}

		public virtual void Purge()
		{
			lock (i_lock)
			{
				Purge1();
			}
		}

		public virtual void Purge(object obj)
		{
			lock (i_lock)
			{
				Purge1(obj);
			}
		}

		internal void Purge1()
		{
			CheckClosed();
			Runtime.Gc();
			Runtime.RunFinalization();
			Runtime.Gc();
			Gc();
			_classCollection.Purge();
		}

		internal void Purge1(object obj)
		{
			if (obj == null)
			{
				return;
			}
			if (obj is ObjectReference)
			{
				RemoveReference((ObjectReference)obj);
				return;
			}
			ObjectReference @ref = ReferenceForObject(obj);
			if (@ref != null)
			{
				RemoveReference(@ref);
			}
		}

		public NativeQueryHandler GetNativeQueryHandler()
		{
			if (null == _nativeQueryHandler)
			{
				_nativeQueryHandler = new NativeQueryHandler(_this);
			}
			return _nativeQueryHandler;
		}

		public IObjectSet Query(Predicate predicate)
		{
			return Query(predicate, (IQueryComparator)null);
		}

		public IObjectSet Query(Predicate predicate, IQueryComparator comparator)
		{
			lock (i_lock)
			{
				return GetNativeQueryHandler().Execute(predicate, comparator);
			}
		}

		public virtual IQuery Query()
		{
			lock (i_lock)
			{
				return Query((Transaction)null);
			}
		}

		public IObjectSet Query(Type clazz)
		{
			return Get(clazz);
		}

		public IQuery Query(Transaction ta)
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
			i_handlers.Decrypt(reader);
			return reader;
		}

		private void CheckAddress(int address)
		{
			if (address <= 0)
			{
				throw new ArgumentException("Invalid address offset: " + address);
			}
		}

		public StatefulBuffer ReadWriterByAddress(Transaction a_trans, int address, int length
			)
		{
			CheckAddress(address);
			StatefulBuffer reader = GetWriter(a_trans, address, length);
			reader.ReadEncrypt(_this, address);
			return reader;
		}

		public abstract StatefulBuffer ReadWriterByID(Transaction a_ta, int a_id);

		public abstract Db4objects.Db4o.Internal.Buffer ReadReaderByID(Transaction a_ta, 
			int a_id);

		public abstract StatefulBuffer[] ReadWritersByIDs(Transaction a_ta, int[] ids);

		private void Reboot()
		{
			Commit();
			Close();
			Open();
		}

		public virtual IReferenceSystem ReferenceSystem()
		{
			return _referenceSystem;
		}

		public virtual GenericReflector Reflector()
		{
			return i_handlers._reflector;
		}

		public virtual void Refresh(object a_refresh, int a_depth)
		{
			lock (i_lock)
			{
				i_refreshInsteadOfActivate = true;
				try
				{
					Activate1(null, a_refresh, a_depth);
				}
				finally
				{
					i_refreshInsteadOfActivate = false;
				}
			}
		}

		internal void RefreshClasses()
		{
			lock (i_lock)
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

		public abstract void ReleaseSemaphores(Transaction ta);

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
				if (Get(ren).Size() == 0)
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
						IObjectSet backren = Get(new Db4objects.Db4o.Rename(ren.rClass, null, ren.rFrom));
						while (backren.HasNext())
						{
							Delete(backren.Next());
						}
						Set(ren);
					}
				}
			}
			return renamedOne;
		}

		public virtual IReplicationProcess ReplicationBegin(IObjectContainer peerB, IReplicationConflictHandler
			 conflictHandler)
		{
			return new ReplicationImpl(_this, peerB, conflictHandler);
		}

		public int OldReplicationHandles(object obj)
		{
			if (_replicationCallState != Const4.OLD)
			{
				return 0;
			}
			if (i_handlers.i_replication == null)
			{
				return 0;
			}
			if (obj is IInternal4)
			{
				return 0;
			}
			ObjectReference reference = ReferenceForObject(obj);
			if (reference != null && HandledInCurrentTopLevelCall(reference))
			{
				return reference.GetID();
			}
			return i_handlers.i_replication.TryToHandle(_this, obj);
		}

		public bool HandledInCurrentTopLevelCall(ObjectReference @ref)
		{
			return @ref.IsFlaggedAsHandled(_topLevelCallId);
		}

		public abstract void Reserve(int byteCount);

		public virtual void Rollback()
		{
			lock (i_lock)
			{
				CheckClosed();
				CheckReadOnly();
				Rollback1();
				_referenceSystem.Rollback();
			}
		}

		public abstract void Rollback1();

		public virtual void Send(object obj)
		{
		}

		public virtual void Set(object a_object)
		{
			Set(a_object, Const4.UNSPECIFIED);
		}

		public void Set(Transaction trans, object obj)
		{
			Set(trans, obj, Const4.UNSPECIFIED);
		}

		public void Set(object obj, int depth)
		{
			Set(i_trans, obj, depth);
		}

		public virtual void Set(Transaction trans, object obj, int depth)
		{
			lock (i_lock)
			{
				SetInternal(trans, obj, depth, true);
			}
		}

		public int SetInternal(Transaction trans, object obj, bool checkJustSet)
		{
			return SetInternal(trans, obj, Const4.UNSPECIFIED, checkJustSet);
		}

		public int SetInternal(Transaction trans, object obj, int depth, bool checkJustSet
			)
		{
			CheckClosed();
			CheckReadOnly();
			BeginTopLevelSet();
			try
			{
				int id = OldReplicationHandles(obj);
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

		public int SetAfterReplication(Transaction trans, object obj, int depth, bool checkJust
			)
		{
			if (obj is IDb4oType)
			{
				IDb4oType db4oType = Db4oTypeStored(trans, obj);
				if (db4oType != null)
				{
					return GetID1(db4oType);
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
			lock (i_lock)
			{
				_replicationCallState = Const4.NEW;
				i_handlers._replicationReferenceProvider = referenceProvider;
				Set2(CheckTransaction(null), obj, 1, false);
				_replicationCallState = Const4.NONE;
				i_handlers._replicationReferenceProvider = null;
			}
		}

		private int Set2(Transaction trans, object obj, int depth, bool checkJust)
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
			while (i_stillToSet != null)
			{
				IEnumerator i = new Iterator4Impl(i_stillToSet);
				i_stillToSet = null;
				while (i.MoveNext())
				{
					int updateDepth = (int)i.Current;
					i.MoveNext();
					ObjectReference @ref = (ObjectReference)i.Current;
					i.MoveNext();
					Transaction trans = (Transaction)i.Current;
					if (!@ref.ContinueSet(trans, updateDepth))
					{
						postponedStillToSet = new List4(postponedStillToSet, trans);
						postponedStillToSet = new List4(postponedStillToSet, @ref);
						postponedStillToSet = new List4(postponedStillToSet, updateDepth);
					}
				}
			}
			i_stillToSet = postponedStillToSet;
		}

		private void NotStorable(IReflectClass claxx, object obj)
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

		public int Set3(Transaction trans, object obj, int updateDepth, bool checkJustSet
			)
		{
			if (obj == null || (obj is ITransientClass))
			{
				return 0;
			}
			if (obj is IDb4oTypeImpl)
			{
				((IDb4oTypeImpl)obj).StoredTo(trans);
			}
			ClassMetadata yc = null;
			ObjectReference @ref = ReferenceForObject(obj);
			if (@ref == null)
			{
				IReflectClass claxx = Reflector().ForObject(obj);
				if (claxx == null)
				{
					NotStorable(claxx, obj);
					return 0;
				}
				yc = GetActiveClassMetadata(claxx);
				if (yc == null)
				{
					yc = ProduceClassMetadata(claxx);
					if (yc == null)
					{
						NotStorable(claxx, obj);
						return 0;
					}
					@ref = ReferenceForObject(obj);
				}
			}
			else
			{
				yc = @ref.GetYapClass();
			}
			if (IsPlainObjectOrPrimitive(yc))
			{
				NotStorable(yc.ClassReflector(), obj);
				return 0;
			}
			if (@ref == null)
			{
				if (!ObjectCanNew(yc, obj))
				{
					return 0;
				}
				@ref = new ObjectReference();
				@ref.Store(trans, yc, obj);
				_referenceSystem.AddNewReference(@ref);
				if (obj is IDb4oTypeImpl)
				{
					((IDb4oTypeImpl)obj).SetTrans(trans);
				}
				if (ConfigImpl().MessageLevel() > Const4.STATE)
				{
					Message(string.Empty + @ref.GetID() + " new " + @ref.GetYapClass().GetName());
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

		private bool IsPlainObjectOrPrimitive(ClassMetadata yc)
		{
			return yc.GetID() == HandlerRegistry.ANY_ID || yc.IsPrimitive();
		}

		private bool ObjectCanNew(ClassMetadata yc, object a_object)
		{
			return Callbacks().ObjectCanNew(a_object) && yc.DispatchEvent(_this, a_object, EventDispatcher
				.CAN_NEW);
		}

		public abstract void SetDirtyInSystemTransaction(PersistentBase a_object);

		public abstract bool SetSemaphore(string name, int timeout);

		internal virtual void SetStringIo(LatinStringIO a_io)
		{
			i_handlers.i_stringHandler.SetStringIo(a_io);
		}

		internal bool ShowInternalClasses()
		{
			return IsServer() || i_showInternalClasses > 0;
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
					i_showInternalClasses++;
				}
				else
				{
					i_showInternalClasses--;
				}
				if (i_showInternalClasses < 0)
				{
					i_showInternalClasses = 0;
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
		internal List4 StillTo1(List4 still, object obj, int depth, bool forceUnknownDeactivate
			)
		{
			if (obj == null || depth <= 0)
			{
				return still;
			}
			ObjectReference @ref = ReferenceForObject(obj);
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
						still = StillTo1(still, arr[i], depth, forceUnknownDeactivate);
					}
				}
			}
			else
			{
				if (obj is Entry)
				{
					still = StillTo1(still, ((Entry)obj).key, depth, false);
					still = StillTo1(still, ((Entry)obj).value, depth, false);
				}
				else
				{
					if (forceUnknownDeactivate)
					{
						ClassMetadata yc = ClassMetadataForReflectClass(Reflector().ForObject(obj));
						if (yc != null)
						{
							yc.Deactivate(i_trans, obj, depth);
						}
					}
				}
			}
			return still;
		}

		public virtual void StillToActivate(object a_object, int a_depth)
		{
			i_stillToActivate = StillTo1(i_stillToActivate, a_object, a_depth, false);
		}

		public virtual void StillToDeactivate(object a_object, int a_depth, bool a_forceUnknownDeactivate
			)
		{
			i_stillToDeactivate = StillTo1(i_stillToDeactivate, a_object, a_depth, a_forceUnknownDeactivate
				);
		}

		internal virtual void StillToSet(Transaction a_trans, ObjectReference a_yapObject
			, int a_updateDepth)
		{
			if (StackIsSmall())
			{
				if (a_yapObject.ContinueSet(a_trans, a_updateDepth))
				{
					return;
				}
			}
			i_stillToSet = new List4(i_stillToSet, a_trans);
			i_stillToSet = new List4(i_stillToSet, a_yapObject);
			i_stillToSet = new List4(i_stillToSet, a_updateDepth);
		}

		protected void StopSession()
		{
			if (HasShutDownHook())
			{
				Platform4.RemoveShutDownHook(this);
			}
			_classCollection = null;
			if (i_references != null)
			{
				i_references.StopTimer();
			}
			i_systemTrans = null;
			i_trans = null;
		}

		public virtual IStoredClass StoredClass(object clazz)
		{
			lock (i_lock)
			{
				CheckClosed();
				IReflectClass claxx = ReflectorUtils.ReflectClassFor(Reflector(), clazz);
				if (claxx == null)
				{
					return null;
				}
				return ClassMetadataForReflectClass(claxx);
			}
		}

		public virtual IStoredClass[] StoredClasses()
		{
			lock (i_lock)
			{
				CheckClosed();
				return _classCollection.StoredClasses();
			}
		}

		public virtual LatinStringIO StringIO()
		{
			return i_handlers.i_stringHandler.i_stringIo;
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

		public void EndTopLevelSet(Transaction trans)
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
			lock (i_lock)
			{
				return CurrentVersion();
			}
		}

		public abstract void Shutdown();

		public abstract void WriteDirty();

		public abstract void WriteEmbedded(StatefulBuffer a_parent, StatefulBuffer a_child
			);

		public abstract void WriteNew(ClassMetadata a_yapClass, StatefulBuffer aWriter);

		public abstract void WriteTransactionPointer(int a_address);

		public abstract void WriteUpdate(ClassMetadata a_yapClass, StatefulBuffer a_bytes
			);

		public void RemoveReference(ObjectReference @ref)
		{
			_referenceSystem.RemoveReference(@ref);
			@ref.SetID(-1);
			Platform4.KillYapRef(@ref.GetObjectReference());
		}

		private static ObjectContainerBase Cast(Db4objects.Db4o.Internal.PartialObjectContainer
			 obj)
		{
			return (ObjectContainerBase)obj;
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
			return i_config;
		}

		public virtual UUIDFieldMetadata GetUUIDIndex()
		{
			return i_handlers.i_indexes.i_fieldUUID;
		}

		public virtual VersionFieldMetadata GetVersionIndex()
		{
			return i_handlers.i_indexes.i_fieldVersion;
		}

		public virtual ClassMetadataRepository ClassCollection()
		{
			return _classCollection;
		}

		public virtual ClassInfoHelper GetClassMetaHelper()
		{
			return _classMetaHelper;
		}

		public abstract long[] GetIDsForClass(Transaction trans, ClassMetadata clazz);

		public abstract IQueryResult ClassOnlyQuery(Transaction trans, ClassMetadata clazz
			);

		public abstract IQueryResult ExecuteQuery(QQuery query);

		public virtual void ReplicationCallState(int state)
		{
			_replicationCallState = state;
		}

		public abstract void OnCommittedListener();
	}
}
