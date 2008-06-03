/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <summary>This is the latest version, the one that should be used.</summary>
	/// <remarks>This is the latest version, the one that should be used.</remarks>
	/// <exclude></exclude>
	public class ArrayHandler : IFirstClassHandler, IComparable4, ITypeHandler4, IVariableLengthTypeHandler
		, IEmbeddedTypeHandler, ICompositeTypeHandler, ICollectIdHandler
	{
		private ITypeHandler4 _handler;

		private bool _usePrimitiveClassReflector;

		protected readonly ArrayVersionHelper _versionHelper;

		public ArrayHandler()
		{
			_versionHelper = CreateVersionHelper();
		}

		public ArrayHandler(ITypeHandler4 handler, bool usePrimitiveClassReflector) : this
			()
		{
			_handler = handler;
			_usePrimitiveClassReflector = usePrimitiveClassReflector;
		}

		protected virtual ArrayVersionHelper CreateVersionHelper()
		{
			return new ArrayVersionHelper();
		}

		protected ArrayHandler(Db4objects.Db4o.Internal.Handlers.ArrayHandler template, HandlerRegistry
			 registry, int version) : this(registry.CorrectHandlerVersion(template._handler, 
			version), template._usePrimitiveClassReflector)
		{
		}

		protected virtual IReflectArray ArrayReflector(ObjectContainerBase container)
		{
			return container.Reflector().Array();
		}

		public virtual IEnumerator AllElements(ObjectContainerBase container, object a_object
			)
		{
			return AllElements(ArrayReflector(container), a_object);
		}

		public static IEnumerator AllElements(IReflectArray reflectArray, object array)
		{
			return new ArrayHandler.ReflectArrayIterator(reflectArray, array);
		}

		public void CascadeActivation(Transaction trans, object onObject, IActivationDepth
			 depth)
		{
			if (!(_handler is ClassMetadata))
			{
				return;
			}
			ObjectContainerBase container = Container(trans);
			IEnumerator all = AllElements(container, onObject);
			while (all.MoveNext())
			{
				object current = all.Current;
				IActivationDepth elementDepth = Descend(container, depth, current);
				if (elementDepth.RequiresActivation())
				{
					if (depth.Mode().IsDeactivate())
					{
						container.StillToDeactivate(trans, current, elementDepth, false);
					}
					else
					{
						container.StillToActivate(trans, current, elementDepth);
					}
				}
			}
		}

		internal virtual ObjectContainerBase Container(Transaction trans)
		{
			return trans.Container();
		}

		private IActivationDepth Descend(ObjectContainerBase container, IActivationDepth 
			depth, object obj)
		{
			if (obj == null)
			{
				return new NonDescendingActivationDepth(depth.Mode());
			}
			ClassMetadata cm = ClassMetaDataForObject(container, obj);
			if (cm == null || cm.IsPrimitive())
			{
				return new NonDescendingActivationDepth(depth.Mode());
			}
			return depth.Descend(cm);
		}

		private ClassMetadata ClassMetaDataForObject(ObjectContainerBase container, object
			 obj)
		{
			return container.ClassMetadataForObject(obj);
		}

		protected virtual IReflectClass ClassReflector(ObjectContainerBase container)
		{
			if (_handler is IBuiltinTypeHandler)
			{
				return ((IBuiltinTypeHandler)_handler).ClassReflector();
			}
			if (_handler is ClassMetadata)
			{
				return ((ClassMetadata)_handler).ClassReflector();
			}
			return container.Handlers().ClassReflectorForHandler(_handler);
		}

		public virtual void CollectIDs(CollectIdContext context)
		{
			ForEachElement(context, new _IRunnable_113(context));
		}

		private sealed class _IRunnable_113 : IRunnable
		{
			public _IRunnable_113(CollectIdContext context)
			{
				this.context = context;
			}

			public void Run()
			{
				context.AddId();
			}

			private readonly CollectIdContext context;
		}

		protected virtual ArrayInfo ForEachElement(AbstractBufferContext context, IRunnable
			 elementRunnable)
		{
			ArrayInfo info = NewArrayInfo();
			WithContent(context, new _IRunnable_122(this, context, info, elementRunnable));
			return info;
		}

		private sealed class _IRunnable_122 : IRunnable
		{
			public _IRunnable_122(ArrayHandler _enclosing, AbstractBufferContext context, ArrayInfo
				 info, IRunnable elementRunnable)
			{
				this._enclosing = _enclosing;
				this.context = context;
				this.info = info;
				this.elementRunnable = elementRunnable;
			}

			public void Run()
			{
				if (context.Buffer() == null)
				{
					return;
				}
				if (this._enclosing.IsUntypedByteArray(context))
				{
					return;
				}
				this._enclosing.ReadInfo(context.Transaction(), context, info);
				int elementCount = info.ElementCount();
				elementCount -= this._enclosing.ReducedCountForNullBitMap(info, context);
				for (int i = 0; i < elementCount; i++)
				{
					elementRunnable.Run();
				}
			}

			private readonly ArrayHandler _enclosing;

			private readonly AbstractBufferContext context;

			private readonly ArrayInfo info;

			private readonly IRunnable elementRunnable;
		}

		protected virtual void WithContent(AbstractBufferContext context, IRunnable runnable
			)
		{
			runnable.Run();
		}

		private int ReducedCountForNullBitMap(ArrayInfo info, IReadBuffer context)
		{
			if (!HasNullBitmap(info))
			{
				return 0;
			}
			return ReducedCountForNullBitMap(info.ElementCount(), ReadNullBitmap(context, info
				.ElementCount()));
		}

		private int ReducedCountForNullBitMap(int count, BitMap4 bitMap)
		{
			int nullCount = 0;
			for (int i = 0; i < count; i++)
			{
				if (bitMap.IsTrue(i))
				{
					nullCount++;
				}
			}
			return nullCount;
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			if (!CascadeDelete(context))
			{
				return;
			}
			ForEachElement((AbstractBufferContext)context, new _IRunnable_169(this, context));
		}

		private sealed class _IRunnable_169 : IRunnable
		{
			public _IRunnable_169(ArrayHandler _enclosing, IDeleteContext context)
			{
				this._enclosing = _enclosing;
				this.context = context;
			}

			public void Run()
			{
				this._enclosing._handler.Delete(context);
			}

			private readonly ArrayHandler _enclosing;

			private readonly IDeleteContext context;
		}

		private bool CascadeDelete(IDeleteContext context)
		{
			return context.CascadeDelete() && _handler is ClassMetadata;
		}

		// FIXME: This code has not been called in any test case when the 
		//        new ArrayMarshaller was written.
		//        Apparently it only frees slots.
		//        For now the code simply returns without freeing.
		/// <param name="classPrimitive"></param>
		public void DeletePrimitiveEmbedded(StatefulBuffer buffer, PrimitiveFieldHandler 
			classPrimitive)
		{
			buffer.ReadInt();
			//int address = a_bytes.readInt();
			buffer.ReadInt();
			//int length = a_bytes.readInt();
			if (true)
			{
				return;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Db4objects.Db4o.Internal.Handlers.ArrayHandler))
			{
				return false;
			}
			Db4objects.Db4o.Internal.Handlers.ArrayHandler other = (Db4objects.Db4o.Internal.Handlers.ArrayHandler
				)obj;
			if (other.Identifier() != Identifier())
			{
				return false;
			}
			if (_handler == null)
			{
				return other._handler == null;
			}
			return _handler.Equals(other._handler) && _usePrimitiveClassReflector == other._usePrimitiveClassReflector;
		}

		public override int GetHashCode()
		{
			if (_handler == null)
			{
				return HashcodeForNull;
			}
			int hc = _handler.GetHashCode() >> 7;
			return _usePrimitiveClassReflector ? hc : -hc;
		}

		protected virtual bool HandleAsByteArray(object obj)
		{
			return obj.GetType() == typeof(byte[]);
			return obj is byte[];
		}

		public virtual byte Identifier()
		{
			return Const4.Yaparray;
		}

		public virtual IReflectClass PrimitiveClassReflector(IReflector reflector)
		{
			return Handlers4.PrimitiveClassReflector(_handler, reflector);
		}

		protected virtual object ReadCreate(Transaction trans, IReadBuffer buffer, ArrayInfo
			 info)
		{
			ReadInfo(trans, buffer, info);
			IReflectClass clazz = NewInstanceReflectClass(trans.Reflector(), info);
			if (clazz == null)
			{
				return null;
			}
			return NewInstance(ArrayReflector(Container(trans)), info, clazz);
		}

		protected object NewInstance(IReflectArray arrayReflector, ArrayInfo info, IReflectClass
			 clazz)
		{
			return arrayReflector.NewInstance(clazz, info);
		}

		protected IReflectClass NewInstanceReflectClass(IReflector reflector, ArrayInfo info
			)
		{
			if (_usePrimitiveClassReflector)
			{
				return PrimitiveClassReflector(reflector);
			}
			return info.ReflectClass();
		}

		public virtual ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, ByteArrayBuffer[] a_bytes)
		{
			return this;
		}

		public virtual void ReadCandidates(QueryingReadContext context)
		{
			QCandidates candidates = context.Candidates();
			ForEachElement(context, new _IRunnable_262(this, candidates, context));
		}

		private sealed class _IRunnable_262 : IRunnable
		{
			public _IRunnable_262(ArrayHandler _enclosing, QCandidates candidates, QueryingReadContext
				 context)
			{
				this._enclosing = _enclosing;
				this.candidates = candidates;
				this.context = context;
			}

			public void Run()
			{
				QCandidate qc = candidates.ReadSubCandidate(context, this._enclosing._handler);
				if (qc != null)
				{
					candidates.AddByIdentity(qc);
				}
			}

			private readonly ArrayHandler _enclosing;

			private readonly QCandidates candidates;

			private readonly QueryingReadContext context;
		}

		protected virtual void ReadSubCandidates(QueryingReadContext context, int count)
		{
			QCandidates candidates = context.Candidates();
			for (int i = 0; i < count; i++)
			{
				QCandidate qc = candidates.ReadSubCandidate(context, _handler);
				if (qc != null)
				{
					candidates.AddByIdentity(qc);
				}
			}
		}

		protected virtual void ReadInfo(Transaction trans, IReadBuffer buffer, ArrayInfo 
			info)
		{
			int classID = buffer.ReadInt();
			if (!IsPreVersion0Format(classID))
			{
				_versionHelper.ReadTypeInfo(trans, buffer, info, classID);
				ReflectClassFromElementsEntry(Container(trans), info, classID);
				ReadDimensions(info, buffer);
			}
			else
			{
				info.ReflectClass(ClassReflector(Container(trans)));
				DetectDimensionsPreVersion0Format(buffer, info, classID);
			}
			if (Debug.ExceedsMaximumArrayEntries(info.ElementCount(), _usePrimitiveClassReflector
				))
			{
				info.ElementCount(0);
			}
		}

		protected virtual void DetectDimensionsPreVersion0Format(IReadBuffer buffer, ArrayInfo
			 info, int classID)
		{
			info.ElementCount(classID);
		}

		protected virtual void ReadDimensions(ArrayInfo info, IReadBuffer buffer)
		{
			info.ElementCount(buffer.ReadInt());
		}

		protected virtual bool IsPreVersion0Format(int elementCount)
		{
			return _versionHelper.IsPreVersion0Format(elementCount);
		}

		private void ReflectClassFromElementsEntry(ObjectContainerBase container, ArrayInfo
			 info, int classID)
		{
			info.ReflectClass(_versionHelper.ReflectClassFromElementsEntry(container, info, classID
				));
		}

		protected IReflectClass ClassReflector(IReflector reflector, ClassMetadata classMetadata
			, bool isPrimitive)
		{
			return _versionHelper.ClassReflector(reflector, classMetadata, isPrimitive);
		}

		public static IEnumerator Iterator(IReflectClass claxx, object obj)
		{
			IReflectArray reflectArray = claxx.Reflector().Array();
			if (reflectArray.IsNDimensional(claxx))
			{
				return MultidimensionalArrayHandler.AllElements(reflectArray, obj);
			}
			return Db4objects.Db4o.Internal.Handlers.ArrayHandler.AllElements(reflectArray, obj
				);
		}

		protected virtual bool UseJavaHandling()
		{
			return _versionHelper.UseJavaHandling();
		}

		protected virtual int ClassIDFromInfo(ObjectContainerBase container, ArrayInfo info
			)
		{
			return _versionHelper.ClassIDFromInfo(container, info);
		}

		private int MarshalledClassID(ObjectContainerBase container, ArrayInfo info)
		{
			return ClassIdToMarshalledClassId(ClassIDFromInfo(container, info), info.Primitive
				());
		}

		public int ClassIdToMarshalledClassId(int classID, bool primitive)
		{
			return _versionHelper.ClassIdToMarshalledClassId(classID, primitive);
		}

		protected bool IsPrimitive(IReflector reflector, IReflectClass claxx, ClassMetadata
			 classMetadata)
		{
			return _versionHelper.IsPrimitive(reflector, claxx, classMetadata);
		}

		private IReflectClass ComponentType(ObjectContainerBase container, object obj)
		{
			return ArrayReflector(container).GetComponentType(container.Reflector().ForObject
				(obj));
		}

		public virtual void Defragment(IDefragmentContext context)
		{
			if (Handlers4.HandlesSimple(_handler))
			{
				context.IncrementOffset(LinkLength());
			}
			else
			{
				DefragmentSlot(context);
			}
		}

		public void DefragmentSlot(IDefragmentContext context)
		{
			if (IsUntypedByteArray(context))
			{
				return;
			}
			int classIdOffset = context.TargetBuffer().Offset();
			ArrayInfo info = NewArrayInfo();
			ReadInfo(context.Transaction(), context, info);
			DefragmentWriteMappedClassId(context, info, classIdOffset);
			int elementCount = info.ElementCount();
			if (HasNullBitmap(info))
			{
				BitMap4 bitMap = ReadNullBitmap(context, elementCount);
				elementCount -= ReducedCountForNullBitMap(elementCount, bitMap);
			}
			for (int i = 0; i < elementCount; i++)
			{
				_handler.Defragment(context);
			}
		}

		private void DefragmentWriteMappedClassId(IDefragmentContext context, ArrayInfo info
			, int classIdOffset)
		{
			ByteArrayBuffer targetBuffer = context.TargetBuffer();
			int currentOffset = targetBuffer.Offset();
			targetBuffer.Seek(classIdOffset);
			int classID = ClassIDFromInfo(Container(context), info);
			int mappedID = context.MappedID(classID);
			int marshalledMappedId = ClassIdToMarshalledClassId(mappedID, info.Primitive());
			targetBuffer.WriteInt(marshalledMappedId);
			targetBuffer.Seek(currentOffset);
		}

		private bool IsUntypedByteArray(IBufferContext context)
		{
			return _handler is UntypedFieldHandler && HandleAsByteArray(context);
		}

		private bool HandleAsByteArray(IBufferContext context)
		{
			int offset = context.Offset();
			ArrayInfo info = NewArrayInfo();
			ReadInfo(context.Transaction(), context, info);
			bool isByteArray = context.Transaction().Reflector().ForClass(typeof(byte)).Equals
				(info.ReflectClass());
			context.Seek(offset);
			return isByteArray;
		}

		public virtual object Read(IReadContext context)
		{
			ArrayInfo info = NewArrayInfo();
			object array = ReadCreate(context.Transaction(), context, info);
			ReadElements(context, info, array);
			return array;
		}

		protected virtual void ReadElements(IReadContext context, ArrayInfo info, object 
			array)
		{
			ReadInto(context, info, array);
		}

		protected virtual ArrayInfo NewArrayInfo()
		{
			return new ArrayInfo();
		}

		protected void ReadInto(IReadContext context, ArrayInfo info, object array)
		{
			if (array == null)
			{
				return;
			}
			if (HandleAsByteArray(array))
			{
				context.ReadBytes((byte[])array);
				// byte[] performance optimisation
				return;
			}
			if (HasNullBitmap(info))
			{
				BitMap4 nullBitMap = ReadNullBitmap(context, info.ElementCount());
				for (int i = 0; i < info.ElementCount(); i++)
				{
					object obj = nullBitMap.IsTrue(i) ? null : context.ReadObject(_handler);
					ArrayReflector(Container(context)).Set(array, i, obj);
				}
			}
			else
			{
				for (int i = 0; i < info.ElementCount(); i++)
				{
					ArrayReflector(Container(context)).Set(array, i, context.ReadObject(_handler));
				}
			}
		}

		protected virtual BitMap4 ReadNullBitmap(IReadBuffer context, int length)
		{
			return context.ReadBitMap(length);
		}

		protected bool HasNullBitmap(ArrayInfo info)
		{
			return _versionHelper.HasNullBitmap(info);
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			ArrayInfo info = NewArrayInfo();
			Analyze(Container(context), obj, info);
			WriteInfo(context, info);
			WriteElements(context, obj, info);
		}

		protected virtual void WriteElements(IWriteContext context, object obj, ArrayInfo
			 info)
		{
			if (HandleAsByteArray(obj))
			{
				context.WriteBytes((byte[])obj);
			}
			else
			{
				// byte[] performance optimisation
				if (HasNullBitmap(info))
				{
					BitMap4 nullItems = NullItemsMap(ArrayReflector(Container(context)), obj);
					WriteNullBitmap(context, nullItems);
					for (int i = 0; i < info.ElementCount(); i++)
					{
						if (!nullItems.IsTrue(i))
						{
							context.WriteObject(_handler, ArrayReflector(Container(context)).Get(obj, i));
						}
					}
				}
				else
				{
					for (int i = 0; i < info.ElementCount(); i++)
					{
						context.WriteObject(_handler, ArrayReflector(Container(context)).Get(obj, i));
					}
				}
			}
		}

		protected virtual void WriteInfo(IWriteContext context, ArrayInfo info)
		{
			WriteHeader(context, info);
			WriteDimensions(context, info);
		}

		private void WriteHeader(IWriteContext context, ArrayInfo info)
		{
			context.WriteInt(MarshalledClassID(Container(context), info));
			_versionHelper.WriteTypeInfo(context, info);
		}

		protected virtual void WriteDimensions(IWriteContext context, ArrayInfo info)
		{
			context.WriteInt(info.ElementCount());
		}

		protected void Analyze(ObjectContainerBase container, object obj, ArrayInfo info)
		{
			// TODO: Move as much analysis as possible to ReflectArray#analyze() 
			ArrayReflector(container).Analyze(obj, info);
			IReflectClass claxx = ComponentType(container, obj);
			ClassMetadata classMetadata = container.ProduceClassMetadata(claxx);
			bool primitive = IsPrimitive(container.Reflector(), claxx, classMetadata);
			if (primitive)
			{
				claxx = classMetadata.ClassReflector();
			}
			info.Primitive(primitive);
			info.ReflectClass(claxx);
			AnalyzeDimensions(container, obj, info);
		}

		protected virtual void AnalyzeDimensions(ObjectContainerBase container, object obj
			, ArrayInfo info)
		{
			info.ElementCount(ArrayReflector(container).GetLength(obj));
		}

		private void WriteNullBitmap(IWriteBuffer context, BitMap4 bitMap)
		{
			context.WriteBytes(bitMap.Bytes());
		}

		protected virtual BitMap4 NullItemsMap(IReflectArray reflector, object array)
		{
			int arrayLength = reflector.GetLength(array);
			BitMap4 nullBitMap = new BitMap4(arrayLength);
			for (int i = 0; i < arrayLength; i++)
			{
				if (reflector.Get(array, i) == null)
				{
					nullBitMap.Set(i, true);
				}
			}
			return nullBitMap;
		}

		internal virtual ObjectContainerBase Container(IContext context)
		{
			return context.Transaction().Container();
		}

		public virtual IPreparedComparison PrepareComparison(IContext context, object obj
			)
		{
			return new PreparedArrayContainsComparison(context, this, _handler, obj);
		}

		public virtual int LinkLength()
		{
			return Const4.IndirectionLength;
		}

		public virtual ITypeHandler4 GenericTemplate()
		{
			return new Db4objects.Db4o.Internal.Handlers.ArrayHandler();
		}

		public virtual object DeepClone(object context)
		{
			TypeHandlerCloneContext typeHandlerCloneContext = (TypeHandlerCloneContext)context;
			Db4objects.Db4o.Internal.Handlers.ArrayHandler original = (Db4objects.Db4o.Internal.Handlers.ArrayHandler
				)typeHandlerCloneContext.original;
			Db4objects.Db4o.Internal.Handlers.ArrayHandler cloned = (Db4objects.Db4o.Internal.Handlers.ArrayHandler
				)Reflection4.NewInstance(this);
			cloned._usePrimitiveClassReflector = original._usePrimitiveClassReflector;
			cloned._handler = typeHandlerCloneContext.CorrectHandlerVersion(original.DelegateTypeHandler
				());
			return cloned;
		}

		public virtual ITypeHandler4 DelegateTypeHandler()
		{
			return _handler;
		}

		private const int HashcodeForNull = 9141078;

		private sealed class ReflectArrayIterator : IndexedIterator
		{
			private readonly object _array;

			private readonly IReflectArray _reflectArray;

			public ReflectArrayIterator(IReflectArray reflectArray, object array) : base(reflectArray
				.GetLength(array))
			{
				_reflectArray = reflectArray;
				_array = array;
			}

			protected override object Get(int index)
			{
				return _reflectArray.Get(_array, index);
			}
		}
	}
}
