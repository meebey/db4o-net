/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class ArrayHandler : IFirstClassHandler, IComparable4, ITypeHandler4, IVariableLengthTypeHandler
	{
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

		public readonly ITypeHandler4 _handler;

		public readonly bool _usePrimitiveClassReflector;

		private readonly ObjectContainerBase _container;

		public ArrayHandler(ObjectContainerBase container, ITypeHandler4 handler, bool usePrimitiveClassReflector
			)
		{
			_container = container;
			_handler = handler;
			_usePrimitiveClassReflector = usePrimitiveClassReflector;
		}

		protected ArrayHandler(ArrayHandler template, HandlerRegistry registry, int version
			) : this(template.Container(), registry.CorrectHandlerVersion(template._handler, 
			version), template._usePrimitiveClassReflector)
		{
		}

		protected virtual IReflectArray ArrayReflector()
		{
			return Container().Reflector().Array();
		}

		public virtual IEnumerator AllElements(object a_object)
		{
			return AllElements(ArrayReflector(), a_object);
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
			IEnumerator all = AllElements(onObject);
			while (all.MoveNext())
			{
				object current = all.Current;
				IActivationDepth elementDepth = Descend(depth, current);
				if (elementDepth.RequiresActivation())
				{
					if (depth.Mode().IsDeactivate())
					{
						Container().StillToDeactivate(trans, current, elementDepth, false);
					}
					else
					{
						Container().StillToActivate(trans, current, elementDepth);
					}
				}
			}
		}

		public virtual ObjectContainerBase Container()
		{
			return _container;
		}

		private IActivationDepth Descend(IActivationDepth depth, object obj)
		{
			if (obj == null)
			{
				return new NonDescendingActivationDepth(depth.Mode());
			}
			ClassMetadata cm = ClassMetaDataForObject(obj);
			if (cm.IsPrimitive())
			{
				return new NonDescendingActivationDepth(depth.Mode());
			}
			return depth.Descend(cm);
		}

		private ClassMetadata ClassMetaDataForObject(object obj)
		{
			return Container().ClassMetadataForObject(obj);
		}

		public virtual IReflectClass ClassReflector()
		{
			if (_handler is IBuiltinTypeHandler)
			{
				return ((IBuiltinTypeHandler)_handler).ClassReflector();
			}
			if (_handler is ClassMetadata)
			{
				return ((ClassMetadata)_handler).ClassReflector();
			}
			return Container().Handlers().ClassReflectorForHandler(_handler);
		}

		/// <exception cref="Db4oIOException"></exception>
		public TreeInt CollectIDs(MarshallerFamily mf, TreeInt tree, StatefulBuffer reader
			)
		{
			return mf._array.CollectIDs(this, tree, reader);
		}

		public TreeInt CollectIDs1(Transaction trans, TreeInt tree, ByteArrayBuffer reader
			)
		{
			if (reader == null)
			{
				return tree;
			}
			int count = ElementCount(trans, reader);
			for (int i = 0; i < count; i++)
			{
				tree = (TreeInt)Tree.Add(tree, new TreeInt(reader.ReadInt()));
			}
			return tree;
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			int address = context.ReadInt();
			context.ReadInt();
			// length, not needed
			if (address <= 0)
			{
				return;
			}
			int linkOffSet = context.Offset();
			if (context.CascadeDeleteDepth() > 0 && _handler is ClassMetadata)
			{
				context.Seek(address);
				for (int i = ElementCount(context.Transaction(), context); i > 0; i--)
				{
					_handler.Delete(context);
				}
			}
			if (linkOffSet > 0)
			{
				context.Seek(linkOffSet);
			}
		}

		/// <param name="classPrimitive"></param>
		public void DeletePrimitiveEmbedded(StatefulBuffer a_bytes, PrimitiveFieldHandler
			 classPrimitive)
		{
			// FIXME: This code has not been called in any test case when the 
			//        new ArrayMarshaller was written.
			//        Apparently it only frees slots.
			//        For now the code simply returns without freeing.
			a_bytes.ReadInt();
			//int address = a_bytes.readInt();
			a_bytes.ReadInt();
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
			if (!(obj is ArrayHandler))
			{
				return false;
			}
			if (((ArrayHandler)obj).Identifier() != Identifier())
			{
				return false;
			}
			return (_handler.Equals(((ArrayHandler)obj)._handler));
		}

		public override int GetHashCode()
		{
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

		public virtual IReflectClass PrimitiveClassReflector()
		{
			return Handlers4.PrimitiveClassReflector(_handler);
		}

		protected virtual object ReadCreate(Transaction trans, IReadBuffer buffer, IntByRef
			 elements)
		{
			ReflectClassByRef classByRef = new ReflectClassByRef();
			elements.value = ReadElementsAndClass(trans, buffer, classByRef);
			IReflectClass clazz = NewInstanceReflectClass(classByRef);
			if (clazz == null)
			{
				return null;
			}
			return ArrayReflector().NewInstance(clazz, elements.value);
		}

		protected virtual IReflectClass NewInstanceReflectClass(ReflectClassByRef byRef)
		{
			if (_usePrimitiveClassReflector)
			{
				return PrimitiveClassReflector();
			}
			return byRef.value;
		}

		public virtual ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, ByteArrayBuffer[] a_bytes)
		{
			return this;
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual void ReadCandidates(int handlerVersion, ByteArrayBuffer reader, QCandidates
			 candidates)
		{
			reader.Seek(reader.ReadInt());
			ReadSubCandidates(handlerVersion, reader, candidates);
		}

		public virtual void ReadSubCandidates(int handlerVersion, ByteArrayBuffer reader, 
			QCandidates candidates)
		{
			IntByRef elements = new IntByRef();
			object arr = ReadCreate(candidates.i_trans, reader, elements);
			if (arr == null)
			{
				return;
			}
			ReadSubCandidates(handlerVersion, reader, candidates, elements.value);
		}

		protected virtual void ReadSubCandidates(int handlerVersion, ByteArrayBuffer reader
			, QCandidates candidates, int count)
		{
			QueryingReadContext context = new QueryingReadContext(candidates.Transaction(), handlerVersion
				, reader);
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
				clazz.value = ClassReflector();
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
			bool primitive = !Deploy.csharp && orig < Const4.Primitive;
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
				int classID = -elements;
				ClassMetadata classMetadata = trans.Container().ClassMetadataForId(classID);
				if (classMetadata != null)
				{
					return (primitive ? Handlers4.PrimitiveClassReflector(classMetadata) : classMetadata
						.ClassReflector());
				}
			}
			return ClassReflector();
		}

		public static IEnumerator Iterator(IReflectClass claxx, object obj)
		{
			IReflectArray reflectArray = claxx.Reflector().Array();
			if (reflectArray.IsNDimensional(claxx))
			{
				return MultidimensionalArrayHandler.AllElements(reflectArray, obj);
			}
			return ArrayHandler.AllElements(reflectArray, obj);
		}

		protected int ClassID(object obj)
		{
			IReflectClass claxx = ComponentType(obj);
			bool primitive = Deploy.csharp ? false : claxx.IsPrimitive();
			if (primitive)
			{
				claxx = Container().ProduceClassMetadata(claxx).ClassReflector();
			}
			ClassMetadata classMetadata = Container().ProduceClassMetadata(claxx);
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

		private IReflectClass ComponentType(object obj)
		{
			return ArrayReflector().GetComponentType(Reflector().ForObject(obj));
		}

		private IReflector Reflector()
		{
			return Container().Reflector();
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

		private int PreparePayloadRead(IDefragmentContext context)
		{
			int newPayLoadOffset = context.ReadInt();
			context.ReadInt();
			// skip length, not needed
			int linkOffSet = context.Offset();
			context.Seek(newPayLoadOffset);
			return linkOffSet;
		}

		public virtual void Defrag1(IDefragmentContext context)
		{
			Defrag2(context);
		}

		public virtual void Defrag2(IDefragmentContext context)
		{
			if (!(_handler is UntypedFieldHandler))
			{
				DefragElements(context);
				return;
			}
			ReflectClassByRef clazzRef = new ReflectClassByRef();
			int offset = context.Offset();
			int numElements = ReadElementsAndClass(context.Transaction(), context, clazzRef);
			if (!(context.Transaction().Reflector().ForClass(typeof(byte)).Equals(clazzRef.value
				)))
			{
				// FIXME behavior should be identical to seek/defrag below - why failure?
				// defragElements(context, numElements);
				context.Seek(offset);
				DefragElements(context);
				return;
			}
			context.IncrementOffset(numElements);
		}

		private void DefragElements(IDefragmentContext context)
		{
			int elements = ReadElementsDefrag(context);
			DefragElements(context, elements);
		}

		private void DefragElements(IDefragmentContext context, int elements)
		{
			for (int i = 0; i < elements; i++)
			{
				_handler.Defragment(context);
			}
		}

		protected virtual int ReadElementsDefrag(IDefragmentContext context)
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
					for (int i = 0; i < elements.value; i++)
					{
						ArrayReflector().Set(array, i, context.ReadObject(_handler));
					}
				}
			}
			return array;
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			int classID = ClassID(obj);
			context.WriteInt(classID);
			int elementCount = ArrayReflector().GetLength(obj);
			context.WriteInt(elementCount);
			if (HandleAsByteArray(obj))
			{
				context.WriteBytes((byte[])obj);
			}
			else
			{
				// byte[] performance optimisation
				for (int i = 0; i < elementCount; i++)
				{
					context.WriteObject(_handler, ArrayReflector().Get(obj, i));
				}
			}
		}

		public virtual IPreparedComparison PrepareComparison(object obj)
		{
			return new PreparedArrayContainsComparison(this, _handler, obj);
		}

		public virtual int LinkLength()
		{
			return Const4.IndirectionLength;
		}
	}
}
