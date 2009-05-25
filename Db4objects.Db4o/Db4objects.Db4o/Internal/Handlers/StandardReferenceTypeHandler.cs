/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
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
			TraverseAllAspects(context, new _TraverseAspectCommand_35(context));
		}

		private sealed class _TraverseAspectCommand_35 : StandardReferenceTypeHandler.TraverseAspectCommand
		{
			public _TraverseAspectCommand_35(IDefragmentContext context)
			{
				this.context = context;
			}

			public override int AspectCount(Db4objects.Db4o.Internal.ClassMetadata classMetadata
				, ByteArrayBuffer reader)
			{
				return context.ReadInt();
			}

			public override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				, Db4objects.Db4o.Internal.ClassMetadata containingClass)
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
			StandardReferenceTypeHandler.TraverseAspectCommand command = new _TraverseAspectCommand_63
				(context, schemaUpdateDetected);
			// TODO: cant the aspect handle it itself?
			// Probably no because old aspect versions might not be able
			// to handle null...
			TraverseAllAspects(context, command);
			if (schemaUpdateDetected.value)
			{
				context.RestoreState(savedState);
				command = new _TraverseFieldCommand_93(context);
				TraverseAllAspects(context, command);
			}
		}

		private sealed class _TraverseAspectCommand_63 : StandardReferenceTypeHandler.TraverseAspectCommand
		{
			public _TraverseAspectCommand_63(UnmarshallingContext context, BooleanByRef schemaUpdateDetected
				)
			{
				this.context = context;
				this.schemaUpdateDetected = schemaUpdateDetected;
			}

			public override bool Accept(ClassAspect aspect)
			{
				return aspect.IsEnabledOn(context);
			}

			public override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				, Db4objects.Db4o.Internal.ClassMetadata containingClass)
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

		private sealed class _TraverseFieldCommand_93 : StandardReferenceTypeHandler.TraverseFieldCommand
		{
			public _TraverseFieldCommand_93(UnmarshallingContext context)
			{
				this.context = context;
			}

			public override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				, Db4objects.Db4o.Internal.ClassMetadata containingClass)
			{
				if (!isNull)
				{
					((FieldMetadata)aspect).AttemptUpdate(context);
				}
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
			StandardReferenceTypeHandler.TraverseAspectCommand command = new _TraverseAspectCommand_115
				(context, obj, trans);
			TraverseAllAspects(context, command);
		}

		private sealed class _TraverseAspectCommand_115 : StandardReferenceTypeHandler.TraverseAspectCommand
		{
			public _TraverseAspectCommand_115(MarshallingContext context, object obj, Transaction
				 trans)
			{
				this.context = context;
				this.obj = obj;
				this.trans = trans;
			}

			public override int AspectCount(Db4objects.Db4o.Internal.ClassMetadata classMetadata
				, ByteArrayBuffer buffer)
			{
				int fieldCount = classMetadata._aspects.Length;
				context.FieldCount(fieldCount);
				return fieldCount;
			}

			public override bool Accept(ClassAspect aspect)
			{
				return aspect.IsEnabledOn(context);
			}

			public override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				, Db4objects.Db4o.Internal.ClassMetadata containingClass)
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

			private readonly MarshallingContext context;

			private readonly object obj;

			private readonly Transaction trans;
		}

		public virtual IPreparedComparison PrepareComparison(IContext context, object source
			)
		{
			if (source == null)
			{
				return new _IPreparedComparison_148();
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
				Transaction transaction = tc._transaction;
				int id = IdFor(obj, transaction);
				return new StandardReferenceTypeHandler.PreparedComparisonImpl(id, ReflectClassFor
					(obj));
			}
			throw new IllegalComparisonException();
		}

		private sealed class _IPreparedComparison_148 : IPreparedComparison
		{
			public _IPreparedComparison_148()
			{
			}

			public int CompareTo(object obj)
			{
				if (obj == null)
				{
					return 0;
				}
				return -1;
			}
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

		public abstract class TraverseAspectCommand
		{
			private bool _cancelled = false;

			public virtual int AspectCount(ClassMetadata classMetadata, ByteArrayBuffer reader
				)
			{
				return classMetadata.ReadAspectCount(reader);
			}

			public virtual bool Cancelled()
			{
				return _cancelled;
			}

			protected virtual void Cancel()
			{
				_cancelled = true;
			}

			public virtual bool Accept(ClassAspect aspect)
			{
				return true;
			}

			public abstract void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				, ClassMetadata containingClass);
		}

		public abstract class TraverseFieldCommand : StandardReferenceTypeHandler.TraverseAspectCommand
		{
			public override bool Accept(ClassAspect aspect)
			{
				return aspect is FieldMetadata;
			}
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

		public void TraverseAllAspects(IMarshallingInfo context, StandardReferenceTypeHandler.TraverseAspectCommand
			 command)
		{
			int currentSlot = 0;
			ClassMetadata classMetadata = ClassMetadata();
			AssertClassMetadata(context.ClassMetadata());
			while (classMetadata != null)
			{
				int aspectCount = command.AspectCount(classMetadata, ((ByteArrayBuffer)context.Buffer
					()));
				context.AspectCount(aspectCount);
				for (int i = 0; i < aspectCount && !command.Cancelled(); i++)
				{
					ClassAspect currentAspect = classMetadata._aspects[i];
					if (command.Accept(currentAspect))
					{
						command.ProcessAspect(currentAspect, currentSlot, IsNull(context, currentSlot), classMetadata
							);
					}
					context.BeginSlot();
					currentSlot++;
				}
				if (command.Cancelled())
				{
					return;
				}
				classMetadata = classMetadata.i_ancestor;
			}
		}

		private void AssertClassMetadata(ClassMetadata contextMetadata)
		{
		}

		//		if (contextMetadata != classMetadata()) {
		//        	throw new IllegalStateException("expecting '" + classMetadata() + "', got '" + contextMetadata + "'");
		//        }
		protected virtual bool IsNull(IFieldListInfo fieldList, int fieldIndex)
		{
			return fieldList.IsNull(fieldIndex);
		}

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
			StandardReferenceTypeHandler.TraverseAspectCommand command = new _TraverseAspectCommand_337
				(predicate, context);
			TraverseAllAspects(context, command);
		}

		private sealed class _TraverseAspectCommand_337 : StandardReferenceTypeHandler.TraverseAspectCommand
		{
			public _TraverseAspectCommand_337(IPredicate4 predicate, CollectIdContext context
				)
			{
				this.predicate = predicate;
				this.context = context;
			}

			public override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				, ClassMetadata containingClass)
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
			StandardReferenceTypeHandler.TraverseAspectCommand command = new _TraverseAspectCommand_374
				(this, aspectFound, subContext);
			TraverseAllAspects(subContext, command);
			return aspectFound.value;
		}

		private sealed class _TraverseAspectCommand_374 : StandardReferenceTypeHandler.TraverseAspectCommand
		{
			public _TraverseAspectCommand_374(StandardReferenceTypeHandler _enclosing, BooleanByRef
				 aspectFound, CollectIdContext subContext)
			{
				this._enclosing = _enclosing;
				this.aspectFound = aspectFound;
				this.subContext = subContext;
			}

			public override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				, ClassMetadata containingClass)
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
			int depth = ClassMetadata().AdjustDepthToBorders(2);
			container.Activate(transaction, obj, container.ActivationDepthProvider().ActivationDepth
				(depth, ActivationMode.Activate));
			Platform4.ForEachCollectionElement(obj, new _IVisitor4_415(context));
		}

		private sealed class _IVisitor4_415 : IVisitor4
		{
			public _IVisitor4_415(QueryingReadContext context)
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
			StandardReferenceTypeHandler.TraverseAspectCommand command = new _TraverseAspectCommand_423
				(context);
			TraverseAllAspects(context, command);
		}

		private sealed class _TraverseAspectCommand_423 : StandardReferenceTypeHandler.TraverseAspectCommand
		{
			public _TraverseAspectCommand_423(ObjectReferenceContext context)
			{
				this.context = context;
			}

			public override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				, ClassMetadata containingClass)
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

		public virtual void AddFieldIndices(ObjectIdContextImpl context, Slot oldSlot)
		{
			StandardReferenceTypeHandler.TraverseAspectCommand command = new _TraverseAspectCommand_438
				(context, oldSlot);
			TraverseAllAspects(context, command);
		}

		private sealed class _TraverseAspectCommand_438 : StandardReferenceTypeHandler.TraverseAspectCommand
		{
			public _TraverseAspectCommand_438(ObjectIdContextImpl context, Slot oldSlot)
			{
				this.context = context;
				this.oldSlot = oldSlot;
			}

			public override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				, ClassMetadata containingClass)
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
						field.AddFieldIndex(context, oldSlot);
					}
				}
				else
				{
					aspect.IncrementOffset(context.Buffer());
				}
			}

			private readonly ObjectIdContextImpl context;

			private readonly Slot oldSlot;
		}

		public virtual void DeleteMembers(DeleteContextImpl context, bool isUpdate)
		{
			StandardReferenceTypeHandler.TraverseAspectCommand command = new _TraverseAspectCommand_456
				(context, isUpdate);
			TraverseAllAspects(context, command);
		}

		private sealed class _TraverseAspectCommand_456 : StandardReferenceTypeHandler.TraverseAspectCommand
		{
			public _TraverseAspectCommand_456(DeleteContextImpl context, bool isUpdate)
			{
				this.context = context;
				this.isUpdate = isUpdate;
			}

			public override void ProcessAspect(ClassAspect aspect, int currentSlot, bool isNull
				, ClassMetadata containingClass)
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
			StandardReferenceTypeHandler.TraverseAspectCommand command = new _TraverseAspectCommand_473
				(context, aspect, found);
			TraverseAllAspects(context, command);
			return found.value;
		}

		private sealed class _TraverseAspectCommand_473 : StandardReferenceTypeHandler.TraverseAspectCommand
		{
			public _TraverseAspectCommand_473(ObjectHeaderContext context, ClassAspect aspect
				, BooleanByRef found)
			{
				this.context = context;
				this.aspect = aspect;
				this.found = found;
			}

			public override bool Accept(ClassAspect aspect)
			{
				return aspect.IsEnabledOn(context);
			}

			public override void ProcessAspect(ClassAspect curField, int currentSlot, bool isNull
				, ClassMetadata containingClass)
			{
				if (curField == aspect)
				{
					found.value = !isNull;
					this.Cancel();
					return;
				}
				if (!isNull)
				{
					curField.IncrementOffset(context);
				}
			}

			private readonly ObjectHeaderContext context;

			private readonly ClassAspect aspect;

			private readonly BooleanByRef found;
		}

		public virtual bool CanHold(IReflectClass type)
		{
			return ClassMetadata().CanHold(type);
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
