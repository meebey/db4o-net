/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericReflector : IReflector, IDeepClone
	{
		private KnownClassesRepository _repository;

		private IReflector _delegate;

		private GenericArrayReflector _array;

		private Collection4 _collectionPredicates = new Collection4();

		private Collection4 _collectionUpdateDepths = new Collection4();

		private readonly Hashtable4 _classByClass = new Hashtable4();

		private Transaction _trans;

		private ObjectContainerBase _stream;

		public GenericReflector(Transaction trans, IReflector delegateReflector)
		{
			_repository = new KnownClassesRepository(new GenericClassBuilder(this, delegateReflector
				));
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
				(null, (IReflector)_delegate.DeepClone(this));
			myClone._collectionPredicates = (Collection4)_collectionPredicates.DeepClone(myClone
				);
			myClone._collectionUpdateDepths = (Collection4)_collectionUpdateDepths.DeepClone(
				myClone);
			return myClone;
		}

		internal virtual ObjectContainerBase GetStream()
		{
			return _stream;
		}

		public virtual bool HasTransaction()
		{
			return _trans != null;
		}

		public virtual void SetTransaction(Transaction trans)
		{
			if (trans != null)
			{
				_trans = trans;
				_stream = trans.Stream();
			}
			_repository.SetTransaction(trans);
		}

		public virtual IReflectArray Array()
		{
			if (_array == null)
			{
				_array = new GenericArrayReflector(this);
			}
			return _array;
		}

		public virtual int CollectionUpdateDepth(IReflectClass candidate)
		{
			IEnumerator i = _collectionUpdateDepths.GetEnumerator();
			while (i.MoveNext())
			{
				CollectionUpdateDepthEntry entry = (CollectionUpdateDepthEntry)i.Current;
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

		internal virtual Db4objects.Db4o.Reflect.Generic.GenericClass EnsureDelegate(IReflectClass
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
				claxx = GenericClass(clazz);
				_repository.Register(claxx);
			}
			return claxx;
		}

		private Db4objects.Db4o.Reflect.Generic.GenericClass GenericClass(IReflectClass clazz
			)
		{
			Db4objects.Db4o.Reflect.Generic.GenericClass ret;
			string name = clazz.GetName();
			if (name.Equals(typeof(GenericArray).FullName))
			{
				ret = new GenericArrayClass(this, clazz, name, null);
			}
			else
			{
				ret = new Db4objects.Db4o.Reflect.Generic.GenericClass(this, clazz, name, null);
			}
			return ret;
		}

		public virtual IReflectClass ForClass(Type clazz)
		{
			if (clazz == null)
			{
				return null;
			}
			IReflectClass claxx = (IReflectClass)_classByClass.Get(clazz);
			if (claxx != null)
			{
				return claxx;
			}
			claxx = ForName(ReflectPlatform.FullyQualifiedName(clazz));
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

		public virtual IReflectClass ForName(string className)
		{
			IReflectClass clazz = _repository.LookupByName(className);
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

		public virtual IReflectClass ForObject(object obj)
		{
			if (obj is GenericObject)
			{
				return ForGenericObject((GenericObject)obj);
			}
			return _delegate.ForObject(obj);
		}

		private IReflectClass ForGenericObject(GenericObject genericObject)
		{
			Db4objects.Db4o.Reflect.Generic.GenericClass claxx = genericObject._class;
			if (claxx == null)
			{
				throw new InvalidOperationException();
			}
			string name = claxx.GetName();
			if (name == null)
			{
				throw new InvalidOperationException();
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
				throw new InvalidOperationException();
			}
			return claxx;
		}

		public virtual IReflector GetDelegate()
		{
			return _delegate;
		}

		public virtual bool IsCollection(IReflectClass candidate)
		{
			IEnumerator i = _collectionPredicates.GetEnumerator();
			while (i.MoveNext())
			{
				if (((IReflectClassPredicate)i.Current).Match(candidate))
				{
					return true;
				}
			}
			return _delegate.IsCollection(candidate.GetDelegate());
		}

		public virtual void RegisterCollection(Type clazz)
		{
			RegisterCollection(ClassPredicate(clazz));
		}

		public virtual void RegisterCollection(IReflectClassPredicate predicate)
		{
			_collectionPredicates.Add(predicate);
		}

		private IReflectClassPredicate ClassPredicate(Type clazz)
		{
			IReflectClass collectionClass = ForClass(clazz);
			IReflectClassPredicate predicate = new _AnonymousInnerClass220(this, collectionClass
				);
			return predicate;
		}

		private sealed class _AnonymousInnerClass220 : IReflectClassPredicate
		{
			public _AnonymousInnerClass220(GenericReflector _enclosing, IReflectClass collectionClass
				)
			{
				this._enclosing = _enclosing;
				this.collectionClass = collectionClass;
			}

			public bool Match(IReflectClass candidate)
			{
				return collectionClass.IsAssignableFrom(candidate);
			}

			private readonly GenericReflector _enclosing;

			private readonly IReflectClass collectionClass;
		}

		public virtual void RegisterCollectionUpdateDepth(Type clazz, int depth)
		{
			RegisterCollectionUpdateDepth(ClassPredicate(clazz), depth);
		}

		public virtual void RegisterCollectionUpdateDepth(IReflectClassPredicate predicate
			, int depth)
		{
			_collectionUpdateDepths.Add(new CollectionUpdateDepthEntry(predicate, depth));
		}

		public virtual void Register(Db4objects.Db4o.Reflect.Generic.GenericClass clazz)
		{
			string name = clazz.GetName();
			if (_repository.LookupByName(name) == null)
			{
				_repository.Register(clazz);
			}
		}

		public virtual IReflectClass[] KnownClasses()
		{
			Collection4 classes = new Collection4();
			CollectKnownClasses(classes);
			return (IReflectClass[])classes.ToArray(new IReflectClass[classes.Size()]);
		}

		private void CollectKnownClasses(Collection4 classes)
		{
			IEnumerator i = _repository.Classes();
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

		public virtual void RegisterPrimitiveClass(int id, string name, IGenericConverter
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
			IReflectClass clazz = _delegate.ForName(name);
			Db4objects.Db4o.Reflect.Generic.GenericClass claxx = null;
			if (clazz != null)
			{
				claxx = EnsureDelegate(clazz);
			}
			else
			{
				claxx = new Db4objects.Db4o.Reflect.Generic.GenericClass(this, null, name, null);
				Register(claxx);
				claxx.InitFields(new GenericField[] { new GenericField(null, null, true, false, false
					) });
				claxx.SetConverter(converter);
			}
			claxx.SetSecondClass();
			claxx.SetPrimitive();
			_repository.Register(id, claxx);
		}

		public virtual void SetParent(IReflector reflector)
		{
		}
	}
}
