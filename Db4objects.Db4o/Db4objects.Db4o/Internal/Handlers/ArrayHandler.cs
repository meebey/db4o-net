/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Mapping;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class ArrayHandler : VariableLengthTypeHandler, IFirstClassHandler, IComparable4
	{
		public readonly ITypeHandler4 _handler;

		public readonly bool _usePrimitiveClassReflector;

		public ArrayHandler(ObjectContainerBase container, ITypeHandler4 handler, bool usePrimitiveClassReflector
			) : base(container)
		{
			_handler = handler;
			_usePrimitiveClassReflector = usePrimitiveClassReflector;
		}

		protected ArrayHandler(ITypeHandler4 template) : this(((Db4objects.Db4o.Internal.Handlers.ArrayHandler
			)template).Container(), ((Db4objects.Db4o.Internal.Handlers.ArrayHandler)template
			)._handler, ((Db4objects.Db4o.Internal.Handlers.ArrayHandler)template)._usePrimitiveClassReflector
			)
		{
		}

		protected virtual IReflectArray ArrayReflector()
		{
			return Container().Reflector().Array();
		}

		public virtual object[] AllElements(object a_object)
		{
			return AllElements(ArrayReflector(), a_object);
		}

		public static object[] AllElements(IReflectArray reflectArray, object array)
		{
			object[] all = new object[reflectArray.GetLength(array)];
			for (int i = all.Length - 1; i >= 0; i--)
			{
				all[i] = reflectArray.Get(array, i);
			}
			return all;
		}

		public void CascadeActivation(Transaction trans, object onObject, int depth, bool
			 activate)
		{
			if (!(_handler is ClassMetadata))
			{
				return;
			}
			depth--;
			object[] all = AllElements(onObject);
			if (activate)
			{
				for (int i = all.Length - 1; i >= 0; i--)
				{
					Container().StillToActivate(trans, all[i], depth);
				}
			}
			else
			{
				for (int i = all.Length - 1; i >= 0; i--)
				{
					Container().StillToDeactivate(trans, all[i], depth, false);
				}
			}
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

		public TreeInt CollectIDs(MarshallerFamily mf, TreeInt tree, StatefulBuffer reader
			)
		{
			return mf._array.CollectIDs(this, tree, reader);
		}

		public TreeInt CollectIDs1(Transaction trans, TreeInt tree, Db4objects.Db4o.Internal.Buffer
			 reader)
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

		public sealed override void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer a_bytes
			)
		{
			mf._array.DeleteEmbedded(this, a_bytes);
		}

		/// <param name="classPrimitive"></param>
		public void DeletePrimitiveEmbedded(StatefulBuffer a_bytes, PrimitiveFieldHandler
			 classPrimitive)
		{
			a_bytes.ReadInt();
			a_bytes.ReadInt();
			if (true)
			{
				return;
			}
		}

		/// <param name="trans"></param>
		public virtual int ElementCount(Transaction trans, ISlotBuffer reader)
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
			if (((Db4objects.Db4o.Internal.Handlers.ArrayHandler)obj).Identifier() != Identifier
				())
			{
				return false;
			}
			return (_handler.Equals(((Db4objects.Db4o.Internal.Handlers.ArrayHandler)obj)._handler
				));
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
			return Const4.YAPARRAY;
		}

		/// <param name="obj"></param>
		public virtual int OwnLength(object obj)
		{
			return OwnLength();
		}

		private int OwnLength()
		{
			return Const4.OBJECT_LENGTH + Const4.INT_LENGTH * 2;
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
			 mf, Db4objects.Db4o.Internal.Buffer[] a_bytes)
		{
			return this;
		}

		public virtual void ReadCandidates(int handlerVersion, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates)
		{
			reader.Seek(reader.ReadInt());
			ReadSubCandidates(handlerVersion, reader, candidates);
		}

		public virtual void ReadSubCandidates(int handlerVersion, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates)
		{
			IntByRef elements = new IntByRef();
			object arr = ReadCreate(candidates.i_trans, reader, elements);
			if (arr == null)
			{
				return;
			}
			ReadSubCandidates(handlerVersion, reader, candidates, elements.value);
		}

		protected virtual void ReadSubCandidates(int handlerVersion, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates, int count)
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
			if (elements < 0)
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

		protected int MapElementsEntry(int orig, IIDMapping mapping)
		{
			if (orig >= 0 || orig == Const4.IGNORE_ID)
			{
				return orig;
			}
			bool primitive = !Deploy.csharp && orig < Const4.PRIMITIVE;
			if (primitive)
			{
				orig -= Const4.PRIMITIVE;
			}
			int origID = -orig;
			int mappedID = mapping.MappedID(origID);
			int mapped = -mappedID;
			if (primitive)
			{
				mapped += Const4.PRIMITIVE;
			}
			return mapped;
		}

		private IReflectClass ReflectClassFromElementsEntry(Transaction trans, int elements
			)
		{
			if (elements != Const4.IGNORE_ID)
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

		public static object[] ToArray(ObjectContainerBase stream, object obj)
		{
			GenericReflector reflector = stream.Reflector();
			IReflectClass claxx = reflector.ForObject(obj);
			IReflectArray reflectArray = reflector.Array();
			if (reflectArray.IsNDimensional(claxx))
			{
				return MultidimensionalArrayHandler.AllElements(reflectArray, obj);
			}
			return Db4objects.Db4o.Internal.Handlers.ArrayHandler.AllElements(reflectArray, obj
				);
		}

		protected int ClassID(object obj)
		{
			IReflectClass claxx = ComponentType(obj);
			bool primitive = Deploy.csharp ? false : claxx.IsPrimitive();
			if (primitive)
			{
				claxx = Container()._handlers.ClassMetadataForClass(Container(), claxx).ClassReflector
					();
			}
			ClassMetadata classMetadata = Container().ProduceClassMetadata(claxx);
			if (classMetadata == null)
			{
				return Const4.IGNORE_ID;
			}
			int classID = classMetadata.GetID();
			if (primitive)
			{
				classID -= Const4.PRIMITIVE;
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

		public override IComparable4 PrepareComparison(object obj)
		{
			_handler.PrepareComparison(obj);
			return this;
		}

		public override int CompareTo(object a_obj)
		{
			return -1;
		}

		public virtual bool IsEqual(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			object[] compareWith = AllElements(obj);
			for (int j = 0; j < compareWith.Length; j++)
			{
				if (_handler.CompareTo(compareWith[j]) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool IsGreater(object obj)
		{
			object[] compareWith = AllElements(obj);
			for (int j = 0; j < compareWith.Length; j++)
			{
				if (_handler.CompareTo(compareWith[j]) > 0)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool IsSmaller(object obj)
		{
			object[] compareWith = AllElements(obj);
			for (int j = 0; j < compareWith.Length; j++)
			{
				if (_handler.CompareTo(compareWith[j]) < 0)
				{
					return true;
				}
			}
			return false;
		}

		public sealed override void Defrag(MarshallerFamily mf, BufferPair readers, bool 
			redirect)
		{
			if (Handlers4.HandlesSimple(_handler))
			{
				readers.IncrementOffset(LinkLength());
			}
			else
			{
				mf._array.DefragIDs(this, readers);
			}
		}

		public virtual void Defrag1(MarshallerFamily mf, BufferPair readers)
		{
			int elements = ReadElementsDefrag(readers);
			for (int i = 0; i < elements; i++)
			{
				_handler.Defrag(mf, readers, true);
			}
		}

		protected virtual int ReadElementsDefrag(BufferPair readers)
		{
			int elements = readers.Source().ReadInt();
			readers.Target().WriteInt(MapElementsEntry(elements, readers.Mapping()));
			if (elements < 0)
			{
				elements = readers.ReadInt();
			}
			return elements;
		}

		public override object Read(IReadContext context)
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
					for (int i = 0; i < elements.value; i++)
					{
						ArrayReflector().Set(array, i, context.ReadObject(_handler));
					}
				}
			}
			return array;
		}

		public override void Write(IWriteContext context, object obj)
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
				for (int i = 0; i < elementCount; i++)
				{
					context.WriteObject(_handler, ArrayReflector().Get(obj, i));
				}
			}
		}
	}
}
