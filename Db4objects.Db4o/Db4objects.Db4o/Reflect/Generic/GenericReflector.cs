namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericReflector : Db4objects.Db4o.Reflect.IReflector, Db4objects.Db4o.Foundation.IDeepClone
	{
		private Db4objects.Db4o.Reflect.Generic.KnownClassesRepository _repository;

		private Db4objects.Db4o.Reflect.IReflector _delegate;

		private Db4objects.Db4o.Reflect.Generic.GenericArrayReflector _array;

		private Db4objects.Db4o.Foundation.Collection4 _collectionPredicates = new Db4objects.Db4o.Foundation.Collection4
			();

		private Db4objects.Db4o.Foundation.Collection4 _collectionUpdateDepths = new Db4objects.Db4o.Foundation.Collection4
			();

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _classByClass = new Db4objects.Db4o.Foundation.Hashtable4
			();

		private Db4objects.Db4o.Transaction _trans;

		private Db4objects.Db4o.YapStream _stream;

		public GenericReflector(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Reflect.IReflector
			 delegateReflector)
		{
			_repository = new Db4objects.Db4o.Reflect.Generic.KnownClassesRepository(new Db4objects.Db4o.Reflect.Generic.GenericClassBuilder
				(this, delegateReflector));
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
			_repository.SetTransaction(trans);
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
				)_repository.LookupByName(clazz.GetName());
			if (claxx == null)
			{
				string name = clazz.GetName();
				claxx = new Db4objects.Db4o.Reflect.Generic.GenericClass(this, clazz, name, null);
				_repository.Register(claxx);
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
			Db4objects.Db4o.Reflect.IReflectClass clazz = _repository.LookupByName(className);
			if (clazz != null)
			{
				return clazz;
			}
			clazz = _delegate.ForName(className);
			if (clazz != null)
			{
				return EnsureDelegate(clazz);
			}
			return _repository.ForName(className);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ForObject(object obj)
		{
			if (obj is Db4objects.Db4o.Reflect.Generic.GenericObject)
			{
				return ForGenericObject((Db4objects.Db4o.Reflect.Generic.GenericObject)obj);
			}
			return _delegate.ForObject(obj);
		}

		private Db4objects.Db4o.Reflect.IReflectClass ForGenericObject(Db4objects.Db4o.Reflect.Generic.GenericObject
			 genericObject)
		{
			Db4objects.Db4o.Reflect.Generic.GenericClass claxx = genericObject._class;
			if (claxx == null)
			{
				throw new System.InvalidOperationException();
			}
			string name = claxx.GetName();
			if (name == null)
			{
				throw new System.InvalidOperationException();
			}
			Db4objects.Db4o.Reflect.Generic.GenericClass existingClass = (Db4objects.Db4o.Reflect.Generic.GenericClass
				)ForName(name);
			if (existingClass == null)
			{
				_repository.Register(claxx);
				return claxx;
			}
			if (existingClass != claxx)
			{
				throw new System.InvalidOperationException();
			}
			return claxx;
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
			Db4objects.Db4o.Reflect.IReflectClassPredicate predicate = new _AnonymousInnerClass209
				(this, collectionClass);
			return predicate;
		}

		private sealed class _AnonymousInnerClass209 : Db4objects.Db4o.Reflect.IReflectClassPredicate
		{
			public _AnonymousInnerClass209(GenericReflector _enclosing, Db4objects.Db4o.Reflect.IReflectClass
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
			if (_repository.LookupByName(name) == null)
			{
				_repository.Register(clazz);
			}
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass[] KnownClasses()
		{
			Db4objects.Db4o.Foundation.Collection4 classes = new Db4objects.Db4o.Foundation.Collection4
				();
			CollectKnownClasses(classes);
			return (Db4objects.Db4o.Reflect.IReflectClass[])classes.ToArray(new Db4objects.Db4o.Reflect.IReflectClass
				[classes.Size()]);
		}

		private void CollectKnownClasses(Db4objects.Db4o.Foundation.Collection4 classes)
		{
			System.Collections.IEnumerator i = _repository.Classes();
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
		}

		public virtual void RegisterPrimitiveClass(int id, string name, Db4objects.Db4o.Reflect.Generic.IGenericConverter
			 converter)
		{
			Db4objects.Db4o.Reflect.Generic.GenericClass existing = (Db4objects.Db4o.Reflect.Generic.GenericClass
				)_repository.LookupByID(id);
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
				Register(claxx);
				claxx.InitFields(new Db4objects.Db4o.Reflect.Generic.GenericField[] { new Db4objects.Db4o.Reflect.Generic.GenericField
					(null, null, true, false, false) });
				claxx.SetConverter(converter);
			}
			claxx.SetSecondClass();
			claxx.SetPrimitive();
			_repository.Register(id, claxx);
		}

		public virtual void SetParent(Db4objects.Db4o.Reflect.IReflector reflector)
		{
		}
	}
}
