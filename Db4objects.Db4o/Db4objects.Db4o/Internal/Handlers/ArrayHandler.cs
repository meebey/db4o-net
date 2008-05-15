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
			if (cm.IsPrimitive())
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
			CollectIDsWith(context, new _IClosure4_106(this, context));
		}

		private sealed class _IClosure4_106 : IClosure4
		{
			public _IClosure4_106(ArrayHandler _enclosing, CollectIdContext context)
			{
				this._enclosing = _enclosing;
				this.context = context;
			}

			public object Run()
			{
				if (context.Buffer() == null)
				{
					return null;
				}
				int elementCount = this._enclosing.ElementCount(context.Transaction(), context);
				elementCount -= this._enclosing.ReducedCountForNullBitMap(context, elementCount);
				for (int i = 0; i < elementCount; i++)
				{
					context.AddId();
				}
				return null;
			}

			private readonly ArrayHandler _enclosing;

			private readonly CollectIdContext context;
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

		protected virtual void CollectIDsWith(CollectIdContext context, IClosure4 closure
			)
		{
			context.Seek(context.ReadInt());
			closure.Run();
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			if (context.CascadeDelete() && _handler is ClassMetadata)
			{
				int elementCount = ElementCount(context.Transaction(), context);
				elementCount -= ReducedCountForNullBitMap(context, elementCount);
				for (int i = elementCount; i > 0; i--)
				{
					_handler.Delete(context);
				}
			}
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

		/// <param name="trans"></param>
		public virtual int ElementCount(Transaction trans, IReadBuffer reader)
		{
			int typeOrLength = reader.ReadInt();
			if (typeOrLength >= 0)
			{
				return typeOrLength;
			}
			return reader.ReadInt();
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

		protected virtual object ReadCreate(Transaction trans, IReadBuffer buffer, IntByRef
			 elements)
		{
			ReflectClassByRef classByRef = new ReflectClassByRef();
			elements.value = ReadElementsAndClass(trans, buffer, classByRef);
			IReflectClass clazz = NewInstanceReflectClass(trans.Reflector(), classByRef);
			if (clazz == null)
			{
				return null;
			}
			return ArrayReflector(Container(trans)).NewInstance(clazz, elements.value);
		}

		protected virtual IReflectClass NewInstanceReflectClass(IReflector reflector, ReflectClassByRef
			 byRef)
		{
			if (_usePrimitiveClassReflector)
			{
				return PrimitiveClassReflector(reflector);
			}
			return byRef.value;
		}

		public virtual ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, ByteArrayBuffer[] a_bytes)
		{
			return this;
		}

		public virtual void ReadCandidates(QueryingReadContext context)
		{
			ReadSubCandidates(context);
		}

		public virtual void ReadSubCandidates(QueryingReadContext context)
		{
			IntByRef elements = new IntByRef();
			object arr = ReadCreate(context.Transaction(), context, elements);
			if (arr == null)
			{
				return;
			}
			int elementCount = elements.value;
			elementCount -= ReducedCountForNullBitMap(context, elementCount);
			ReadSubCandidates(context, elementCount);
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

		internal int ReadElementsAndClass(Transaction trans, IReadBuffer buffer, ReflectClassByRef
			 clazz)
		{
			int elements = buffer.ReadInt();
			if (NewerArrayFormat(elements))
			{
				clazz.value = ReflectClassFromElementsEntry(trans, elements);
				elements = buffer.ReadInt();
			}
			else
			{
				clazz.value = ClassReflector(Container(trans));
			}
			if (Debug.ExceedsMaximumArrayEntries(elements, _usePrimitiveClassReflector))
			{
				return 0;
			}
			return elements;
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

		private IReflectClass ReflectClassFromElementsEntry(Transaction trans, int elements
			)
		{
			// TODO: Here is a low-frequency mistake, extremely unlikely.
			// If YapClass-ID == 99999 by accident then we will get ignore.
			if (elements != Const4.IgnoreId)
			{
				bool primitive = false;
				if (UseJavaHandling())
				{
					if (elements < Const4.Primitive)
					{
						primitive = true;
						elements -= Const4.Primitive;
					}
				}
				int classID = -elements;
				ClassMetadata classMetadata = Container(trans).ClassMetadataForId(classID);
				if (classMetadata != null)
				{
					return ClassReflector(trans.Reflector(), classMetadata, primitive);
				}
			}
			return ClassReflector(Container(trans));
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
			bool primitive = IsPrimitive(claxx);
			if (primitive)
			{
				claxx = container.ProduceClassMetadata(claxx).ClassReflector();
			}
			ClassMetadata classMetadata = container.ProduceClassMetadata(claxx);
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

		protected virtual bool IsPrimitive(IReflectClass claxx)
		{
			if (NullableArrayHandling.Enabled())
			{
				return claxx.IsPrimitive();
			}
			return false;
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
			context.Seek(offset);
		}

		protected virtual int PreparePayloadRead(IDefragmentContext context)
		{
			return context.Offset();
		}

		public virtual void Defrag1(IDefragmentContext context)
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
			ReflectClassByRef clazzRef = new ReflectClassByRef();
			int offset = context.Offset();
			ReadElementsAndClass(context.Transaction(), context, clazzRef);
			bool isByteArray = context.Transaction().Reflector().ForClass(typeof(byte)).Equals
				(clazzRef.value);
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
			IntByRef elements = new IntByRef();
			object array = ReadCreate(context.Transaction(), context, elements);
			if (array != null)
			{
				if (HandleAsByteArray(array))
				{
					context.ReadBytes((byte[])array);
				}
				else
				{
					// byte[] performance optimisation
					if (HasNullBitmap())
					{
						BitMap4 nullBitMap = ReadNullBitmap(context, elements.value);
						for (int i = 0; i < elements.value; i++)
						{
							object obj = nullBitMap.IsTrue(i) ? null : context.ReadObject(_handler);
							ArrayReflector(Container(context)).Set(array, i, obj);
						}
					}
					else
					{
						for (int i = 0; i < elements.value; i++)
						{
							ArrayReflector(Container(context)).Set(array, i, context.ReadObject(_handler));
						}
					}
				}
			}
			return array;
		}

		private BitMap4 ReadNullBitmap(IReadBuffer context, int length)
		{
			return context.ReadBitMap(length);
		}

		protected virtual bool HasNullBitmap()
		{
			return NullableArrayHandling.Enabled();
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			int classID = ClassID(Container(context), obj);
			context.WriteInt(classID);
			int elementCount = ArrayReflector(Container(context)).GetLength(obj);
			context.WriteInt(elementCount);
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
					for (int i = 0; i < elementCount; i++)
					{
						if (!nullItems.IsTrue(i))
						{
							context.WriteObject(_handler, ArrayReflector(Container(context)).Get(obj, i));
						}
					}
				}
				else
				{
					for (int i = 0; i < elementCount; i++)
					{
						context.WriteObject(_handler, ArrayReflector(Container(context)).Get(obj, i));
					}
				}
			}
		}

		private void WriteNullBitmap(IWriteBuffer context, BitMap4 bitMap)
		{
			context.WriteBytes(bitMap.Bytes());
		}

		private BitMap4 NullItemsMap(IReflectArray reflector, object array)
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
