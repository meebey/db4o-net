/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Replication;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Types;
using Sharpen;

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
		private readonly ObjectContainerBase _masterStream;

		private static readonly IDb4oTypeImpl[] i_db4oTypes = new IDb4oTypeImpl[] { new BlobImpl
			() };

		public const int ANY_ARRAY_ID = 12;

		public const int ANY_ARRAY_N_ID = 13;

		private const int CLASSCOUNT = 11;

		private ClassMetadata i_anyArray;

		private ClassMetadata i_anyArrayN;

		public readonly StringHandler i_stringHandler;

		private ITypeHandler4[] i_handlers;

		private int i_maxTypeID = ANY_ARRAY_N_ID + 1;

		private NetTypeHandler[] i_platformTypes;

		private const int PRIMITIVECOUNT = 8;

		internal ClassMetadata[] i_yapClasses;

		private const int ANY_INDEX = 10;

		public const int ANY_ID = 11;

		private readonly VirtualFieldMetadata[] _virtualFields = new VirtualFieldMetadata
			[2];

		private readonly Hashtable4 i_classByClass = new Hashtable4(32);

		internal IDb4oCollections i_collections;

		internal SharedIndexedFields i_indexes;

		internal ReplicationImpl i_replication;

		internal Db4objects.Db4o.Internal.Replication.MigrationConnection i_migration;

		internal IDb4oReplicationReferenceProvider _replicationReferenceProvider;

		public readonly DiagnosticProcessor _diagnosticProcessor;

		public bool i_encrypt;

		internal byte[] i_encryptor;

		internal int i_lastEncryptorByte;

		internal readonly GenericReflector _reflector;

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

		internal HandlerRegistry(ObjectContainerBase a_stream, byte stringEncoding, GenericReflector
			 reflector)
		{
			_masterStream = a_stream;
			a_stream.i_handlers = this;
			_reflector = reflector;
			_diagnosticProcessor = a_stream.ConfigImpl().DiagnosticProcessor();
			InitClassReflectors(reflector);
			i_indexes = new SharedIndexedFields(a_stream);
			_virtualFields[0] = i_indexes.i_fieldVersion;
			_virtualFields[1] = i_indexes.i_fieldUUID;
			i_stringHandler = new StringHandler(a_stream, LatinStringIO.ForEncoding(stringEncoding
				));
			i_handlers = new ITypeHandler4[] { new IntHandler(a_stream), new LongHandler(a_stream
				), new FloatHandler(a_stream), new BooleanHandler(a_stream), new DoubleHandler(a_stream
				), new ByteHandler(a_stream), new CharHandler(a_stream), new ShortHandler(a_stream
				), i_stringHandler, new DateHandler(a_stream), new UntypedFieldHandler(a_stream)
				 };
			i_platformTypes = Platform4.Types(a_stream);
			if (i_platformTypes.Length > 0)
			{
				for (int i = 0; i < i_platformTypes.Length; i++)
				{
					i_platformTypes[i].Initialize();
					if (i_platformTypes[i].GetID() > i_maxTypeID)
					{
						i_maxTypeID = i_platformTypes[i].GetID();
					}
				}
				ITypeHandler4[] temp = i_handlers;
				i_handlers = new ITypeHandler4[i_maxTypeID];
				System.Array.Copy(temp, 0, i_handlers, 0, temp.Length);
				for (int i = 0; i < i_platformTypes.Length; i++)
				{
					int idx = i_platformTypes[i].GetID() - 1;
					i_handlers[idx] = i_platformTypes[i];
				}
			}
			i_yapClasses = new ClassMetadata[i_maxTypeID + 1];
			for (int i = 0; i < CLASSCOUNT; i++)
			{
				int id = i + 1;
				i_yapClasses[i] = new PrimitiveFieldHandler(a_stream, i_handlers[i]);
				i_yapClasses[i].SetID(id);
				i_classByClass.Put(i_handlers[i].ClassReflector(), i_yapClasses[i]);
				if (i < ANY_INDEX)
				{
					reflector.RegisterPrimitiveClass(id, i_handlers[i].ClassReflector().GetName(), null
						);
				}
			}
			for (int i = 0; i < i_platformTypes.Length; i++)
			{
				int id = i_platformTypes[i].GetID();
				int idx = id - 1;
				IGenericConverter converter = (i_platformTypes[i] is IGenericConverter) ? (IGenericConverter
					)i_platformTypes[i] : null;
				reflector.RegisterPrimitiveClass(id, i_platformTypes[i].GetName(), converter);
				i_handlers[idx] = i_platformTypes[i];
				i_yapClasses[idx] = new PrimitiveFieldHandler(a_stream, i_platformTypes[i]);
				i_yapClasses[idx].SetID(id);
				if (id > i_maxTypeID)
				{
					i_maxTypeID = idx;
				}
				i_classByClass.Put(i_platformTypes[i].ClassReflector(), i_yapClasses[idx]);
			}
			i_anyArray = new PrimitiveFieldHandler(a_stream, new ArrayHandler(_masterStream, 
				UntypedHandler(), false));
			i_anyArray.SetID(ANY_ARRAY_ID);
			i_yapClasses[ANY_ARRAY_ID - 1] = i_anyArray;
			i_anyArrayN = new PrimitiveFieldHandler(a_stream, new MultidimensionalArrayHandler
				(_masterStream, UntypedHandler(), false));
			i_anyArrayN.SetID(ANY_ARRAY_N_ID);
			i_yapClasses[ANY_ARRAY_N_ID - 1] = i_anyArrayN;
		}

		internal int ArrayType(object a_object)
		{
			IReflectClass claxx = _masterStream.Reflector().ForObject(a_object);
			if (!claxx.IsArray())
			{
				return 0;
			}
			if (_masterStream.Reflector().Array().IsNDimensional(claxx))
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
				if (claxx.SkipConstructor(skipConstructor))
				{
					return true;
				}
			}
			if (!_masterStream.ConfigImpl().TestConstructors())
			{
				return true;
			}
			if (claxx.NewInstance() != null)
			{
				return true;
			}
			if (_masterStream.Reflector().ConstructorCallsSupported())
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
			for (int k = 0; k < PRIMITIVECOUNT; k++)
			{
				if (clazz.Equals(i_handlers[k].PrimitiveClassReflector()))
				{
					return ((PrimitiveHandler)i_handlers[k]).PrimitiveNull();
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

		public void Decrypt(Db4objects.Db4o.Internal.Buffer reader)
		{
			if (i_encrypt)
			{
				int encryptorOffSet = i_lastEncryptorByte;
				byte[] bytes = reader._buffer;
				for (int i = reader.GetLength() - 1; i >= 0; i--)
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

		public void Encrypt(Db4objects.Db4o.Internal.Buffer reader)
		{
			if (i_encrypt)
			{
				byte[] bytes = reader._buffer;
				int encryptorOffSet = i_lastEncryptorByte;
				for (int i = reader.GetLength() - 1; i >= 0; i--)
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
			_masterStream.ConfigImpl().OldEncryptionOff();
		}

		internal ITypeHandler4 GetHandler(int a_index)
		{
			return i_handlers[a_index - 1];
		}

		internal ITypeHandler4 HandlerForClass(IReflectClass a_class, IReflectClass[] a_Supported
			)
		{
			for (int i = 0; i < a_Supported.Length; i++)
			{
				if (a_Supported[i].Equals(a_class))
				{
					return i_handlers[i];
				}
			}
			return null;
		}

		public ITypeHandler4 HandlerForClass(ObjectContainerBase a_stream, IReflectClass 
			a_class)
		{
			if (a_class == null)
			{
				return null;
			}
			if (a_class.IsArray())
			{
				return HandlerForClass(a_stream, a_class.GetComponentType());
			}
			ClassMetadata yc = GetYapClassStatic(a_class);
			if (yc != null)
			{
				return ((PrimitiveFieldHandler)yc).i_handler;
			}
			return a_stream.ProduceClassMetadata(a_class);
		}

		public ITypeHandler4 UntypedHandler()
		{
			return i_handlers[ANY_INDEX];
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
					i_encryptor[i] = (byte)(Sharpen.Runtime.GetCharAt(a_config.Password(), i) & unchecked(
						(int)(0xff)));
				}
				i_lastEncryptorByte = a_config.Password().Length - 1;
				return;
			}
			OldEncryptionOff();
		}

		internal static IDb4oTypeImpl GetDb4oType(IReflectClass clazz)
		{
			for (int i = 0; i < i_db4oTypes.Length; i++)
			{
				if (clazz.IsInstance(i_db4oTypes[i]))
				{
					return i_db4oTypes[i];
				}
			}
			return null;
		}

		public ClassMetadata GetYapClassStatic(int a_id)
		{
			if (a_id > 0 && a_id <= i_maxTypeID)
			{
				return i_yapClasses[a_id - 1];
			}
			return null;
		}

		internal ClassMetadata GetYapClassStatic(IReflectClass a_class)
		{
			if (a_class == null)
			{
				return null;
			}
			if (a_class.IsArray())
			{
				if (_masterStream.Reflector().Array().IsNDimensional(a_class))
				{
					return i_anyArrayN;
				}
				return i_anyArray;
			}
			return (ClassMetadata)i_classByClass.Get(a_class);
		}

		public bool IsSecondClass(object a_object)
		{
			if (a_object != null)
			{
				IReflectClass claxx = _masterStream.Reflector().ForObject(a_object);
				if (i_classByClass.Get(claxx) != null)
				{
					return true;
				}
				return Platform4.IsValueType(claxx);
			}
			return false;
		}

		public bool IsSystemHandler(int id)
		{
			return id <= i_maxTypeID;
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

		public void Replication(ReplicationImpl impl)
		{
			i_replication = impl;
		}

		public ReplicationImpl Replication()
		{
			return i_replication;
		}

		public ClassMetadata PrimitiveClassById(int id)
		{
			return i_yapClasses[id - 1];
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
	}
}
