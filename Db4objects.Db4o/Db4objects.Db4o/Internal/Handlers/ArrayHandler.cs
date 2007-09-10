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
	public class ArrayHandler : BuiltinTypeHandler, IFirstClassHandler
	{
		public readonly ITypeHandler4 _handler;

		public readonly bool _isPrimitive;

		public ArrayHandler(ObjectContainerBase container, ITypeHandler4 handler, bool isPrimitive
			) : base(container)
		{
			_handler = handler;
			_isPrimitive = isPrimitive;
		}

		protected ArrayHandler(ITypeHandler4 template) : this(((Db4objects.Db4o.Internal.Handlers.ArrayHandler
			)template).Container(), ((Db4objects.Db4o.Internal.Handlers.ArrayHandler)template
			)._handler, ((Db4objects.Db4o.Internal.Handlers.ArrayHandler)template)._isPrimitive
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

		public sealed override void CascadeActivation(Transaction a_trans, object a_object
			, int a_depth, bool a_activate)
		{
			if (_handler is ClassMetadata)
			{
				a_depth--;
				object[] all = AllElements(a_object);
				if (a_activate)
				{
					for (int i = all.Length - 1; i >= 0; i--)
					{
						Container().StillToActivate(a_trans, all[i], a_depth);
					}
				}
				else
				{
					for (int i = all.Length - 1; i >= 0; i--)
					{
						Container().StillToDeactivate(a_trans, all[i], a_depth, false);
					}
				}
			}
		}

		public override IReflectClass ClassReflector()
		{
			return _handler.ClassReflector();
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
			return _isPrimitive ? hc : -hc;
		}

		public sealed override int GetID()
		{
			return _handler.GetID();
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

		public virtual int ObjectLength(object obj)
		{
			return OwnLength(obj) + (ArrayReflector().GetLength(obj) * _handler.LinkLength());
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

		public sealed override object Read(MarshallerFamily mf, StatefulBuffer a_bytes, bool
			 redirect)
		{
			return mf._array.Read(this, a_bytes);
		}

		public sealed override object ReadQuery(Transaction a_trans, MarshallerFamily mf, 
			bool withRedirection, Db4objects.Db4o.Internal.Buffer a_reader, bool a_toArray)
		{
			return mf._array.ReadQuery(this, a_trans, a_reader);
		}

		public virtual object Read1Query(Transaction a_trans, MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 a_reader)
		{
			IntByRef elements = new IntByRef();
			object ret = ReadCreate(a_trans, a_reader, elements);
			if (ret != null)
			{
				for (int i = 0; i < elements.value; i++)
				{
					ArrayReflector().Set(ret, i, _handler.ReadQuery(a_trans, mf, true, a_reader, true
						));
				}
			}
			return ret;
		}

		public virtual object Read1(MarshallerFamily mf, StatefulBuffer reader)
		{
			IntByRef elements = new IntByRef();
			object array = ReadCreate(reader.GetTransaction(), reader, elements);
			if (array != null)
			{
				if (HandleAsByteArray(array))
				{
					reader.ReadBytes((byte[])array);
				}
				else
				{
					for (int i = 0; i < elements.value; i++)
					{
						ArrayReflector().Set(array, i, _handler.Read(mf, reader, true));
					}
				}
			}
			return array;
		}

		protected virtual object ReadCreate(Transaction trans, IReadBuffer buffer, IntByRef
			 elements)
		{
			ReflectClassByRef clazz = new ReflectClassByRef();
			elements.value = ReadElementsAndClass(trans, buffer, clazz);
			if (_isPrimitive)
			{
				return ArrayReflector().NewInstance(PrimitiveClassReflector(), elements.value);
			}
			if (clazz.value != null)
			{
				return ArrayReflector().NewInstance(clazz.value, elements.value);
			}
			return null;
		}

		public virtual ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer[] a_bytes)
		{
			return this;
		}

		public virtual void ReadCandidates(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates)
		{
			mf._array.ReadCandidates(this, reader, candidates);
		}

		public virtual void Read1Candidates(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates)
		{
			IntByRef elements = new IntByRef();
			object ret = ReadCreate(candidates.i_trans, reader, elements);
			if (ret != null)
			{
				for (int i = 0; i < elements.value; i++)
				{
					QCandidate qc = _handler.ReadSubCandidate(mf, reader, candidates, true);
					if (qc != null)
					{
						candidates.AddByIdentity(qc);
					}
				}
			}
		}

		public override QCandidate ReadSubCandidate(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates, bool withIndirection)
		{
			reader.IncrementOffset(LinkLength());
			return null;
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
				clazz.value = _handler.ClassReflector();
			}
			if (Debug.ExceedsMaximumArrayEntries(elements, _isPrimitive))
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
			return _handler.ClassReflector();
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
				claxx = Container()._handlers.HandlerForClass(Container(), claxx).ClassReflector(
					);
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
