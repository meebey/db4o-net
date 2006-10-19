namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericReflector : Db4objects.Db4o.Reflect.IReflector, Db4objects.Db4o.Foundation.IDeepClone
	{
		private Db4objects.Db4o.Reflect.IReflector _delegate;

		private Db4objects.Db4o.Reflect.Generic.GenericArrayReflector _array;

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _classByName = new Db4objects.Db4o.Foundation.Hashtable4
			();

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _classByClass = new Db4objects.Db4o.Foundation.Hashtable4
			();

		private readonly Db4objects.Db4o.Foundation.Collection4 _classes = new Db4objects.Db4o.Foundation.Collection4
			();

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _classByID = new Db4objects.Db4o.Foundation.Hashtable4
			();

		private Db4objects.Db4o.Foundation.Collection4 _collectionPredicates = new Db4objects.Db4o.Foundation.Collection4
			();

		private Db4objects.Db4o.Foundation.Collection4 _collectionUpdateDepths = new Db4objects.Db4o.Foundation.Collection4
			();

		private Db4objects.Db4o.Foundation.Collection4 _pendingClasses = new Db4objects.Db4o.Foundation.Collection4
			();

		private Db4objects.Db4o.Transaction _trans;

		private Db4objects.Db4o.YapStream _stream;

		public GenericReflector(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Reflect.IReflector
			 delegateReflector)
		{
			SetTransaction(trans);
			_delegate = delegateReflector;
			if (_delegate != null)
			{
				_delegate.SetParent(this);
			}
		}

		public virtual object DeepClone(object obj)
		{
			Db4objects.Db4o.Reflect.Generic.GenericReflector myClone = new Db4objects.Db4o.Reflect.Generic.GenericReflector
				(null, (Db4objects.Db4o.Reflect.IReflector)_delegate.DeepClone(this));
			myClone._collectionPredicates = (Db4objects.Db4o.Foundation.Collection4)_collectionPredicates
				.DeepClone(myClone);
			myClone._collectionUpdateDepths = (Db4objects.Db4o.Foundation.Collection4)_collectionUpdateDepths
				.DeepClone(myClone);
			return myClone;
		}

		internal virtual Db4objects.Db4o.YapStream GetStream()
		{
			return _stream;
		}

		public virtual bool HasTransaction()
		{
			return _trans != null;
		}

		public virtual void SetTransaction(Db4objects.Db4o.Transaction trans)
		{
			if (trans != null)
			{
				_trans = trans;
				_stream = trans.Stream();
			}
		}

		public virtual Db4objects.Db4o.Reflect.IReflectArray Array()
		{
			if (_array == null)
			{
				_array = new Db4objects.Db4o.Reflect.Generic.GenericArrayReflector(this);
			}
			return _array;
		}

		public virtual int CollectionUpdateDepth(Db4objects.Db4o.Reflect.IReflectClass candidate
			)
		{
			System.Collections.IEnumerator i = _collectionUpdateDepths.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Reflect.Generic.CollectionUpdateDepthEntry entry = (Db4objects.Db4o.Reflect.Generic.CollectionUpdateDepthEntry
					)i.Current;
				if (entry._predicate.Match(candidate))
				{
					return entry._depth;
				}
			}
			return 2;
		}

		public virtual bool ConstructorCallsSupported()
		{
			return _delegate.ConstructorCallsSupported();
		}

		internal virtual Db4objects.Db4o.Reflect.Generic.GenericClass EnsureDelegate(Db4objects.Db4o.Reflect.IReflectClass
			 clazz)
		{
			if (clazz == null)
			{
				return null;
			}
			Db4objects.Db4o.Reflect.Generic.GenericClass claxx = (Db4objects.Db4o.Reflect.Generic.GenericClass
				)_classByName.Get(clazz.GetName());
			if (claxx == null)
			{
				string name = clazz.GetName();
				claxx = new Db4objects.Db4o.Reflect.Generic.GenericClass(this, clazz, name, null);
				_classes.Add(claxx);
				_classByName.Put(name, claxx);
			}
			return claxx;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForClass(System.Type clazz)
		{
			if (clazz == null)
			{
				return null;
			}
			Db4objects.Db4o.Reflect.IReflectClass claxx = (Db4objects.Db4o.Reflect.IReflectClass
				)_classByClass.Get(clazz);
			if (claxx != null)
			{
				return claxx;
			}
			claxx = ForName(clazz.FullName);
			if (claxx != null)
			{
				_classByClass.Put(clazz, claxx);
				return claxx;
			}
			claxx = _delegate.ForClass(clazz);
			if (claxx == null)
			{
				return null;
			}
			claxx = EnsureDelegate(claxx);
			_classByClass.Put(clazz, claxx);
			return claxx;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForName(string className)
		{
			Db4objects.Db4o.Reflect.IReflectClass clazz = (Db4objects.Db4o.Reflect.IReflectClass
				)_classByName.Get(className);
			if (clazz != null)
			{
				return clazz;
			}
			clazz = _delegate.ForName(className);
			if (clazz != null)
			{
				return EnsureDelegate(clazz);
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

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForObject(object obj)
		{
			if (obj is Db4objects.Db4o.Reflect.Generic.GenericObject)
			{
				return ((Db4objects.Db4o.Reflect.Generic.GenericObject)obj)._class;
			}
			return _delegate.ForObject(obj);
		}

		public virtual Db4objects.Db4o.Reflect.IReflector GetDelegate()
		{
			return _delegate;
		}

		public virtual bool IsCollection(Db4objects.Db4o.Reflect.IReflectClass candidate)
		{
			System.Collections.IEnumerator i = _collectionPredicates.GetEnumerator();
			while (i.MoveNext())
			{
				if (((Db4objects.Db4o.Reflect.IReflectClassPredicate)i.Current).Match(candidate))
				{
					return true;
				}
			}
			return _delegate.IsCollection(candidate.GetDelegate());
		}

		public virtual void RegisterCollection(System.Type clazz)
		{
			RegisterCollection(ClassPredicate(clazz));
		}

		public virtual void RegisterCollection(Db4objects.Db4o.Reflect.IReflectClassPredicate
			 predicate)
		{
			_collectionPredicates.Add(predicate);
		}

		private Db4objects.Db4o.Reflect.IReflectClassPredicate ClassPredicate(System.Type
			 clazz)
		{
			Db4objects.Db4o.Reflect.IReflectClass collectionClass = ForClass(clazz);
			Db4objects.Db4o.Reflect.IReflectClassPredicate predicate = new _AnonymousInnerClass199
				(this, collectionClass);
			return predicate;
		}

		private sealed class _AnonymousInnerClass199 : Db4objects.Db4o.Reflect.IReflectClassPredicate
		{
			public _AnonymousInnerClass199(GenericReflector _enclosing, Db4objects.Db4o.Reflect.IReflectClass
				 collectionClass)
			{
				this._enclosing = _enclosing;
				this.collectionClass = collectionClass;
			}

			public bool Match(Db4objects.Db4o.Reflect.IReflectClass candidate)
			{
				return collectionClass.IsAssignableFrom(candidate);
			}

			private readonly GenericReflector _enclosing;

			private readonly Db4objects.Db4o.Reflect.IReflectClass collectionClass;
		}

		public virtual void RegisterCollectionUpdateDepth(System.Type clazz, int depth)
		{
			RegisterCollectionUpdateDepth(ClassPredicate(clazz), depth);
		}

		public virtual void RegisterCollectionUpdateDepth(Db4objects.Db4o.Reflect.IReflectClassPredicate
			 predicate, int depth)
		{
			_collectionUpdateDepths.Add(new Db4objects.Db4o.Reflect.Generic.CollectionUpdateDepthEntry
				(predicate, depth));
		}

		public virtual void Register(Db4objects.Db4o.Reflect.Generic.GenericClass clazz)
		{
			string name = clazz.GetName();
			if (_classByName.Get(name) == null)
			{
				_classByName.Put(name, clazz);
				_classes.Add(clazz);
			}
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass[] KnownClasses()
		{
			ReadAll();
			Db4objects.Db4o.Foundation.Collection4 classes = new Db4objects.Db4o.Foundation.Collection4
				();
			System.Collections.IEnumerator i = _classes.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Reflect.Generic.GenericClass clazz = (Db4objects.Db4o.Reflect.Generic.GenericClass
					)i.Current;
				if (!_stream.i_handlers.ICLASS_INTERNAL.IsAssignableFrom(clazz))
				{
					if (!clazz.IsSecondClass())
					{
						if (!clazz.IsArray())
						{
							classes.Add(clazz);
						}
					}
				}
			}
			Db4objects.Db4o.Reflect.IReflectClass[] ret = new Db4objects.Db4o.Reflect.IReflectClass
				[classes.Size()];
			int j = 0;
			i = classes.GetEnumerator();
			while (i.MoveNext())
			{
				ret[j++] = (Db4objects.Db4o.Reflect.IReflectClass)i.Current;
			}
			return ret;
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

		private Db4objects.Db4o.Reflect.Generic.GenericClass EnsureClassInitialised(int id
			)
		{
			Db4objects.Db4o.Reflect.Generic.GenericClass ret = EnsureClassAvailability(id);
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

		private Db4objects.Db4o.Reflect.Generic.GenericClass EnsureClassAvailability(int 
			id)
		{
			if (id == 0)
			{
				return null;
			}
			Db4objects.Db4o.Reflect.Generic.GenericClass ret = (Db4objects.Db4o.Reflect.Generic.GenericClass
				)_classByID.Get(id);
			if (ret != null)
			{
				return ret;
			}
			Db4objects.Db4o.YapReader classreader = _stream.ReadWriterByID(_trans, id);
			Db4objects.Db4o.Inside.Marshall.ClassMarshaller marshaller = MarshallerFamily()._class;
			Db4objects.Db4o.Inside.Marshall.RawClassSpec spec = marshaller.ReadSpec(_trans, classreader
				);
			string className = spec.Name();
			ret = (Db4objects.Db4o.Reflect.Generic.GenericClass)_classByName.Get(className);
			if (ret != null)
			{
				_classByID.Put(id, ret);
				_pendingClasses.Add(id);
				return ret;
			}
			Db4objects.Db4o.Reflect.IReflectClass nativeClass = _delegate.ForName(className);
			ret = new Db4objects.Db4o.Reflect.Generic.GenericClass(this, nativeClass, className
				, EnsureClassAvailability(spec.SuperClassID()));
			ret.SetDeclaredFieldCount(spec.NumFields());
			_classByID.Put(id, ret);
			_pendingClasses.Add(id);
			return ret;
		}

		private Db4objects.Db4o.Inside.Marshall.MarshallerFamily MarshallerFamily()
		{
			return Db4objects.Db4o.Inside.Marshall.MarshallerFamily.ForConverterVersion(_stream
				.ConverterVersion());
		}

		private void EnsureClassRead(int id)
		{
			Db4objects.Db4o.Reflect.Generic.GenericClass clazz = (Db4objects.Db4o.Reflect.Generic.GenericClass
				)_classByID.Get(id);
			Db4objects.Db4o.YapReader classreader = _stream.ReadWriterByID(_trans, id);
			Db4objects.Db4o.Inside.Marshall.ClassMarshaller classMarshaller = MarshallerFamily
				()._class;
			Db4objects.Db4o.Inside.Marshall.RawClassSpec classInfo = classMarshaller.ReadSpec
				(_trans, classreader);
			string className = classInfo.Name();
			if (_classByName.Get(className) != null)
			{
				return;
			}
			_classByName.Put(className, clazz);
			_classes.Add(clazz);
			int numFields = classInfo.NumFields();
			Db4objects.Db4o.Reflect.Generic.GenericField[] fields = new Db4objects.Db4o.Reflect.Generic.GenericField
				[numFields];
			Db4objects.Db4o.Inside.Marshall.IFieldMarshaller fieldMarshaller = MarshallerFamily
				()._field;
			for (int i = 0; i < numFields; i++)
			{
				Db4objects.Db4o.Inside.Marshall.RawFieldSpec fieldInfo = fieldMarshaller.ReadSpec
					(_stream, classreader);
				string fieldName = fieldInfo.Name();
				int handlerID = fieldInfo.HandlerID();
				if (fieldInfo.IsVirtual())
				{
					fields[i] = new Db4objects.Db4o.Reflect.Generic.GenericVirtualField(fieldName);
					continue;
				}
				Db4objects.Db4o.Reflect.Generic.GenericClass fieldClass = null;
				switch (handlerID)
				{
					case Db4objects.Db4o.YapHandlers.ANY_ID:
					{
						fieldClass = (Db4objects.Db4o.Reflect.Generic.GenericClass)ForClass(typeof(object
							));
						break;
					}

					case Db4objects.Db4o.YapHandlers.ANY_ARRAY_ID:
					{
						fieldClass = ((Db4objects.Db4o.Reflect.Generic.GenericClass)ForClass(typeof(object
							))).ArrayClass();
						break;
					}

					default:
					{
						EnsureClassAvailability(handlerID);
						fieldClass = (Db4objects.Db4o.Reflect.Generic.GenericClass)_classByID.Get(handlerID
							);
						break;
					}
				}
				fields[i] = new Db4objects.Db4o.Reflect.Generic.GenericField(fieldName, fieldClass
					, fieldInfo.IsPrimitive(), fieldInfo.IsArray(), fieldInfo.IsNArray());
			}
			clazz.InitFields(fields);
		}

		public virtual void RegisterPrimitiveClass(int id, string name, Db4objects.Db4o.Reflect.Generic.IGenericConverter
			 converter)
		{
			Db4objects.Db4o.Reflect.Generic.GenericClass existing = (Db4objects.Db4o.Reflect.Generic.GenericClass
				)_classByID.Get(id);
			if (existing != null)
			{
				if (null != converter)
				{
					existing.SetSecondClass();
				}
				else
				{
					existing.SetConverter(null);
				}
				return;
			}
			Db4objects.Db4o.Reflect.IReflectClass clazz = _delegate.ForName(name);
			Db4objects.Db4o.Reflect.Generic.GenericClass claxx = null;
			if (clazz != null)
			{
				claxx = EnsureDelegate(clazz);
			}
			else
			{
				claxx = new Db4objects.Db4o.Reflect.Generic.GenericClass(this, null, name, null);
				_classByName.Put(name, claxx);
				claxx.InitFields(new Db4objects.Db4o.Reflect.Generic.GenericField[] { new Db4objects.Db4o.Reflect.Generic.GenericField
					(null, null, true, false, false) });
				claxx.SetConverter(converter);
				_classes.Add(claxx);
			}
			claxx.SetSecondClass();
			claxx.SetPrimitive();
			_classByID.Put(id, claxx);
		}

		public virtual void SetParent(Db4objects.Db4o.Reflect.IReflector reflector)
		{
		}
	}
}
