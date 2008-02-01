/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Internal.Fieldhandlers;
using Db4objects.Db4o.Internal.Handlers;
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
		public const byte HandlerVersion = (byte)2;

		private readonly ObjectContainerBase _container;

		private static readonly IDb4oTypeImpl[] _db4oTypes = new IDb4oTypeImpl[] { new BlobImpl
			() };

		private ClassMetadata _untypedArrayHandler;

		private ClassMetadata _untypedMultiDimensionalArrayHandler;

		private IFieldHandler _untypedFieldHandler;

		public StringHandler _stringHandler;

		private Hashtable4 _mapIdToTypeInfo = NewHashtable();

		private Hashtable4 _mapFieldHandlerToId = NewHashtable();

		private Hashtable4 _mapTypeHandlerToId = NewHashtable();

		private Hashtable4 _mapReflectorToClassMetadata = NewHashtable();

		private int _highestBuiltinTypeID = Handlers4.AnyArrayNId + 1;

		private const int Primitivecount = 8;

		private readonly VirtualFieldMetadata[] _virtualFields = new VirtualFieldMetadata
			[2];

		private readonly Hashtable4 _mapReflectorToFieldHandler = NewHashtable();

		private readonly Hashtable4 _mapReflectorToTypeHandler = NewHashtable();

		private readonly Hashtable4 _mapFieldHandlerToReflector = NewHashtable();

		private SharedIndexedFields _indexes;

		internal Db4objects.Db4o.Internal.Replication.MigrationConnection i_migration;

		internal IDb4oReplicationReferenceProvider _replicationReferenceProvider;

		public readonly DiagnosticProcessor _diagnosticProcessor;

		public bool i_encrypt;

		internal byte[] i_encryptor;

		internal int i_lastEncryptorByte;

		internal readonly GenericReflector _reflector;

		private readonly Hashtable4 _handlerVersions = NewHashtable();

		private LatinStringIO _stringIO;

		public IReflectClass IclassCompare;

		internal IReflectClass IclassDb4otype;

		internal IReflectClass IclassDb4otypeimpl;

		public IReflectClass IclassInternal;

		internal IReflectClass IclassUnversioned;

		public IReflectClass IclassObject;

		internal IReflectClass IclassObjectcontainer;

		public IReflectClass IclassStaticclass;

		public IReflectClass IclassString;

		internal IReflectClass IclassTransientclass;

		internal HandlerRegistry(ObjectContainerBase container, byte stringEncoding, GenericReflector
			 reflector)
		{
			// this is the master container and not valid
			// for TransportObjectContainer
			// see comment in classReflectorForHandler
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
			Platform4.RegisterPlatformHandlers(container);
		}

		private void InitArrayHandlers()
		{
			ITypeHandler4 handler = (ITypeHandler4)FieldHandlerForId(Handlers4.UntypedId);
			_untypedArrayHandler = new PrimitiveFieldHandler(Container(), new ArrayHandler(Container
				(), handler, false), Handlers4.AnyArrayId, IclassObject);
			MapTypeInfo(Handlers4.AnyArrayId, _untypedArrayHandler, new UntypedArrayFieldHandler
				(), _untypedArrayHandler, null);
			_untypedMultiDimensionalArrayHandler = new PrimitiveFieldHandler(Container(), new 
				MultidimensionalArrayHandler(Container(), handler, false), Handlers4.AnyArrayNId
				, IclassObject);
			MapTypeInfo(Handlers4.AnyArrayNId, _untypedMultiDimensionalArrayHandler, new UntypedMultidimensionalArrayFieldHandler
				(), _untypedMultiDimensionalArrayHandler, null);
		}

		private void RegisterPlatformTypes()
		{
			NetTypeHandler[] handlers = Platform4.Types(Container());
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
			IntHandler intHandler = new IntHandler(Container());
			RegisterBuiltinHandler(Handlers4.IntId, intHandler);
			RegisterHandlerVersion(intHandler, 0, new IntHandler0(Container()));
			LongHandler longHandler = new LongHandler(Container());
			RegisterBuiltinHandler(Handlers4.LongId, longHandler);
			RegisterHandlerVersion(longHandler, 0, new LongHandler0(Container()));
			FloatHandler floatHandler = new FloatHandler(Container());
			RegisterBuiltinHandler(Handlers4.FloatId, floatHandler);
			RegisterHandlerVersion(floatHandler, 0, new FloatHandler0(Container()));
			BooleanHandler booleanHandler = new BooleanHandler(Container());
			RegisterBuiltinHandler(Handlers4.BooleanId, booleanHandler);
			// TODO: Are we missing a boolean handler version?
			DoubleHandler doubleHandler = new DoubleHandler(Container());
			RegisterBuiltinHandler(Handlers4.DoubleId, doubleHandler);
			RegisterHandlerVersion(doubleHandler, 0, new DoubleHandler0(Container()));
			ByteHandler byteHandler = new ByteHandler(Container());
			RegisterBuiltinHandler(Handlers4.ByteId, byteHandler);
			// TODO: Are we missing a byte handler version?
			CharHandler charHandler = new CharHandler(Container());
			RegisterBuiltinHandler(Handlers4.CharId, charHandler);
			// TODO: Are we missing a char handler version?
			ShortHandler shortHandler = new ShortHandler(Container());
			RegisterBuiltinHandler(Handlers4.ShortId, shortHandler);
			RegisterHandlerVersion(shortHandler, 0, new ShortHandler0(Container()));
			_stringHandler = new StringHandler(Container());
			RegisterBuiltinHandler(Handlers4.StringId, _stringHandler);
			RegisterHandlerVersion(_stringHandler, 0, new StringHandler0(_stringHandler));
			DateHandler dateHandler = new DateHandler(Container());
			RegisterBuiltinHandler(Handlers4.DateId, dateHandler);
			RegisterHandlerVersion(dateHandler, 0, new DateHandler0(Container()));
			RegisterUntypedHandlers();
		}

		private void RegisterUntypedHandlers()
		{
			int id = Handlers4.UntypedId;
			_untypedFieldHandler = new Db4objects.Db4o.Internal.UntypedFieldHandler(Container
				());
			PrimitiveFieldHandler classMetadata = new PrimitiveFieldHandler(Container(), (ITypeHandler4
				)_untypedFieldHandler, id, IclassObject);
			Map(id, classMetadata, _untypedFieldHandler, new PlainObjectHandler(), IclassObject
				);
			RegisterHandlerVersion(_untypedFieldHandler, 0, new UntypedFieldHandler0(Container
				()));
		}

		private void RegisterBuiltinHandler(int id, IBuiltinTypeHandler handler)
		{
			RegisterBuiltinHandler(id, handler, true, handler.ClassReflector().GetName(), null
				);
		}

		private void RegisterBuiltinHandler(int id, IBuiltinTypeHandler typeHandler, bool
			 registerPrimitiveClass, string primitiveName, IGenericConverter converter)
		{
			if (registerPrimitiveClass)
			{
				_reflector.RegisterPrimitiveClass(id, primitiveName, converter);
			}
			IReflectClass classReflector = typeHandler.ClassReflector();
			PrimitiveFieldHandler classMetadata = new PrimitiveFieldHandler(Container(), typeHandler
				, id, classReflector);
			Map(id, classMetadata, typeHandler, typeHandler, classReflector);
		}

		private void Map(int id, ClassMetadata classMetadata, IFieldHandler fieldHandler, 
			ITypeHandler4 typeHandler, IReflectClass classReflector)
		{
			// TODO: remove when _mapIdToClassMetadata is gone 
			MapTypeInfo(id, classMetadata, fieldHandler, typeHandler, classReflector);
			MapPrimitive(id, classMetadata, fieldHandler, typeHandler, classReflector);
			if (id > _highestBuiltinTypeID)
			{
				_highestBuiltinTypeID = id;
			}
		}

		private void MapTypeInfo(int id, ClassMetadata classMetadata, IFieldHandler fieldHandler
			, ITypeHandler4 typeHandler, IReflectClass classReflector)
		{
			_mapIdToTypeInfo.Put(id, new TypeInfo(classMetadata, fieldHandler, typeHandler, classReflector
				));
		}

		private void MapPrimitive(int id, ClassMetadata classMetadata, IFieldHandler fieldHandler
			, ITypeHandler4 typeHandler, IReflectClass classReflector)
		{
			_mapFieldHandlerToReflector.Put(fieldHandler, classReflector);
			MapFieldHandler(classReflector, fieldHandler);
			_mapReflectorToTypeHandler.Put(classReflector, typeHandler);
			if (classReflector != null)
			{
				_mapReflectorToClassMetadata.Put(classReflector, classMetadata);
			}
			if (id != 0)
			{
				int wrappedID = id;
				_mapFieldHandlerToId.Put(fieldHandler, wrappedID);
				_mapTypeHandlerToId.Put(typeHandler, wrappedID);
			}
		}

		public void MapFieldHandler(IReflectClass classReflector, IFieldHandler fieldHandler
			)
		{
			_mapReflectorToFieldHandler.Put(classReflector, fieldHandler);
		}

		private void RegisterHandlerVersion(IFieldHandler handler, int version, ITypeHandler4
			 replacement)
		{
			_handlerVersions.Put(new HandlerVersionKey(handler, version), replacement);
		}

		public ITypeHandler4 CorrectHandlerVersion(ITypeHandler4 handler, int version)
		{
			if (version == Db4objects.Db4o.Internal.HandlerRegistry.HandlerVersion)
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

		internal int ArrayType(object obj)
		{
			IReflectClass claxx = Reflector().ForObject(obj);
			if (!claxx.IsArray())
			{
				return 0;
			}
			if (Reflector().Array().IsNDimensional(claxx))
			{
				return Const4.TypeNarray;
			}
			return Const4.TypeArray;
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
				if (claxx.SkipConstructor(skipConstructor, Container().Config().TestConstructors(
					)))
				{
					return true;
				}
			}
			if (!Container().ConfigImpl().TestConstructors())
			{
				return true;
			}
			if (claxx.NewInstance() != null)
			{
				return true;
			}
			if (Reflector().ConstructorCallsSupported())
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
			for (int k = 1; k <= Primitivecount; k++)
			{
				PrimitiveHandler handler = (PrimitiveHandler)TypeHandlerForID(k);
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
			// sort constructors by parameter count
			for (int i = 0; i < constructors.Length; i++)
			{
				constructors[i].SetAccessible();
				int parameterCount = constructors[i].GetParameterTypes().Length;
				sortedConstructors = Tree.Add(sortedConstructors, new TreeIntObject(i + constructors
					.Length * parameterCount, constructors[i]));
			}
			return sortedConstructors;
		}

		public void Decrypt(ByteArrayBuffer reader)
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

		public void Encrypt(ByteArrayBuffer reader)
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
			Container().ConfigImpl().OldEncryptionOff();
		}

		public IReflectClass ClassForID(int id)
		{
			TypeInfo typeInfo = TypeInfoForID(id);
			if (typeInfo == null)
			{
				return null;
			}
			return typeInfo.classReflector;
		}

		public ITypeHandler4 TypeHandlerForID(int id)
		{
			TypeInfo typeInfo = TypeInfoForID(id);
			if (typeInfo == null)
			{
				return null;
			}
			return typeInfo.typeHandler;
		}

		private TypeInfo TypeInfoForID(int id)
		{
			return (TypeInfo)_mapIdToTypeInfo.Get(id);
		}

		public int TypeHandlerID(ITypeHandler4 handler)
		{
			if (handler is ClassMetadata)
			{
				return ((ClassMetadata)handler).GetID();
			}
			object idAsInt = _mapTypeHandlerToId.Get(handler);
			if (idAsInt == null)
			{
				return 0;
			}
			return ((int)idAsInt);
		}

		private void InitClassReflectors(GenericReflector reflector)
		{
			IclassCompare = reflector.ForClass(Const4.ClassCompare);
			IclassDb4otype = reflector.ForClass(Const4.ClassDb4otype);
			IclassDb4otypeimpl = reflector.ForClass(Const4.ClassDb4otypeimpl);
			IclassInternal = reflector.ForClass(Const4.ClassInternal);
			IclassUnversioned = reflector.ForClass(Const4.ClassUnversioned);
			IclassObject = reflector.ForClass(Const4.ClassObject);
			IclassObjectcontainer = reflector.ForClass(Const4.ClassObjectcontainer);
			IclassStaticclass = reflector.ForClass(Const4.ClassStaticclass);
			IclassString = reflector.ForClass(typeof(string));
			IclassTransientclass = reflector.ForClass(Const4.ClassTransientclass);
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
			TypeInfo typeInfo = TypeInfoForID(id);
			if (typeInfo == null)
			{
				return null;
			}
			return typeInfo.classMetadata;
		}

		public IFieldHandler FieldHandlerForId(int id)
		{
			TypeInfo typeInfo = TypeInfoForID(id);
			if (typeInfo == null)
			{
				return null;
			}
			return typeInfo.fieldHandler;
		}

		public IFieldHandler FieldHandlerForClass(IReflectClass clazz)
		{
			// TODO: maybe need special handling for arrays here?
			if (clazz == null)
			{
				return null;
			}
			if (clazz.IsInterface())
			{
				return UntypedFieldHandler();
			}
			if (clazz.IsArray())
			{
				if (Reflector().Array().IsNDimensional(clazz))
				{
					return _untypedMultiDimensionalArrayHandler;
				}
				return _untypedArrayHandler;
			}
			IFieldHandler fieldHandler = (IFieldHandler)_mapReflectorToFieldHandler.Get(clazz
				);
			if (fieldHandler != null)
			{
				return fieldHandler;
			}
			ITypeHandler4 configuredHandler = Container().ConfigImpl().TypeHandlerForClass(clazz
				, Db4objects.Db4o.Internal.HandlerRegistry.HandlerVersion);
			if (configuredHandler != null)
			{
				MapFieldHandler(clazz, configuredHandler);
				return configuredHandler;
			}
			return null;
		}

		internal ClassMetadata ClassMetadataForClass(IReflectClass clazz)
		{
			if (clazz == null)
			{
				return null;
			}
			if (clazz.IsArray())
			{
				return (ClassMetadata)UntypedArrayHandler(clazz);
			}
			return (ClassMetadata)_mapReflectorToClassMetadata.Get(clazz);
		}

		public IFieldHandler UntypedFieldHandler()
		{
			return _untypedFieldHandler;
		}

		public ITypeHandler4 UntypedArrayHandler(IReflectClass clazz)
		{
			if (clazz.IsArray())
			{
				if (Reflector().Array().IsNDimensional(clazz))
				{
					return _untypedMultiDimensionalArrayHandler;
				}
				return _untypedArrayHandler;
			}
			return null;
		}

		public ITypeHandler4 TypeHandlerForClass(IReflectClass clazz)
		{
			if (clazz == null)
			{
				return null;
			}
			return (ITypeHandler4)_mapReflectorToTypeHandler.Get(clazz);
		}

		public IReflectClass ClassReflectorForHandler(ITypeHandler4 handler)
		{
			// This method never gets called from test cases so far.
			// It is written for the usecase of custom Typehandlers and
			// it is only require for arrays.
			// The methodology is highly problematic since it implies that 
			// one Typehandler can only be used for one ReflectClass.
			return (IReflectClass)_mapFieldHandlerToReflector.Get(handler);
		}

		public bool IsSecondClass(object a_object)
		{
			if (a_object != null)
			{
				IReflectClass claxx = Reflector().ForObject(a_object);
				if (_mapReflectorToFieldHandler.Get(claxx) != null)
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
			return handler is IVariableLengthTypeHandler;
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

		private GenericReflector Reflector()
		{
			return Container().Reflector();
		}

		private ObjectContainerBase Container()
		{
			return _container;
		}

		private static Hashtable4 NewHashtable()
		{
			return new Hashtable4(32);
		}

		public int FieldHandlerIdForFieldHandler(IFieldHandler fieldHandler)
		{
			int wrappedId = (int)_mapFieldHandlerToId.Get(fieldHandler);
			if (wrappedId != null)
			{
				return wrappedId;
			}
			return 0;
		}
	}
}
