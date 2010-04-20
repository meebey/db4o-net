/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Metadata;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class StandardReferenceTypeHandler : IFieldAwareTypeHandler, IIndexableTypeHandler
		, IReadsObjectIds
	{
		private const int HashcodeForNull = 72483944;

		private Db4objects.Db4o.Internal.ClassMetadata _classMetadata;

		public StandardReferenceTypeHandler(Db4objects.Db4o.Internal.ClassMetadata classMetadata
			)
		{
			ClassMetadata(classMetadata);
		}

		public StandardReferenceTypeHandler()
		{
		}

		public virtual void Defragment(IDefragmentContext context)
		{
			TraverseAllAspects(context, new _MarshallingInfoTraverseAspectCommand_35(context, 
				EnsureFieldList(context)));
		}

		private sealed class _MarshallingInfoTraverseAspectCommand_35 : MarshallingInfoTraverseAspectCommand
		{
			public _MarshallingInfoTraverseAspectCommand_35(IDefragmentContext context, IMarshallingInfo
				 baseArg1) : base(baseArg1)
			{
				this.context = context;
			}

			protected override int InternalDeclaredAspectCount(Db4objects.Db4o.Internal.ClassMetadata
				 classMetadata)
			{
				return context.ReadInt();
			}

			protected override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				)
			{
				if (!isNull)
				{
					aspect.DefragAspect(context);
				}
			}

			public override bool Accept(ClassAspect aspect)
			{
				return aspect.IsEnabledOn(context);
			}

			private readonly IDefragmentContext context;
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			context.DeleteObject();
		}

		public void ActivateAspects(UnmarshallingContext context)
		{
			BooleanByRef schemaUpdateDetected = new BooleanByRef();
			ContextState savedState = context.SaveState();
			ITraverseAspectCommand command = new _MarshallingInfoTraverseAspectCommand_63(context
				, schemaUpdateDetected, EnsureFieldList(context));
			// TODO: cant the aspect handle it itself?
			// Probably no because old aspect versions might not be able
			// to handle null...
			TraverseAllAspects(context, command);
			if (schemaUpdateDetected.value)
			{
				context.RestoreState(savedState);
				command = new _MarshallingInfoTraverseAspectCommand_93(context, EnsureFieldList(context
					));
				TraverseAllAspects(context, command);
			}
		}

		private sealed class _MarshallingInfoTraverseAspectCommand_63 : MarshallingInfoTraverseAspectCommand
		{
			public _MarshallingInfoTraverseAspectCommand_63(UnmarshallingContext context, BooleanByRef
				 schemaUpdateDetected, IMarshallingInfo baseArg1) : base(baseArg1)
			{
				this.context = context;
				this.schemaUpdateDetected = schemaUpdateDetected;
			}

			public override bool Accept(ClassAspect aspect)
			{
				return aspect.IsEnabledOn(context);
			}

			protected override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				)
			{
				if (aspect is FieldMetadata)
				{
					FieldMetadata field = (FieldMetadata)aspect;
					if (field.Updating())
					{
						schemaUpdateDetected.value = true;
					}
					if (isNull)
					{
						field.Set(context.PersistentObject(), null);
						return;
					}
				}
				aspect.Activate(context);
			}

			private readonly UnmarshallingContext context;

			private readonly BooleanByRef schemaUpdateDetected;
		}

		private sealed class _MarshallingInfoTraverseAspectCommand_93 : MarshallingInfoTraverseAspectCommand
		{
			public _MarshallingInfoTraverseAspectCommand_93(UnmarshallingContext context, IMarshallingInfo
				 baseArg1) : base(baseArg1)
			{
				this.context = context;
			}

			protected override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				)
			{
				if (!isNull)
				{
					((FieldMetadata)aspect).AttemptUpdate(context);
				}
			}

			public override bool Accept(ClassAspect aspect)
			{
				return aspect is FieldMetadata;
			}

			private readonly UnmarshallingContext context;
		}

		public virtual void Activate(IReferenceActivationContext context)
		{
			ActivateAspects((UnmarshallingContext)context);
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			MarshallAspects(obj, (MarshallingContext)context);
		}

		public virtual void MarshallAspects(object obj, MarshallingContext context)
		{
			Transaction trans = context.Transaction();
			ITraverseAspectCommand command = new _MarshallingInfoTraverseAspectCommand_120(context
				, obj, trans, EnsureFieldList(context));
			TraverseAllAspects(context, command);
		}

		private sealed class _MarshallingInfoTraverseAspectCommand_120 : MarshallingInfoTraverseAspectCommand
		{
			public _MarshallingInfoTraverseAspectCommand_120(MarshallingContext context, object
				 obj, Transaction trans, IMarshallingInfo baseArg1) : base(baseArg1)
			{
				this.context = context;
				this.obj = obj;
				this.trans = trans;
			}

			protected override int InternalDeclaredAspectCount(Db4objects.Db4o.Internal.ClassMetadata
				 classMetadata)
			{
				int aspectCount = classMetadata._aspects.Length;
				context.WriteDeclaredAspectCount(aspectCount);
				return aspectCount;
			}

			public override bool Accept(ClassAspect aspect)
			{
				return aspect.IsEnabledOn(context);
			}

			protected override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				)
			{
				object marshalledObject = obj;
				if (aspect is FieldMetadata)
				{
					FieldMetadata field = (FieldMetadata)aspect;
					marshalledObject = field.GetOrCreate(trans, obj);
					if (marshalledObject == null)
					{
						context.IsNull(currentSlot, true);
						field.AddIndexEntry(trans, context.ObjectID(), null);
						return;
					}
				}
				aspect.Marshall(context, marshalledObject);
			}

			public override void ProcessAspectOnMissingClass(ClassAspect aspect, int currentSlot
				)
			{
				((MarshallingContext)context).IsNull(currentSlot, true);
			}

			private readonly MarshallingContext context;

			private readonly object obj;

			private readonly Transaction trans;
		}

		public virtual IPreparedComparison PrepareComparison(IContext context, object source
			)
		{
			if (source == null)
			{
				return Null.Instance;
			}
			if (source is int)
			{
				int id = ((int)source);
				return new StandardReferenceTypeHandler.PreparedComparisonImpl(id, null);
			}
			if (source is TransactionContext)
			{
				TransactionContext tc = (TransactionContext)source;
				object obj = tc._object;
				int id = IdFor(obj, tc._transaction);
				return new StandardReferenceTypeHandler.PreparedComparisonImpl(id, ReflectClassFor
					(obj));
			}
			return PlatformComparisonFor(source);
		}

		private IPreparedComparison PlatformComparisonFor(object source)
		{
			//TODO: Move the comparable wrapping to a .Net specific StandardStructHandler
			if (source is IComparable)
			{
				return new _IPreparedComparison_178(source);
			}
			throw new IllegalComparisonException();
		}

		private sealed class _IPreparedComparison_178 : IPreparedComparison
		{
			public _IPreparedComparison_178(object source)
			{
				this.source = source;
			}

			public int CompareTo(object obj)
			{
				IComparable self = (IComparable)source;
				return self.CompareTo(obj);
			}

			private readonly object source;
		}

		private IReflectClass ReflectClassFor(object obj)
		{
			return ClassMetadata().Reflector().ForObject(obj);
		}

		private int IdFor(object @object, Transaction inTransaction)
		{
			return Stream().GetID(inTransaction, @object);
		}

		private ObjectContainerBase Stream()
		{
			return ClassMetadata().Container();
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
					return _id == 0 ? 0 : 1;
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

		public void TraverseAllAspects(IMarshallingInfo context, ITraverseAspectCommand command
			)
		{
			ClassMetadata classMetadata = ClassMetadata();
			AssertClassMetadata(context.ClassMetadata());
			classMetadata.TraverseAllAspects(command);
		}

		protected virtual IMarshallingInfo EnsureFieldList(IMarshallingInfo context)
		{
			return context;
		}

		private void AssertClassMetadata(ClassMetadata contextMetadata)
		{
		}

		//		if (contextMetadata != classMetadata()) {
		//        	throw new IllegalStateException("expecting '" + classMetadata() + "', got '" + contextMetadata + "'");
		//        }
		public virtual ClassMetadata ClassMetadata()
		{
			return _classMetadata;
		}

		public virtual void ClassMetadata(ClassMetadata classMetadata)
		{
			_classMetadata = classMetadata;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is StandardReferenceTypeHandler))
			{
				return false;
			}
			StandardReferenceTypeHandler other = (StandardReferenceTypeHandler)obj;
			if (ClassMetadata() == null)
			{
				return other.ClassMetadata() == null;
			}
			return ClassMetadata().Equals(other.ClassMetadata());
		}

		public override int GetHashCode()
		{
			if (ClassMetadata() != null)
			{
				return ClassMetadata().GetHashCode();
			}
			return HashcodeForNull;
		}

		public virtual ITypeHandler4 UnversionedTemplate()
		{
			return new StandardReferenceTypeHandler(null);
		}

		public virtual object DeepClone(object context)
		{
			TypeHandlerCloneContext typeHandlerCloneContext = (TypeHandlerCloneContext)context;
			StandardReferenceTypeHandler cloned = (StandardReferenceTypeHandler)Reflection4.NewInstance
				(this);
			if (typeHandlerCloneContext.original is StandardReferenceTypeHandler)
			{
				StandardReferenceTypeHandler original = (StandardReferenceTypeHandler)typeHandlerCloneContext
					.original;
				cloned.ClassMetadata(original.ClassMetadata());
			}
			else
			{
				// New logic: ClassMetadata takes the responsibility in 
				//           #correctHandlerVersion() to set the 
				//           ClassMetadata directly on cloned handler.
				//            if(_classMetadata == null){
				//                throw new IllegalStateException();
				//            }
				cloned.ClassMetadata(_classMetadata);
			}
			return cloned;
		}

		public virtual void CollectIDs(CollectIdContext context, IPredicate4 predicate)
		{
			ITraverseAspectCommand command = new _MarshallingInfoTraverseAspectCommand_299(predicate
				, context, EnsureFieldList(context));
			TraverseAllAspects(context, command);
		}

		private sealed class _MarshallingInfoTraverseAspectCommand_299 : MarshallingInfoTraverseAspectCommand
		{
			public _MarshallingInfoTraverseAspectCommand_299(IPredicate4 predicate, CollectIdContext
				 context, IMarshallingInfo baseArg1) : base(baseArg1)
			{
				this.predicate = predicate;
				this.context = context;
			}

			protected override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				)
			{
				if (isNull)
				{
					return;
				}
				if (predicate.Match(aspect))
				{
					aspect.CollectIDs(context);
				}
				else
				{
					aspect.IncrementOffset(context);
				}
			}

			private readonly IPredicate4 predicate;

			private readonly CollectIdContext context;
		}

		public virtual void CascadeActivation(IActivationContext context)
		{
			AssertClassMetadata(context.ClassMetadata());
			context.CascadeActivationToTarget();
		}

		public virtual ITypeHandler4 ReadCandidateHandler(QueryingReadContext context)
		{
			if (ClassMetadata().IsArray())
			{
				return this;
			}
			return null;
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual void CollectIDs(QueryingReadContext context)
		{
			if (CollectIDsByTypehandlerAspect(context))
			{
				return;
			}
			CollectIDsByInstantiatingCollection(context);
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		private bool CollectIDsByTypehandlerAspect(QueryingReadContext context)
		{
			BooleanByRef aspectFound = new BooleanByRef(false);
			CollectIdContext subContext = CollectIdContext.ForID(context.Transaction(), context
				.Collector(), context.CollectionID());
			ITraverseAspectCommand command = new _MarshallingInfoTraverseAspectCommand_337(this
				, aspectFound, subContext, EnsureFieldList(subContext));
			TraverseAllAspects(subContext, command);
			return aspectFound.value;
		}

		private sealed class _MarshallingInfoTraverseAspectCommand_337 : MarshallingInfoTraverseAspectCommand
		{
			public _MarshallingInfoTraverseAspectCommand_337(StandardReferenceTypeHandler _enclosing
				, BooleanByRef aspectFound, CollectIdContext subContext, IMarshallingInfo baseArg1
				) : base(baseArg1)
			{
				this._enclosing = _enclosing;
				this.aspectFound = aspectFound;
				this.subContext = subContext;
			}

			protected override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				)
			{
				if (isNull)
				{
					return;
				}
				if (this._enclosing.IsCollectIdTypehandlerAspect(aspect))
				{
					aspectFound.value = true;
					aspect.CollectIDs(subContext);
				}
				else
				{
					aspect.IncrementOffset(subContext);
				}
			}

			private readonly StandardReferenceTypeHandler _enclosing;

			private readonly BooleanByRef aspectFound;

			private readonly CollectIdContext subContext;
		}

		private bool IsCollectIdTypehandlerAspect(ClassAspect aspect)
		{
			if (!(aspect is TypeHandlerAspect))
			{
				return false;
			}
			ITypeHandler4 typehandler = ((TypeHandlerAspect)aspect)._typeHandler;
			return Handlers4.IsCascading(typehandler);
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		private void CollectIDsByInstantiatingCollection(QueryingReadContext context)
		{
			int id = context.CollectionID();
			if (id == 0)
			{
				return;
			}
			Transaction transaction = context.Transaction();
			ObjectContainerBase container = context.Container();
			object obj = container.GetByID(transaction, id);
			if (obj == null)
			{
				return;
			}
			// FIXME: [TA] review activation depth
			int depth = DepthUtil.AdjustDepthToBorders(2);
			container.Activate(transaction, obj, container.ActivationDepthProvider().ActivationDepth
				(depth, ActivationMode.Activate));
			Platform4.ForEachCollectionElement(obj, new _IVisitor4_378(context));
		}

		private sealed class _IVisitor4_378 : IVisitor4
		{
			public _IVisitor4_378(QueryingReadContext context)
			{
				this.context = context;
			}

			public void Visit(object elem)
			{
				context.Add(elem);
			}

			private readonly QueryingReadContext context;
		}

		public virtual void ReadVirtualAttributes(ObjectReferenceContext context)
		{
			ITraverseAspectCommand command = new _MarshallingInfoTraverseAspectCommand_386(context
				, EnsureFieldList(context));
			TraverseAllAspects(context, command);
		}

		private sealed class _MarshallingInfoTraverseAspectCommand_386 : MarshallingInfoTraverseAspectCommand
		{
			public _MarshallingInfoTraverseAspectCommand_386(ObjectReferenceContext context, 
				IMarshallingInfo baseArg1) : base(baseArg1)
			{
				this.context = context;
			}

			protected override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				)
			{
				if (!isNull)
				{
					if (aspect is VirtualFieldMetadata)
					{
						((VirtualFieldMetadata)aspect).ReadVirtualAttribute(context);
					}
					else
					{
						aspect.IncrementOffset(context);
					}
				}
			}

			private readonly ObjectReferenceContext context;
		}

		public virtual void AddFieldIndices(ObjectIdContextImpl context)
		{
			ITraverseAspectCommand command = new _MarshallingInfoTraverseAspectCommand_402(context
				, EnsureFieldList(context));
			TraverseAllAspects(context, command);
		}

		private sealed class _MarshallingInfoTraverseAspectCommand_402 : MarshallingInfoTraverseAspectCommand
		{
			public _MarshallingInfoTraverseAspectCommand_402(ObjectIdContextImpl context, IMarshallingInfo
				 baseArg1) : base(baseArg1)
			{
				this.context = context;
			}

			protected override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				)
			{
				if (aspect is FieldMetadata)
				{
					FieldMetadata field = (FieldMetadata)aspect;
					if (isNull)
					{
						field.AddIndexEntry(context.Transaction(), context.Id(), null);
					}
					else
					{
						field.AddFieldIndex(context);
					}
				}
				else
				{
					aspect.IncrementOffset(context.Buffer());
				}
			}

			private readonly ObjectIdContextImpl context;
		}

		public virtual void DeleteMembers(DeleteContextImpl context, bool isUpdate)
		{
			ITraverseAspectCommand command = new _MarshallingInfoTraverseAspectCommand_421(context
				, isUpdate, EnsureFieldList(context));
			TraverseAllAspects(context, command);
		}

		private sealed class _MarshallingInfoTraverseAspectCommand_421 : MarshallingInfoTraverseAspectCommand
		{
			public _MarshallingInfoTraverseAspectCommand_421(DeleteContextImpl context, bool 
				isUpdate, IMarshallingInfo baseArg1) : base(baseArg1)
			{
				this.context = context;
				this.isUpdate = isUpdate;
			}

			protected override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				)
			{
				if (isNull)
				{
					if (aspect is FieldMetadata)
					{
						FieldMetadata field = (FieldMetadata)aspect;
						field.RemoveIndexEntry(context.Transaction(), context.Id(), null);
					}
					return;
				}
				aspect.Delete(context, isUpdate);
			}

			private readonly DeleteContextImpl context;

			private readonly bool isUpdate;
		}

		public virtual bool SeekToField(ObjectHeaderContext context, ClassAspect aspect)
		{
			BooleanByRef found = new BooleanByRef(false);
			ITraverseAspectCommand command = new _MarshallingInfoTraverseAspectCommand_439(aspect
				, found, EnsureFieldList(context));
			TraverseAllAspects(context, command);
			return found.value;
		}

		private sealed class _MarshallingInfoTraverseAspectCommand_439 : MarshallingInfoTraverseAspectCommand
		{
			public _MarshallingInfoTraverseAspectCommand_439(ClassAspect aspect, BooleanByRef
				 found, IMarshallingInfo baseArg1) : base(baseArg1)
			{
				this.aspect = aspect;
				this.found = found;
			}

			public override bool Accept(ClassAspect aspect)
			{
				return aspect.IsEnabledOn(this._marshallingInfo);
			}

			protected override void ProcessAspect(ClassAspect curField, int currentSlot, bool
				 isNull)
			{
				if (curField == aspect)
				{
					found.value = !isNull;
					this.Cancel();
					return;
				}
				if (!isNull)
				{
					curField.IncrementOffset(this._marshallingInfo.Buffer());
				}
			}

			private readonly ClassAspect aspect;

			private readonly BooleanByRef found;
		}

		public object IndexEntryToObject(IContext context, object indexEntry)
		{
			if (indexEntry == null)
			{
				return null;
			}
			int id = ((int)indexEntry);
			return ((ObjectContainerBase)context.ObjectContainer()).GetByID2(context.Transaction
				(), id);
		}

		public void DefragIndexEntry(DefragmentContextImpl context)
		{
			context.CopyID();
		}

		public object ReadIndexEntry(IContext context, ByteArrayBuffer a_reader)
		{
			return a_reader.ReadInt();
		}

		/// <exception cref="Db4objects.Db4o.CorruptionException"></exception>
		public object ReadIndexEntryFromObjectSlot(MarshallerFamily mf, StatefulBuffer statefulBuffer
			)
		{
			return ReadIndexEntry(statefulBuffer.Transaction().Context(), statefulBuffer);
		}

		/// <exception cref="Db4objects.Db4o.CorruptionException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual object ReadIndexEntry(IObjectIdContext context)
		{
			return context.ReadInt();
		}

		public virtual int LinkLength()
		{
			return Const4.IdLength;
		}

		public virtual void WriteIndexEntry(IContext context, ByteArrayBuffer a_writer, object
			 a_object)
		{
			if (a_object == null)
			{
				a_writer.WriteInt(0);
				return;
			}
			a_writer.WriteInt(((int)a_object));
		}

		public virtual ITypeHandler4 DelegateTypeHandler(IContext context)
		{
			return ClassMetadata().DelegateTypeHandler(context);
		}

		public virtual ObjectID ReadObjectID(IInternalReadContext context)
		{
			return ObjectID.Read(context);
		}
	}
}
