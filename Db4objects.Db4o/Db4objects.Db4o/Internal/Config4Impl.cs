namespace Db4objects.Db4o.Internal
{
	/// <summary>Configuration template for creating new db4o files</summary>
	/// <exclude></exclude>
	public sealed class Config4Impl : Db4objects.Db4o.Config.IConfiguration, Db4objects.Db4o.Foundation.IDeepClone
		, Db4objects.Db4o.Messaging.IMessageSender, Db4objects.Db4o.Config.IFreespaceConfiguration
		, Db4objects.Db4o.Config.IQueryConfiguration, Db4objects.Db4o.Config.IClientServerConfiguration
	{
		private Db4objects.Db4o.Foundation.KeySpecHashtable4 _config = new Db4objects.Db4o.Foundation.KeySpecHashtable4
			(50);

		private static readonly Db4objects.Db4o.Foundation.KeySpec ACTIVATION_DEPTH = new 
			Db4objects.Db4o.Foundation.KeySpec(5);

		private static readonly Db4objects.Db4o.Foundation.KeySpec ALLOW_VERSION_UPDATES = 
			new Db4objects.Db4o.Foundation.KeySpec(false);

		private static readonly Db4objects.Db4o.Foundation.KeySpec AUTOMATIC_SHUTDOWN = new 
			Db4objects.Db4o.Foundation.KeySpec(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec BLOCKSIZE = new Db4objects.Db4o.Foundation.KeySpec
			((byte)1);

		private static readonly Db4objects.Db4o.Foundation.KeySpec BLOBPATH = new Db4objects.Db4o.Foundation.KeySpec
			(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec BTREE_NODE_SIZE = new 
			Db4objects.Db4o.Foundation.KeySpec(119);

		private static readonly Db4objects.Db4o.Foundation.KeySpec BTREE_CACHE_HEIGHT = new 
			Db4objects.Db4o.Foundation.KeySpec(1);

		private static readonly Db4objects.Db4o.Foundation.KeySpec CALLBACKS = new Db4objects.Db4o.Foundation.KeySpec
			(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec CALL_CONSTRUCTORS = new 
			Db4objects.Db4o.Foundation.KeySpec(Db4objects.Db4o.Foundation.TernaryBool.UNSPECIFIED
			);

		private static readonly Db4objects.Db4o.Foundation.KeySpec CLASS_ACTIVATION_DEPTH_CONFIGURABLE
			 = new Db4objects.Db4o.Foundation.KeySpec(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec CLASSLOADER = new Db4objects.Db4o.Foundation.KeySpec
			(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec DETECT_SCHEMA_CHANGES = 
			new Db4objects.Db4o.Foundation.KeySpec(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec DIAGNOSTIC = new Db4objects.Db4o.Foundation.KeySpec
			(new Db4objects.Db4o.Internal.Diagnostic.DiagnosticProcessor());

		private static readonly Db4objects.Db4o.Foundation.KeySpec DISABLE_COMMIT_RECOVERY
			 = new Db4objects.Db4o.Foundation.KeySpec(false);

		private static readonly Db4objects.Db4o.Foundation.KeySpec DISCARD_FREESPACE = new 
			Db4objects.Db4o.Foundation.KeySpec(0);

		private static readonly Db4objects.Db4o.Foundation.KeySpec ENCODING = new Db4objects.Db4o.Foundation.KeySpec
			(Db4objects.Db4o.Internal.Const4.UNICODE);

		private static readonly Db4objects.Db4o.Foundation.KeySpec ENCRYPT = new Db4objects.Db4o.Foundation.KeySpec
			(false);

		private static readonly Db4objects.Db4o.Foundation.KeySpec EXCEPTIONAL_CLASSES = 
			new Db4objects.Db4o.Foundation.KeySpec(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec EXCEPTIONS_ON_NOT_STORABLE
			 = new Db4objects.Db4o.Foundation.KeySpec(false);

		private static readonly Db4objects.Db4o.Foundation.KeySpec FLUSH_FILE_BUFFERS = new 
			Db4objects.Db4o.Foundation.KeySpec(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec FREESPACE_FILLER = new 
			Db4objects.Db4o.Foundation.KeySpec(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec FREESPACE_SYSTEM = new 
			Db4objects.Db4o.Foundation.KeySpec(Db4objects.Db4o.Internal.Freespace.FreespaceManager
			.FM_DEFAULT);

		private static readonly Db4objects.Db4o.Foundation.KeySpec GENERATE_UUIDS = new Db4objects.Db4o.Foundation.KeySpec
			(Db4objects.Db4o.Config.ConfigScope.INDIVIDUALLY);

		private static readonly Db4objects.Db4o.Foundation.KeySpec GENERATE_VERSION_NUMBERS
			 = new Db4objects.Db4o.Foundation.KeySpec(Db4objects.Db4o.Config.ConfigScope.INDIVIDUALLY
			);

		private static readonly Db4objects.Db4o.Foundation.KeySpec IS_SERVER = new Db4objects.Db4o.Foundation.KeySpec
			(false);

		private static readonly Db4objects.Db4o.Foundation.KeySpec QUERY_EVALUATION_MODE = 
			new Db4objects.Db4o.Foundation.KeySpec(Db4objects.Db4o.Config.QueryEvaluationMode
			.IMMEDIATE);

		private static readonly Db4objects.Db4o.Foundation.KeySpec LOCK_FILE = new Db4objects.Db4o.Foundation.KeySpec
			(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec MESSAGE_RECIPIENT = new 
			Db4objects.Db4o.Foundation.KeySpec(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec OPTIMIZE_NQ = new Db4objects.Db4o.Foundation.KeySpec
			(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec OUTSTREAM = new Db4objects.Db4o.Foundation.KeySpec
			(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec PASSWORD = new Db4objects.Db4o.Foundation.KeySpec
			((string)null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec CLIENT_QUERY_RESULT_ITERATOR_FACTORY
			 = new Db4objects.Db4o.Foundation.KeySpec(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec PREFETCH_ID_COUNT = new 
			Db4objects.Db4o.Foundation.KeySpec(10);

		private static readonly Db4objects.Db4o.Foundation.KeySpec PREFETCH_OBJECT_COUNT = 
			new Db4objects.Db4o.Foundation.KeySpec(10);

		private static readonly Db4objects.Db4o.Foundation.KeySpec READ_AS = new Db4objects.Db4o.Foundation.KeySpec
			(new Db4objects.Db4o.Foundation.Hashtable4(16));

		private static readonly Db4objects.Db4o.Foundation.KeySpec CONFIGURED_REFLECTOR = 
			new Db4objects.Db4o.Foundation.KeySpec(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec REFLECTOR = new Db4objects.Db4o.Foundation.KeySpec
			(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec RENAME = new Db4objects.Db4o.Foundation.KeySpec
			(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec RESERVED_STORAGE_SPACE
			 = new Db4objects.Db4o.Foundation.KeySpec(0);

		private static readonly Db4objects.Db4o.Foundation.KeySpec SINGLE_THREADED_CLIENT
			 = new Db4objects.Db4o.Foundation.KeySpec(false);

		private static readonly Db4objects.Db4o.Foundation.KeySpec TEST_CONSTRUCTORS = new 
			Db4objects.Db4o.Foundation.KeySpec(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec TIMEOUT_CLIENT_SOCKET = 
			new Db4objects.Db4o.Foundation.KeySpec(Db4objects.Db4o.Internal.Const4.CLIENT_SOCKET_TIMEOUT
			);

		private static readonly Db4objects.Db4o.Foundation.KeySpec TIMEOUT_PING_CLIENTS = 
			new Db4objects.Db4o.Foundation.KeySpec(Db4objects.Db4o.Internal.Const4.CONNECTION_TIMEOUT
			);

		private static readonly Db4objects.Db4o.Foundation.KeySpec TIMEOUT_SERVER_SOCKET = 
			new Db4objects.Db4o.Foundation.KeySpec(Db4objects.Db4o.Internal.Const4.SERVER_SOCKET_TIMEOUT
			);

		private static readonly Db4objects.Db4o.Foundation.KeySpec UPDATE_DEPTH = new Db4objects.Db4o.Foundation.KeySpec
			(0);

		private static readonly Db4objects.Db4o.Foundation.KeySpec WEAK_REFERENCE_COLLECTION_INTERVAL
			 = new Db4objects.Db4o.Foundation.KeySpec(1000);

		private static readonly Db4objects.Db4o.Foundation.KeySpec WEAK_REFERENCES = new 
			Db4objects.Db4o.Foundation.KeySpec(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec IOADAPTER = new Db4objects.Db4o.Foundation.KeySpec
			(new Db4objects.Db4o.IO.RandomAccessFileAdapter());

		private static readonly Db4objects.Db4o.Foundation.KeySpec ALIASES = new Db4objects.Db4o.Foundation.KeySpec
			(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec BATCH_MESSAGES = new Db4objects.Db4o.Foundation.KeySpec
			(false);

		private static readonly Db4objects.Db4o.Foundation.KeySpec MAX_BATCH_QUEUE_SIZE = 
			new Db4objects.Db4o.Foundation.KeySpec(int.MaxValue);

		private Db4objects.Db4o.Internal.ObjectContainerBase i_stream;

		private bool _internStrings;

		private int _messageLevel;

		private bool _readOnly;

		public int ActivationDepth()
		{
			return _config.GetAsInt(ACTIVATION_DEPTH);
		}

		public void ActivationDepth(int depth)
		{
			_config.Put(ACTIVATION_DEPTH, depth);
		}

		public void AllowVersionUpdates(bool flag)
		{
			_config.Put(ALLOW_VERSION_UPDATES, flag);
		}

		public void AutomaticShutDown(bool flag)
		{
			_config.Put(AUTOMATIC_SHUTDOWN, flag);
		}

		public void BlockSize(int bytes)
		{
			if (bytes < 1 || bytes > 127)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(1);
			}
			if (i_stream != null)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(46);
			}
			_config.Put(BLOCKSIZE, (byte)bytes);
		}

		public void BTreeNodeSize(int size)
		{
			_config.Put(BTREE_NODE_SIZE, size);
		}

		public void BTreeCacheHeight(int height)
		{
			_config.Put(BTREE_CACHE_HEIGHT, height);
		}

		public void Callbacks(bool turnOn)
		{
			_config.Put(CALLBACKS, turnOn);
		}

		public void CallConstructors(bool flag)
		{
			_config.Put(CALL_CONSTRUCTORS, Db4objects.Db4o.Foundation.TernaryBool.ForBoolean(
				flag));
		}

		public void ClassActivationDepthConfigurable(bool turnOn)
		{
			_config.Put(CLASS_ACTIVATION_DEPTH_CONFIGURABLE, turnOn);
		}

		internal Db4objects.Db4o.Internal.Config4Class ConfigClass(string className)
		{
			Db4objects.Db4o.Internal.Config4Class config = (Db4objects.Db4o.Internal.Config4Class
				)ExceptionalClasses().Get(className);
			return config;
		}

		public object DeepClone(object param)
		{
			Db4objects.Db4o.Internal.Config4Impl ret = new Db4objects.Db4o.Internal.Config4Impl
				();
			ret._config = (Db4objects.Db4o.Foundation.KeySpecHashtable4)_config.DeepClone(this
				);
			ret._internStrings = _internStrings;
			ret._messageLevel = _messageLevel;
			ret._readOnly = _readOnly;
			return ret;
		}

		public void Stream(Db4objects.Db4o.Internal.ObjectContainerBase stream)
		{
			i_stream = stream;
		}

		public void DetectSchemaChanges(bool flag)
		{
			_config.Put(DETECT_SCHEMA_CHANGES, flag);
		}

		public void DisableCommitRecovery()
		{
			_config.Put(DISABLE_COMMIT_RECOVERY, true);
		}

		public void DiscardFreeSpace(int bytes)
		{
			_config.Put(DISCARD_FREESPACE, bytes);
		}

		public void DiscardSmallerThan(int byteCount)
		{
			DiscardFreeSpace(byteCount);
		}

		public void Encrypt(bool flag)
		{
			GlobalSettingOnly();
			_config.Put(ENCRYPT, flag);
		}

		internal void OldEncryptionOff()
		{
			_config.Put(ENCRYPT, false);
		}

		internal void EnsureDirExists(string path)
		{
			Sharpen.IO.File file = new Sharpen.IO.File(path);
			if (!file.Exists())
			{
				file.Mkdirs();
			}
			if (file.Exists() && file.IsDirectory())
			{
			}
			else
			{
				throw new System.IO.IOException(Db4objects.Db4o.Internal.Messages.Get(37, path));
			}
		}

		internal System.IO.TextWriter ErrStream()
		{
			System.IO.TextWriter outStream = OutStreamOrNull();
			return outStream == null ? Sharpen.Runtime.Err : outStream;
		}

		public void ExceptionsOnNotStorable(bool flag)
		{
			_config.Put(EXCEPTIONS_ON_NOT_STORABLE, flag);
		}

		public void FlushFileBuffers(bool flag)
		{
			_config.Put(FLUSH_FILE_BUFFERS, flag);
		}

		public Db4objects.Db4o.Config.IFreespaceConfiguration Freespace()
		{
			return this;
		}

		public void FreespaceFiller(Db4objects.Db4o.Config.IFreespaceFiller freespaceFiller
			)
		{
			_config.Put(FREESPACE_FILLER, freespaceFiller);
		}

		public Db4objects.Db4o.Config.IFreespaceFiller FreespaceFiller()
		{
			return (Db4objects.Db4o.Config.IFreespaceFiller)_config.Get(FREESPACE_FILLER);
		}

		/// <deprecated>
		/// Use
		/// <see cref="Db4objects.Db4o.Internal.Config4Impl.GenerateUUIDs">Db4objects.Db4o.Internal.Config4Impl.GenerateUUIDs
		/// 	</see>
		/// instead.
		/// </deprecated>
		public void GenerateUUIDs(int setting)
		{
			GenerateUUIDs(Db4objects.Db4o.Config.ConfigScope.ForID(setting));
		}

		public void GenerateUUIDs(Db4objects.Db4o.Config.ConfigScope scope)
		{
			_config.Put(GENERATE_UUIDS, scope);
		}

		/// <deprecated>
		/// Use
		/// <see cref="Db4objects.Db4o.Internal.Config4Impl.GenerateVersionNumbers">Db4objects.Db4o.Internal.Config4Impl.GenerateVersionNumbers
		/// 	</see>
		/// instead.
		/// </deprecated>
		public void GenerateVersionNumbers(int setting)
		{
			GenerateVersionNumbers(Db4objects.Db4o.Config.ConfigScope.ForID(setting));
		}

		public void GenerateVersionNumbers(Db4objects.Db4o.Config.ConfigScope scope)
		{
			_config.Put(GENERATE_VERSION_NUMBERS, scope);
		}

		public Db4objects.Db4o.Messaging.IMessageSender GetMessageSender()
		{
			return this;
		}

		private void GlobalSettingOnly()
		{
			if (i_stream != null)
			{
				Sharpen.Runtime.PrintStackTrace(new System.Exception());
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(46);
			}
		}

		public void InternStrings(bool doIntern)
		{
			_internStrings = doIntern;
		}

		public void Io(Db4objects.Db4o.IO.IoAdapter adapter)
		{
			GlobalSettingOnly();
			_config.Put(IOADAPTER, adapter);
		}

		public void LockDatabaseFile(bool flag)
		{
			_config.Put(LOCK_FILE, flag);
		}

		public void MarkTransient(string marker)
		{
			Db4objects.Db4o.Internal.Platform4.MarkTransient(marker);
		}

		public void MessageLevel(int level)
		{
			_messageLevel = level;
			if (OutStream() == null)
			{
				SetOut(Sharpen.Runtime.Out);
			}
		}

		public void OptimizeNativeQueries(bool optimizeNQ)
		{
			_config.Put(OPTIMIZE_NQ, optimizeNQ);
		}

		public bool OptimizeNativeQueries()
		{
			return _config.GetAsBoolean(OPTIMIZE_NQ);
		}

		public Db4objects.Db4o.Config.IObjectClass ObjectClass(object clazz)
		{
			string className = null;
			if (clazz is string)
			{
				className = (string)clazz;
			}
			else
			{
				Db4objects.Db4o.Reflect.IReflectClass claxx = ReflectorFor(clazz);
				if (claxx == null)
				{
					return null;
				}
				className = claxx.GetName();
			}
			Db4objects.Db4o.Foundation.Hashtable4 xClasses = ExceptionalClasses();
			Db4objects.Db4o.Internal.Config4Class c4c = (Db4objects.Db4o.Internal.Config4Class
				)xClasses.Get(className);
			if (c4c == null)
			{
				c4c = new Db4objects.Db4o.Internal.Config4Class(this, className);
				xClasses.Put(className, c4c);
			}
			return c4c;
		}

		private System.IO.TextWriter OutStreamOrNull()
		{
			return (System.IO.TextWriter)_config.Get(OUTSTREAM);
		}

		internal System.IO.TextWriter OutStream()
		{
			System.IO.TextWriter outStream = OutStreamOrNull();
			return outStream == null ? Sharpen.Runtime.Out : outStream;
		}

		public void Password(string pw)
		{
			GlobalSettingOnly();
			_config.Put(PASSWORD, pw);
		}

		public void ReadOnly(bool flag)
		{
			_readOnly = flag;
		}

		public Db4objects.Db4o.Reflect.Generic.GenericReflector Reflector()
		{
			Db4objects.Db4o.Reflect.Generic.GenericReflector reflector = (Db4objects.Db4o.Reflect.Generic.GenericReflector
				)_config.Get(REFLECTOR);
			if (reflector == null)
			{
				Db4objects.Db4o.Reflect.IReflector configuredReflector = (Db4objects.Db4o.Reflect.IReflector
					)_config.Get(CONFIGURED_REFLECTOR);
				if (configuredReflector == null)
				{
					configuredReflector = Db4objects.Db4o.Internal.Platform4.CreateReflector(ClassLoader
						());
					_config.Put(CONFIGURED_REFLECTOR, configuredReflector);
				}
				reflector = new Db4objects.Db4o.Reflect.Generic.GenericReflector(null, configuredReflector
					);
				_config.Put(REFLECTOR, reflector);
				configuredReflector.SetParent(reflector);
			}
			return reflector;
		}

		public void ReflectWith(Db4objects.Db4o.Reflect.IReflector reflect)
		{
			if (i_stream != null)
			{
				Db4objects.Db4o.Internal.Exceptions4.ThrowRuntimeException(46);
			}
			if (reflect == null)
			{
				throw new System.ArgumentNullException();
			}
			_config.Put(CONFIGURED_REFLECTOR, reflect);
			_config.Put(REFLECTOR, null);
		}

		public void RefreshClasses()
		{
			if (i_stream != null)
			{
				i_stream.RefreshClasses();
			}
		}

		internal void Rename(Db4objects.Db4o.Rename a_rename)
		{
			Db4objects.Db4o.Foundation.Collection4 renameCollection = Rename();
			if (renameCollection == null)
			{
				renameCollection = new Db4objects.Db4o.Foundation.Collection4();
				_config.Put(RENAME, renameCollection);
			}
			renameCollection.Add(a_rename);
		}

		public void ReserveStorageSpace(long byteCount)
		{
			int reservedStorageSpace = (int)byteCount;
			if (reservedStorageSpace < 0)
			{
				reservedStorageSpace = 0;
			}
			_config.Put(RESERVED_STORAGE_SPACE, reservedStorageSpace);
			if (i_stream != null)
			{
				i_stream.Reserve(reservedStorageSpace);
			}
		}

		/// <summary>The ConfigImpl also is our messageSender</summary>
		public void Send(object obj)
		{
			if (i_stream != null)
			{
				i_stream.Send(obj);
			}
		}

		public void SetBlobPath(string path)
		{
			EnsureDirExists(path);
			_config.Put(BLOBPATH, path);
		}

		public void SetClassLoader(object classLoader)
		{
			ReflectWith(Db4objects.Db4o.Internal.Platform4.CreateReflector(classLoader));
		}

		public void SetMessageRecipient(Db4objects.Db4o.Messaging.IMessageRecipient messageRecipient
			)
		{
			_config.Put(MESSAGE_RECIPIENT, messageRecipient);
		}

		public void SetOut(System.IO.TextWriter outStream)
		{
			_config.Put(OUTSTREAM, outStream);
			if (i_stream != null)
			{
				i_stream.LogMsg(19, Db4objects.Db4o.Db4oFactory.Version());
			}
			else
			{
				Db4objects.Db4o.Internal.Messages.LogMsg(Db4objects.Db4o.Db4oFactory.Configure(), 
					19, Db4objects.Db4o.Db4oFactory.Version());
			}
		}

		public void SingleThreadedClient(bool flag)
		{
			_config.Put(SINGLE_THREADED_CLIENT, flag);
		}

		public void TestConstructors(bool flag)
		{
			_config.Put(TEST_CONSTRUCTORS, flag);
		}

		public void TimeoutClientSocket(int milliseconds)
		{
			_config.Put(TIMEOUT_CLIENT_SOCKET, milliseconds);
		}

		public void TimeoutPingClients(int milliseconds)
		{
			_config.Put(TIMEOUT_PING_CLIENTS, milliseconds);
		}

		public void TimeoutServerSocket(int milliseconds)
		{
			_config.Put(TIMEOUT_SERVER_SOCKET, milliseconds);
		}

		public void Unicode(bool unicodeOn)
		{
			_config.Put(ENCODING, (unicodeOn ? Db4objects.Db4o.Internal.Const4.UNICODE : Db4objects.Db4o.Internal.Const4
				.ISO8859));
		}

		public void UpdateDepth(int depth)
		{
			Db4objects.Db4o.Internal.Diagnostic.DiagnosticProcessor dp = DiagnosticProcessor(
				);
			if (dp.Enabled())
			{
				dp.CheckUpdateDepth(depth);
			}
			_config.Put(UPDATE_DEPTH, depth);
		}

		public void UseRamSystem()
		{
			_config.Put(FREESPACE_SYSTEM, Db4objects.Db4o.Internal.Freespace.FreespaceManager
				.FM_RAM);
		}

		public void UseIndexSystem()
		{
			_config.Put(FREESPACE_SYSTEM, Db4objects.Db4o.Internal.Freespace.FreespaceManager
				.FM_IX);
		}

		public void WeakReferenceCollectionInterval(int milliseconds)
		{
			_config.Put(WEAK_REFERENCE_COLLECTION_INTERVAL, milliseconds);
		}

		public void WeakReferences(bool flag)
		{
			_config.Put(WEAK_REFERENCES, flag);
		}

		private Db4objects.Db4o.Foundation.Collection4 Aliases()
		{
			Db4objects.Db4o.Foundation.Collection4 aliasesCollection = (Db4objects.Db4o.Foundation.Collection4
				)_config.Get(ALIASES);
			if (null == aliasesCollection)
			{
				aliasesCollection = new Db4objects.Db4o.Foundation.Collection4();
				_config.Put(ALIASES, aliasesCollection);
			}
			return aliasesCollection;
		}

		public void AddAlias(Db4objects.Db4o.Config.IAlias alias)
		{
			if (null == alias)
			{
				throw new System.ArgumentNullException("alias");
			}
			Aliases().Add(alias);
		}

		public void RemoveAlias(Db4objects.Db4o.Config.IAlias alias)
		{
			if (null == alias)
			{
				throw new System.ArgumentNullException("alias");
			}
			Aliases().Remove(alias);
		}

		public string ResolveAliasRuntimeName(string runtimeType)
		{
			Db4objects.Db4o.Foundation.Collection4 configuredAliases = Aliases();
			if (null == configuredAliases)
			{
				return runtimeType;
			}
			System.Collections.IEnumerator i = configuredAliases.GetEnumerator();
			while (i.MoveNext())
			{
				string resolved = ((Db4objects.Db4o.Config.IAlias)i.Current).ResolveRuntimeName(runtimeType
					);
				if (null != resolved)
				{
					return resolved;
				}
			}
			return runtimeType;
		}

		public string ResolveAliasStoredName(string storedType)
		{
			Db4objects.Db4o.Foundation.Collection4 configuredAliases = Aliases();
			if (null == configuredAliases)
			{
				return storedType;
			}
			System.Collections.IEnumerator i = configuredAliases.GetEnumerator();
			while (i.MoveNext())
			{
				string resolved = ((Db4objects.Db4o.Config.IAlias)i.Current).ResolveStoredName(storedType
					);
				if (null != resolved)
				{
					return resolved;
				}
			}
			return storedType;
		}

		internal Db4objects.Db4o.Reflect.IReflectClass ReflectorFor(object clazz)
		{
			clazz = Db4objects.Db4o.Internal.Platform4.GetClassForType(clazz);
			if (clazz is Db4objects.Db4o.Reflect.IReflectClass)
			{
				return (Db4objects.Db4o.Reflect.IReflectClass)clazz;
			}
			if (clazz is System.Type)
			{
				return Reflector().ForClass((System.Type)clazz);
			}
			if (clazz is string)
			{
				return Reflector().ForName((string)clazz);
			}
			return Reflector().ForObject(clazz);
		}

		public bool AllowVersionUpdates()
		{
			return _config.GetAsBoolean(ALLOW_VERSION_UPDATES);
		}

		internal bool AutomaticShutDown()
		{
			return _config.GetAsBoolean(AUTOMATIC_SHUTDOWN);
		}

		internal byte BlockSize()
		{
			return _config.GetAsByte(BLOCKSIZE);
		}

		public int BTreeNodeSize()
		{
			return _config.GetAsInt(BTREE_NODE_SIZE);
		}

		public int BTreeCacheHeight()
		{
			return _config.GetAsInt(BTREE_CACHE_HEIGHT);
		}

		internal string BlobPath()
		{
			return _config.GetAsString(BLOBPATH);
		}

		internal bool Callbacks()
		{
			return _config.GetAsBoolean(CALLBACKS);
		}

		internal Db4objects.Db4o.Foundation.TernaryBool CallConstructors()
		{
			return _config.GetAsTernaryBool(CALL_CONSTRUCTORS);
		}

		internal bool ClassActivationDepthConfigurable()
		{
			return _config.GetAsBoolean(CLASS_ACTIVATION_DEPTH_CONFIGURABLE);
		}

		internal object ClassLoader()
		{
			return _config.Get(CLASSLOADER);
		}

		internal bool DetectSchemaChanges()
		{
			return _config.GetAsBoolean(DETECT_SCHEMA_CHANGES);
		}

		internal bool CommitRecoveryDisabled()
		{
			return _config.GetAsBoolean(DISABLE_COMMIT_RECOVERY);
		}

		public Db4objects.Db4o.Diagnostic.IDiagnosticConfiguration Diagnostic()
		{
			return (Db4objects.Db4o.Diagnostic.IDiagnosticConfiguration)_config.Get(DIAGNOSTIC
				);
		}

		public Db4objects.Db4o.Internal.Diagnostic.DiagnosticProcessor DiagnosticProcessor
			()
		{
			return (Db4objects.Db4o.Internal.Diagnostic.DiagnosticProcessor)_config.Get(DIAGNOSTIC
				);
		}

		public int DiscardFreeSpace()
		{
			return _config.GetAsInt(DISCARD_FREESPACE);
		}

		internal byte Encoding()
		{
			return _config.GetAsByte(ENCODING);
		}

		internal bool Encrypt()
		{
			return _config.GetAsBoolean(ENCRYPT);
		}

		public Db4objects.Db4o.Foundation.Hashtable4 ExceptionalClasses()
		{
			Db4objects.Db4o.Foundation.Hashtable4 exceptionalClasses = (Db4objects.Db4o.Foundation.Hashtable4
				)_config.Get(EXCEPTIONAL_CLASSES);
			if (exceptionalClasses == null)
			{
				exceptionalClasses = new Db4objects.Db4o.Foundation.Hashtable4(16);
				_config.Put(EXCEPTIONAL_CLASSES, exceptionalClasses);
			}
			return exceptionalClasses;
		}

		public bool ExceptionsOnNotStorable()
		{
			return _config.GetAsBoolean(EXCEPTIONS_ON_NOT_STORABLE);
		}

		public bool FlushFileBuffers()
		{
			return _config.GetAsBoolean(FLUSH_FILE_BUFFERS);
		}

		internal byte FreespaceSystem()
		{
			return _config.GetAsByte(FREESPACE_SYSTEM);
		}

		public Db4objects.Db4o.Config.ConfigScope GenerateUUIDs()
		{
			return (Db4objects.Db4o.Config.ConfigScope)_config.Get(GENERATE_UUIDS);
		}

		public Db4objects.Db4o.Config.ConfigScope GenerateVersionNumbers()
		{
			return (Db4objects.Db4o.Config.ConfigScope)_config.Get(GENERATE_VERSION_NUMBERS);
		}

		public bool InternStrings()
		{
			return _internStrings;
		}

		public void IsServer(bool flag)
		{
			_config.Put(IS_SERVER, flag);
		}

		internal bool IsServer()
		{
			return _config.GetAsBoolean(IS_SERVER);
		}

		internal bool LockFile()
		{
			return _config.GetAsBoolean(LOCK_FILE);
		}

		internal int MessageLevel()
		{
			return _messageLevel;
		}

		public Db4objects.Db4o.Messaging.IMessageRecipient MessageRecipient()
		{
			return (Db4objects.Db4o.Messaging.IMessageRecipient)_config.Get(MESSAGE_RECIPIENT
				);
		}

		internal bool OptimizeNQ()
		{
			return _config.GetAsBoolean(OPTIMIZE_NQ);
		}

		internal string Password()
		{
			return _config.GetAsString(PASSWORD);
		}

		public void PrefetchIDCount(int prefetchIDCount)
		{
			_config.Put(PREFETCH_ID_COUNT, prefetchIDCount);
		}

		public int PrefetchIDCount()
		{
			return _config.GetAsInt(PREFETCH_ID_COUNT);
		}

		public void PrefetchObjectCount(int prefetchObjectCount)
		{
			_config.Put(PREFETCH_OBJECT_COUNT, prefetchObjectCount);
		}

		public int PrefetchObjectCount()
		{
			return _config.GetAsInt(PREFETCH_OBJECT_COUNT);
		}

		internal Db4objects.Db4o.Foundation.Hashtable4 ReadAs()
		{
			return (Db4objects.Db4o.Foundation.Hashtable4)_config.Get(READ_AS);
		}

		public bool IsReadOnly()
		{
			return _readOnly;
		}

		internal Db4objects.Db4o.Foundation.Collection4 Rename()
		{
			return (Db4objects.Db4o.Foundation.Collection4)_config.Get(RENAME);
		}

		internal int ReservedStorageSpace()
		{
			return _config.GetAsInt(RESERVED_STORAGE_SPACE);
		}

		public bool SingleThreadedClient()
		{
			return _config.GetAsBoolean(SINGLE_THREADED_CLIENT);
		}

		internal bool TestConstructors()
		{
			return _config.GetAsBoolean(TEST_CONSTRUCTORS);
		}

		public int TimeoutClientSocket()
		{
			return _config.GetAsInt(TIMEOUT_CLIENT_SOCKET);
		}

		public int TimeoutPingClients()
		{
			return _config.GetAsInt(TIMEOUT_PING_CLIENTS);
		}

		public int TimeoutServerSocket()
		{
			return _config.GetAsInt(TIMEOUT_SERVER_SOCKET);
		}

		internal int UpdateDepth()
		{
			return _config.GetAsInt(UPDATE_DEPTH);
		}

		internal int WeakReferenceCollectionInterval()
		{
			return _config.GetAsInt(WEAK_REFERENCE_COLLECTION_INTERVAL);
		}

		internal bool WeakReferences()
		{
			return _config.GetAsBoolean(WEAK_REFERENCES);
		}

		internal Db4objects.Db4o.IO.IoAdapter IoAdapter()
		{
			return (Db4objects.Db4o.IO.IoAdapter)_config.Get(IOADAPTER);
		}

		public Db4objects.Db4o.Config.IQueryConfiguration Queries()
		{
			return this;
		}

		public void EvaluationMode(Db4objects.Db4o.Config.QueryEvaluationMode mode)
		{
			_config.Put(QUERY_EVALUATION_MODE, mode);
		}

		public Db4objects.Db4o.Config.QueryEvaluationMode QueryEvaluationMode()
		{
			return (Db4objects.Db4o.Config.QueryEvaluationMode)_config.Get(QUERY_EVALUATION_MODE
				);
		}

		public void QueryResultIteratorFactory(Db4objects.Db4o.Internal.CS.IQueryResultIteratorFactory
			 factory)
		{
			_config.Put(CLIENT_QUERY_RESULT_ITERATOR_FACTORY, factory);
		}

		public Db4objects.Db4o.Internal.CS.IQueryResultIteratorFactory QueryResultIteratorFactory
			()
		{
			return (Db4objects.Db4o.Internal.CS.IQueryResultIteratorFactory)_config.Get(CLIENT_QUERY_RESULT_ITERATOR_FACTORY
				);
		}

		public Db4objects.Db4o.Config.IClientServerConfiguration ClientServer()
		{
			return this;
		}

		public void BatchMessages(bool flag)
		{
			_config.Put(BATCH_MESSAGES, flag);
		}

		public bool BatchMessages()
		{
			return _config.GetAsBoolean(BATCH_MESSAGES);
		}

		public void MaxBatchQueueSize(int maxSize)
		{
			_config.Put(MAX_BATCH_QUEUE_SIZE, maxSize);
		}

		public int MaxBatchQueueSize()
		{
			return _config.GetAsInt(MAX_BATCH_QUEUE_SIZE);
		}
	}
}
