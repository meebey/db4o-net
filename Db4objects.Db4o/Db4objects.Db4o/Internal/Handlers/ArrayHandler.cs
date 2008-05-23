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

		public ArrayHandler(ITypeHandler4 handler, bool usePrimitiveClassReflector)
		{
			_handler = handler;
			_usePrimitiveClassReflector = usePrimitiveClassReflector;
		}

		public ArrayHandler()
		{
		}

		protected ArrayHandler(Db4objects.Db4o.Internal.Handlers.ArrayHandler template, HandlerRegistry
			 registry, int version) : this(registry.CorrectHandlerVersion(template._handler, 
			version), template._usePrimitiveClassReflector)
		{
		}

		// required for reflection cloning
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

		private IReflectClass ClassReflector(ObjectContainerBase container)
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
			ForEachElement(context, new _IRunnable_106(context));
		}

		private sealed class _IRunnable_106 : IRunnable
		{
			public _IRunnable_106(CollectIdContext context)
			{
				this.context = context;
			}

			public void Run()
			{
				context.AddId();
			}

			private readonly CollectIdContext context;
		}

		protected virtual void ForEachElement(BufferContext context, IRunnable elementRunnable
			)
		{
			WithContent(context, new _IRunnable_114(this, context, elementRunnable));
		}

		private sealed class _IRunnable_114 : IRunnable
		{
			public _IRunnable_114(ArrayHandler _enclosing, BufferContext context, IRunnable elementRunnable
				)
			{
				this._enclosing = _enclosing;
				this.context = context;
				this.elementRunnable = elementRunnable;
			}

			public void Run()
			{
				if (context.Buffer() == null)
				{
					return;
				}
				ArrayInfo info = this._enclosing.NewArrayInfo();
				this._enclosing.ReadInfo(context.Transaction(), context, info);
				int elementCount = info.ElementCount();
				elementCount -= this._enclosing.ReducedCountForNullBitMap(context, elementCount);
				for (int i = 0; i < elementCount; i++)
				{
					elementRunnable.Run();
				}
			}

			private readonly ArrayHandler _enclosing;

			private readonly BufferContext context;

			private readonly IRunnable elementRunnable;
		}

		protected virtual void WithContent(BufferContext context, IRunnable runnable)
		{
			runnable.Run();
		}

		private int ReducedCountForNullBitMap(IReadBuffer context, int count)
		{
			if (!HasNullBitmap())
			{
				return 0;
			}
			return ReducedCountForNullBitMap(count, ReadNullBitmap(context, count));
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
			ForEachElement((BufferContext)context, new _IRunnable_158(this, context));
		}

		private sealed class _IRunnable_158 : IRunnable
		{
			public _IRunnable_158(ArrayHandler _enclosing, IDeleteContext context)
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

		/// <param name="obj"></param>
		public virtual int OwnLength(object obj)
		{
			return OwnLength();
		}

		private int OwnLength()
		{
			return Const4.ObjectLength + Const4.IntLength * 2;
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
			return NewInstance(trans, info, clazz);
		}

		protected virtual object NewInstance(Transaction trans, ArrayInfo info, IReflectClass
			 clazz)
		{
			return ArrayReflector(Container(trans)).NewInstance(clazz, info.ElementCount());
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
			ForEachElement(context, new _IRunnable_260(this, candidates, context));
		}

		private sealed class _IRunnable_260 : IRunnable
		{
			public _IRunnable_260(ArrayHandler _enclosing, QCandidates candidates, QueryingReadContext
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

		protected void ReadInfo(Transaction trans, IReadBuffer buffer, ArrayInfo info)
		{
			int classID = buffer.ReadInt();
			if (NewerArrayFormat(classID))
			{
				ReflectClassFromElementsEntry(trans, info, classID);
				ReadDimensions(info, buffer);
			}
			else
			{
				info.ReflectClass(ClassReflector(Container(trans)));
				ReadDimensionsOldFormat(buffer, info, classID);
			}
			if (Debug.ExceedsMaximumArrayEntries(info.ElementCount(), _usePrimitiveClassReflector
				))
			{
				info.ElementCount(0);
			}
		}

		protected virtual void ReadDimensionsOldFormat(IReadBuffer buffer, ArrayInfo info
			, int classID)
		{
			info.ElementCount(classID);
		}

		protected virtual void ReadDimensions(ArrayInfo info, IReadBuffer buffer)
		{
			info.ElementCount(buffer.ReadInt());
		}

		private bool NewerArrayFormat(int elements)
		{
			return elements < 0;
		}

		protected int MapElementsEntry(IDefragmentContext context, int orig)
		{
			if (orig >= 0 || orig == Const4.IgnoreId)
			{
				return orig;
			}
			// TODO: We changed the following line in the NullableArrayHandling 
			//       refactoring. Behaviour may have to be different for older
			//       ArrayHandler versions.
			bool primitive = UseJavaHandling() && (orig < Const4.Primitive);
			if (primitive)
			{
				orig -= Const4.Primitive;
			}
			int origID = -orig;
			int mappedID = context.MappedID(origID);
			int mapped = -mappedID;
			if (primitive)
			{
				mapped += Const4.Primitive;
			}
			return mapped;
		}

		private void ReflectClassFromElementsEntry(Transaction trans, ArrayInfo info, int
			 classID)
		{
			// TODO: Here is a low-frequency mistake, extremely unlikely.
			// If classID == 99999 by accident then we will get ignore.
			if (classID != Const4.IgnoreId)
			{
				info.Primitive(false);
				if (UseJavaHandling())
				{
					if (classID < Const4.Primitive)
					{
						info.Primitive(true);
						classID -= Const4.Primitive;
					}
				}
				classID = -classID;
				ClassMetadata classMetadata = Container(trans).ClassMetadataForId(classID);
				if (classMetadata != null)
				{
					info.ReflectClass(ClassReflector(trans.Reflector(), classMetadata, info.Primitive
						()));
					return;
				}
			}
			info.ReflectClass(ClassReflector(Container(trans)));
		}

		protected virtual IReflectClass ClassReflector(IReflector reflector, ClassMetadata
			 classMetadata, bool isPrimitive)
		{
			return (isPrimitive ? Handlers4.PrimitiveClassReflector(classMetadata, reflector)
				 : classMetadata.ClassReflector());
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
			if (NullableArrayHandling.Enabled())
			{
				return true;
			}
			return !Deploy.csharp;
		}

		protected int ClassID(ObjectContainerBase container, object obj)
		{
			IReflectClass claxx = ComponentType(container, obj);
			ClassMetadata classMetadata = container.ProduceClassMetadata(claxx);
			bool primitive = IsPrimitive(container.Reflector(), claxx, classMetadata);
			if (primitive)
			{
				claxx = classMetadata.ClassReflector();
			}
			classMetadata = container.ProduceClassMetadata(claxx);
			if (classMetadata == null)
			{
				// TODO: This one is a terrible low-frequency blunder !!!
				// If YapClass-ID == 99999 then we will get IGNORE back.
				// Discovered on adding the primitives
				return Const4.IgnoreId;
			}
			int classID = classMetadata.GetID();
			if (primitive)
			{
				classID -= Const4.Primitive;
			}
			return -classID;
		}

		private int ClassID(ObjectContainerBase container, ArrayInfo info)
		{
			ClassMetadata classMetadata = container.ProduceClassMetadata(info.ReflectClass());
			if (classMetadata == null)
			{
				// TODO: This one is a terrible low-frequency blunder !!!
				// If YapClass-ID == 99999 then we will get IGNORE back.
				// Discovered on adding the primitives
				return Const4.IgnoreId;
			}
			int classID = classMetadata.GetID();
			if (info.Primitive())
			{
				classID -= Const4.Primitive;
			}
			return -classID;
		}

		protected virtual bool IsPrimitive(IReflector reflector, IReflectClass claxx, ClassMetadata
			 classMetadata)
		{
			if (NullableArrayHandling.Disabled())
			{
				return false;
			}
			return claxx.IsPrimitive();
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
				DefragIDs(context);
			}
		}

		private void DefragIDs(IDefragmentContext context)
		{
			int offset = PreparePayloadRead(context);
			Defrag1(context);
			// FIXME: Shouldn't we be beyound the array slot now?
			context.Seek(offset);
		}

		protected virtual int PreparePayloadRead(IDefragmentContext context)
		{
			return context.Offset();
		}

		public void Defrag1(IDefragmentContext context)
		{
			Defrag2(context);
		}

		public virtual void Defrag2(IDefragmentContext context)
		{
			if (IsUntypedByteArray(context))
			{
				return;
			}
			int elementCount = ReadElementCountDefrag(context);
			if (HasNullBitmap())
			{
				BitMap4 bitMap = DefragmentNullBitmap(context, elementCount);
				elementCount -= ReducedCountForNullBitMap(elementCount, bitMap);
			}
			for (int i = 0; i < elementCount; i++)
			{
				_handler.Defragment(context);
			}
		}

		private bool IsUntypedByteArray(IDefragmentContext context)
		{
			return _handler is UntypedFieldHandler && HandleAsByteArray(context);
		}

		private bool HandleAsByteArray(IDefragmentContext context)
		{
			int offset = context.Offset();
			ArrayInfo info = NewArrayInfo();
			ReadInfo(context.Transaction(), context, info);
			bool isByteArray = context.Transaction().Reflector().ForClass(typeof(byte)).Equals
				(info.ReflectClass());
			context.Seek(offset);
			return isByteArray;
		}

		private BitMap4 DefragmentNullBitmap(IDefragmentContext context, int elements)
		{
			if (!HasNullBitmap())
			{
				return null;
			}
			BitMap4 nullBitmap = ReadNullBitmap(context.SourceBuffer(), elements);
			WriteNullBitmap(context.TargetBuffer(), nullBitmap);
			return nullBitmap;
		}

		protected virtual int ReadElementCountDefrag(IDefragmentContext context)
		{
			int elements = context.SourceBuffer().ReadInt();
			context.TargetBuffer().WriteInt(MapElementsEntry(context, elements));
			if (elements < 0)
			{
				elements = context.ReadInt();
			}
			return elements;
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
			if (HasNullBitmap())
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

		protected virtual bool HasNullBitmap()
		{
			return NullableArrayHandling.Enabled();
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
				if (HasNullBitmap())
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
			context.WriteInt(ClassID(Container(context), info));
			WriteDimensions(context, info);
		}

		protected virtual void WriteDimensions(IWriteContext context, ArrayInfo info)
		{
			context.WriteInt(info.ElementCount());
		}

		protected virtual void Analyze(ObjectContainerBase container, object obj, ArrayInfo
			 info)
		{
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
