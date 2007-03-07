namespace Db4objects.Db4o.Reflect.Generic
{
	public class KnownClassesRepository
	{
		private static readonly Db4objects.Db4o.Foundation.Hashtable4 PRIMITIVES;

		static KnownClassesRepository()
		{
			PRIMITIVES = new Db4objects.Db4o.Foundation.Hashtable4();
			RegisterPrimitive(typeof(bool), typeof(bool));
			RegisterPrimitive(typeof(byte), typeof(byte));
			RegisterPrimitive(typeof(short), typeof(short));
			RegisterPrimitive(typeof(char), typeof(char));
			RegisterPrimitive(typeof(int), typeof(int));
			RegisterPrimitive(typeof(long), typeof(long));
			RegisterPrimitive(typeof(float), typeof(float));
			RegisterPrimitive(typeof(double), typeof(double));
		}

		private static void RegisterPrimitive(System.Type wrapper, System.Type primitive)
		{
			PRIMITIVES.Put(wrapper.FullName, primitive);
		}

		private Db4objects.Db4o.Internal.ObjectContainerBase _stream;

		private Db4objects.Db4o.Internal.Transaction _trans;

		private Db4objects.Db4o.Reflect.Generic.IReflectClassBuilder _builder;

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _classByName = new Db4objects.Db4o.Foundation.Hashtable4
			();

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _classByID = new Db4objects.Db4o.Foundation.Hashtable4
			();

		private Db4objects.Db4o.Foundation.Collection4 _pendingClasses = new Db4objects.Db4o.Foundation.Collection4
			();

		private readonly Db4objects.Db4o.Foundation.Collection4 _classes = new Db4objects.Db4o.Foundation.Collection4
			();

		public KnownClassesRepository(Db4objects.Db4o.Reflect.Generic.IReflectClassBuilder
			 builder)
		{
			_builder = builder;
		}

		public virtual void SetTransaction(Db4objects.Db4o.Internal.Transaction trans)
		{
			if (trans != null)
			{
				_trans = trans;
				_stream = trans.Stream();
			}
		}

		public virtual void Register(Db4objects.Db4o.Reflect.IReflectClass clazz)
		{
			_classByName.Put(clazz.GetName(), clazz);
			_classes.Add(clazz);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForID(int id)
		{
			if (_stream.Handlers().IsSystemHandler(id))
			{
				return _stream.HandlerByID(id).ClassReflector();
			}
			EnsureClassAvailability(id);
			return LookupByID(id);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForName(string className)
		{
			Db4objects.Db4o.Reflect.IReflectClass clazz = (Db4objects.Db4o.Reflect.IReflectClass
				)_classByName.Get(className);
			if (clazz != null)
			{
				return clazz;
			}
			if (_stream == null)
			{
				return null;
			}
			if (_stream.ClassCollection() != null)
			{
				int classID = _stream.ClassCollection().GetYapClassID(className);
				if (classID > 0)
				{
					clazz = EnsureClassInitialised(classID);
					_classByName.Put(className, clazz);
					return clazz;
				}
			}
			return null;
		}

		private void ReadAll()
		{
			for (System.Collections.IEnumerator idIter = _stream.ClassCollection().Ids(); idIter
				.MoveNext(); )
			{
				EnsureClassAvailability(((int)idIter.Current));
			}
			for (System.Collections.IEnumerator idIter = _stream.ClassCollection().Ids(); idIter
				.MoveNext(); )
			{
				EnsureClassRead(((int)idIter.Current));
			}
		}

		private Db4objects.Db4o.Reflect.IReflectClass EnsureClassAvailability(int id)
		{
			if (id == 0)
			{
				return null;
			}
			Db4objects.Db4o.Reflect.IReflectClass ret = (Db4objects.Db4o.Reflect.IReflectClass
				)_classByID.Get(id);
			if (ret != null)
			{
				return ret;
			}
			Db4objects.Db4o.Internal.Buffer classreader = _stream.ReadWriterByID(_trans, id);
			Db4objects.Db4o.Internal.Marshall.ClassMarshaller marshaller = MarshallerFamily()
				._class;
			Db4objects.Db4o.Internal.Marshall.RawClassSpec spec = marshaller.ReadSpec(_trans, 
				classreader);
			string className = spec.Name();
			ret = (Db4objects.Db4o.Reflect.IReflectClass)_classByName.Get(className);
			if (ret != null)
			{
				_classByID.Put(id, ret);
				_pendingClasses.Add(id);
				return ret;
			}
			ret = _builder.CreateClass(className, EnsureClassAvailability(spec.SuperClassID()
				), spec.NumFields());
			_classByID.Put(id, ret);
			_pendingClasses.Add(id);
			return ret;
		}

		private void EnsureClassRead(int id)
		{
			Db4objects.Db4o.Reflect.IReflectClass clazz = LookupByID(id);
			Db4objects.Db4o.Internal.Buffer classreader = _stream.ReadWriterByID(_trans, id);
			Db4objects.Db4o.Internal.Marshall.ClassMarshaller classMarshaller = MarshallerFamily
				()._class;
			Db4objects.Db4o.Internal.Marshall.RawClassSpec classInfo = classMarshaller.ReadSpec
				(_trans, classreader);
			string className = classInfo.Name();
			if (_classByName.Get(className) != null)
			{
				return;
			}
			_classByName.Put(className, clazz);
			_classes.Add(clazz);
			int numFields = classInfo.NumFields();
			Db4objects.Db4o.Reflect.IReflectField[] fields = _builder.FieldArray(numFields);
			Db4objects.Db4o.Internal.Marshall.IFieldMarshaller fieldMarshaller = MarshallerFamily
				()._field;
			for (int i = 0; i < numFields; i++)
			{
				Db4objects.Db4o.Internal.Marshall.RawFieldSpec fieldInfo = fieldMarshaller.ReadSpec
					(_stream, classreader);
				string fieldName = fieldInfo.Name();
				Db4objects.Db4o.Reflect.IReflectClass fieldClass = ReflectClassForFieldSpec(fieldInfo
					);
				fields[i] = _builder.CreateField(clazz, fieldName, fieldClass, fieldInfo.IsVirtual
					(), fieldInfo.IsPrimitive(), fieldInfo.IsArray(), fieldInfo.IsNArray());
			}
			_builder.InitFields(clazz, fields);
		}

		private Db4objects.Db4o.Reflect.IReflectClass ReflectClassForFieldSpec(Db4objects.Db4o.Internal.Marshall.RawFieldSpec
			 fieldInfo)
		{
			if (fieldInfo.IsVirtual())
			{
				Db4objects.Db4o.Internal.VirtualFieldMetadata fieldMeta = _stream.Handlers().VirtualFieldByName
					(fieldInfo.Name());
				return fieldMeta.GetHandler().ClassReflector();
			}
			int handlerID = fieldInfo.HandlerID();
			Db4objects.Db4o.Reflect.IReflectClass fieldClass = null;
			switch (handlerID)
			{
				case Db4objects.Db4o.Internal.HandlerRegistry.ANY_ID:
				{
					fieldClass = _stream.Reflector().ForClass(typeof(object));
					break;
				}

				case Db4objects.Db4o.Internal.HandlerRegistry.ANY_ARRAY_ID:
				{
					fieldClass = ArrayClass(_stream.Reflector().ForClass(typeof(object)));
					break;
				}

				default:
				{
					fieldClass = ForID(handlerID);
					fieldClass = _stream.Reflector().ForName(fieldClass.GetName());
					if (fieldInfo.IsPrimitive())
					{
						fieldClass = PrimitiveClass(fieldClass);
					}
					if (fieldInfo.IsArray())
					{
						fieldClass = ArrayClass(fieldClass);
					}
					break;
				}
			}
			return fieldClass;
		}

		private Db4objects.Db4o.Internal.Marshall.MarshallerFamily MarshallerFamily()
		{
			return Db4objects.Db4o.Internal.Marshall.MarshallerFamily.ForConverterVersion(_stream
				.ConverterVersion());
		}

		private Db4objects.Db4o.Reflect.IReflectClass EnsureClassInitialised(int id)
		{
			Db4objects.Db4o.Reflect.IReflectClass ret = EnsureClassAvailability(id);
			while (_pendingClasses.Size() > 0)
			{
				Db4objects.Db4o.Foundation.Collection4 pending = _pendingClasses;
				_pendingClasses = new Db4objects.Db4o.Foundation.Collection4();
				System.Collections.IEnumerator i = pending.GetEnumerator();
				while (i.MoveNext())
				{
					EnsureClassRead(((int)i.Current));
				}
			}
			return ret;
		}

		public virtual System.Collections.IEnumerator Classes()
		{
			ReadAll();
			return _classes.GetEnumerator();
		}

		public virtual void Register(int id, Db4objects.Db4o.Reflect.IReflectClass clazz)
		{
			_classByID.Put(id, clazz);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass LookupByID(int id)
		{
			return (Db4objects.Db4o.Reflect.IReflectClass)_classByID.Get(id);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass LookupByName(string name)
		{
			return (Db4objects.Db4o.Reflect.IReflectClass)_classByName.Get(name);
		}

		private Db4objects.Db4o.Reflect.IReflectClass ArrayClass(Db4objects.Db4o.Reflect.IReflectClass
			 clazz)
		{
			object proto = clazz.Reflector().Array().NewInstance(clazz, 0);
			return clazz.Reflector().ForObject(proto);
		}

		private Db4objects.Db4o.Reflect.IReflectClass PrimitiveClass(Db4objects.Db4o.Reflect.IReflectClass
			 baseClass)
		{
			System.Type primitive = (System.Type)PRIMITIVES.Get(baseClass.GetName());
			if (primitive != null)
			{
				return baseClass.Reflector().ForClass(primitive);
			}
			return baseClass;
		}
	}
}
