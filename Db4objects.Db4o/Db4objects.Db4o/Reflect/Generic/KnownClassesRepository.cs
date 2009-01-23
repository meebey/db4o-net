/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class KnownClassesRepository
	{
		private static readonly Hashtable4 Primitives;

		static KnownClassesRepository()
		{
			// FIXME very java-centric, what about .NET?
			Primitives = new Hashtable4();
			RegisterPrimitive(typeof(bool), typeof(bool));
			RegisterPrimitive(typeof(byte), typeof(byte));
			RegisterPrimitive(typeof(short), typeof(short));
			RegisterPrimitive(typeof(char), typeof(char));
			RegisterPrimitive(typeof(int), typeof(int));
			RegisterPrimitive(typeof(long), typeof(long));
			RegisterPrimitive(typeof(float), typeof(float));
			RegisterPrimitive(typeof(double), typeof(double));
		}

		private static void RegisterPrimitive(Type wrapper, Type primitive)
		{
			Primitives.Put(wrapper.FullName, primitive);
		}

		private ObjectContainerBase _stream;

		private Transaction _trans;

		private IReflectClassBuilder _builder;

		private readonly ListenerRegistry _listeners = ListenerRegistry.NewInstance();

		private readonly Hashtable4 _classByName = new Hashtable4();

		private readonly Hashtable4 _classByID = new Hashtable4();

		private Collection4 _pendingClasses = new Collection4();

		private readonly Collection4 _classes = new Collection4();

		public KnownClassesRepository(IReflectClassBuilder builder)
		{
			_builder = builder;
		}

		public virtual void SetTransaction(Transaction trans)
		{
			if (trans != null)
			{
				_trans = trans;
				_stream = trans.Container();
			}
		}

		public virtual void Register(IReflectClass clazz)
		{
			Register(clazz.GetName(), clazz);
		}

		public virtual IReflectClass ForID(int id)
		{
			lock (_stream.Lock())
			{
				if (_stream.Handlers().IsSystemHandler(id))
				{
					return _stream.Handlers().ClassForID(id);
				}
				EnsureClassAvailability(id);
				return LookupByID(id);
			}
		}

		public virtual IReflectClass ForName(string className)
		{
			IReflectClass clazz = LookupByName(className);
			if (clazz != null)
			{
				return clazz;
			}
			if (_stream == null)
			{
				return null;
			}
			lock (_stream.Lock())
			{
				if (_stream.ClassCollection() == null)
				{
					return null;
				}
				int classID = _stream.ClassMetadataIdForName(className);
				if (classID <= 0)
				{
					return null;
				}
				return InitializeClass(classID, className);
			}
		}

		private IReflectClass InitializeClass(int classID, string className)
		{
			IReflectClass newClazz = EnsureClassInitialised(classID);
			_classByName.Put(className, newClazz);
			return newClazz;
		}

		private void ReadAll()
		{
			ForEachClassId(new _IProcedure4_103(this));
			ForEachClassId(new _IProcedure4_106(this));
		}

		private sealed class _IProcedure4_103 : IProcedure4
		{
			public _IProcedure4_103(KnownClassesRepository _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Apply(object id)
			{
				this._enclosing.EnsureClassAvailability((((int)id)));
			}

			private readonly KnownClassesRepository _enclosing;
		}

		private sealed class _IProcedure4_106 : IProcedure4
		{
			public _IProcedure4_106(KnownClassesRepository _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Apply(object id)
			{
				this._enclosing.EnsureClassRead((((int)id)));
			}

			private readonly KnownClassesRepository _enclosing;
		}

		private void ForEachClassId(IProcedure4 procedure)
		{
			for (IEnumerator ids = _stream.ClassCollection().Ids(); ids.MoveNext(); )
			{
				procedure.Apply((int)ids.Current);
			}
		}

		private IReflectClass EnsureClassAvailability(int id)
		{
			if (id == 0)
			{
				return null;
			}
			IReflectClass ret = (IReflectClass)_classByID.Get(id);
			if (ret != null)
			{
				return ret;
			}
			ByteArrayBuffer classreader = _stream.ReadWriterByID(_trans, id);
			ClassMarshaller marshaller = MarshallerFamily()._class;
			RawClassSpec spec = marshaller.ReadSpec(_trans, classreader);
			string className = spec.Name();
			ret = LookupByName(className);
			if (ret != null)
			{
				_classByID.Put(id, ret);
				_pendingClasses.Add(id);
				return ret;
			}
			ret = _builder.CreateClass(className, EnsureClassAvailability(spec.SuperClassID()
				), spec.NumFields());
			// step 1 only add to _classByID, keep the class out of _classByName and _classes
			_classByID.Put(id, ret);
			_pendingClasses.Add(id);
			return ret;
		}

		private void EnsureClassRead(int id)
		{
			IReflectClass clazz = LookupByID(id);
			ByteArrayBuffer classreader = _stream.ReadWriterByID(_trans, id);
			ClassMarshaller classMarshaller = MarshallerFamily()._class;
			RawClassSpec classInfo = classMarshaller.ReadSpec(_trans, classreader);
			string className = classInfo.Name();
			// Having the class in the _classByName Map for now indicates
			// that the class is fully read. This is breakable if we start
			// returning GenericClass'es in other methods like forName
			// even if a native class has not been found
			if (LookupByName(className) != null)
			{
				return;
			}
			// step 2 add the class to _classByName and _classes to denote reading is completed
			Register(className, clazz);
			int numFields = classInfo.NumFields();
			IReflectField[] fields = _builder.FieldArray(numFields);
			IFieldMarshaller fieldMarshaller = MarshallerFamily()._field;
			for (int i = 0; i < numFields; i++)
			{
				RawFieldSpec fieldInfo = fieldMarshaller.ReadSpec(_stream, classreader);
				string fieldName = fieldInfo.Name();
				IReflectClass fieldClass = ReflectClassForFieldSpec(fieldInfo, _stream.Reflector(
					));
				fields[i] = _builder.CreateField(clazz, fieldName, fieldClass, fieldInfo.IsVirtual
					(), fieldInfo.IsPrimitive(), fieldInfo.IsArray(), fieldInfo.IsNArray());
			}
			_builder.InitFields(clazz, fields);
		}

		private void Register(string className, IReflectClass clazz)
		{
			if (LookupByName(className) != null)
			{
				throw new ArgumentException();
			}
			_classByName.Put(className, clazz);
			_classes.Add(clazz);
			_listeners.NotifyListeners(clazz);
		}

		private IReflectClass ReflectClassForFieldSpec(RawFieldSpec fieldInfo, IReflector
			 reflector)
		{
			if (fieldInfo.IsVirtualField())
			{
				return VirtualFieldByName(fieldInfo.Name()).ClassReflector(reflector);
			}
			int handlerID = fieldInfo.HandlerID();
			switch (handlerID)
			{
				case Handlers4.UntypedId:
				{
					// need to take care of special handlers here
					return ObjectClass();
				}

				case Handlers4.AnyArrayId:
				{
					return ArrayClass(ObjectClass());
				}

				default:
				{
					IReflectClass fieldClass = ForID(handlerID);
					if (null != fieldClass)
					{
						return NormalizeFieldClass(fieldInfo, fieldClass);
					}
					break;
					break;
				}
			}
			return null;
		}

		private IReflectClass NormalizeFieldClass(RawFieldSpec fieldInfo, IReflectClass fieldClass
			)
		{
			// TODO: why the following line is necessary?
			IReflectClass theClass = _stream.Reflector().ForName(fieldClass.GetName());
			if (fieldInfo.IsPrimitive())
			{
				theClass = PrimitiveClass(theClass);
			}
			if (fieldInfo.IsArray())
			{
				theClass = ArrayClass(theClass);
			}
			return theClass;
		}

		private IReflectClass ObjectClass()
		{
			return _stream.Reflector().ForClass(typeof(object));
		}

		private VirtualFieldMetadata VirtualFieldByName(string fieldName)
		{
			return _stream.Handlers().VirtualFieldByName(fieldName);
		}

		private Db4objects.Db4o.Internal.Marshall.MarshallerFamily MarshallerFamily()
		{
			return Db4objects.Db4o.Internal.Marshall.MarshallerFamily.ForConverterVersion(_stream
				.ConverterVersion());
		}

		private IReflectClass EnsureClassInitialised(int id)
		{
			IReflectClass ret = EnsureClassAvailability(id);
			while (_pendingClasses.Size() > 0)
			{
				Collection4 pending = _pendingClasses;
				_pendingClasses = new Collection4();
				IEnumerator i = pending.GetEnumerator();
				while (i.MoveNext())
				{
					EnsureClassRead(((int)i.Current));
				}
			}
			return ret;
		}

		public virtual IEnumerator Classes()
		{
			ReadAll();
			return _classes.GetEnumerator();
		}

		public virtual void Register(int id, IReflectClass clazz)
		{
			_classByID.Put(id, clazz);
		}

		public virtual IReflectClass LookupByID(int id)
		{
			return (IReflectClass)_classByID.Get(id);
		}

		public virtual IReflectClass LookupByName(string name)
		{
			return (IReflectClass)_classByName.Get(name);
		}

		private IReflectClass ArrayClass(IReflectClass clazz)
		{
			object proto = clazz.Reflector().Array().NewInstance(clazz, 0);
			return clazz.Reflector().ForObject(proto);
		}

		private IReflectClass PrimitiveClass(IReflectClass baseClass)
		{
			Type primitive = (Type)Primitives.Get(baseClass.GetName());
			if (primitive != null)
			{
				return baseClass.Reflector().ForClass(primitive);
			}
			return baseClass;
		}

		public virtual void AddListener(IListener4 listener)
		{
			_listeners.Register(listener);
		}

		public virtual void RemoveListener(IListener4 listener)
		{
			_listeners.Remove(listener);
		}
	}
}
