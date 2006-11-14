namespace Db4objects.Db4o
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
	public abstract partial class YapStreamBase : Db4objects.Db4o.Types.ITransientClass
		, Db4objects.Db4o.IInternal4, Db4objects.Db4o.IYapStreamSpec
	#else
	public abstract class YapStreamBase : Db4objects.Db4o.Types.ITransientClass, Db4objects.Db4o.IInternal4
		, Db4objects.Db4o.IYapStreamSpec
	#endif
	{
		private bool i_amDuringFatalExit = false;

		protected Db4objects.Db4o.YapClassCollection _classCollection;

		protected Db4objects.Db4o.Config4Impl i_config;

		protected int i_entryCounter;

		private Db4objects.Db4o.YapObject i_hcTree;

		private Db4objects.Db4o.YapObject i_idTree;

		private Db4objects.Db4o.Foundation.Tree[] i_justActivated;

		private Db4objects.Db4o.Foundation.Tree[] i_justDeactivated;

		private Db4objects.Db4o.Foundation.Tree i_justPeeked;

		private Db4objects.Db4o.Foundation.Tree i_justSet;

		public readonly object i_lock;

		private Db4objects.Db4o.Foundation.List4 i_needsUpdate;

		internal readonly Db4objects.Db4o.YapStream i_parent;

		internal bool i_refreshInsteadOfActivate;

		internal int i_showInternalClasses = 0;

		private Db4objects.Db4o.Foundation.List4 i_stillToActivate;

		private Db4objects.Db4o.Foundation.List4 i_stillToDeactivate;

		private Db4objects.Db4o.Foundation.List4 i_stillToSet;

		protected Db4objects.Db4o.Transaction i_systemTrans;

		protected Db4objects.Db4o.Transaction i_trans;

		private bool i_instantiating;

		public Db4objects.Db4o.YapHandlers i_handlers;

		internal int _replicationCallState;

		internal Db4objects.Db4o.YapReferences i_references;

		private Db4objects.Db4o.Inside.Query.NativeQueryHandler _nativeQueryHandler;

		private readonly Db4objects.Db4o.YapStream _this;

		private Db4objects.Db4o.Inside.Callbacks.ICallbacks _callbacks = new Db4objects.Db4o.Inside.Callbacks.NullCallbacks
			();

		protected readonly Db4objects.Db4o.Foundation.PersistentTimeStampIdGenerator _timeStampIdGenerator
			 = new Db4objects.Db4o.Foundation.PersistentTimeStampIdGenerator();

		protected YapStreamBase(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.YapStream
			 a_parent)
		{
			_this = Cast(this);
			i_parent = a_parent == null ? _this : a_parent;
			i_lock = a_parent == null ? new object() : a_parent.i_lock;
			Initialize0();
			InitializeTransactions();
			Initialize1(config);
		}

		public virtual void Activate(object a_activate, int a_depth)
		{
			lock (i_lock)
			{
				Activate1(null, a_activate, a_depth);
			}
		}

		internal void Activate1(Db4objects.Db4o.Transaction ta, object a_activate)
		{
			Activate1(ta, a_activate, ConfigImpl().ActivationDepth());
		}

		public void Activate1(Db4objects.Db4o.Transaction ta, object a_activate, int a_depth
			)
		{
			ta = CheckTransaction(ta);
			BeginEndActivation();
			Activate2(ta, a_activate, a_depth);
			BeginEndActivation();
		}

		internal void BeginEndActivation()
		{
			i_justActivated[0] = null;
		}

		internal void BeginEndSet(Db4objects.Db4o.Transaction ta)
		{
			i_justSet = null;
			if (ta != null)
			{
				ta.BeginEndSet();
			}
		}

		/// <summary>internal call interface, does not reset i_justActivated</summary>
		internal void Activate2(Db4objects.Db4o.Transaction ta, object a_activate, int a_depth
			)
		{
			i_entryCounter++;
			try
			{
				StillToActivate(a_activate, a_depth);
				Activate3CheckStill(ta);
			}
			catch (System.Exception t)
			{
				FatalException(t);
			}
			i_entryCounter--;
		}

		internal void Activate3CheckStill(Db4objects.Db4o.Transaction ta)
		{
			while (i_stillToActivate != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_stillToActivate
					);
				i_stillToActivate = null;
				while (i.MoveNext())
				{
					Db4objects.Db4o.YapObject yo = (Db4objects.Db4o.YapObject)i.Current;
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

		public virtual int AlignToBlockSize(int length)
		{
			return BlocksFor(length) * BlockSize();
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
		internal void Bind1(Db4objects.Db4o.Transaction ta, object obj, long id)
		{
			ta = CheckTransaction(ta);
			int intID = (int)id;
			if (obj != null)
			{
				object oldObject = GetByID(id);
				if (oldObject != null)
				{
					Db4objects.Db4o.YapObject yo = GetYapObject(intID);
					if (yo != null)
					{
						if (ta.Reflector().ForObject(obj) == yo.GetYapClass().ClassReflector())
						{
							Bind2(yo, obj);
						}
						else
						{
							throw new System.Exception(Db4objects.Db4o.Messages.Get(57));
						}
					}
				}
			}
		}

		internal void Bind2(Db4objects.Db4o.YapObject a_yapObject, object obj)
		{
			int id = a_yapObject.GetID();
			RemoveReference(a_yapObject);
			a_yapObject = new Db4objects.Db4o.YapObject(GetYapClass(Reflector().ForObject(obj
				), false), id);
			a_yapObject.SetObjectWeak(_this, obj);
			a_yapObject.SetStateDirty();
			IdTreeAdd(a_yapObject);
			HcTreeAdd(a_yapObject);
		}

		public virtual byte BlockSize()
		{
			return 1;
		}

		public virtual int BlocksFor(long bytes)
		{
			int blockLen = BlockSize();
			int result = (int)(bytes / blockLen);
			if (bytes % blockLen != 0)
			{
				result++;
			}
			return result;
		}

		private bool BreakDeleteForEnum(Db4objects.Db4o.YapObject reference, bool userCall
			)
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
			return Db4objects.Db4o.Platform4.Jdk().IsEnum(Reflector(), reference.GetYapClass(
				).ClassReflector());
		}

		internal virtual bool CanUpdate()
		{
			return true;
		}

		public void CheckClosed()
		{
			if (_classCollection == null)
			{
				Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(20, ToString());
			}
		}

		internal void CheckNeededUpdates()
		{
			if (i_needsUpdate != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_needsUpdate
					);
				while (i.MoveNext())
				{
					Db4objects.Db4o.YapClass yapClass = (Db4objects.Db4o.YapClass)i.Current;
					yapClass.SetStateDirty();
					yapClass.Write(i_systemTrans);
				}
				i_needsUpdate = null;
			}
		}

		internal Db4objects.Db4o.Transaction CheckTransaction(Db4objects.Db4o.Transaction
			 ta)
		{
			CheckClosed();
			if (ta != null)
			{
				return ta;
			}
			return GetTransaction();
		}

		public virtual bool Close()
		{
			lock (Db4objects.Db4o.Inside.Global4.Lock)
			{
				lock (i_lock)
				{
					bool ret = Close1();
					return ret;
				}
			}
		}

		internal bool Close1()
		{
			if (_classCollection == null)
			{
				return true;
			}
			Db4objects.Db4o.Platform4.PreClose(_this);
			CheckNeededUpdates();
			if (StateMessages())
			{
				LogMsg(2, ToString());
			}
			bool closeResult = Close2();
			return closeResult;
		}

		protected virtual bool Close2()
		{
			StopSession();
			i_hcTree = null;
			i_idTree = null;
			i_systemTrans = null;
			i_trans = null;
			if (StateMessages())
			{
				LogMsg(3, ToString());
			}
			return true;
		}

		public virtual Db4objects.Db4o.Types.IDb4oCollections Collections()
		{
			lock (i_lock)
			{
				if (i_handlers.i_collections == null)
				{
					i_handlers.i_collections = Db4objects.Db4o.Platform4.Collections(this);
				}
				return i_handlers.i_collections;
			}
		}

		public virtual void Commit()
		{
			lock (i_lock)
			{
				Commit1();
			}
		}

		public abstract void Commit1();

		public virtual Db4objects.Db4o.Config.IConfiguration Configure()
		{
			return ConfigImpl();
		}

		public virtual Db4objects.Db4o.Config4Impl Config()
		{
			return ConfigImpl();
		}

		public abstract int ConverterVersion();

		public abstract Db4objects.Db4o.Inside.Query.AbstractQueryResult NewQueryResult(Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.Config.QueryEvaluationMode mode);

		protected virtual void CreateStringIO(byte encoding)
		{
			SetStringIo(Db4objects.Db4o.YapStringIO.ForEncoding(encoding));
		}

		protected void InitializeTransactions()
		{
			i_systemTrans = NewTransaction(null);
			i_trans = NewTransaction();
		}

		public virtual Db4objects.Db4o.Transaction NewTransaction(Db4objects.Db4o.Transaction
			 parentTransaction)
		{
			return new Db4objects.Db4o.Transaction(_this, parentTransaction);
		}

		public virtual Db4objects.Db4o.Transaction NewTransaction()
		{
			return NewTransaction(i_systemTrans);
		}

		public abstract long CurrentVersion();

		public virtual bool CreateYapClass(Db4objects.Db4o.YapClass a_yapClass, Db4objects.Db4o.Reflect.IReflectClass
			 a_class, Db4objects.Db4o.YapClass a_superYapClass)
		{
			return a_yapClass.Init(_this, a_superYapClass, a_class);
		}

		/// <summary>allows special handling for all Db4oType objects.</summary>
		/// <remarks>
		/// allows special handling for all Db4oType objects.
		/// Redirected here from #set() so only instanceof check is necessary
		/// in the #set() method.
		/// </remarks>
		/// <returns>object if handled here and #set() should not continue processing</returns>
		public virtual Db4objects.Db4o.Types.IDb4oType Db4oTypeStored(Db4objects.Db4o.Transaction
			 a_trans, object a_object)
		{
			if (a_object is Db4objects.Db4o.Ext.Db4oDatabase)
			{
				Db4objects.Db4o.Ext.Db4oDatabase database = (Db4objects.Db4o.Ext.Db4oDatabase)a_object;
				if (GetYapObject(a_object) != null)
				{
					return database;
				}
				ShowInternalClasses(true);
				Db4objects.Db4o.Ext.Db4oDatabase res = database.Query(a_trans);
				ShowInternalClasses(false);
				return res;
			}
			return null;
		}

		public virtual void Deactivate(object a_deactivate, int a_depth)
		{
			lock (i_lock)
			{
				Deactivate1(a_deactivate, a_depth);
			}
		}

		internal void Deactivate1(object a_deactivate, int a_depth)
		{
			CheckClosed();
			i_entryCounter++;
			try
			{
				i_justDeactivated[0] = null;
				Deactivate2(a_deactivate, a_depth);
				i_justDeactivated[0] = null;
			}
			catch (System.Exception t)
			{
				FatalException(t);
			}
			i_entryCounter--;
		}

		private void Deactivate2(object a_activate, int a_depth)
		{
			StillToDeactivate(a_activate, a_depth, true);
			while (i_stillToDeactivate != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_stillToDeactivate
					);
				i_stillToDeactivate = null;
				while (i.MoveNext())
				{
					Db4objects.Db4o.YapObject currentObject = (Db4objects.Db4o.YapObject)i.Current;
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

		public virtual void Delete(Db4objects.Db4o.Transaction trans, object a_object)
		{
			lock (i_lock)
			{
				Db4objects.Db4o.Transaction ta = Delete1(trans, a_object, true);
				ta.BeginEndSet();
			}
		}

		public Db4objects.Db4o.Transaction Delete1(Db4objects.Db4o.Transaction ta, object
			 a_object, bool userCall)
		{
			ta = CheckTransaction(ta);
			if (a_object != null)
			{
				i_entryCounter++;
				try
				{
					Delete2(ta, a_object, userCall);
				}
				catch (System.Exception t)
				{
					FatalException(t);
				}
				i_entryCounter--;
			}
			return ta;
		}

		private void Delete2(Db4objects.Db4o.Transaction ta, object a_object, bool userCall
			)
		{
			Db4objects.Db4o.YapObject yo = GetYapObject(a_object);
			if (yo != null)
			{
				Delete3(ta, yo, a_object, 0, userCall);
			}
		}

		internal void Delete3(Db4objects.Db4o.Transaction ta, Db4objects.Db4o.YapObject yo
			, object a_object, int a_cascade, bool userCall)
		{
			if (BreakDeleteForEnum(yo, userCall))
			{
				return;
			}
			if (a_object is Db4objects.Db4o.Types.ISecondClass)
			{
				Delete4(ta, yo, a_cascade, userCall);
			}
			else
			{
				ta.Delete(yo, a_cascade);
			}
		}

		internal void Delete4(Db4objects.Db4o.Transaction ta, Db4objects.Db4o.YapObject yo
			, int a_cascade, bool userCall)
		{
			if (yo != null)
			{
				if (yo.BeginProcessing())
				{
					if (BreakDeleteForEnum(yo, userCall))
					{
						return;
					}
					Db4objects.Db4o.YapClass yc = yo.GetYapClass();
					object obj = yo.GetObject();
					yo.EndProcessing();
					if (!ObjectCanDelete(yc, obj))
					{
						return;
					}
					yo.BeginProcessing();
					if (Delete5(ta, yo, a_cascade, userCall))
					{
						ObjectOnDelete(yc, obj);
						if (ConfigImpl().MessageLevel() > Db4objects.Db4o.YapConst.STATE)
						{
							Message(string.Empty + yo.GetID() + " delete " + yo.GetYapClass().GetName());
						}
					}
					yo.EndProcessing();
				}
			}
		}

		private bool ObjectCanDelete(Db4objects.Db4o.YapClass yc, object obj)
		{
			return _this.Callbacks().ObjectCanDelete(obj) && yc.DispatchEvent(_this, obj, Db4objects.Db4o.EventDispatcher
				.CAN_DELETE);
		}

		private void ObjectOnDelete(Db4objects.Db4o.YapClass yc, object obj)
		{
			_this.Callbacks().ObjectOnDelete(obj);
			yc.DispatchEvent(_this, obj, Db4objects.Db4o.EventDispatcher.DELETE);
		}

		public abstract bool Delete5(Db4objects.Db4o.Transaction ta, Db4objects.Db4o.YapObject
			 yapObject, int a_cascade, bool userCall);

		public virtual object Descend(object obj, string[] path)
		{
			lock (i_lock)
			{
				return Descend1(CheckTransaction(null), obj, path);
			}
		}

		private object Descend1(Db4objects.Db4o.Transaction trans, object obj, string[] path
			)
		{
			Db4objects.Db4o.YapObject yo = GetYapObject(obj);
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
			Db4objects.Db4o.YapClass yc = yo.GetYapClass();
			Db4objects.Db4o.YapField[] field = new Db4objects.Db4o.YapField[] { null };
			yc.ForEachYapField(new _AnonymousInnerClass558(this, fieldName, field));
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
				Db4objects.Db4o.YapReader reader = ReadReaderByID(trans, yo.GetID());
				if (reader == null)
				{
					return null;
				}
				Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf = yc.FindOffset(reader, field
					[0]);
				if (mf == null)
				{
					return null;
				}
				try
				{
					child = field[0].ReadQuery(trans, mf, reader);
				}
				catch (Db4objects.Db4o.CorruptionException)
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

		private sealed class _AnonymousInnerClass558 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass558(YapStreamBase _enclosing, string fieldName, Db4objects.Db4o.YapField[]
				 field)
			{
				this._enclosing = _enclosing;
				this.fieldName = fieldName;
				this.field = field;
			}

			public void Visit(object yf)
			{
				Db4objects.Db4o.YapField yapField = (Db4objects.Db4o.YapField)yf;
				if (yapField.CanAddToQuery(fieldName))
				{
					field[0] = yapField;
				}
			}

			private readonly YapStreamBase _enclosing;

			private readonly string fieldName;

			private readonly Db4objects.Db4o.YapField[] field;
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

		internal virtual void EmergencyClose()
		{
			StopSession();
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectContainer Ext()
		{
			return _this;
		}

		internal virtual void FailedToShutDown()
		{
			lock (Db4objects.Db4o.Inside.Global4.Lock)
			{
				if (_classCollection != null)
				{
					if (i_entryCounter == 0)
					{
						Db4objects.Db4o.Messages.LogErr(ConfigImpl(), 50, ToString(), null);
						while (!Close())
						{
						}
					}
					else
					{
						EmergencyClose();
						if (i_entryCounter > 0)
						{
							Db4objects.Db4o.Messages.LogErr(ConfigImpl(), 24, null, null);
						}
					}
				}
			}
		}

		internal virtual void FatalException(int msgID)
		{
			FatalException(null, msgID);
		}

		internal virtual void FatalException(System.Exception t)
		{
			FatalException(t, Db4objects.Db4o.Messages.FATAL_MSG_ID);
		}

		internal virtual void FatalException(System.Exception t, int msgID)
		{
			if (!i_amDuringFatalExit)
			{
				i_amDuringFatalExit = true;
				EmergencyClose();
				Db4objects.Db4o.Messages.LogErr(ConfigImpl(), (msgID == Db4objects.Db4o.Messages.
					FATAL_MSG_ID ? 18 : msgID), null, t);
			}
			throw new System.Exception(Db4objects.Db4o.Messages.Get(msgID));
		}

		~YapStreamBase()
		{
			if (DoFinalize() && (ConfigImpl() == null || ConfigImpl().AutomaticShutDown()))
			{
				FailedToShutDown();
			}
		}

		internal virtual void Gc()
		{
			i_references.PollReferenceQueue();
		}

		public virtual Db4objects.Db4o.IObjectSet Get(object template)
		{
			lock (i_lock)
			{
				return Get1(null, template);
			}
		}

		internal virtual Db4objects.Db4o.Inside.Query.ObjectSetFacade Get1(Db4objects.Db4o.Transaction
			 ta, object template)
		{
			ta = CheckTransaction(ta);
			Db4objects.Db4o.Inside.Query.IQueryResult res = null;
			i_entryCounter++;
			try
			{
				res = Get2(ta, template);
			}
			catch (System.Exception t)
			{
				Db4objects.Db4o.Inside.Exceptions4.CatchAllExceptDb4oException(t);
				FatalException(t);
			}
			i_entryCounter--;
			return new Db4objects.Db4o.Inside.Query.ObjectSetFacade(res);
		}

		private Db4objects.Db4o.Inside.Query.IQueryResult Get2(Db4objects.Db4o.Transaction
			 ta, object template)
		{
			if (template == null || template.GetType() == Db4objects.Db4o.YapConst.CLASS_OBJECT
				)
			{
				return GetAll(ta);
			}
			Db4objects.Db4o.Query.IQuery q = Query(ta);
			q.Constrain(template);
			return ExecuteQuery((Db4objects.Db4o.QQuery)q);
		}

		public abstract Db4objects.Db4o.Inside.Query.AbstractQueryResult GetAll(Db4objects.Db4o.Transaction
			 ta);

		public virtual object GetByID(long id)
		{
			lock (i_lock)
			{
				return GetByID1(null, id);
			}
		}

		public object GetByID1(Db4objects.Db4o.Transaction ta, long id)
		{
			ta = CheckTransaction(ta);
			try
			{
				return GetByID2(ta, (int)id);
			}
			catch
			{
				return null;
			}
		}

		internal object GetByID2(Db4objects.Db4o.Transaction ta, int a_id)
		{
			if (a_id > 0)
			{
				object obj = ObjectForIDFromCache(a_id);
				if (obj != null)
				{
					return obj;
				}
				try
				{
					return new Db4objects.Db4o.YapObject(a_id).Read(ta, null, null, 0, Db4objects.Db4o.YapConst
						.ADD_TO_ID_TREE, true);
				}
				catch (System.Exception t)
				{
				}
			}
			return null;
		}

		public object GetActivatedObjectFromCache(Db4objects.Db4o.Transaction ta, int id)
		{
			object obj = ObjectForIDFromCache(id);
			if (obj == null)
			{
				return null;
			}
			Activate1(ta, obj, ConfigImpl().ActivationDepth());
			return obj;
		}

		public object ReadActivatedObjectNotInCache(Db4objects.Db4o.Transaction ta, int id
			)
		{
			object obj = null;
			BeginEndActivation();
			try
			{
				obj = new Db4objects.Db4o.YapObject(id).Read(ta, null, null, ConfigImpl().ActivationDepth
					(), Db4objects.Db4o.YapConst.ADD_TO_ID_TREE, true);
			}
			catch (System.Exception t)
			{
			}
			Activate3CheckStill(ta);
			BeginEndActivation();
			return obj;
		}

		public object GetByUUID(Db4objects.Db4o.Ext.Db4oUUID uuid)
		{
			lock (i_lock)
			{
				if (uuid == null)
				{
					return null;
				}
				Db4objects.Db4o.Transaction ta = CheckTransaction(null);
				object[] arr = ta.ObjectAndYapObjectBySignature(uuid.GetLongPart(), uuid.GetSignaturePart
					());
				return arr[0];
			}
		}

		public virtual long GetID(object a_object)
		{
			lock (i_lock)
			{
				return GetID1(null, a_object);
			}
		}

		public int GetID1(Db4objects.Db4o.Transaction ta, object a_object)
		{
			CheckClosed();
			if (a_object == null)
			{
				return 0;
			}
			Db4objects.Db4o.YapObject yo = i_hcTree.Hc_find(a_object);
			if (yo != null)
			{
				return yo.GetID();
			}
			return 0;
		}

		public virtual Db4objects.Db4o.Ext.IObjectInfo GetObjectInfo(object obj)
		{
			lock (i_lock)
			{
				return GetYapObject(obj);
			}
		}

		public object[] GetObjectAndYapObjectByID(Db4objects.Db4o.Transaction ta, int a_id
			)
		{
			object[] arr = new object[2];
			if (a_id > 0)
			{
				Db4objects.Db4o.YapObject yo = GetYapObject(a_id);
				if (yo != null)
				{
					object candidate = yo.GetObject();
					if (candidate != null)
					{
						arr[0] = candidate;
						arr[1] = yo;
						return arr;
					}
					RemoveReference(yo);
				}
				try
				{
					yo = new Db4objects.Db4o.YapObject(a_id);
					arr[0] = yo.Read(ta, null, null, 0, Db4objects.Db4o.YapConst.ADD_TO_ID_TREE, true
						);
					if (arr[0] == null)
					{
						return arr;
					}
					if (arr[0] != yo.GetObject())
					{
						return GetObjectAndYapObjectByID(ta, a_id);
					}
					arr[1] = yo;
				}
				catch (System.Exception t)
				{
				}
			}
			return arr;
		}

		public Db4objects.Db4o.YapWriter GetWriter(Db4objects.Db4o.Transaction a_trans, int
			 a_address, int a_length)
		{
			if (Db4objects.Db4o.Debug.ExceedsMaximumBlockSize(a_length))
			{
				return null;
			}
			return new Db4objects.Db4o.YapWriter(a_trans, a_address, a_length);
		}

		public Db4objects.Db4o.Transaction GetSystemTransaction()
		{
			return i_systemTrans;
		}

		public Db4objects.Db4o.Transaction GetTransaction()
		{
			return i_trans;
		}

		public Db4objects.Db4o.YapClass GetYapClass(Db4objects.Db4o.Reflect.IReflectClass
			 a_class, bool a_create)
		{
			if (a_class == null)
			{
				return null;
			}
			if ((!ShowInternalClasses()) && i_handlers.ICLASS_INTERNAL.IsAssignableFrom(a_class
				))
			{
				return null;
			}
			Db4objects.Db4o.YapClass yc = i_handlers.GetYapClassStatic(a_class);
			if (yc != null)
			{
				return yc;
			}
			return _classCollection.GetYapClass(a_class, a_create);
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
		internal Db4objects.Db4o.YapClass GetActiveYapClass(Db4objects.Db4o.Reflect.IReflectClass
			 a_class)
		{
			if (a_class == null)
			{
				return null;
			}
			if ((!ShowInternalClasses()) && i_handlers.ICLASS_INTERNAL.IsAssignableFrom(a_class
				))
			{
				return null;
			}
			Db4objects.Db4o.YapClass yc = i_handlers.GetYapClassStatic(a_class);
			if (yc != null)
			{
				return yc;
			}
			return _classCollection.GetActiveYapClass(a_class);
		}

		public virtual Db4objects.Db4o.YapClass GetYapClass(int a_id)
		{
			if (a_id == 0)
			{
				return null;
			}
			Db4objects.Db4o.YapClass yc = i_handlers.GetYapClassStatic(a_id);
			if (yc != null)
			{
				return yc;
			}
			return _classCollection.GetYapClass(a_id);
		}

		public virtual object ObjectForIDFromCache(int id)
		{
			Db4objects.Db4o.YapObject yo = GetYapObject(id);
			if (yo == null)
			{
				return null;
			}
			object candidate = yo.GetObject();
			if (candidate == null)
			{
				RemoveReference(yo);
			}
			return candidate;
		}

		public Db4objects.Db4o.YapObject GetYapObject(int id)
		{
			if (id <= 0)
			{
				return null;
			}
			return i_idTree.Id_find(id);
		}

		public Db4objects.Db4o.YapObject GetYapObject(object a_object)
		{
			return i_hcTree.Hc_find(a_object);
		}

		public virtual Db4objects.Db4o.YapHandlers Handlers()
		{
			return i_handlers;
		}

		public virtual bool NeedsLockFileThread()
		{
			if (!Db4objects.Db4o.Platform4.HasLockFileThread())
			{
				return false;
			}
			if (Db4objects.Db4o.Platform4.HasNio())
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

		internal void HcTreeAdd(Db4objects.Db4o.YapObject a_yo)
		{
			i_hcTree = i_hcTree.Hc_add(a_yo);
		}

		internal void HcTreeRemove(Db4objects.Db4o.YapObject a_yo)
		{
			i_hcTree = i_hcTree.Hc_remove(a_yo);
		}

		internal void IdTreeAdd(Db4objects.Db4o.YapObject a_yo)
		{
			i_idTree = i_idTree.Id_add(a_yo);
		}

		internal void IdTreeRemove(int a_id)
		{
			i_idTree = i_idTree.Id_remove(a_id);
		}

		protected virtual void Initialize0()
		{
			Initialize0b();
			i_stillToSet = null;
			i_justActivated = new Db4objects.Db4o.Foundation.Tree[1];
		}

		internal virtual void Initialize0b()
		{
			i_justDeactivated = new Db4objects.Db4o.Foundation.Tree[1];
		}

		protected virtual void Initialize1(Db4objects.Db4o.Config.IConfiguration config)
		{
			i_config = InitializeConfig(config);
			i_handlers = new Db4objects.Db4o.YapHandlers(_this, ConfigImpl().Encoding(), ConfigImpl
				().Reflector());
			if (i_references != null)
			{
				Gc();
				i_references.StopTimer();
			}
			i_references = new Db4objects.Db4o.YapReferences(_this);
			if (HasShutDownHook())
			{
				Db4objects.Db4o.Platform4.AddShutDownHook(this, i_lock);
			}
			i_handlers.InitEncryption(ConfigImpl());
			Initialize2();
			i_stillToSet = null;
		}

		private Db4objects.Db4o.Config4Impl InitializeConfig(Db4objects.Db4o.Config.IConfiguration
			 config)
		{
			Db4objects.Db4o.Config4Impl impl = ((Db4objects.Db4o.Config4Impl)config);
			impl.Stream(_this);
			return impl;
		}

		/// <summary>before file is open</summary>
		internal virtual void Initialize2()
		{
			i_idTree = new Db4objects.Db4o.YapObject(0);
			i_idTree.SetObject(new object());
			i_hcTree = i_idTree;
			Initialize2NObjectCarrier();
		}

		/// <summary>overridden in YapObjectCarrier</summary>
		internal virtual void Initialize2NObjectCarrier()
		{
			_classCollection = new Db4objects.Db4o.YapClassCollection(i_systemTrans);
			i_references.StartTimer();
		}

		protected virtual void Initialize3()
		{
			i_showInternalClasses = 100000;
			Initialize4NObjectCarrier();
			i_showInternalClasses = 0;
		}

		internal virtual void Initialize4NObjectCarrier()
		{
			InitializeEssentialClasses();
			Rename(ConfigImpl());
			_classCollection.InitOnUp(i_systemTrans);
			if (ConfigImpl().DetectSchemaChanges())
			{
				i_systemTrans.Commit();
			}
		}

		internal virtual void InitializeEssentialClasses()
		{
			for (int i = 0; i < Db4objects.Db4o.YapConst.ESSENTIAL_CLASSES.Length; i++)
			{
				GetYapClass(Reflector().ForClass(Db4objects.Db4o.YapConst.ESSENTIAL_CLASSES[i]), 
					true);
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
				Db4objects.Db4o.YapObject yo = GetYapObject(obj);
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
				return ObjectForIDFromCache((int)a_id) != null;
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

		public virtual bool IsClosed()
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
			Db4objects.Db4o.Transaction ta = CheckTransaction(null);
			if (obj == null)
			{
				return false;
			}
			Db4objects.Db4o.YapObject yo = GetYapObject(obj);
			if (yo == null)
			{
				return false;
			}
			return !ta.IsDeleted(yo.GetID());
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass[] KnownClasses()
		{
			lock (i_lock)
			{
				CheckClosed();
				return Reflector().KnownClasses();
			}
		}

		public virtual Db4objects.Db4o.ITypeHandler4 HandlerByID(int id)
		{
			if (id < 1)
			{
				return null;
			}
			if (i_handlers.IsSystemHandler(id))
			{
				return i_handlers.GetHandler(id);
			}
			return GetYapClass(id);
		}

		public virtual object Lock()
		{
			return i_lock;
		}

		public void LogMsg(int code, string msg)
		{
			Db4objects.Db4o.Messages.LogMsg(ConfigImpl(), code, msg);
		}

		public virtual bool MaintainsIndices()
		{
			return true;
		}

		protected virtual Db4objects.Db4o.YapWriter Marshall(Db4objects.Db4o.Transaction 
			ta, object obj)
		{
			int[] id = { 0 };
			byte[] bytes = Marshall(obj, id);
			Db4objects.Db4o.YapWriter yapBytes = new Db4objects.Db4o.YapWriter(ta, bytes.Length
				);
			yapBytes.Append(bytes);
			yapBytes.UseSlot(id[0], 0, bytes.Length);
			return yapBytes;
		}

		internal virtual byte[] Marshall(object obj, int[] id)
		{
			Db4objects.Db4o.Ext.MemoryFile memoryFile = new Db4objects.Db4o.Ext.MemoryFile();
			memoryFile.SetInitialSize(223);
			memoryFile.SetIncrementSizeBy(300);
			GetYapClass(Reflector().ForObject(obj), true);
			Db4objects.Db4o.YapObjectCarrier carrier = new Db4objects.Db4o.YapObjectCarrier(Config
				(), _this, memoryFile);
			carrier.i_showInternalClasses = i_showInternalClasses;
			carrier.Set(obj);
			id[0] = (int)carrier.GetID(obj);
			carrier.Close();
			return memoryFile.GetBytes();
		}

		internal virtual void Message(string msg)
		{
			new Db4objects.Db4o.Message(_this, msg);
		}

		public virtual void MigrateFrom(Db4objects.Db4o.IObjectContainer objectContainer)
		{
			if (objectContainer == null)
			{
				if (_replicationCallState == Db4objects.Db4o.YapConst.NONE)
				{
					return;
				}
				_replicationCallState = Db4objects.Db4o.YapConst.NONE;
				if (i_handlers.i_migration != null)
				{
					i_handlers.i_migration.Terminate();
				}
				i_handlers.i_migration = null;
			}
			else
			{
				Db4objects.Db4o.YapStream peer = (Db4objects.Db4o.YapStream)objectContainer;
				_replicationCallState = Db4objects.Db4o.YapConst.OLD;
				peer._replicationCallState = Db4objects.Db4o.YapConst.OLD;
				i_handlers.i_migration = new Db4objects.Db4o.Inside.Replication.MigrationConnection
					(_this, (Db4objects.Db4o.YapStream)objectContainer);
				peer.i_handlers.i_migration = i_handlers.i_migration;
			}
		}

		public void NeedsUpdate(Db4objects.Db4o.YapClass a_yapClass)
		{
			i_needsUpdate = new Db4objects.Db4o.Foundation.List4(i_needsUpdate, a_yapClass);
		}

		public virtual long GenerateTimeStampId()
		{
			return _timeStampIdGenerator.Next();
		}

		public abstract int NewUserObject();

		public virtual object PeekPersisted(object a_object, int a_depth, bool a_committed
			)
		{
			lock (i_lock)
			{
				CheckClosed();
				i_entryCounter++;
				i_justPeeked = null;
				Db4objects.Db4o.Transaction ta = a_committed ? i_systemTrans : CheckTransaction(null
					);
				object cloned = null;
				Db4objects.Db4o.YapObject yo = GetYapObject(a_object);
				if (yo != null)
				{
					cloned = PeekPersisted1(ta, yo.GetID(), a_depth);
				}
				i_justPeeked = null;
				i_entryCounter--;
				return cloned;
			}
		}

		internal virtual object PeekPersisted1(Db4objects.Db4o.Transaction a_ta, int a_id
			, int a_depth)
		{
			if (a_depth < 0)
			{
				return null;
			}
			Db4objects.Db4o.TreeInt ti = new Db4objects.Db4o.TreeInt(a_id);
			Db4objects.Db4o.TreeIntObject tio = (Db4objects.Db4o.TreeIntObject)Db4objects.Db4o.Foundation.Tree
				.Find(i_justPeeked, ti);
			if (tio == null)
			{
				return new Db4objects.Db4o.YapObject(a_id).Read(a_ta, null, null, a_depth, Db4objects.Db4o.YapConst
					.TRANSIENT, false);
			}
			return tio._object;
		}

		internal virtual void Peeked(int a_id, object a_object)
		{
			i_justPeeked = Db4objects.Db4o.Foundation.Tree.Add(i_justPeeked, new Db4objects.Db4o.TreeIntObject
				(a_id, a_object));
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
			Sharpen.Runtime.Gc();
			Sharpen.Runtime.RunFinalization();
			Sharpen.Runtime.Gc();
			Gc();
			_classCollection.Purge();
		}

		internal void Purge1(object obj)
		{
			if (obj != null)
			{
				if (i_hcTree != null)
				{
					Db4objects.Db4o.YapObject yo = null;
					if (obj is Db4objects.Db4o.YapObject)
					{
						yo = (Db4objects.Db4o.YapObject)obj;
					}
					else
					{
						yo = i_hcTree.Hc_find(obj);
					}
					if (yo != null)
					{
						RemoveReference(yo);
					}
				}
			}
		}

		public Db4objects.Db4o.Inside.Query.NativeQueryHandler GetNativeQueryHandler()
		{
			if (null == _nativeQueryHandler)
			{
				_nativeQueryHandler = new Db4objects.Db4o.Inside.Query.NativeQueryHandler(_this);
			}
			return _nativeQueryHandler;
		}

		public Db4objects.Db4o.IObjectSet Query(Db4objects.Db4o.Query.Predicate predicate
			)
		{
			return Query(predicate, (Db4objects.Db4o.Query.IQueryComparator)null);
		}

		public Db4objects.Db4o.IObjectSet Query(Db4objects.Db4o.Query.Predicate predicate
			, Db4objects.Db4o.Query.IQueryComparator comparator)
		{
			lock (i_lock)
			{
				return GetNativeQueryHandler().Execute(predicate, comparator);
			}
		}

		public virtual Db4objects.Db4o.Query.IQuery Query()
		{
			lock (i_lock)
			{
				return Query((Db4objects.Db4o.Transaction)null);
			}
		}

		public Db4objects.Db4o.IObjectSet Query(System.Type clazz)
		{
			return Get(clazz);
		}

		public Db4objects.Db4o.Query.IQuery Query(Db4objects.Db4o.Transaction ta)
		{
			i_entryCounter++;
			Db4objects.Db4o.Query.IQuery q = new Db4objects.Db4o.QQuery(CheckTransaction(ta), 
				null, null);
			i_entryCounter--;
			return q;
		}

		public abstract void RaiseVersion(long a_minimumVersion);

		public abstract void ReadBytes(byte[] a_bytes, int a_address, int a_length);

		public abstract void ReadBytes(byte[] bytes, int address, int addressOffset, int 
			length);

		public Db4objects.Db4o.YapReader ReadReaderByAddress(int a_address, int a_length)
		{
			if (a_address > 0)
			{
				Db4objects.Db4o.YapReader reader = new Db4objects.Db4o.YapReader(a_length);
				ReadBytes(reader._buffer, a_address, a_length);
				i_handlers.Decrypt(reader);
				return reader;
			}
			return null;
		}

		public Db4objects.Db4o.YapWriter ReadWriterByAddress(Db4objects.Db4o.Transaction 
			a_trans, int a_address, int a_length)
		{
			if (a_address > 0)
			{
				Db4objects.Db4o.YapWriter reader = GetWriter(a_trans, a_address, a_length);
				reader.ReadEncrypt(_this, a_address);
				return reader;
			}
			return null;
		}

		public abstract Db4objects.Db4o.YapWriter ReadWriterByID(Db4objects.Db4o.Transaction
			 a_ta, int a_id);

		public abstract Db4objects.Db4o.YapReader ReadReaderByID(Db4objects.Db4o.Transaction
			 a_ta, int a_id);

		private void Reboot()
		{
			Commit();
			int ccID = _classCollection.GetID();
			i_references.StopTimer();
			Initialize2();
			_classCollection.SetID(ccID);
			_classCollection.Read(i_systemTrans);
		}

		public virtual Db4objects.Db4o.Reflect.Generic.GenericReflector Reflector()
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

		internal virtual void RememberJustSet(int id)
		{
			if (i_justSet == null)
			{
				i_justSet = new Db4objects.Db4o.TreeInt(id);
			}
			else
			{
				i_justSet = i_justSet.Add(new Db4objects.Db4o.TreeInt(id));
			}
		}

		public abstract void ReleaseSemaphores(Db4objects.Db4o.Transaction ta);

		internal virtual void Rename(Db4objects.Db4o.Config4Impl config)
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

		protected virtual bool Rename1(Db4objects.Db4o.Config4Impl config)
		{
			bool renamedOne = false;
			try
			{
				System.Collections.IEnumerator i = config.Rename().GetEnumerator();
				while (i.MoveNext())
				{
					Db4objects.Db4o.Rename ren = (Db4objects.Db4o.Rename)i.Current;
					if (Get(ren).Size() == 0)
					{
						bool renamed = false;
						bool isField = ren.rClass.Length > 0;
						Db4objects.Db4o.YapClass yapClass = _classCollection.GetYapClass(isField ? ren.rClass
							 : ren.rFrom);
						if (yapClass != null)
						{
							if (isField)
							{
								renamed = yapClass.RenameField(ren.rFrom, ren.rTo);
							}
							else
							{
								Db4objects.Db4o.YapClass existing = _classCollection.GetYapClass(ren.rTo);
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
							Db4objects.Db4o.IObjectSet backren = Get(new Db4objects.Db4o.Rename(ren.rClass, null
								, ren.rFrom));
							while (backren.HasNext())
							{
								Delete(backren.Next());
							}
							Set(ren);
						}
					}
				}
			}
			catch (System.Exception t)
			{
				Db4objects.Db4o.Messages.LogErr(ConfigImpl(), 10, null, t);
			}
			return renamedOne;
		}

		public virtual Db4objects.Db4o.Replication.IReplicationProcess ReplicationBegin(Db4objects.Db4o.IObjectContainer
			 peerB, Db4objects.Db4o.Replication.IReplicationConflictHandler conflictHandler)
		{
			return new Db4objects.Db4o.ReplicationImpl(_this, peerB, conflictHandler);
		}

		internal int OldReplicationHandles(object obj)
		{
			if (_replicationCallState != Db4objects.Db4o.YapConst.OLD)
			{
				return 0;
			}
			if (i_handlers.i_replication == null)
			{
				return 0;
			}
			if (obj is Db4objects.Db4o.IInternal4)
			{
				return 0;
			}
			Db4objects.Db4o.YapObject reference = GetYapObject(obj);
			if (reference != null)
			{
				int id = reference.GetID();
				if (id > 0 && (Db4objects.Db4o.TreeInt.Find(i_justSet, id) != null))
				{
					return id;
				}
			}
			return i_handlers.i_replication.TryToHandle(_this, obj);
		}

		internal virtual void Reserve(int byteCount)
		{
		}

		public virtual void Rollback()
		{
			lock (i_lock)
			{
				Rollback1();
			}
		}

		public abstract void Rollback1();

		public virtual void Send(object obj)
		{
		}

		public virtual void Set(object a_object)
		{
			Set(a_object, Db4objects.Db4o.YapConst.UNSPECIFIED);
		}

		public void Set(Db4objects.Db4o.Transaction trans, object a_object)
		{
			Set(trans, a_object, Db4objects.Db4o.YapConst.UNSPECIFIED);
		}

		public void Set(object a_object, int a_depth)
		{
			Set(i_trans, a_object, a_depth);
		}

		public virtual void Set(Db4objects.Db4o.Transaction trans, object a_object, int a_depth
			)
		{
			lock (i_lock)
			{
				CheckClosed();
				BeginEndSet(trans);
				SetInternal(trans, a_object, a_depth, true);
				BeginEndSet(trans);
			}
		}

		public int SetInternal(Db4objects.Db4o.Transaction ta, object a_object, bool a_checkJustSet
			)
		{
			return SetInternal(ta, a_object, Db4objects.Db4o.YapConst.UNSPECIFIED, a_checkJustSet
				);
		}

		public int SetInternal(Db4objects.Db4o.Transaction ta, object a_object, int a_depth
			, bool a_checkJustSet)
		{
			int id = OldReplicationHandles(a_object);
			if (id != 0)
			{
				if (id < 0)
				{
					return 0;
				}
				return id;
			}
			return SetAfterReplication(ta, a_object, a_depth, a_checkJustSet);
		}

		internal int SetAfterReplication(Db4objects.Db4o.Transaction ta, object obj, int 
			depth, bool checkJust)
		{
			if (obj is Db4objects.Db4o.Types.IDb4oType)
			{
				Db4objects.Db4o.Types.IDb4oType db4oType = Db4oTypeStored(ta, obj);
				if (db4oType != null)
				{
					return GetID1(ta, db4oType);
				}
			}
			int id;
			i_entryCounter++;
			try
			{
				id = Set2(ta, obj, depth, checkJust);
			}
			catch (Db4objects.Db4o.Ext.ObjectNotStorableException e)
			{
				i_entryCounter--;
				throw;
			}
			catch (Db4objects.Db4o.Ext.Db4oException exc)
			{
				id = 0;
				throw;
			}
			catch (System.Exception t)
			{
				id = 0;
				FatalException(t);
			}
			i_entryCounter--;
			return id;
		}

		public void SetByNewReplication(Db4objects.Db4o.Inside.Replication.IDb4oReplicationReferenceProvider
			 referenceProvider, object obj)
		{
			lock (i_lock)
			{
				_replicationCallState = Db4objects.Db4o.YapConst.NEW;
				i_handlers._replicationReferenceProvider = referenceProvider;
				Set2(CheckTransaction(null), obj, 1, false);
				_replicationCallState = Db4objects.Db4o.YapConst.NONE;
				i_handlers._replicationReferenceProvider = null;
			}
		}

		private int Set2(Db4objects.Db4o.Transaction ta, object obj, int depth, bool checkJust
			)
		{
			int id = Set3(ta, obj, depth, checkJust);
			if (StackIsSmall())
			{
				CheckStillToSet();
			}
			return id;
		}

		public virtual void CheckStillToSet()
		{
			Db4objects.Db4o.Foundation.List4 postponedStillToSet = null;
			while (i_stillToSet != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_stillToSet
					);
				i_stillToSet = null;
				while (i.MoveNext())
				{
					int updateDepth = (int)i.Current;
					i.MoveNext();
					Db4objects.Db4o.YapObject yo = (Db4objects.Db4o.YapObject)i.Current;
					i.MoveNext();
					Db4objects.Db4o.Transaction trans = (Db4objects.Db4o.Transaction)i.Current;
					if (!yo.ContinueSet(trans, updateDepth))
					{
						postponedStillToSet = new Db4objects.Db4o.Foundation.List4(postponedStillToSet, trans
							);
						postponedStillToSet = new Db4objects.Db4o.Foundation.List4(postponedStillToSet, yo
							);
						postponedStillToSet = new Db4objects.Db4o.Foundation.List4(postponedStillToSet, updateDepth
							);
					}
				}
			}
			i_stillToSet = postponedStillToSet;
		}

		private void NotStorable(Db4objects.Db4o.Reflect.IReflectClass claxx, object obj)
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
				throw new Db4objects.Db4o.Ext.ObjectNotStorableException(claxx);
			}
			throw new Db4objects.Db4o.Ext.ObjectNotStorableException(obj.ToString());
		}

		public int Set3(Db4objects.Db4o.Transaction a_trans, object a_object, int a_updateDepth
			, bool a_checkJustSet)
		{
			if (a_object == null || (a_object is Db4objects.Db4o.Types.ITransientClass))
			{
				return 0;
			}
			if (a_object is Db4objects.Db4o.IDb4oTypeImpl)
			{
				((Db4objects.Db4o.IDb4oTypeImpl)a_object).StoredTo(a_trans);
			}
			Db4objects.Db4o.YapClass yc = null;
			Db4objects.Db4o.YapObject yapObject = i_hcTree.Hc_find(a_object);
			if (yapObject == null)
			{
				Db4objects.Db4o.Reflect.IReflectClass claxx = Reflector().ForObject(a_object);
				if (claxx == null)
				{
					NotStorable(claxx, a_object);
					return 0;
				}
				yc = GetActiveYapClass(claxx);
				if (yc == null)
				{
					yc = GetYapClass(claxx, true);
					if (yc == null)
					{
						NotStorable(claxx, a_object);
						return 0;
					}
					yapObject = i_hcTree.Hc_find(a_object);
				}
			}
			else
			{
				yc = yapObject.GetYapClass();
			}
			if (IsPlainObjectOrPrimitive(yc))
			{
				NotStorable(yc.ClassReflector(), a_object);
				return 0;
			}
			bool dontDelete = true;
			if (yapObject == null)
			{
				if (!ObjectCanNew(yc, a_object))
				{
					return 0;
				}
				yapObject = new Db4objects.Db4o.YapObject(0);
				if (yapObject.Store(a_trans, yc, a_object))
				{
					IdTreeAdd(yapObject);
					HcTreeAdd(yapObject);
					if (a_object is Db4objects.Db4o.IDb4oTypeImpl)
					{
						((Db4objects.Db4o.IDb4oTypeImpl)a_object).SetTrans(a_trans);
					}
					if (ConfigImpl().MessageLevel() > Db4objects.Db4o.YapConst.STATE)
					{
						Message(string.Empty + yapObject.GetID() + " new " + yapObject.GetYapClass().GetName
							());
					}
					if (a_checkJustSet && CanUpdate())
					{
						if (!yapObject.GetYapClass().IsPrimitive())
						{
							RememberJustSet(yapObject.GetID());
							a_checkJustSet = false;
						}
					}
					StillToSet(a_trans, yapObject, a_updateDepth);
				}
			}
			else
			{
				if (CanUpdate())
				{
					int oid = yapObject.GetID();
					if (a_checkJustSet)
					{
						if (oid > 0 && (Db4objects.Db4o.TreeInt.Find(i_justSet, oid) != null))
						{
							return oid;
						}
					}
					bool doUpdate = (a_updateDepth == Db4objects.Db4o.YapConst.UNSPECIFIED) || (a_updateDepth
						 > 0);
					if (doUpdate)
					{
						dontDelete = false;
						a_trans.DontDelete(yapObject.GetYapClass().GetID(), oid);
						if (a_checkJustSet)
						{
							a_checkJustSet = false;
							RememberJustSet(oid);
						}
						yapObject.WriteUpdate(a_trans, a_updateDepth);
					}
				}
			}
			CheckNeededUpdates();
			int id = yapObject.GetID();
			if (a_checkJustSet && CanUpdate())
			{
				if (!yapObject.GetYapClass().IsPrimitive())
				{
					RememberJustSet(id);
				}
			}
			if (dontDelete)
			{
				a_trans.DontDelete(yapObject.GetYapClass().GetID(), id);
			}
			return id;
		}

		private bool IsPlainObjectOrPrimitive(Db4objects.Db4o.YapClass yc)
		{
			return yc.GetID() == Db4objects.Db4o.YapHandlers.ANY_ID || yc.IsPrimitive();
		}

		private bool ObjectCanNew(Db4objects.Db4o.YapClass yc, object a_object)
		{
			return Callbacks().ObjectCanNew(a_object) && yc.DispatchEvent(_this, a_object, Db4objects.Db4o.EventDispatcher
				.CAN_NEW);
		}

		public abstract void SetDirtyInSystemTransaction(Db4objects.Db4o.YapMeta a_object
			);

		public abstract bool SetSemaphore(string name, int timeout);

		internal virtual void SetStringIo(Db4objects.Db4o.YapStringIO a_io)
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
			return i_entryCounter < Db4objects.Db4o.YapConst.MAX_STACK_DEPTH;
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
		internal Db4objects.Db4o.Foundation.List4 StillTo1(Db4objects.Db4o.Foundation.List4
			 a_still, Db4objects.Db4o.Foundation.Tree[] a_just, object a_object, int a_depth
			, bool a_forceUnknownDeactivate)
		{
			if (a_object == null || a_depth <= 0)
			{
				return a_still;
			}
			Db4objects.Db4o.YapObject yapObject = i_hcTree.Hc_find(a_object);
			if (yapObject != null)
			{
				int id = yapObject.GetID();
				if (a_just[0] != null)
				{
					if (((Db4objects.Db4o.TreeInt)a_just[0]).Find(id) != null)
					{
						return a_still;
					}
					a_just[0] = a_just[0].Add(new Db4objects.Db4o.TreeInt(id));
				}
				else
				{
					a_just[0] = new Db4objects.Db4o.TreeInt(id);
				}
				return new Db4objects.Db4o.Foundation.List4(new Db4objects.Db4o.Foundation.List4(
					a_still, a_depth), yapObject);
			}
			Db4objects.Db4o.Reflect.IReflectClass clazz = Reflector().ForObject(a_object);
			if (clazz.IsArray())
			{
				if (!clazz.GetComponentType().IsPrimitive())
				{
					object[] arr = Db4objects.Db4o.YapArray.ToArray(_this, a_object);
					for (int i = 0; i < arr.Length; i++)
					{
						a_still = StillTo1(a_still, a_just, arr[i], a_depth, a_forceUnknownDeactivate);
					}
				}
			}
			else
			{
				if (a_object is Db4objects.Db4o.Config.Entry)
				{
					a_still = StillTo1(a_still, a_just, ((Db4objects.Db4o.Config.Entry)a_object).key, 
						a_depth, false);
					a_still = StillTo1(a_still, a_just, ((Db4objects.Db4o.Config.Entry)a_object).value
						, a_depth, false);
				}
				else
				{
					if (a_forceUnknownDeactivate)
					{
						Db4objects.Db4o.YapClass yc = GetYapClass(Reflector().ForObject(a_object), false);
						if (yc != null)
						{
							yc.Deactivate(i_trans, a_object, a_depth);
						}
					}
				}
			}
			return a_still;
		}

		internal virtual void StillToActivate(object a_object, int a_depth)
		{
			i_stillToActivate = StillTo1(i_stillToActivate, i_justActivated, a_object, a_depth
				, false);
		}

		internal virtual void StillToDeactivate(object a_object, int a_depth, bool a_forceUnknownDeactivate
			)
		{
			i_stillToDeactivate = StillTo1(i_stillToDeactivate, i_justDeactivated, a_object, 
				a_depth, a_forceUnknownDeactivate);
		}

		internal virtual void StillToSet(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.YapObject
			 a_yapObject, int a_updateDepth)
		{
			if (StackIsSmall())
			{
				if (a_yapObject.ContinueSet(a_trans, a_updateDepth))
				{
					return;
				}
			}
			i_stillToSet = new Db4objects.Db4o.Foundation.List4(i_stillToSet, a_trans);
			i_stillToSet = new Db4objects.Db4o.Foundation.List4(i_stillToSet, a_yapObject);
			i_stillToSet = new Db4objects.Db4o.Foundation.List4(i_stillToSet, a_updateDepth);
		}

		protected virtual void StopSession()
		{
			if (HasShutDownHook())
			{
				Db4objects.Db4o.Platform4.RemoveShutDownHook(this, i_lock);
			}
			_classCollection = null;
			i_references.StopTimer();
		}

		public virtual Db4objects.Db4o.Ext.IStoredClass StoredClass(object clazz)
		{
			lock (i_lock)
			{
				CheckClosed();
				return StoredClass1(clazz);
			}
		}

		internal virtual Db4objects.Db4o.YapClass StoredClass1(object clazz)
		{
			try
			{
				Db4objects.Db4o.Reflect.IReflectClass claxx = ConfigImpl().ReflectorFor(clazz);
				if (claxx != null)
				{
					return GetYapClass(claxx, false);
				}
			}
			catch
			{
			}
			return null;
		}

		public virtual Db4objects.Db4o.Ext.IStoredClass[] StoredClasses()
		{
			lock (i_lock)
			{
				CheckClosed();
				return _classCollection.StoredClasses();
			}
		}

		public virtual Db4objects.Db4o.YapStringIO StringIO()
		{
			return i_handlers.i_stringHandler.i_stringIo;
		}

		public abstract Db4objects.Db4o.Ext.ISystemInfo SystemInfo();

		public virtual object Unmarshall(Db4objects.Db4o.YapWriter yapBytes)
		{
			return Unmarshall(yapBytes._buffer, yapBytes.GetID());
		}

		internal virtual object Unmarshall(byte[] bytes, int id)
		{
			Db4objects.Db4o.Ext.MemoryFile memoryFile = new Db4objects.Db4o.Ext.MemoryFile(bytes
				);
			Db4objects.Db4o.YapObjectCarrier carrier = new Db4objects.Db4o.YapObjectCarrier(Configure
				(), _this, memoryFile);
			object obj = carrier.GetByID(id);
			carrier.Activate(obj, int.MaxValue);
			carrier.Close();
			return obj;
		}

		public virtual long Version()
		{
			lock (i_lock)
			{
				return CurrentVersion();
			}
		}

		public abstract void Write(bool shuttingDown);

		public abstract void WriteDirty();

		public abstract void WriteEmbedded(Db4objects.Db4o.YapWriter a_parent, Db4objects.Db4o.YapWriter
			 a_child);

		public abstract void WriteNew(Db4objects.Db4o.YapClass a_yapClass, Db4objects.Db4o.YapWriter
			 aWriter);

		public abstract void WriteTransactionPointer(int a_address);

		public abstract void WriteUpdate(Db4objects.Db4o.YapClass a_yapClass, Db4objects.Db4o.YapWriter
			 a_bytes);

		public void RemoveReference(Db4objects.Db4o.YapObject yo)
		{
			HcTreeRemove(yo);
			IdTreeRemove(yo.GetID());
			if (i_justActivated != null && i_justActivated[0] != null)
			{
				i_justActivated[0] = i_justActivated[0].RemoveLike(new Db4objects.Db4o.TreeInt(yo
					.GetID()));
			}
			yo.SetID(-1);
			Db4objects.Db4o.Platform4.KillYapRef(yo.i_object);
		}

		private static Db4objects.Db4o.YapStream Cast(Db4objects.Db4o.YapStreamBase obj)
		{
			return (Db4objects.Db4o.YapStream)obj;
		}

		public virtual Db4objects.Db4o.Inside.Callbacks.ICallbacks Callbacks()
		{
			return _callbacks;
		}

		public virtual void Callbacks(Db4objects.Db4o.Inside.Callbacks.ICallbacks cb)
		{
			if (cb == null)
			{
				throw new System.ArgumentException();
			}
			_callbacks = cb;
		}

		public virtual Db4objects.Db4o.Config4Impl ConfigImpl()
		{
			return i_config;
		}

		public virtual Db4objects.Db4o.YapFieldUUID GetFieldUUID()
		{
			return i_handlers.i_indexes.i_fieldUUID;
		}

		public virtual Db4objects.Db4o.YapClassCollection ClassCollection()
		{
			return _classCollection;
		}

		public abstract long[] GetIDsForClass(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapClass
			 clazz);

		public abstract Db4objects.Db4o.Inside.Query.IQueryResult ClassOnlyQuery(Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.YapClass clazz);

		public abstract Db4objects.Db4o.Inside.Query.IQueryResult ExecuteQuery(Db4objects.Db4o.QQuery
			 query);
	}
}
