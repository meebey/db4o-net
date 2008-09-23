/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Internal.Encoding;
using Db4objects.Db4o.Internal.Fieldhandlers;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Handlers.Array;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Core;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class ClassMetadata : PersistentBase, IIndexableTypeHandler, IFirstClassHandler
		, IStoredClass, IFieldHandler, IReadsObjectIds
	{
		private ITypeHandler4 _typeHandler;

		public Db4objects.Db4o.Internal.ClassMetadata i_ancestor;

		private Config4Class i_config;

		public ClassAspect[] _aspects;

		private readonly IClassIndexStrategy _index;

		protected string i_name;

		private readonly ObjectContainerBase _container;

		internal byte[] i_nameBytes;

		private ByteArrayBuffer i_reader;

		private bool _classIndexed;

		private IReflectClass _classReflector;

		private bool _isEnum;

		private EventDispatcher _eventDispatcher;

		private bool _internal;

		private bool _unversioned;

		private TernaryBool _canUpdateFast = TernaryBool.Unspecified;

		private TranslatedAspect _translator;

		private IModificationAware _modificationChecker = ClassMetadata.AlwaysModified.Instance;

		public ObjectContainerBase Stream()
		{
			return _container;
		}

		public bool CanUpdateFast()
		{
			if (_canUpdateFast == TernaryBool.Unspecified)
			{
				_canUpdateFast = TernaryBool.ForBoolean(CheckCanUpdateFast());
			}
			return _canUpdateFast.BooleanValue(false);
		}

		private bool CheckCanUpdateFast()
		{
			if (i_ancestor != null && !i_ancestor.CanUpdateFast())
			{
				return false;
			}
			if (i_config != null && i_config.CascadeOnDelete() == TernaryBool.Yes)
			{
				return false;
			}
			BooleanByRef hasIndex = new BooleanByRef(false);
			ForEachDeclaredField(new _IProcedure4_87(hasIndex));
			return !hasIndex.value;
		}

		private sealed class _IProcedure4_87 : IProcedure4
		{
			public _IProcedure4_87(BooleanByRef hasIndex)
			{
				this.hasIndex = hasIndex;
			}

			public void Apply(object arg)
			{
				if (((FieldMetadata)arg).HasIndex())
				{
					hasIndex.value = true;
				}
			}

			private readonly BooleanByRef hasIndex;
		}

		internal virtual bool IsInternal()
		{
			return _internal;
		}

		private IClassIndexStrategy CreateIndexStrategy()
		{
			return new BTreeClassIndexStrategy(this);
		}

		public ClassMetadata(ObjectContainerBase container, IReflectClass claxx)
		{
			_typeHandler = CreateDefaultTypeHandler();
			_container = container;
			_classReflector = claxx;
			_index = CreateIndexStrategy();
			_classIndexed = true;
		}

		private IFieldAwareTypeHandler CreateDefaultTypeHandler()
		{
			return new FirstClassObjectHandler(this);
		}

		public virtual void ActivateFields(Transaction trans, object obj, IActivationDepth
			 depth)
		{
			if (ObjectCanActivate(trans, obj))
			{
				ForEachAspect(new _IProcedure4_119(trans, obj, depth));
			}
		}

		private sealed class _IProcedure4_119 : IProcedure4
		{
			public _IProcedure4_119(Transaction trans, object obj, IActivationDepth depth)
			{
				this.trans = trans;
				this.obj = obj;
				this.depth = depth;
			}

			public void Apply(object arg)
			{
				((ClassAspect)arg).CascadeActivation(trans, obj, depth);
			}

			private readonly Transaction trans;

			private readonly object obj;

			private readonly IActivationDepth depth;
		}

		public void AddFieldIndices(StatefulBuffer buffer, Slot slot)
		{
			if (!FirstClassObjectHandlerIsUsed())
			{
				return;
			}
			if (HasClassIndex() || HasVirtualAttributes())
			{
				ObjectHeader oh = new ObjectHeader(_container, this, buffer);
				ObjectIdContextImpl context = new ObjectIdContextImpl(buffer.Transaction(), buffer
					, oh, buffer.GetID());
				FieldAwareTypeHandler(CorrectHandlerVersion(context)).AddFieldIndices(context, slot
					);
			}
		}

		// FIXME: This method wants to be removed.
		private bool FirstClassObjectHandlerIsUsed()
		{
			return _typeHandler is FirstClassObjectHandler;
		}

		internal virtual void AddMembers(ObjectContainerBase container)
		{
			BitTrue(Const4.CheckedChanges);
			Collection4 aspects = new Collection4();
			if (null != _aspects)
			{
				aspects.AddAll(_aspects);
			}
			ITypeHandler4 customTypeHandler = container.Handlers().ConfiguredTypeHandler(ClassReflector
				());
			bool dirty = IsDirty();
			if (InstallTranslator(aspects, customTypeHandler))
			{
				dirty = true;
			}
			if (container.DetectSchemaChanges())
			{
				if (GenerateVersionNumbers())
				{
					if (!HasVersionField())
					{
						aspects.Add(container.VersionIndex());
						dirty = true;
					}
				}
				if (GenerateUUIDs())
				{
					if (!HasUUIDField())
					{
						aspects.Add(container.UUIDIndex());
						dirty = true;
					}
				}
			}
			if (InstallCustomTypehandler(aspects, customTypeHandler))
			{
				dirty = true;
			}
			bool defaultFieldBehaviour = _translator == null && customTypeHandler == null;
			if (container.DetectSchemaChanges())
			{
				if (defaultFieldBehaviour)
				{
					dirty = CollectReflectFields(container, aspects) | dirty;
				}
				if (dirty)
				{
					_container.SetDirtyInSystemTransaction(this);
				}
			}
			if (dirty || !defaultFieldBehaviour)
			{
				_aspects = new ClassAspect[aspects.Size()];
				aspects.ToArray(_aspects);
				for (int i = 0; i < _aspects.Length; i++)
				{
					_aspects[i].SetHandle(i);
				}
			}
			DiagnosticProcessor dp = _container._handlers._diagnosticProcessor;
			if (dp.Enabled())
			{
				dp.CheckClassHasFields(this);
			}
			if (_aspects == null)
			{
				_aspects = new FieldMetadata[0];
			}
			_container.Callbacks().ClassOnRegistered(this);
			SetStateOK();
		}

		private bool InstallCustomTypehandler(Collection4 aspects, ITypeHandler4 customTypeHandler
			)
		{
			if (customTypeHandler == null)
			{
				return false;
			}
			if (customTypeHandler is IEmbeddedTypeHandler)
			{
				_typeHandler = customTypeHandler;
			}
			if (customTypeHandler is IModificationAware)
			{
				_modificationChecker = (IModificationAware)customTypeHandler;
			}
			bool dirty = false;
			TypeHandlerAspect typeHandlerAspect = new TypeHandlerAspect(customTypeHandler);
			if (!ReplaceAspectByName(aspects, typeHandlerAspect))
			{
				aspects.Add(typeHandlerAspect);
				dirty = true;
			}
			DisableAspectsBefore(aspects, typeHandlerAspect);
			return dirty;
		}

		private void DisableAspectsBefore(Collection4 aspects, TypeHandlerAspect typeHandlerAspect
			)
		{
			int disableFromVersion = aspects.IndexOf(typeHandlerAspect) + 1;
			IEnumerator i = aspects.GetEnumerator();
			while (i.MoveNext())
			{
				ClassAspect aspect = (ClassAspect)i.Current;
				if (aspect == typeHandlerAspect)
				{
					break;
				}
				aspect.DisableFromAspectCountVersion(disableFromVersion);
			}
		}

		private bool InstallTranslator(Collection4 aspects, ITypeHandler4 customTypeHandler
			)
		{
			if (i_config == null)
			{
				return false;
			}
			IObjectTranslator ot = i_config.GetTranslator();
			if (ot == null)
			{
				return false;
			}
			TranslatedAspect translator = new TranslatedAspect(this, ot);
			if (ReplaceAspectByName(aspects, translator))
			{
				_translator = translator;
				return false;
			}
			if (customTypeHandler == null)
			{
				aspects.Add(translator);
				_translator = translator;
				return true;
			}
			return false;
		}

		private bool ReplaceAspectByName(Collection4 aspects, ClassAspect aspect)
		{
			IEnumerator i = aspects.GetEnumerator();
			while (i.MoveNext())
			{
				ClassAspect current = (ClassAspect)i.Current;
				if (current.GetName().Equals(aspect.GetName()))
				{
					aspects.Replace(current, aspect);
					return true;
				}
			}
			return false;
		}

		private bool CollectReflectFields(ObjectContainerBase container, Collection4 collectedAspects
			)
		{
			bool dirty = false;
			IReflectField[] fields = ReflectFields();
			for (int i = 0; i < fields.Length; i++)
			{
				if (StoreField(fields[i]))
				{
					IReflectClass fieldType = Handlers4.BaseType(fields[i].GetFieldType());
					IFieldHandler fieldHandler = container.FieldHandlerForClass(fieldType);
					if (fieldHandler == null)
					{
						continue;
					}
					int fieldHandlerId = container.FieldHandlerIdForFieldHandler(fieldHandler);
					FieldMetadata field = new FieldMetadata(this, fields[i], (ITypeHandler4)fieldHandler
						, fieldHandlerId);
					bool found = false;
					IEnumerator aspectIterator = collectedAspects.GetEnumerator();
					while (aspectIterator.MoveNext())
					{
						if (((ClassAspect)aspectIterator.Current).Equals(field))
						{
							found = true;
							break;
						}
					}
					if (found)
					{
						continue;
					}
					dirty = true;
					collectedAspects.Add(field);
				}
			}
			return dirty;
		}

		internal virtual void AddToIndex(Transaction trans, int id)
		{
			if (!trans.Container().MaintainsIndices())
			{
				return;
			}
			AddToIndex1(trans, id);
		}

		internal void AddToIndex1(Transaction a_trans, int a_id)
		{
			if (i_ancestor != null)
			{
				i_ancestor.AddToIndex1(a_trans, a_id);
			}
			if (HasClassIndex())
			{
				_index.Add(a_trans, a_id);
			}
		}

		internal virtual bool AllowsQueries()
		{
			return HasClassIndex();
		}

		public virtual void CascadeActivation(ActivationContext4 context)
		{
			FieldAwareTypeHandler().CascadeActivation(context);
		}

		private IFieldAwareTypeHandler FieldAwareTypeHandler()
		{
			if (_typeHandler is IFieldAwareTypeHandler)
			{
				return (IFieldAwareTypeHandler)_typeHandler;
			}
			return NullFieldAwareTypeHandler.Instance;
		}

		private IFieldAwareTypeHandler FieldAwareTypeHandler(ITypeHandler4 typeHandler)
		{
			if (typeHandler is IFieldAwareTypeHandler)
			{
				return (IFieldAwareTypeHandler)typeHandler;
			}
			return NullFieldAwareTypeHandler.Instance;
		}

		public virtual bool DescendOnCascadingActivation()
		{
			return true;
		}

		internal virtual void CheckChanges()
		{
			if (StateOK())
			{
				if (!BitIsTrue(Const4.CheckedChanges))
				{
					BitTrue(Const4.CheckedChanges);
					if (i_ancestor != null)
					{
						i_ancestor.CheckChanges();
					}
					// Ancestor first, so the object length calculates
					// correctly
					if (_classReflector != null)
					{
						AddMembers(_container);
						Transaction trans = _container.SystemTransaction();
						if (!_container.IsClient() && !IsReadOnlyContainer(trans))
						{
							Write(trans);
						}
					}
				}
			}
		}

		public virtual void CheckType()
		{
			IReflectClass claxx = ClassReflector();
			if (claxx == null)
			{
				return;
			}
			if (_container._handlers.IclassInternal.IsAssignableFrom(claxx))
			{
				_internal = true;
			}
			if (_container._handlers.IclassUnversioned.IsAssignableFrom(claxx))
			{
				_unversioned = true;
			}
			if (IsDb4oTypeImpl())
			{
				IDb4oTypeImpl db4oTypeImpl = (IDb4oTypeImpl)claxx.NewInstance();
				_classIndexed = (db4oTypeImpl == null || db4oTypeImpl.HasClassIndex());
			}
			else
			{
				if (i_config != null)
				{
					_classIndexed = i_config.Indexed();
				}
			}
		}

		public virtual bool IsDb4oTypeImpl()
		{
			return _container._handlers.IclassDb4otypeimpl.IsAssignableFrom(ClassReflector());
		}

		public int AdjustUpdateDepth(Transaction trans, int depth)
		{
			Config4Class config = ConfigOrAncestorConfig();
			if (depth == Const4.Unspecified)
			{
				depth = CheckUpdateDepthUnspecified(trans.Container().ConfigImpl());
				depth = AdjustCollectionDepthToBorders(depth);
			}
			if (config == null)
			{
				return depth - 1;
			}
			bool cascadeOnDelete = config.CascadeOnDelete() == TernaryBool.Yes;
			bool cascadeOnUpdate = config.CascadeOnUpdate() == TernaryBool.Yes;
			if (cascadeOnDelete || cascadeOnUpdate)
			{
				depth = AdjustDepthToBorders(depth);
			}
			return depth - 1;
		}

		public virtual int AdjustCollectionDepthToBorders(int depth)
		{
			if (!ClassReflector().IsCollection())
			{
				return depth;
			}
			return AdjustDepthToBorders(depth);
		}

		public virtual int AdjustDepthToBorders(int depth)
		{
			int depthBorder = Reflector().CollectionUpdateDepth(ClassReflector());
			if (depth > int.MinValue && depth < depthBorder)
			{
				depth = depthBorder;
			}
			return depth;
		}

		private int CheckUpdateDepthUnspecified(Config4Impl config)
		{
			int depth = config.UpdateDepth() + 1;
			if (i_config != null && i_config.UpdateDepth() != 0)
			{
				depth = i_config.UpdateDepth() + 1;
			}
			if (i_ancestor != null)
			{
				int ancestordepth = i_ancestor.CheckUpdateDepthUnspecified(config);
				if (ancestordepth > depth)
				{
					return ancestordepth;
				}
			}
			return depth;
		}

		public virtual void CollectConstraints(Transaction trans, QConObject parentConstraint
			, object obj, IVisitor4 visitor)
		{
			ForEachField(new _IProcedure4_449(trans, parentConstraint, obj, visitor));
		}

		private sealed class _IProcedure4_449 : IProcedure4
		{
			public _IProcedure4_449(Transaction trans, QConObject parentConstraint, object obj
				, IVisitor4 visitor)
			{
				this.trans = trans;
				this.parentConstraint = parentConstraint;
				this.obj = obj;
				this.visitor = visitor;
			}

			public void Apply(object arg)
			{
				FieldMetadata fieldMetadata = (FieldMetadata)arg;
				if (fieldMetadata.Enabled(AspectVersionContextImpl.CheckAlwaysEnabled))
				{
					fieldMetadata.CollectConstraints(trans, parentConstraint, obj, visitor);
				}
			}

			private readonly Transaction trans;

			private readonly QConObject parentConstraint;

			private readonly object obj;

			private readonly IVisitor4 visitor;
		}

		public void CollectIDs(CollectIdContext context, string fieldName)
		{
			if (!FirstClassObjectHandlerIsUsed())
			{
				throw new InvalidOperationException();
			}
			((FirstClassObjectHandler)CorrectHandlerVersion(context)).CollectIDs(context, fieldName
				);
		}

		public virtual void CollectIDs(QueryingReadContext context)
		{
			if (!FirstClassObjectHandlerIsUsed())
			{
				throw new InvalidOperationException();
			}
			ITypeHandler4 typeHandler = CorrectHandlerVersion(context);
			if (typeHandler is IFirstClassHandler)
			{
				((IFirstClassHandler)typeHandler).CollectIDs(context);
			}
		}

		public virtual bool CustomizedNewInstance()
		{
			return ConfigInstantiates();
		}

		public virtual Config4Class Config()
		{
			return i_config;
		}

		public virtual Config4Class ConfigOrAncestorConfig()
		{
			if (i_config != null)
			{
				return i_config;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.ConfigOrAncestorConfig();
			}
			return null;
		}

		private bool CreateConstructor(ObjectContainerBase container, string className)
		{
			IReflectClass claxx = container.Reflector().ForName(className);
			return CreateConstructor(container, claxx, className, true);
		}

		public virtual bool CreateConstructor(ObjectContainerBase container, IReflectClass
			 claxx, string name, bool errMessages)
		{
			ClassReflector(claxx);
			_eventDispatcher = EventDispatcher.ForClass(container, claxx);
			if (CustomizedNewInstance())
			{
				return true;
			}
			if (claxx != null)
			{
				if (container._handlers.IclassTransientclass.IsAssignableFrom(claxx) || Platform4
					.IsTransient(claxx))
				{
					claxx = null;
				}
			}
			if (claxx == null)
			{
				if (name == null || !Platform4.IsDb4oClass(name))
				{
					if (errMessages)
					{
						container.LogMsg(23, name);
					}
				}
				SetStateDead();
				return false;
			}
			if (claxx.IsAbstract() || claxx.IsInterface())
			{
				return true;
			}
			if (claxx.EnsureCanBeInstantiated())
			{
				return true;
			}
			SetStateDead();
			if (errMessages)
			{
				container.LogMsg(7, name);
			}
			if (container.ConfigImpl().ExceptionsOnNotStorable())
			{
				throw new ObjectNotStorableException(claxx);
			}
			return false;
		}

		protected virtual void ClassReflector(IReflectClass claxx)
		{
			_classReflector = claxx;
			if (claxx == null)
			{
				_typeHandler = null;
				return;
			}
			_typeHandler = CreateDefaultTypeHandler();
		}

		public virtual void Deactivate(Transaction trans, object obj, IActivationDepth depth
			)
		{
			if (ObjectCanDeactivate(trans, obj))
			{
				DeactivateFields(trans, obj, depth);
				ObjectOnDeactivate(trans, obj);
			}
		}

		private void ObjectOnDeactivate(Transaction transaction, object obj)
		{
			ObjectContainerBase container = transaction.Container();
			container.Callbacks().ObjectOnDeactivate(transaction, obj);
			DispatchEvent(transaction, obj, EventDispatcher.Deactivate);
		}

		private bool ObjectCanDeactivate(Transaction transaction, object obj)
		{
			ObjectContainerBase container = transaction.Container();
			return container.Callbacks().ObjectCanDeactivate(transaction, obj) && DispatchEvent
				(transaction, obj, EventDispatcher.CanDeactivate);
		}

		internal void DeactivateFields(Transaction trans, object obj, IActivationDepth depth
			)
		{
			ForEachAspect(new _IProcedure4_580(trans, obj, depth));
		}

		private sealed class _IProcedure4_580 : IProcedure4
		{
			public _IProcedure4_580(Transaction trans, object obj, IActivationDepth depth)
			{
				this.trans = trans;
				this.obj = obj;
				this.depth = depth;
			}

			public void Apply(object arg)
			{
				ClassAspect classAspect = (ClassAspect)arg;
				if (classAspect.Enabled(AspectVersionContextImpl.CheckAlwaysEnabled))
				{
					classAspect.Deactivate(trans, obj, depth);
				}
			}

			private readonly Transaction trans;

			private readonly object obj;

			private readonly IActivationDepth depth;
		}

		internal void Delete(StatefulBuffer buffer, object obj)
		{
			ObjectHeader oh = new ObjectHeader(_container, this, buffer);
			Transaction trans = buffer.Transaction();
			int id = buffer.GetID();
			int typeId = trans.Container()._handlers.ArrayType(obj);
			RemoveFromIndex(trans, id);
			DeleteContextImpl context = new DeleteContextImpl(buffer, oh, ClassReflector(), null
				);
			DeleteMembers(context, typeId, false);
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			CorrectHandlerVersion(context).Delete(context);
		}

		internal virtual void DeleteMembers(DeleteContextImpl context, int a_type, bool isUpdate
			)
		{
			StatefulBuffer buffer = (StatefulBuffer)context.Buffer();
			int preserveCascade = context.CascadeDeleteDepth();
			try
			{
				if (CascadeOnDelete())
				{
					if (ClassReflector().IsCollection())
					{
						buffer.SetCascadeDeletes(CollectionDeleteDepth(context));
					}
					else
					{
						buffer.SetCascadeDeletes(1);
					}
				}
				FieldAwareTypeHandler(CorrectHandlerVersion(context)).DeleteMembers(context, isUpdate
					);
			}
			catch (Exception e)
			{
				// This a catch for changed class hierarchies.
				// It's very ugly to catch all here but it does
				// help to heal migration from earlier db4o
				// versions.
				DiagnosticProcessor dp = Container()._handlers._diagnosticProcessor;
				if (dp.Enabled())
				{
					dp.DeletionFailed();
				}
			}
			buffer.SetCascadeDeletes(preserveCascade);
		}

		private int CollectionDeleteDepth(DeleteContextImpl context)
		{
			int depth = Reflector().CollectionUpdateDepth(ClassReflector()) - 2;
			// Minus two ???  
			if (depth < 1)
			{
				depth = 1;
			}
			return depth;
		}

		public virtual TernaryBool CascadeOnDeleteTernary()
		{
			Config4Class config = Config();
			TernaryBool cascadeOnDelete = TernaryBool.Unspecified;
			if (config != null && (cascadeOnDelete = config.CascadeOnDelete()) != TernaryBool
				.Unspecified)
			{
				return cascadeOnDelete;
			}
			if (i_ancestor == null)
			{
				return cascadeOnDelete;
			}
			return i_ancestor.CascadeOnDeleteTernary();
		}

		public virtual bool CascadeOnDelete()
		{
			return CascadeOnDeleteTernary() == TernaryBool.Yes;
		}

		public bool DispatchEvent(Transaction trans, object obj, int message)
		{
			if (!DispatchingEvents(trans))
			{
				return true;
			}
			return _eventDispatcher.Dispatch(trans, obj, message);
		}

		private bool DispatchingEvents(Transaction trans)
		{
			return _eventDispatcher != null && trans.Container().DispatchsEvents();
		}

		public bool HasEventRegistered(Transaction trans, int eventID)
		{
			if (!DispatchingEvents(trans))
			{
				return false;
			}
			return _eventDispatcher.HasEventRegistered(eventID);
		}

		public int DeclaredAspectCount()
		{
			if (_aspects == null)
			{
				return 0;
			}
			return _aspects.Length;
		}

		public int AspectCount()
		{
			int count = DeclaredAspectCount();
			if (i_ancestor != null)
			{
				count += i_ancestor.AspectCount();
			}
			return count;
		}

		// Scrolls offset in passed reader to the offset the passed field should
		// be read at.
		public HandlerVersion SeekToField(Transaction trans, ByteArrayBuffer buffer, FieldMetadata
			 field)
		{
			if (buffer == null)
			{
				return HandlerVersion.Invalid;
			}
			if (!FirstClassObjectHandlerIsUsed())
			{
				return HandlerVersion.Invalid;
			}
			buffer.Seek(0);
			ObjectHeader oh = new ObjectHeader(_container, this, buffer);
			bool res = SeekToField(new ObjectHeaderContext(trans, buffer, oh), field);
			if (!res)
			{
				return HandlerVersion.Invalid;
			}
			return new HandlerVersion(oh.HandlerVersion());
		}

		public bool SeekToField(ObjectHeaderContext context, FieldMetadata field)
		{
			return FieldAwareTypeHandler(CorrectHandlerVersion(context)).SeekToField(context, 
				field);
		}

		public virtual bool GenerateUUIDs()
		{
			if (!GenerateVirtual())
			{
				return false;
			}
			bool configValue = (i_config == null) ? false : i_config.GenerateUUIDs();
			return Generate1(_container.Config().GenerateUUIDs(), configValue);
		}

		private bool GenerateVersionNumbers()
		{
			if (!GenerateVirtual())
			{
				return false;
			}
			bool configValue = (i_config == null) ? false : i_config.GenerateVersionNumbers();
			return Generate1(_container.Config().GenerateVersionNumbers(), configValue);
		}

		private bool GenerateVirtual()
		{
			if (_unversioned)
			{
				return false;
			}
			if (_internal)
			{
				return false;
			}
			return true;
		}

		private bool Generate1(ConfigScope globalConfig, bool individualConfig)
		{
			return globalConfig.ApplyConfig(individualConfig);
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata GetAncestor()
		{
			return i_ancestor;
		}

		public virtual object GetComparableObject(object forObject)
		{
			if (i_config != null)
			{
				if (i_config.QueryAttributeProvider() != null)
				{
					return i_config.QueryAttributeProvider().Attribute(forObject);
				}
			}
			return forObject;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata GetHigherHierarchy(Db4objects.Db4o.Internal.ClassMetadata
			 a_yapClass)
		{
			Db4objects.Db4o.Internal.ClassMetadata yc = GetHigherHierarchy1(a_yapClass);
			if (yc != null)
			{
				return yc;
			}
			return a_yapClass.GetHigherHierarchy1(this);
		}

		private Db4objects.Db4o.Internal.ClassMetadata GetHigherHierarchy1(Db4objects.Db4o.Internal.ClassMetadata
			 a_yapClass)
		{
			if (a_yapClass == this)
			{
				return this;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.GetHigherHierarchy1(a_yapClass);
			}
			return null;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata GetHigherOrCommonHierarchy(
			Db4objects.Db4o.Internal.ClassMetadata a_yapClass)
		{
			Db4objects.Db4o.Internal.ClassMetadata yc = GetHigherHierarchy1(a_yapClass);
			if (yc != null)
			{
				return yc;
			}
			if (i_ancestor != null)
			{
				yc = i_ancestor.GetHigherOrCommonHierarchy(a_yapClass);
				if (yc != null)
				{
					return yc;
				}
			}
			return a_yapClass.GetHigherHierarchy1(this);
		}

		public override byte GetIdentifier()
		{
			return Const4.Yapclass;
		}

		public virtual long[] GetIDs()
		{
			lock (Lock())
			{
				if (!StateOK())
				{
					return new long[0];
				}
				return GetIDs(_container.Transaction());
			}
		}

		public virtual long[] GetIDs(Transaction trans)
		{
			lock (Lock())
			{
				if (!StateOK())
				{
					return new long[0];
				}
				if (!HasClassIndex())
				{
					return new long[0];
				}
				return trans.Container().GetIDsForClass(trans, this);
			}
		}

		public virtual bool HasClassIndex()
		{
			if (!_classIndexed)
			{
				return false;
			}
			return FirstClassObjectHandlerIsUsed() || !(_typeHandler is IEmbeddedTypeHandler);
		}

		private bool AncestorHasUUIDField()
		{
			if (i_ancestor == null)
			{
				return false;
			}
			return i_ancestor.HasUUIDField();
		}

		private bool HasUUIDField()
		{
			if (AncestorHasUUIDField())
			{
				return true;
			}
			return Arrays4.ContainsInstanceOf(_aspects, typeof(UUIDFieldMetadata));
		}

		private bool AncestorHasVersionField()
		{
			if (i_ancestor == null)
			{
				return false;
			}
			return i_ancestor.HasVersionField();
		}

		private bool HasVersionField()
		{
			if (AncestorHasVersionField())
			{
				return true;
			}
			return Arrays4.ContainsInstanceOf(_aspects, typeof(VersionFieldMetadata));
		}

		public virtual IClassIndexStrategy Index()
		{
			return _index;
		}

		public virtual int IndexEntryCount(Transaction ta)
		{
			if (!StateOK())
			{
				return 0;
			}
			return _index.EntryCount(ta);
		}

		public object IndexEntryToObject(IContext context, object indexEntry)
		{
			if (indexEntry == null)
			{
				return null;
			}
			int id = ((int)indexEntry);
			return Container().GetByID2(context.Transaction(), id);
		}

		public virtual IReflectClass ClassReflector()
		{
			return _classReflector;
		}

		public virtual string GetName()
		{
			if (i_name == null)
			{
				if (_classReflector != null)
				{
					i_name = _classReflector.GetName();
				}
			}
			return i_name;
		}

		public virtual IStoredClass GetParentStoredClass()
		{
			return GetAncestor();
		}

		public virtual IStoredField[] GetStoredFields()
		{
			lock (Lock())
			{
				if (_aspects == null)
				{
					return new IStoredField[0];
				}
				Collection4 storedFields = new Collection4();
				ForEachDeclaredField(new _IProcedure4_903(storedFields));
				IStoredField[] fields = new IStoredField[storedFields.Size()];
				storedFields.ToArray(fields);
				return fields;
			}
		}

		private sealed class _IProcedure4_903 : IProcedure4
		{
			public _IProcedure4_903(Collection4 storedFields)
			{
				this.storedFields = storedFields;
			}

			public void Apply(object field)
			{
				storedFields.Add(field);
			}

			private readonly Collection4 storedFields;
		}

		internal ObjectContainerBase Container()
		{
			return _container;
		}

		public virtual FieldMetadata FieldMetadataForName(string name)
		{
			ByReference byReference = new ByReference();
			ForEachField(new _IProcedure4_920(name, byReference));
			return (FieldMetadata)byReference.value;
		}

		private sealed class _IProcedure4_920 : IProcedure4
		{
			public _IProcedure4_920(string name, ByReference byReference)
			{
				this.name = name;
				this.byReference = byReference;
			}

			public void Apply(object arg)
			{
				if (name.Equals(((FieldMetadata)arg).GetName()))
				{
					byReference.value = arg;
				}
			}

			private readonly string name;

			private readonly ByReference byReference;
		}

		/// <param name="container"></param>
		public virtual bool HasField(ObjectContainerBase container, string fieldName)
		{
			if (ClassReflector().IsCollection())
			{
				return true;
			}
			return FieldMetadataForName(fieldName) != null;
		}

		internal virtual bool HasVirtualAttributes()
		{
			if (_internal)
			{
				return false;
			}
			return HasVersionField() || HasUUIDField();
		}

		public virtual bool HoldsAnyClass()
		{
			return ClassReflector().IsCollection();
		}

		internal virtual void IncrementFieldsOffset1(ByteArrayBuffer a_bytes)
		{
			int length = ReadAspectCount(a_bytes);
			for (int i = 0; i < length; i++)
			{
				_aspects[i].IncrementOffset(a_bytes);
			}
		}

		internal bool Init(ObjectContainerBase a_stream, Db4objects.Db4o.Internal.ClassMetadata
			 a_ancestor, IReflectClass claxx)
		{
			if (DTrace.enabled)
			{
				DTrace.ClassmetadataInit.Log(GetID());
			}
			SetAncestor(a_ancestor);
			Config4Impl config = a_stream.ConfigImpl();
			string className = claxx.GetName();
			SetConfig(config.ConfigClass(className));
			if (!CreateConstructor(a_stream, claxx, className, false))
			{
				return false;
			}
			CheckType();
			if (AllowsQueries())
			{
				_index.Initialize(a_stream);
			}
			i_name = className;
			i_ancestor = a_ancestor;
			BitTrue(Const4.CheckedChanges);
			return true;
		}

		internal void InitConfigOnUp(Transaction systemTrans)
		{
			Config4Class extendedConfig = Platform4.ExtendConfiguration(_classReflector, _container
				.Configure(), i_config);
			if (extendedConfig != null)
			{
				i_config = extendedConfig;
			}
			if (i_config == null)
			{
				return;
			}
			if (!StateOK())
			{
				return;
			}
			if (_aspects == null)
			{
				return;
			}
			for (int i = 0; i < _aspects.Length; i++)
			{
				if (_aspects[i] is FieldMetadata)
				{
					FieldMetadata field = (FieldMetadata)_aspects[i];
					string fieldName = field.GetName();
					if (!field.HasConfig() && extendedConfig != null && extendedConfig.ConfigField(fieldName
						) != null)
					{
						field.InitIndex(this, fieldName);
					}
					field.InitConfigOnUp(systemTrans);
				}
			}
		}

		internal virtual void InitOnUp(Transaction systemTrans)
		{
			if (!StateOK())
			{
				return;
			}
			InitConfigOnUp(systemTrans);
			StoreStaticFieldValues(systemTrans, false);
		}

		public virtual object Instantiate(UnmarshallingContext context)
		{
			// overridden in YapClassPrimitive
			// never called for primitive YapAny
			// FIXME: [TA] no longer necessary?
			//        context.adjustInstantiationDepth();
			object obj = context.PersistentObject();
			bool instantiating = (obj == null);
			if (instantiating)
			{
				obj = InstantiateObject(context);
				if (obj == null)
				{
					return null;
				}
				ShareTransaction(obj, context.Transaction());
				ShareObjectReference(obj, context.ObjectReference());
				context.SetObjectWeak(obj);
				context.Transaction().ReferenceSystem().AddExistingReference(context.ObjectReference
					());
				ObjectOnInstantiate(context.Transaction(), obj);
				if (!context.ActivationDepth().RequiresActivation())
				{
					context.ObjectReference().SetStateDeactivated();
				}
				else
				{
					obj = Activate(context);
				}
			}
			else
			{
				if (ActivatingActiveObject(context.ActivationDepth().Mode(), context.ObjectReference
					()))
				{
					IActivationDepth child = context.ActivationDepth().Descend(this);
					if (child.RequiresActivation())
					{
						ActivateFields(context.Transaction(), obj, child);
					}
				}
				else
				{
					obj = Activate(context);
				}
			}
			return obj;
		}

		public virtual object InstantiateTransient(UnmarshallingContext context)
		{
			// overridden in YapClassPrimitive
			// never called for primitive YapAny
			object obj = InstantiateObject(context);
			if (obj == null)
			{
				return null;
			}
			context.Container().Peeked(context.ObjectID(), obj);
			if (context.ActivationDepth().RequiresActivation())
			{
				obj = InstantiateFields(context);
			}
			return obj;
		}

		private bool ActivatingActiveObject(ActivationMode mode, ObjectReference @ref)
		{
			return !mode.IsRefresh() && @ref.IsActive();
		}

		private object Activate(UnmarshallingContext context)
		{
			object obj = context.PersistentObject();
			if (!ObjectCanActivate(context.Transaction(), obj))
			{
				context.ObjectReference().SetStateDeactivated();
				return obj;
			}
			context.ObjectReference().SetStateClean();
			if (context.ActivationDepth().RequiresActivation())
			{
				obj = InstantiateFields(context);
			}
			ObjectOnActivate(context.Transaction(), obj);
			return obj;
		}

		private bool ConfigInstantiates()
		{
			return Config() != null && Config().Instantiates();
		}

		private object InstantiateObject(UnmarshallingContext context)
		{
			object obj = ConfigInstantiates() ? InstantiateFromConfig(context) : InstantiateFromReflector
				(context.Container());
			context.PersistentObject(obj);
			return obj;
		}

		private void ObjectOnInstantiate(Transaction transaction, object instance)
		{
			transaction.Container().Callbacks().ObjectOnInstantiate(transaction, instance);
		}

		public object InstantiateFromReflector(ObjectContainerBase stream)
		{
			if (_classReflector == null)
			{
				return null;
			}
			stream.Instantiating(true);
			try
			{
				return _classReflector.NewInstance();
			}
			catch (MissingMethodException)
			{
				stream.LogMsg(7, ClassReflector().GetName());
				return null;
			}
			catch (Exception)
			{
				// TODO: be more helpful here
				return null;
			}
			finally
			{
				stream.Instantiating(false);
			}
		}

		private object InstantiateFromConfig(ObjectReferenceContext context)
		{
			ContextState contextState = context.SaveState();
			bool fieldHasValue = SeekToField(context, _translator);
			try
			{
				return i_config.Instantiate(context.Container(), fieldHasValue ? _translator.Read
					(context) : null);
			}
			finally
			{
				context.RestoreState(contextState);
			}
		}

		private void ShareObjectReference(object obj, ObjectReference @ref)
		{
			if (obj is IDb4oTypeImpl)
			{
				((IDb4oTypeImpl)obj).SetObjectReference(@ref);
			}
		}

		private void ShareTransaction(object obj, Transaction transaction)
		{
			if (obj is ITransactionAware)
			{
				((ITransactionAware)obj).SetTrans(transaction);
			}
		}

		private void ObjectOnActivate(Transaction transaction, object obj)
		{
			ObjectContainerBase container = transaction.Container();
			container.Callbacks().ObjectOnActivate(transaction, obj);
			DispatchEvent(transaction, obj, EventDispatcher.Activate);
		}

		private bool ObjectCanActivate(Transaction transaction, object obj)
		{
			ObjectContainerBase container = transaction.Container();
			return container.Callbacks().ObjectCanActivate(transaction, obj) && DispatchEvent
				(transaction, obj, EventDispatcher.CanActivate);
		}

		internal virtual object InstantiateFields(UnmarshallingContext context)
		{
			return Read(context);
		}

		public virtual bool IsArray()
		{
			return ClassReflector().IsCollection();
		}

		internal virtual bool IsCollection(object obj)
		{
			return Reflector().ForObject(obj).IsCollection();
		}

		public override bool IsDirty()
		{
			if (!StateOK())
			{
				return false;
			}
			return base.IsDirty();
		}

		internal virtual bool IsEnum()
		{
			return _isEnum;
		}

		public virtual bool IsPrimitive()
		{
			return false;
		}

		/// <summary>no any, primitive, array or other tricks.</summary>
		/// <remarks>
		/// no any, primitive, array or other tricks. overriden in YapClassAny and
		/// YapClassPrimitive
		/// </remarks>
		public virtual bool IsStrongTyped()
		{
			return true;
		}

		public virtual bool IsValueType()
		{
			return Platform4.IsValueType(ClassReflector());
		}

		private object Lock()
		{
			return _container.Lock();
		}

		public virtual string NameToWrite()
		{
			if (i_config != null && i_config.WriteAs() != null)
			{
				return i_config.WriteAs();
			}
			if (i_name == null)
			{
				return string.Empty;
			}
			return _container.ConfigImpl().ResolveAliasRuntimeName(i_name);
		}

		public bool CallConstructor()
		{
			TernaryBool specialized = CallConstructorSpecialized();
			// FIXME: If specified, return yes?!?
			if (!specialized.IsUnspecified())
			{
				return specialized.DefiniteYes();
			}
			return _container.ConfigImpl().CallConstructors().DefiniteYes();
		}

		private TernaryBool CallConstructorSpecialized()
		{
			if (i_config != null)
			{
				TernaryBool res = i_config.CallConstructor();
				if (!res.IsUnspecified())
				{
					return res;
				}
			}
			if (_isEnum)
			{
				return TernaryBool.No;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.CallConstructorSpecialized();
			}
			return TernaryBool.Unspecified;
		}

		public override int OwnLength()
		{
			return MarshallerFamily.Current()._class.MarshalledLength(_container, this);
		}

		public virtual int PrefetchActivationDepth()
		{
			// We only allow prefetching, if there is no special configuration for the class. 
			// This was a fix for a problem instantiating Hashtables. There may be a better 
			// workaround that also works for configured objects.
			//
			// An instantiation depth of 1 makes use of possibly prefetched strings and 
			// arrays that are carried around in the buffer anyway
			//
			// TODO: optimize
			return ConfigOrAncestorConfig() == null ? 1 : 0;
		}

		internal virtual void Purge()
		{
			_index.Purge();
		}

		// TODO: may want to add manual purge to Btree
		//       indexes here
		// FIXME: [TA] ActivationDepth review
		public virtual object ReadAndActivate(Transaction trans, int id, IActivationDepth
			 depth)
		{
			// Method for C# value types and for map members:
			// they need to be instantiated before setting them
			// on the parent object. 
			// For value types the set call modifies identity.
			// In maps, adding the object to the map calls #hashCode and #equals,
			// so the object needs to be activated.
			// TODO: Question: Do we want value types in the ID tree?
			// Shouldn't we treat them like strings and update
			// them every time ???		
			ObjectReference @ref = trans.ReferenceForId(id);
			if (@ref != null)
			{
				object obj = @ref.GetObject();
				if (obj == null)
				{
					trans.RemoveReference(@ref);
				}
				else
				{
					@ref.Activate(trans, obj, depth);
					return @ref.GetObject();
				}
			}
			return new ObjectReference(id).Read(trans, depth, Const4.AddToIdTree, false);
		}

		public virtual ITypeHandler4 ReadCandidateHandler(QueryingReadContext context)
		{
			ITypeHandler4 typeHandler = CorrectHandlerVersion(context);
			if (typeHandler is IFirstClassHandler)
			{
				return ((IFirstClassHandler)typeHandler).ReadCandidateHandler(context);
			}
			return null;
		}

		public virtual ITypeHandler4 SeekCandidateHandler(QueryingReadContext context)
		{
			if (IsArray())
			{
				if (Platform4.IsCollectionTranslator(this.i_config))
				{
					context.Seek(context.Offset() + Const4.IntLength);
					return new ArrayHandler(null, false);
				}
				IncrementFieldsOffset1((ByteArrayBuffer)context.Buffer());
				if (i_ancestor != null)
				{
					return i_ancestor.SeekCandidateHandler(context);
				}
			}
			return null;
		}

		public virtual ObjectID ReadObjectID(IInternalReadContext context)
		{
			return ObjectID.Read(context);
		}

		public int ReadAspectCount(IReadBuffer buffer)
		{
			int count = buffer.ReadInt();
			if (count > _aspects.Length)
			{
				return _aspects.Length;
			}
			return count;
		}

		public object ReadIndexEntry(ByteArrayBuffer a_reader)
		{
			return a_reader.ReadInt();
		}

		/// <exception cref="CorruptionException"></exception>
		public object ReadIndexEntryFromObjectSlot(MarshallerFamily mf, StatefulBuffer a_writer
			)
		{
			return ReadIndexEntry(a_writer);
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="Db4oIOException"></exception>
		public virtual object ReadIndexEntry(IObjectIdContext context)
		{
			return context.ReadInt();
		}

		internal virtual byte[] ReadName(Transaction a_trans)
		{
			i_reader = a_trans.Container().ReadReaderByID(a_trans, GetID());
			return ReadName1(a_trans, i_reader);
		}

		public byte[] ReadName1(Transaction trans, ByteArrayBuffer reader)
		{
			if (reader == null)
			{
				return null;
			}
			i_reader = reader;
			bool ok = false;
			try
			{
				ClassMarshaller marshaller = MarshallerFamily.Current()._class;
				i_nameBytes = marshaller.ReadName(trans, reader);
				marshaller.ReadMetaClassID(reader);
				// never used ???
				SetStateUnread();
				BitFalse(Const4.CheckedChanges);
				BitFalse(Const4.StaticFieldsStored);
				ok = true;
				return i_nameBytes;
			}
			finally
			{
				if (!ok)
				{
					SetStateDead();
				}
			}
		}

		public virtual void ReadVirtualAttributes(Transaction trans, ObjectReference @ref
			, bool lastCommitted)
		{
			int id = @ref.GetID();
			ObjectContainerBase stream = trans.Container();
			ByteArrayBuffer buffer = stream.ReadReaderByID(trans, id, lastCommitted);
			ObjectHeader oh = new ObjectHeader(stream, this, buffer);
			ObjectReferenceContext context = new ObjectReferenceContext(trans, buffer, oh, @ref
				);
			FieldAwareTypeHandler(CorrectHandlerVersion(context)).ReadVirtualAttributes(context
				);
		}

		public virtual GenericReflector Reflector()
		{
			return _container.Reflector();
		}

		public virtual void Rename(string newName)
		{
			if (!_container.IsClient())
			{
				int tempState = _state;
				SetStateOK();
				i_name = newName;
				i_nameBytes = AsBytes(i_name);
				SetStateDirty();
				Write(_container.SystemTransaction());
				IReflectClass oldReflector = _classReflector;
				ClassReflector(Container().Reflector().ForName(newName));
				Container().ClassCollection().RefreshClassCache(this, oldReflector);
				Refresh();
				_state = tempState;
			}
			else
			{
				Exceptions4.ThrowRuntimeException(58);
			}
		}

		//TODO: duplicates ClassMetadataRepository#asBytes
		private byte[] AsBytes(string str)
		{
			return Container().StringIO().Write(str);
		}

		internal void CreateConfigAndConstructor(Hashtable4 a_byteHashTable, IReflectClass
			 claxx, string name)
		{
			i_name = name;
			SetConfig(_container.ConfigImpl().ConfigClass(i_name));
			if (claxx == null)
			{
				CreateConstructor(_container, i_name);
			}
			else
			{
				CreateConstructor(_container, claxx, i_name, true);
			}
			if (i_nameBytes != null)
			{
				a_byteHashTable.Remove(i_nameBytes);
				i_nameBytes = null;
			}
		}

		internal virtual string ResolveName(IReflectClass claxx)
		{
			if (claxx != null)
			{
				return claxx.GetName();
			}
			if (i_nameBytes != null)
			{
				string name = _container.StringIO().Read(i_nameBytes);
				return _container.ConfigImpl().ResolveAliasStoredName(name);
			}
			throw new InvalidOperationException();
		}

		internal virtual bool ReadThis()
		{
			bool stateUnread = StateUnread();
			if (stateUnread)
			{
				SetStateOK();
				SetStateClean();
			}
			if (stateUnread || StateDead())
			{
				ForceRead();
				return true;
			}
			return false;
		}

		internal void ForceRead()
		{
			if (i_reader == null || BitIsTrue(Const4.Reading))
			{
				return;
			}
			BitTrue(Const4.Reading);
			MarshallerFamily.ForConverterVersion(_container.ConverterVersion())._class.Read(_container
				, this, i_reader);
			i_nameBytes = null;
			i_reader = null;
			BitFalse(Const4.Reading);
		}

		public override void ReadThis(Transaction a_trans, ByteArrayBuffer a_reader)
		{
			throw Exceptions4.VirtualException();
		}

		public virtual void Refresh()
		{
			if (!StateUnread())
			{
				CreateConstructor(_container, i_name);
				BitFalse(Const4.CheckedChanges);
				CheckChanges();
				ForEachDeclaredField(new _IProcedure4_1486());
			}
		}

		private sealed class _IProcedure4_1486 : IProcedure4
		{
			public _IProcedure4_1486()
			{
			}

			public void Apply(object arg)
			{
				((FieldMetadata)arg).Refresh();
			}
		}

		internal virtual void RemoveFromIndex(Transaction ta, int id)
		{
			if (HasClassIndex())
			{
				_index.Remove(ta, id);
			}
			if (i_ancestor != null)
			{
				i_ancestor.RemoveFromIndex(ta, id);
			}
		}

		internal virtual bool RenameField(string oldName, string newName)
		{
			BooleanByRef renamed = new BooleanByRef(false);
			for (int i = 0; i < _aspects.Length; i++)
			{
				if (_aspects[i].GetName().Equals(newName))
				{
					_container.LogMsg(9, "class:" + GetName() + " field:" + newName);
					return false;
				}
			}
			ForEachDeclaredField(new _IProcedure4_1511(oldName, newName, renamed));
			return renamed.value;
		}

		private sealed class _IProcedure4_1511 : IProcedure4
		{
			public _IProcedure4_1511(string oldName, string newName, BooleanByRef renamed)
			{
				this.oldName = oldName;
				this.newName = newName;
				this.renamed = renamed;
			}

			public void Apply(object arg)
			{
				FieldMetadata field = (FieldMetadata)arg;
				if (field.GetName().Equals(oldName))
				{
					field.SetName(newName);
					renamed.value = true;
				}
			}

			private readonly string oldName;

			private readonly string newName;

			private readonly BooleanByRef renamed;
		}

		internal virtual void SetConfig(Config4Class config)
		{
			if (config == null)
			{
				return;
			}
			// The configuration can be set by a ObjectClass#readAs setting
			// from YapClassCollection, right after reading the meta information
			// for the first time. In that case we never change the setting
			if (i_config == null)
			{
				i_config = config;
			}
		}

		internal virtual void SetName(string a_name)
		{
			i_name = a_name;
		}

		internal void SetStateDead()
		{
			BitTrue(Const4.Dead);
			BitFalse(Const4.Continue);
		}

		private void SetStateUnread()
		{
			BitFalse(Const4.Dead);
			BitTrue(Const4.Continue);
		}

		internal void SetStateOK()
		{
			BitFalse(Const4.Dead);
			BitFalse(Const4.Continue);
		}

		internal virtual bool StateDead()
		{
			return BitIsTrue(Const4.Dead);
		}

		private bool StateOK()
		{
			return BitIsFalse(Const4.Continue) && BitIsFalse(Const4.Dead) && BitIsFalse(Const4
				.Reading);
		}

		internal bool StateOKAndAncestors()
		{
			if (!StateOK() || _aspects == null)
			{
				return false;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.StateOKAndAncestors();
			}
			return true;
		}

		internal virtual bool StateUnread()
		{
			return BitIsTrue(Const4.Continue) && BitIsFalse(Const4.Dead) && BitIsFalse(Const4
				.Reading);
		}

		[System.ObsoleteAttribute]
		internal virtual bool StoreField(IReflectField a_field)
		{
			if (a_field.IsStatic())
			{
				return false;
			}
			if (a_field.IsTransient())
			{
				Config4Class config = ConfigOrAncestorConfig();
				if (config == null)
				{
					return false;
				}
				if (!config.StoreTransientFields())
				{
					return false;
				}
			}
			return Platform4.CanSetAccessible() || a_field.IsPublic();
		}

		public virtual IStoredField StoredField(string name, object clazz)
		{
			lock (Lock())
			{
				Db4objects.Db4o.Internal.ClassMetadata classMetadata = _container.ClassMetadataForReflectClass
					(ReflectorUtils.ReflectClassFor(Reflector(), clazz));
				ByReference foundField = new ByReference();
				ForEachField(new _IProcedure4_1607(this, foundField, name, classMetadata));
				// FIXME: The == comparison in the following line could be wrong. 
				//TODO: implement field creation
				return (IStoredField)foundField.value;
			}
		}

		private sealed class _IProcedure4_1607 : IProcedure4
		{
			public _IProcedure4_1607(ClassMetadata _enclosing, ByReference foundField, string
				 name, Db4objects.Db4o.Internal.ClassMetadata classMetadata)
			{
				this._enclosing = _enclosing;
				this.foundField = foundField;
				this.name = name;
				this.classMetadata = classMetadata;
			}

			public void Apply(object arg)
			{
				if (foundField.value != null)
				{
					return;
				}
				FieldMetadata field = (FieldMetadata)arg;
				if (field.GetName().Equals(name))
				{
					if (classMetadata == null || classMetadata == field.HandlerClassMetadata(this._enclosing
						._container))
					{
						foundField.value = field;
					}
				}
			}

			private readonly ClassMetadata _enclosing;

			private readonly ByReference foundField;

			private readonly string name;

			private readonly Db4objects.Db4o.Internal.ClassMetadata classMetadata;
		}

		internal virtual void StoreStaticFieldValues(Transaction trans, bool force)
		{
			if (BitIsTrue(Const4.StaticFieldsStored) && !force)
			{
				return;
			}
			BitTrue(Const4.StaticFieldsStored);
			if (!ShouldStoreStaticFields(trans))
			{
				return;
			}
			ObjectContainerBase stream = trans.Container();
			stream.ShowInternalClasses(true);
			try
			{
				StaticClass sc = QueryStaticClass(trans);
				if (sc == null)
				{
					CreateStaticClass(trans);
				}
				else
				{
					UpdateStaticClass(trans, sc);
				}
			}
			finally
			{
				stream.ShowInternalClasses(false);
			}
		}

		private bool ShouldStoreStaticFields(Transaction trans)
		{
			return !IsReadOnlyContainer(trans) && (StaticFieldValuesArePersisted() || Platform4
				.StoreStaticFieldValues(trans.Reflector(), ClassReflector()));
		}

		private bool IsReadOnlyContainer(Transaction trans)
		{
			return trans.Container().Config().IsReadOnly();
		}

		private void UpdateStaticClass(Transaction trans, StaticClass sc)
		{
			ObjectContainerBase stream = trans.Container();
			stream.Activate(trans, sc, new FixedActivationDepth(4));
			StaticField[] existingFields = sc.fields;
			IEnumerator staticFields = Iterators.Map(StaticReflectFields(), new _IFunction4_1670
				(this, existingFields, trans));
			sc.fields = ToStaticFieldArray(staticFields);
			if (!stream.IsClient())
			{
				SetStaticClass(trans, sc);
			}
		}

		private sealed class _IFunction4_1670 : IFunction4
		{
			public _IFunction4_1670(ClassMetadata _enclosing, StaticField[] existingFields, Transaction
				 trans)
			{
				this._enclosing = _enclosing;
				this.existingFields = existingFields;
				this.trans = trans;
			}

			public object Apply(object arg)
			{
				IReflectField reflectField = (IReflectField)arg;
				StaticField existingField = this._enclosing.FieldByName(existingFields, reflectField
					.GetName());
				if (existingField != null)
				{
					this._enclosing.UpdateExistingStaticField(trans, existingField, reflectField);
					return existingField;
				}
				return this._enclosing.ToStaticField(reflectField);
			}

			private readonly ClassMetadata _enclosing;

			private readonly StaticField[] existingFields;

			private readonly Transaction trans;
		}

		private void CreateStaticClass(Transaction trans)
		{
			if (trans.Container().IsClient())
			{
				return;
			}
			StaticClass sc = new StaticClass(i_name, ToStaticFieldArray(StaticReflectFieldsToStaticFields
				()));
			SetStaticClass(trans, sc);
		}

		private IEnumerator StaticReflectFieldsToStaticFields()
		{
			return Iterators.Map(StaticReflectFields(), new _IFunction4_1698(this));
		}

		private sealed class _IFunction4_1698 : IFunction4
		{
			public _IFunction4_1698(ClassMetadata _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object arg)
			{
				return this._enclosing.ToStaticField((IReflectField)arg);
			}

			private readonly ClassMetadata _enclosing;
		}

		protected virtual StaticField ToStaticField(IReflectField reflectField)
		{
			return new StaticField(reflectField.GetName(), StaticReflectFieldValue(reflectField
				));
		}

		private object StaticReflectFieldValue(IReflectField reflectField)
		{
			return reflectField.Get(null);
		}

		private void SetStaticClass(Transaction trans, StaticClass sc)
		{
			// TODO: we should probably use a specific update depth here, 4?
			trans.Container().StoreInternal(trans, sc, true);
		}

		private StaticField[] ToStaticFieldArray(IEnumerator iterator4)
		{
			return ToStaticFieldArray(new Collection4(iterator4));
		}

		private StaticField[] ToStaticFieldArray(Collection4 fields)
		{
			return (StaticField[])fields.ToArray(new StaticField[fields.Size()]);
		}

		private IEnumerator StaticReflectFields()
		{
			return Iterators.Filter(ReflectFields(), new _IPredicate4_1727());
		}

		private sealed class _IPredicate4_1727 : IPredicate4
		{
			public _IPredicate4_1727()
			{
			}

			public bool Match(object candidate)
			{
				return ((IReflectField)candidate).IsStatic();
			}
		}

		private IReflectField[] ReflectFields()
		{
			return ClassReflector().GetDeclaredFields();
		}

		protected virtual void UpdateExistingStaticField(Transaction trans, StaticField existingField
			, IReflectField reflectField)
		{
			ObjectContainerBase stream = trans.Container();
			object newValue = StaticReflectFieldValue(reflectField);
			if (existingField.value != null && newValue != null && existingField.value.GetType
				() == newValue.GetType())
			{
				int id = stream.GetID(trans, existingField.value);
				if (id > 0)
				{
					if (existingField.value != newValue)
					{
						// This is the clue:
						// Bind the current static member to it's old database identity,
						// so constants and enums will work with '=='
						stream.Bind(trans, newValue, id);
						// This may produce unwanted side effects if the static field object
						// was modified in the current session. TODO:Add documentation case.
						stream.Refresh(trans, newValue, int.MaxValue);
						existingField.value = newValue;
					}
					return;
				}
			}
			if (newValue == null)
			{
				try
				{
					reflectField.Set(null, existingField.value);
				}
				catch (Exception)
				{
				}
				// fail silently
				// TODO: why?
				return;
			}
			existingField.value = newValue;
		}

		private bool StaticFieldValuesArePersisted()
		{
			return (i_config != null && i_config.StaticFieldValuesArePersisted());
		}

		protected virtual StaticField FieldByName(StaticField[] fields, string fieldName)
		{
			for (int i = 0; i < fields.Length; i++)
			{
				StaticField field = fields[i];
				if (fieldName.Equals(field.name))
				{
					return field;
				}
			}
			return null;
		}

		private StaticClass QueryStaticClass(Transaction trans)
		{
			IQuery q = trans.Container().Query(trans);
			q.Constrain(Const4.ClassStaticclass);
			q.Descend("name").Constrain(i_name);
			IObjectSet os = q.Execute();
			return os.Count > 0 ? (StaticClass)os.Next() : null;
		}

		public override string ToString()
		{
			if (i_name != null)
			{
				return i_name;
			}
			if (i_nameBytes == null)
			{
				return "*CLASS NAME UNKNOWN*";
			}
			LatinStringIO stringIO = _container == null ? Const4.stringIO : _container.StringIO
				();
			return stringIO.Read(i_nameBytes);
		}

		public override bool WriteObjectBegin()
		{
			if (!StateOK())
			{
				return false;
			}
			return base.WriteObjectBegin();
		}

		public virtual void WriteIndexEntry(ByteArrayBuffer a_writer, object a_object)
		{
			if (a_object == null)
			{
				a_writer.WriteInt(0);
				return;
			}
			a_writer.WriteInt(((int)a_object));
		}

		public sealed override void WriteThis(Transaction trans, ByteArrayBuffer writer)
		{
			MarshallerFamily.Current()._class.Write(trans, this, writer);
		}

		public virtual IPreparedComparison PrepareComparison(IContext context, object source
			)
		{
			return _typeHandler.PrepareComparison(context, source);
		}

		public static void DefragObject(DefragmentContextImpl context)
		{
			ObjectHeader header = ObjectHeader.Defrag(context);
			DefragmentContextImpl childContext = new DefragmentContextImpl(context, header);
			header.ClassMetadata().Defragment(childContext);
		}

		public virtual void Defragment(IDefragmentContext context)
		{
			CorrectHandlerVersion(context).Defragment(context);
		}

		public virtual void DefragClass(DefragmentContextImpl context, int classIndexID)
		{
			MarshallerFamily mf = MarshallerFamily.ForConverterVersion(Container().ConverterVersion
				());
			mf._class.Defrag(this, _container.StringIO(), context, classIndexID);
		}

		public static Db4objects.Db4o.Internal.ClassMetadata ReadClass(ObjectContainerBase
			 stream, ByteArrayBuffer reader)
		{
			ObjectHeader oh = new ObjectHeader(stream, reader);
			return oh.ClassMetadata();
		}

		public virtual bool IsAssignableFrom(Db4objects.Db4o.Internal.ClassMetadata other
			)
		{
			return ClassReflector().IsAssignableFrom(other.ClassReflector());
		}

		public void DefragIndexEntry(DefragmentContextImpl context)
		{
			context.CopyID();
		}

		public virtual void SetAncestor(Db4objects.Db4o.Internal.ClassMetadata ancestor)
		{
			if (ancestor == this)
			{
				throw new InvalidOperationException();
			}
			i_ancestor = ancestor;
		}

		public virtual object WrapWithTransactionContext(Transaction transaction, object 
			value)
		{
			if (value is int)
			{
				return value;
			}
			return new TransactionContext(transaction, value);
		}

		public virtual object Read(IReadContext context)
		{
			return CorrectHandlerVersion((IHandlerVersionContext)context).Read(context);
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			_typeHandler.Write(context, obj);
		}

		public virtual ITypeHandler4 TypeHandler()
		{
			return this;
		}

		public sealed class PreparedComparisonImpl : IPreparedComparison
		{
			private readonly int _id;

			private readonly IReflectClass _claxx;

			public PreparedComparisonImpl(int id, IReflectClass claxx)
			{
				_id = id;
				_claxx = claxx;
			}

			public int CompareTo(object obj)
			{
				if (obj is TransactionContext)
				{
					obj = ((TransactionContext)obj)._object;
				}
				if (obj == null)
				{
					return 1;
				}
				if (obj is int)
				{
					int targetInt = ((int)obj);
					return _id == targetInt ? 0 : (_id < targetInt ? -1 : 1);
				}
				if (_claxx != null)
				{
					if (_claxx.IsAssignableFrom(_claxx.Reflector().ForObject(obj)))
					{
						return 0;
					}
				}
				throw new IllegalComparisonException();
			}
		}

		protected virtual bool IsSecondClass(ITypeHandler4 handler)
		{
			return Handlers4.BaseTypeHandler(handler) is IEmbeddedTypeHandler;
		}

		public virtual ITypeHandler4 DelegateTypeHandler()
		{
			return _typeHandler;
		}

		public virtual bool IsSecondClass()
		{
			return IsSecondClass(_typeHandler);
		}

		private ITypeHandler4 CorrectHandlerVersion(IHandlerVersionContext context)
		{
			return Handlers4.CorrectHandlerVersion(context, _typeHandler);
		}

		public virtual void ForEachField(IProcedure4 procedure)
		{
			ForEachAspect(new SubTypePredicate(typeof(FieldMetadata)), procedure);
		}

		public virtual void ForEachDeclaredField(IProcedure4 procedure)
		{
			ForEachDeclaredAspect(new SubTypePredicate(typeof(FieldMetadata)), procedure);
		}

		public virtual void ForEachAspect(IPredicate4 predicate, IProcedure4 procedure)
		{
			ClassMetadata classMetadata = this;
			while (classMetadata != null)
			{
				classMetadata.ForEachDeclaredAspect(predicate, procedure);
				classMetadata = classMetadata.i_ancestor;
			}
		}

		public virtual void ForEachAspect(IProcedure4 procedure)
		{
			ClassMetadata classMetadata = this;
			while (classMetadata != null)
			{
				classMetadata.ForEachDeclaredAspect(procedure);
				classMetadata = classMetadata.i_ancestor;
			}
		}

		public virtual void ForEachDeclaredAspect(IPredicate4 predicate, IProcedure4 procedure
			)
		{
			if (_aspects == null)
			{
				return;
			}
			for (int i = 0; i < _aspects.Length; i++)
			{
				if (predicate.Match(_aspects[i]))
				{
					procedure.Apply(_aspects[i]);
				}
			}
		}

		public virtual void ForEachDeclaredAspect(IProcedure4 procedure)
		{
			if (_aspects == null)
			{
				return;
			}
			for (int i = 0; i < _aspects.Length; i++)
			{
				procedure.Apply(_aspects[i]);
			}
		}

		public virtual bool AspectsAreNull()
		{
			return _aspects == null;
		}

		private sealed class AlwaysModified : IModificationAware
		{
			internal static readonly ClassMetadata.AlwaysModified Instance = new ClassMetadata.AlwaysModified
				();

			public bool IsModified(object obj)
			{
				return true;
			}
		}

		public virtual bool IsModified(object obj)
		{
			return _modificationChecker.IsModified(obj);
		}

		public virtual int InstanceCount()
		{
			return InstanceCount(_container.Transaction());
		}

		public virtual int InstanceCount(Transaction trans)
		{
			return _container.InstanceCount(this, trans);
		}
	}
}
