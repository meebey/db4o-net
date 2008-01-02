/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Replication;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Internal
{
	/// <exclude>
	/// TODO: This class was written to make ObjectContainerBase
	/// leaner, so TransportObjectContainer has less members.
	/// All funcionality of this class should become part of
	/// ObjectContainerBase and the functionality in
	/// ObjectContainerBase should delegate to independant
	/// modules without circular references.
	/// </exclude>
	public sealed class HandlerRegistry
	{
		private readonly ObjectContainerBase _container;

		private static readonly IDb4oTypeImpl[] _db4oTypes = new IDb4oTypeImpl[] { new BlobImpl
			() };

		private ClassMetadata i_anyArray;

		private ClassMetadata i_anyArrayN;

		public StringHandler _stringHandler;

		private Hashtable4 _handlers = new Hashtable4(16);

		private Hashtable4 _classes = new Hashtable4(16);

		private Hashtable4 _classMetadata = new Hashtable4(16);

		private Hashtable4 _ids = new Hashtable4(16);

		private int _highestBuiltinTypeID = Handlers4.ANY_ARRAY_N_ID + 1;

		private const int PRIMITIVECOUNT = 8;

		public const int ANY_ID = 11;

		private readonly VirtualFieldMetadata[] _virtualFields = new VirtualFieldMetadata
			[2];

		private readonly Hashtable4 _mapReflectorToHandler = new Hashtable4(32);

		private readonly Hashtable4 _mapHandlerToReflector = new Hashtable4(32);

		private SharedIndexedFields _indexes;

		internal Db4objects.Db4o.Internal.Replication.MigrationConnection i_migration;

		internal IDb4oReplicationReferenceProvider _replicationReferenceProvider;

		public readonly DiagnosticProcessor _diagnosticProcessor;

		public bool i_encrypt;

		internal byte[] i_encryptor;

		internal int i_lastEncryptorByte;

		internal readonly GenericReflector _reflector;

		private readonly Hashtable4 _handlerVersions = new Hashtable4(16);

		private LatinStringIO _stringIO;

		public IReflectClass ICLASS_COMPARE;

		internal IReflectClass ICLASS_DB4OTYPE;

		internal IReflectClass ICLASS_DB4OTYPEIMPL;

		public IReflectClass ICLASS_INTERNAL;

		internal IReflectClass ICLASS_UNVERSIONED;

		public IReflectClass ICLASS_OBJECT;

		internal IReflectClass ICLASS_OBJECTCONTAINER;

		public IReflectClass ICLASS_STATICCLASS;

		public IReflectClass ICLASS_STRING;

		internal IReflectClass ICLASS_TRANSIENTCLASS;

		internal HandlerRegistry(ObjectContainerBase container, byte stringEncoding, GenericReflector
			 reflector)
		{
			_stringIO = LatinStringIO.ForEncoding(stringEncoding);
			_container = container;
			container._handlers = this;
			_reflector = reflector;
			_diagnosticProcessor = container.ConfigImpl().DiagnosticProcessor();
			InitClassReflectors(reflector);
			_indexes = new SharedIndexedFields(container);
			_virtualFields[0] = _indexes._version;
			_virtualFields[1] = _indexes._uUID;
			RegisterBuiltinHandlers();
			RegisterPlatformTypes();
			InitArrayHandlers();
		}

		private void InitArrayHandlers()
		{
			UntypedFieldHandler handler = UntypedHandler();
			IReflectClass classReflector = handler.ClassReflector();
			i_anyArray = new PrimitiveFieldHandler(_container, new ArrayHandler(_container, handler
				, false), Handlers4.ANY_ARRAY_ID, classReflector);
			_classMetadata.Put(Handlers4.ANY_ARRAY_ID, i_anyArray);
			i_anyArrayN = new PrimitiveFieldHandler(_container, new MultidimensionalArrayHandler
				(_container, handler, false), Handlers4.ANY_ARRAY_N_ID, classReflector);
			_classMetadata.Put(Handlers4.ANY_ARRAY_N_ID, i_anyArrayN);
		}

		private void RegisterPlatformTypes()
		{
			NetTypeHandler[] handlers = Platform4.Types(_container);
			for (int i = 0; i < handlers.Length; i++)
			{
				handlers[i].Initialize();
				IGenericConverter converter = (handlers[i] is IGenericConverter) ? (IGenericConverter
					)handlers[i] : null;
				RegisterBuiltinHandler(handlers[i].GetID(), handlers[i], true, handlers[i].GetName
					(), converter);
			}
		}

		private void RegisterBuiltinHandlers()
		{
			IntHandler intHandler = new IntHandler(_container);
			RegisterBuiltinHandler(Handlers4.INT_ID, intHandler);
			RegisterHandlerVersion(intHandler, 0, new IntHandler0(_container));
			LongHandler longHandler = new LongHandler(_container);
			RegisterBuiltinHandler(Handlers4.LONG_ID, longHandler);
			RegisterHandlerVersion(longHandler, 0, new LongHandler0(_container));
			FloatHandler floatHandler = new FloatHandler(_container);
			RegisterBuiltinHandler(Handlers4.FLOAT_ID, floatHandler);
			RegisterHandlerVersion(floatHandler, 0, new FloatHandler0(_container));
			BooleanHandler booleanHandler = new BooleanHandler(_container);
			RegisterBuiltinHandler(Handlers4.BOOLEAN_ID, booleanHandler);
			DoubleHandler doubleHandler = new DoubleHandler(_container);
			RegisterBuiltinHandler(Handlers4.DOUBLE_ID, doubleHandler);
			RegisterHandlerVersion(doubleHandler, 0, new DoubleHandler0(_container));
			ByteHandler byteHandler = new ByteHandler(_container);
			RegisterBuiltinHandler(Handlers4.BYTE_ID, byteHandler);
			CharHandler charHandler = new CharHandler(_container);
			RegisterBuiltinHandler(Handlers4.CHAR_ID, charHandler);
			ShortHandler shortHandler = new ShortHandler(_container);
			RegisterBuiltinHandler(Handlers4.SHORT_ID, shortHandler);
			RegisterHandlerVersion(shortHandler, 0, new ShortHandler0(_container));
			_stringHandler = new StringHandler(_container);
			RegisterBuiltinHandler(Handlers4.STRING_ID, _stringHandler);
			RegisterHandlerVersion(_stringHandler, 0, new StringHandler0(_stringHandler));
			DateHandler dateHandler = new DateHandler(_container);
			RegisterBuiltinHandler(Handlers4.DATE_ID, dateHandler);
			RegisterHandlerVersion(dateHandler, 0, new DateHandler0(_container));
			UntypedFieldHandler untypedFieldHandler = new UntypedFieldHandler(_container);
			RegisterBuiltinHandler(Handlers4.UNTYPED_ID, untypedFieldHandler, false, null, null
				);
			RegisterHandlerVersion(untypedFieldHandler, 0, new UntypedFieldHandler0(_container
				));
		}

		private void RegisterBuiltinHandler(int id, IBuiltinTypeHandler handler)
		{
			RegisterBuiltinHandler(id, handler, true, handler.ClassReflector().GetName(), null
				);
		}

		private void RegisterBuiltinHandler(int id, IBuiltinTypeHandler handler, bool registerPrimitiveClass
			, string primitiveName, IGenericConverter converter)
		{
			if (registerPrimitiveClass)
			{
				_reflector.RegisterPrimitiveClass(id, primitiveName, converter);
			}
			IReflectClass classReflector = handler.ClassReflector();
			_handlers.Put(id, handler);
			_classes.Put(id, classReflector);
			PrimitiveFieldHandler primitiveFieldHandler = new PrimitiveFieldHandler(_container
				, handler, id, classReflector);
			_classMetadata.Put(id, primitiveFieldHandler);
			Map(id, primitiveFieldHandler, classReflector);
			if (id > _highestBuiltinTypeID)
			{
				_highestBuiltinTypeID = id;
			}
		}

		private void Map(int id, ITypeHandler4 handler, IReflectClass classReflector)
		{
			_mapReflectorToHandler.Put(classReflector, handler);
			_mapHandlerToReflector.Put(handler, classReflector);
			if (id != 0)
			{
				_ids.Put(handler, id);
			}
		}

		private void RegisterHandlerVersion(ITypeHandler4 handler, int version, ITypeHandler4
			 replacement)
		{
			_handlerVersions.Put(new HandlerVersionKey(handler, version), replacement);
		}

		public ITypeHandler4 CorrectHandlerVersion(ITypeHandler4 handler, int version)
		{
			if (version == MarshallingContext.HANDLER_VERSION)
			{
				return handler;
			}
			ITypeHandler4 replacement = (ITypeHandler4)_handlerVersions.Get(new HandlerVersionKey
				(handler, version));
			if (replacement != null)
			{
				return replacement;
			}
			if (handler is MultidimensionalArrayHandler && (version == 0))
			{
				return new MultidimensionalArrayHandler0((ArrayHandler)handler, this, version);
			}
			if (handler is ArrayHandler && (version == 0))
			{
				return new ArrayHandler0((ArrayHandler)handler, this, version);
			}
			if (handler is PrimitiveFieldHandler && (version == 0))
			{
				return new PrimitiveFieldHandler((PrimitiveFieldHandler)handler, this, version);
			}
			return handler;
		}

		internal int ArrayType(object a_object)
		{
			IReflectClass claxx = _container.Reflector().ForObject(a_object);
			if (!claxx.IsArray())
			{
				return 0;
			}
			if (_container.Reflector().Array().IsNDimensional(claxx))
			{
				return Const4.TYPE_NARRAY;
			}
			return Const4.TYPE_ARRAY;
		}

		internal bool CreateConstructor(IReflectClass claxx, bool skipConstructor)
		{
			if (claxx == null)
			{
				return false;
			}
			if (claxx.IsAbstract() || claxx.IsInterface())
			{
				return true;
			}
			if (!Platform4.CallConstructor())
			{
				if (claxx.SkipConstructor(skipConstructor, _container.Config().TestConstructors()
					))
				{
					return true;
				}
			}
			if (!_container.ConfigImpl().TestConstructors())
			{
				return true;
			}
			if (claxx.NewInstance() != null)
			{
				return true;
			}
			if (_container.Reflector().ConstructorCallsSupported())
			{
				Tree sortedConstructors = SortConstructorsByParamsCount(claxx);
				return FindConstructor(claxx, sortedConstructors);
			}
			return false;
		}

		private bool FindConstructor(IReflectClass claxx, Tree sortedConstructors)
		{
			if (sortedConstructors == null)
			{
				return false;
			}
			IEnumerator iter = new TreeNodeIterator(sortedConstructors);
			while (iter.MoveNext())
			{
				object obj = iter.Current;
				IReflectConstructor constructor = (IReflectConstructor)((TreeIntObject)obj)._object;
				IReflectClass[] paramTypes = constructor.GetParameterTypes();
				object[] @params = new object[paramTypes.Length];
				for (int j = 0; j < @params.Length; j++)
				{
					@params[j] = NullValue(paramTypes[j]);
				}
				object res = constructor.NewInstance(@params);
				if (res != null)
				{
					claxx.UseConstructor(constructor, @params);
					return true;
				}
			}
			return false;
		}

		private object NullValue(IReflectClass clazz)
		{
			for (int k = 1; k <= PRIMITIVECOUNT; k++)
			{
				PrimitiveHandler handler = (PrimitiveHandler)HandlerForID(k);
				if (clazz.Equals(handler.PrimitiveClassReflector()))
				{
					return handler.PrimitiveNull();
				}
			}
			return null;
		}

		private Tree SortConstructorsByParamsCount(IReflectClass claxx)
		{
			IReflectConstructor[] constructors = claxx.GetDeclaredConstructors();
			Tree sortedConstructors = null;
			for (int i = 0; i < constructors.Length; i++)
			{
				constructors[i].SetAccessible();
				int parameterCount = constructors[i].GetParameterTypes().Length;
				sortedConstructors = Tree.Add(sortedConstructors, new TreeIntObject(i + constructors
					.Length * parameterCount, constructors[i]));
			}
			return sortedConstructors;
		}

		public void Decrypt(BufferImpl reader)
		{
			if (i_encrypt)
			{
				int encryptorOffSet = i_lastEncryptorByte;
				byte[] bytes = reader._buffer;
				for (int i = reader.Length() - 1; i >= 0; i--)
				{
					bytes[i] += i_encryptor[encryptorOffSet];
					if (encryptorOffSet == 0)
					{
						encryptorOffSet = i_lastEncryptorByte;
					}
					else
					{
						encryptorOffSet--;
					}
				}
			}
		}

		public void Encrypt(BufferImpl reader)
		{
			if (i_encrypt)
			{
				byte[] bytes = reader._buffer;
				int encryptorOffSet = i_lastEncryptorByte;
				for (int i = reader.Length() - 1; i >= 0; i--)
				{
					bytes[i] -= i_encryptor[encryptorOffSet];
					if (encryptorOffSet == 0)
					{
						encryptorOffSet = i_lastEncryptorByte;
					}
					else
					{
						encryptorOffSet--;
					}
				}
			}
		}

		public void OldEncryptionOff()
		{
			i_encrypt = false;
			i_encryptor = null;
			i_lastEncryptorByte = 0;
			_container.ConfigImpl().OldEncryptionOff();
		}

		public IReflectClass ClassForID(int id)
		{
			return (IReflectClass)_classes.Get(id);
		}

		public ITypeHandler4 HandlerForID(int id)
		{
			return (ITypeHandler4)_handlers.Get(id);
		}

		public int HandlerID(ITypeHandler4 handler)
		{
			if (handler is ClassMetadata)
			{
				return ((ClassMetadata)handler).GetID();
			}
			object idAsInt = _ids.Get(handler);
			if (idAsInt == null)
			{
				return 0;
			}
			return ((int)idAsInt);
		}

		public ITypeHandler4 HandlerForClass(ObjectContainerBase container, IReflectClass
			 clazz)
		{
			return ClassMetadataForClass(container, clazz).TypeHandler();
		}

		public ClassMetadata ClassMetadataForClass(ObjectContainerBase container, IReflectClass
			 clazz)
		{
			if (clazz == null)
			{
				return null;
			}
			IReflectClass baseType = Handlers4.BaseType(clazz);
			ClassMetadata classMetadata = ClassMetadataForClass(baseType);
			if (classMetadata != null)
			{
				return classMetadata;
			}
			return container.ProduceClassMetadata(baseType);
		}

		public UntypedFieldHandler UntypedHandler()
		{
			return (UntypedFieldHandler)HandlerForID(Handlers4.UNTYPED_ID);
		}

		private void InitClassReflectors(GenericReflector reflector)
		{
			ICLASS_COMPARE = reflector.ForClass(Const4.CLASS_COMPARE);
			ICLASS_DB4OTYPE = reflector.ForClass(Const4.CLASS_DB4OTYPE);
			ICLASS_DB4OTYPEIMPL = reflector.ForClass(Const4.CLASS_DB4OTYPEIMPL);
			ICLASS_INTERNAL = reflector.ForClass(Const4.CLASS_INTERNAL);
			ICLASS_UNVERSIONED = reflector.ForClass(Const4.CLASS_UNVERSIONED);
			ICLASS_OBJECT = reflector.ForClass(Const4.CLASS_OBJECT);
			ICLASS_OBJECTCONTAINER = reflector.ForClass(Const4.CLASS_OBJECTCONTAINER);
			ICLASS_STATICCLASS = reflector.ForClass(Const4.CLASS_STATICCLASS);
			ICLASS_STRING = reflector.ForClass(typeof(string));
			ICLASS_TRANSIENTCLASS = reflector.ForClass(Const4.CLASS_TRANSIENTCLASS);
			Platform4.RegisterCollections(reflector);
		}

		internal void InitEncryption(Config4Impl a_config)
		{
			if (a_config.Encrypt() && a_config.Password() != null && a_config.Password().Length
				 > 0)
			{
				i_encrypt = true;
				i_encryptor = new byte[a_config.Password().Length];
				for (int i = 0; i < i_encryptor.Length; i++)
				{
					i_encryptor[i] = (byte)(a_config.Password()[i] & unchecked((int)(0xff)));
				}
				i_lastEncryptorByte = a_config.Password().Length - 1;
				return;
			}
			OldEncryptionOff();
		}

		internal static IDb4oTypeImpl GetDb4oType(IReflectClass clazz)
		{
			for (int i = 0; i < _db4oTypes.Length; i++)
			{
				if (clazz.IsInstance(_db4oTypes[i]))
				{
					return _db4oTypes[i];
				}
			}
			return null;
		}

		public ClassMetadata ClassMetadataForId(int id)
		{
			return (ClassMetadata)_classMetadata.Get(id);
		}

		internal ClassMetadata ClassMetadataForClass(IReflectClass clazz)
		{
			if (clazz == null)
			{
				return null;
			}
			if (clazz.IsArray())
			{
				if (_container.Reflector().Array().IsNDimensional(clazz))
				{
					return i_anyArrayN;
				}
				return i_anyArray;
			}
			return (ClassMetadata)_mapReflectorToHandler.Get(clazz);
		}

		public IReflectClass ClassReflectorForHandler(ITypeHandler4 handler)
		{
			return (IReflectClass)_mapHandlerToReflector.Get(handler);
		}

		public bool IsSecondClass(object a_object)
		{
			if (a_object != null)
			{
				IReflectClass claxx = _container.Reflector().ForObject(a_object);
				if (_mapReflectorToHandler.Get(claxx) != null)
				{
					return true;
				}
				return Platform4.IsValueType(claxx);
			}
			return false;
		}

		public bool IsSystemHandler(int id)
		{
			return id <= _highestBuiltinTypeID;
		}

		public void MigrationConnection(Db4objects.Db4o.Internal.Replication.MigrationConnection
			 mgc)
		{
			i_migration = mgc;
		}

		public Db4objects.Db4o.Internal.Replication.MigrationConnection MigrationConnection
			()
		{
			return i_migration;
		}

		public VirtualFieldMetadata VirtualFieldByName(string name)
		{
			for (int i = 0; i < _virtualFields.Length; i++)
			{
				if (name.Equals(_virtualFields[i].GetName()))
				{
					return _virtualFields[i];
				}
			}
			return null;
		}

		public bool IsVariableLength(ITypeHandler4 handler)
		{
			return handler is VariableLengthTypeHandler;
		}

		public SharedIndexedFields Indexes()
		{
			return _indexes;
		}

		public LatinStringIO StringIO()
		{
			return _stringIO;
		}

		public void StringIO(LatinStringIO io)
		{
			_stringIO = io;
		}
	}
}
