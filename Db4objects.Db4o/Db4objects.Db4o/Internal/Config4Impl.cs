/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Messaging;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Internal
{
	/// <summary>Configuration template for creating new db4o files</summary>
	/// <exclude></exclude>
	public sealed class Config4Impl : IConfiguration, IDeepClone, IMessageSender, IFreespaceConfiguration
		, IQueryConfiguration, IClientServerConfiguration
	{
		private KeySpecHashtable4 _config = new KeySpecHashtable4(50);

		private static readonly KeySpec ACTIVATION_DEPTH = new KeySpec(5);

		private static readonly KeySpec ACTIVATION_DEPTH_PROVIDER = new KeySpec(LegacyActivationDepthProvider
			.INSTANCE);

		private static readonly KeySpec ALLOW_VERSION_UPDATES = new KeySpec(false);

		private static readonly KeySpec AUTOMATIC_SHUTDOWN = new KeySpec(true);

		private static readonly KeySpec BLOCKSIZE = new KeySpec((byte)1);

		private static readonly KeySpec BLOBPATH = new KeySpec(null);

		private static readonly KeySpec BTREE_NODE_SIZE = new KeySpec(119);

		private static readonly KeySpec BTREE_CACHE_HEIGHT = new KeySpec(1);

		private static readonly KeySpec CALLBACKS = new KeySpec(true);

		private static readonly KeySpec CALL_CONSTRUCTORS = new KeySpec(TernaryBool.UNSPECIFIED
			);

		private static readonly KeySpec CONFIGURATION_ITEMS = new KeySpec(null);

		private static readonly KeySpec CLASS_ACTIVATION_DEPTH_CONFIGURABLE = new KeySpec
			(true);

		private static readonly KeySpec CLASSLOADER = new KeySpec(null);

		private static readonly KeySpec DETECT_SCHEMA_CHANGES = new KeySpec(true);

		private static readonly KeySpec DIAGNOSTIC = new KeySpec(new Db4objects.Db4o.Internal.Diagnostic.DiagnosticProcessor
			());

		private static readonly KeySpec DISABLE_COMMIT_RECOVERY = new KeySpec(false);

		private static readonly KeySpec DISCARD_FREESPACE = new KeySpec(0);

		private static readonly KeySpec ENCODING = new KeySpec(Const4.UNICODE);

		private static readonly KeySpec ENCRYPT = new KeySpec(false);

		private static readonly KeySpec EXCEPTIONAL_CLASSES = new KeySpec(null);

		private static readonly KeySpec EXCEPTIONS_ON_NOT_STORABLE = new KeySpec(false);

		private static readonly KeySpec FLUSH_FILE_BUFFERS = new KeySpec(true);

		private static readonly KeySpec FREESPACE_FILLER = new KeySpec(null);

		private static readonly KeySpec FREESPACE_SYSTEM = new KeySpec(AbstractFreespaceManager
			.FM_DEFAULT);

		private static readonly KeySpec GENERATE_UUIDS = new KeySpec(ConfigScope.INDIVIDUALLY
			);

		private static readonly KeySpec GENERATE_VERSION_NUMBERS = new KeySpec(ConfigScope
			.INDIVIDUALLY);

		private static readonly KeySpec IS_SERVER = new KeySpec(false);

		private static readonly KeySpec QUERY_EVALUATION_MODE = new KeySpec(Db4objects.Db4o.Config.QueryEvaluationMode
			.IMMEDIATE);

		private static readonly KeySpec LOCK_FILE = new KeySpec(true);

		private static readonly KeySpec MESSAGE_RECIPIENT = new KeySpec(null);

		private static readonly KeySpec OPTIMIZE_NQ = new KeySpec(true);

		private static readonly KeySpec OUTSTREAM = new KeySpec(null);

		private static readonly KeySpec PASSWORD = new KeySpec((string)null);

		private static readonly KeySpec CLIENT_QUERY_RESULT_ITERATOR_FACTORY = new KeySpec
			(null);

		private static readonly KeySpec PREFETCH_ID_COUNT = new KeySpec(10);

		private static readonly KeySpec PREFETCH_OBJECT_COUNT = new KeySpec(10);

		private static readonly KeySpec READ_AS = new KeySpec(new Hashtable4(16));

		private static readonly KeySpec CONFIGURED_REFLECTOR = new KeySpec(null);

		private static readonly KeySpec REFLECTOR = new KeySpec(null);

		private static readonly KeySpec RENAME = new KeySpec(null);

		private static readonly KeySpec RESERVED_STORAGE_SPACE = new KeySpec(0);

		private static readonly KeySpec SINGLE_THREADED_CLIENT = new KeySpec(false);

		private static readonly KeySpec TEST_CONSTRUCTORS = new KeySpec(true);

		private static readonly KeySpec TIMEOUT_CLIENT_SOCKET = new KeySpec(Const4.CLIENT_SOCKET_TIMEOUT
			);

		private static readonly KeySpec TIMEOUT_SERVER_SOCKET = new KeySpec(Const4.SERVER_SOCKET_TIMEOUT
			);

		private static readonly KeySpec UPDATE_DEPTH = new KeySpec(0);

		private static readonly KeySpec WEAK_REFERENCE_COLLECTION_INTERVAL = new KeySpec(
			1000);

		private static readonly KeySpec WEAK_REFERENCES = new KeySpec(true);

		private static readonly KeySpec IOADAPTER = new KeySpec(new CachedIoAdapter(new RandomAccessFileAdapter
			()));

		private static readonly KeySpec ALIASES = new KeySpec(null);

		private static readonly KeySpec BATCH_MESSAGES = new KeySpec(true);

		private static readonly KeySpec MAX_BATCH_QUEUE_SIZE = new KeySpec(int.MaxValue);

		private ObjectContainerBase i_stream;

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

		public void Add(IConfigurationItem item)
		{
			item.Prepare(this);
			SafeConfigurationItems().Put(item, item);
		}

		private Hashtable4 SafeConfigurationItems()
		{
			Hashtable4 items = ConfigurationItems();
			if (items == null)
			{
				items = new Hashtable4(16);
				_config.Put(CONFIGURATION_ITEMS, items);
			}
			return items;
		}

		public void AllowVersionUpdates(bool flag)
		{
			_config.Put(ALLOW_VERSION_UPDATES, flag);
		}

		private Hashtable4 ConfigurationItems()
		{
			return (Hashtable4)_config.Get(CONFIGURATION_ITEMS);
		}

		public void ApplyConfigurationItems(IInternalObjectContainer container)
		{
			Hashtable4 items = ConfigurationItems();
			if (items == null)
			{
				return;
			}
			IEnumerator i = items.Iterator();
			while (i.MoveNext())
			{
				IEntry4 entry = (IEntry4)i.Current;
				IConfigurationItem item = (IConfigurationItem)entry.Value();
				item.Apply(container);
			}
		}

		public void AutomaticShutDown(bool flag)
		{
			_config.Put(AUTOMATIC_SHUTDOWN, flag);
		}

		public void BlockSize(int bytes)
		{
			if (bytes < 1 || bytes > 127)
			{
				throw new ArgumentException();
			}
			GlobalSettingOnly();
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
			_config.Put(CALL_CONSTRUCTORS, TernaryBool.ForBoolean(flag));
		}

		public void ClassActivationDepthConfigurable(bool turnOn)
		{
			_config.Put(CLASS_ACTIVATION_DEPTH_CONFIGURABLE, turnOn);
		}

		internal Config4Class ConfigClass(string className)
		{
			Config4Class config = (Config4Class)ExceptionalClasses().Get(className);
			return config;
		}

		[System.ObsoleteAttribute(@"using deprecated api")]
		private bool IsIgnoredClass(string className)
		{
			Type[] ignore = new Type[] { typeof(P1HashElement), typeof(P1ListElement), typeof(
				P1Object), typeof(P1Collection), typeof(StaticClass), typeof(StaticField) };
			for (int i = 0; i < ignore.Length; i++)
			{
				if (ignore[i].FullName.Equals(className))
				{
					return true;
				}
			}
			return false;
		}

		public object DeepClone(object param)
		{
			Config4Impl ret = new Config4Impl();
			ret._config = (KeySpecHashtable4)_config.DeepClone(this);
			ret._internStrings = _internStrings;
			ret._messageLevel = _messageLevel;
			ret._readOnly = _readOnly;
			return ret;
		}

		public void Stream(ObjectContainerBase stream)
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

		[System.ObsoleteAttribute]
		public void DiscardFreeSpace(int bytes)
		{
			if (bytes < 0)
			{
				throw new ArgumentException();
			}
			_config.Put(DISCARD_FREESPACE, bytes);
		}

		public void DiscardSmallerThan(int byteCount)
		{
			DiscardFreeSpace(byteCount);
		}

		[System.ObsoleteAttribute]
		public void Encrypt(bool flag)
		{
			GlobalSettingOnly();
			_config.Put(ENCRYPT, flag);
		}

		internal void OldEncryptionOff()
		{
			_config.Put(ENCRYPT, false);
		}

		/// <exception cref="IOException"></exception>
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
				throw new IOException(Db4objects.Db4o.Internal.Messages.Get(37, path));
			}
		}

		internal TextWriter ErrStream()
		{
			TextWriter outStream = OutStreamOrNull();
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

		public IFreespaceConfiguration Freespace()
		{
			return this;
		}

		public void FreespaceFiller(IFreespaceFiller freespaceFiller)
		{
			_config.Put(FREESPACE_FILLER, freespaceFiller);
		}

		public IFreespaceFiller FreespaceFiller()
		{
			return (IFreespaceFiller)_config.Get(FREESPACE_FILLER);
		}

		[System.ObsoleteAttribute(@"Use")]
		public void GenerateUUIDs(int setting)
		{
			GenerateUUIDs(ConfigScope.ForID(setting));
		}

		public void GenerateUUIDs(ConfigScope scope)
		{
			_config.Put(GENERATE_UUIDS, scope);
		}

		[System.ObsoleteAttribute(@"Use")]
		public void GenerateVersionNumbers(int setting)
		{
			GenerateVersionNumbers(ConfigScope.ForID(setting));
		}

		public void GenerateVersionNumbers(ConfigScope scope)
		{
			_config.Put(GENERATE_VERSION_NUMBERS, scope);
		}

		public IMessageSender GetMessageSender()
		{
			return this;
		}

		private void GlobalSettingOnly()
		{
			if (i_stream != null)
			{
				throw new GlobalOnlyConfigException();
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
			Platform4.MarkTransient(marker);
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

		public IObjectClass ObjectClass(object clazz)
		{
			string className = null;
			if (clazz is string)
			{
				className = (string)clazz;
			}
			else
			{
				IReflectClass claxx = ReflectorFor(clazz);
				if (claxx == null)
				{
					return null;
				}
				className = claxx.GetName();
			}
			Hashtable4 xClasses = ExceptionalClasses();
			Config4Class c4c = (Config4Class)xClasses.Get(className);
			if (c4c == null)
			{
				c4c = new Config4Class(this, className);
				xClasses.Put(className, c4c);
			}
			return c4c;
		}

		private TextWriter OutStreamOrNull()
		{
			return (TextWriter)_config.Get(OUTSTREAM);
		}

		internal TextWriter OutStream()
		{
			TextWriter outStream = OutStreamOrNull();
			return outStream == null ? Sharpen.Runtime.Out : outStream;
		}

		[System.ObsoleteAttribute]
		public void Password(string pw)
		{
			GlobalSettingOnly();
			_config.Put(PASSWORD, pw);
		}

		public void ReadOnly(bool flag)
		{
			_readOnly = flag;
		}

		public GenericReflector Reflector()
		{
			GenericReflector reflector = (GenericReflector)_config.Get(REFLECTOR);
			if (reflector == null)
			{
				IReflector configuredReflector = (IReflector)_config.Get(CONFIGURED_REFLECTOR);
				if (configuredReflector == null)
				{
					configuredReflector = Platform4.CreateReflector(ClassLoader());
					_config.Put(CONFIGURED_REFLECTOR, configuredReflector);
				}
				reflector = new GenericReflector(null, configuredReflector);
				_config.Put(REFLECTOR, reflector);
				configuredReflector.SetParent(reflector);
			}
			return reflector;
		}

		public void ReflectWith(IReflector reflect)
		{
			if (i_stream != null)
			{
				Exceptions4.ThrowRuntimeException(46);
			}
			if (reflect == null)
			{
				throw new ArgumentNullException();
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
			Collection4 renameCollection = Rename();
			if (renameCollection == null)
			{
				renameCollection = new Collection4();
				_config.Put(RENAME, renameCollection);
			}
			renameCollection.Add(a_rename);
		}

		/// <exception cref="DatabaseReadOnlyException"></exception>
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

		/// <exception cref="IOException"></exception>
		public void SetBlobPath(string path)
		{
			EnsureDirExists(path);
			_config.Put(BLOBPATH, path);
		}

		[System.ObsoleteAttribute]
		public void SetClassLoader(object classLoader)
		{
			ReflectWith(Platform4.CreateReflector(classLoader));
		}

		public void SetMessageRecipient(IMessageRecipient messageRecipient)
		{
			_config.Put(MESSAGE_RECIPIENT, messageRecipient);
		}

		public void SetOut(TextWriter outStream)
		{
			_config.Put(OUTSTREAM, outStream);
			if (i_stream != null)
			{
				i_stream.LogMsg(19, Db4oFactory.Version());
			}
			else
			{
				Db4objects.Db4o.Internal.Messages.LogMsg(Db4oFactory.Configure(), 19, Db4oFactory
					.Version());
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

		public void TimeoutServerSocket(int milliseconds)
		{
			_config.Put(TIMEOUT_SERVER_SOCKET, milliseconds);
		}

		public void Unicode(bool unicodeOn)
		{
			_config.Put(ENCODING, (unicodeOn ? Const4.UNICODE : Const4.ISO8859));
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

		public void UseBTreeSystem()
		{
			_config.Put(FREESPACE_SYSTEM, AbstractFreespaceManager.FM_BTREE);
		}

		public void UseRamSystem()
		{
			_config.Put(FREESPACE_SYSTEM, AbstractFreespaceManager.FM_RAM);
		}

		public void UseIndexSystem()
		{
			throw new NotSupportedException();
		}

		public void WeakReferenceCollectionInterval(int milliseconds)
		{
			_config.Put(WEAK_REFERENCE_COLLECTION_INTERVAL, milliseconds);
		}

		public void WeakReferences(bool flag)
		{
			_config.Put(WEAK_REFERENCES, flag);
		}

		private Collection4 Aliases()
		{
			Collection4 aliasesCollection = (Collection4)_config.Get(ALIASES);
			if (null == aliasesCollection)
			{
				aliasesCollection = new Collection4();
				_config.Put(ALIASES, aliasesCollection);
			}
			return aliasesCollection;
		}

		public void AddAlias(IAlias alias)
		{
			if (null == alias)
			{
				throw new ArgumentNullException("alias");
			}
			Aliases().Add(alias);
		}

		public void RemoveAlias(IAlias alias)
		{
			if (null == alias)
			{
				throw new ArgumentNullException("alias");
			}
			Aliases().Remove(alias);
		}

		public string ResolveAliasRuntimeName(string runtimeType)
		{
			Collection4 configuredAliases = Aliases();
			if (null == configuredAliases)
			{
				return runtimeType;
			}
			IEnumerator i = configuredAliases.GetEnumerator();
			while (i.MoveNext())
			{
				string resolved = ((IAlias)i.Current).ResolveRuntimeName(runtimeType);
				if (null != resolved)
				{
					return resolved;
				}
			}
			return runtimeType;
		}

		public string ResolveAliasStoredName(string storedType)
		{
			Collection4 configuredAliases = Aliases();
			if (null == configuredAliases)
			{
				return storedType;
			}
			IEnumerator i = configuredAliases.GetEnumerator();
			while (i.MoveNext())
			{
				string resolved = ((IAlias)i.Current).ResolveStoredName(storedType);
				if (null != resolved)
				{
					return resolved;
				}
			}
			return storedType;
		}

		internal IReflectClass ReflectorFor(object clazz)
		{
			return ReflectorUtils.ReflectClassFor(Reflector(), clazz);
		}

		public bool AllowVersionUpdates()
		{
			return _config.GetAsBoolean(ALLOW_VERSION_UPDATES);
		}

		internal bool AutomaticShutDown()
		{
			return _config.GetAsBoolean(AUTOMATIC_SHUTDOWN);
		}

		public byte BlockSize()
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

		internal TernaryBool CallConstructors()
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

		public IDiagnosticConfiguration Diagnostic()
		{
			return (IDiagnosticConfiguration)_config.Get(DIAGNOSTIC);
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

		public Hashtable4 ExceptionalClasses()
		{
			Hashtable4 exceptionalClasses = (Hashtable4)_config.Get(EXCEPTIONAL_CLASSES);
			if (exceptionalClasses == null)
			{
				exceptionalClasses = new Hashtable4(16);
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

		public ConfigScope GenerateUUIDs()
		{
			return (ConfigScope)_config.Get(GENERATE_UUIDS);
		}

		public ConfigScope GenerateVersionNumbers()
		{
			return (ConfigScope)_config.Get(GENERATE_VERSION_NUMBERS);
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

		public IMessageRecipient MessageRecipient()
		{
			return (IMessageRecipient)_config.Get(MESSAGE_RECIPIENT);
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

		internal Hashtable4 ReadAs()
		{
			return (Hashtable4)_config.Get(READ_AS);
		}

		public bool IsReadOnly()
		{
			return _readOnly;
		}

		internal Collection4 Rename()
		{
			return (Collection4)_config.Get(RENAME);
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

		public IQueryConfiguration Queries()
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

		public void QueryResultIteratorFactory(IQueryResultIteratorFactory factory)
		{
			_config.Put(CLIENT_QUERY_RESULT_ITERATOR_FACTORY, factory);
		}

		public IQueryResultIteratorFactory QueryResultIteratorFactory()
		{
			return (IQueryResultIteratorFactory)_config.Get(CLIENT_QUERY_RESULT_ITERATOR_FACTORY
				);
		}

		public IClientServerConfiguration ClientServer()
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

		public void ActivationDepthProvider(IActivationDepthProvider provider)
		{
			_config.Put(ACTIVATION_DEPTH_PROVIDER, provider);
		}

		public IActivationDepthProvider ActivationDepthProvider()
		{
			return (IActivationDepthProvider)_config.Get(ACTIVATION_DEPTH_PROVIDER);
		}
	}
}
