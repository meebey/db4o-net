/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Mapping;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class ArrayHandler : BuiltinTypeHandler, IFirstClassHandler
	{
		public readonly ITypeHandler4 i_handler;

		public readonly bool i_isPrimitive;

		public readonly IReflectArray _reflectArray;

		public ArrayHandler(ObjectContainerBase stream, ITypeHandler4 a_handler, bool a_isPrimitive
			) : base(stream)
		{
			i_handler = a_handler;
			i_isPrimitive = a_isPrimitive;
			_reflectArray = stream.Reflector().Array();
		}

		public virtual object[] AllElements(object a_object)
		{
			return AllElements(_reflectArray, a_object);
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
			if (i_handler is ClassMetadata)
			{
				a_depth--;
				object[] all = AllElements(a_object);
				if (a_activate)
				{
					for (int i = all.Length - 1; i >= 0; i--)
					{
						_stream.StillToActivate(all[i], a_depth);
					}
				}
				else
				{
					for (int i = all.Length - 1; i >= 0; i--)
					{
						_stream.StillToDeactivate(all[i], a_depth, false);
					}
				}
			}
		}

		public override IReflectClass ClassReflector()
		{
			return i_handler.ClassReflector();
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
		public virtual int ElementCount(Transaction trans, ISlotReader reader)
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
			return (i_handler.Equals(((Db4objects.Db4o.Internal.Handlers.ArrayHandler)obj).i_handler
				));
		}

		public sealed override int GetID()
		{
			return i_handler.GetID();
		}

		private bool HandleAsByteArray(object obj)
		{
			return obj.GetType() == typeof(byte[]);
			return obj is byte[];
		}

		public virtual byte Identifier()
		{
			return Const4.YAPARRAY;
		}

		public override void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, bool topLevel, object obj, bool withIndirection)
		{
			MarshallerFamily.Current()._array.CalculateLengths(trans, header, this, obj, topLevel
				);
		}

		public virtual int ObjectLength(object obj)
		{
			return OwnLength(obj) + (_reflectArray.GetLength(obj) * i_handler.LinkLength());
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
			return Handlers4.PrimitiveClassReflector(i_handler);
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
					_reflectArray.Set(ret, i, i_handler.ReadQuery(a_trans, mf, true, a_reader, true));
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
						_reflectArray.Set(array, i, i_handler.Read(mf, reader, true));
					}
				}
			}
			return array;
		}

		private object ReadCreate(Transaction trans, Db4objects.Db4o.Internal.Buffer buffer
			, IntByRef elements)
		{
			ReflectClassByRef clazz = new ReflectClassByRef();
			elements.value = ReadElementsAndClass(trans, buffer, clazz);
			if (i_isPrimitive)
			{
				return _reflectArray.NewInstance(PrimitiveClassReflector(), elements.value);
			}
			if (clazz.value != null)
			{
				return _reflectArray.NewInstance(clazz.value, elements.value);
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
					QCandidate qc = i_handler.ReadSubCandidate(mf, reader, candidates, true);
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

		internal int ReadElementsAndClass(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 buffer, ReflectClassByRef clazz)
		{
			int elements = buffer.ReadInt();
			if (elements < 0)
			{
				clazz.value = ReflectClassFromElementsEntry(trans, elements);
				elements = buffer.ReadInt();
			}
			else
			{
				clazz.value = i_handler.ClassReflector();
			}
			if (Debug.ExceedsMaximumArrayEntries(elements, i_isPrimitive))
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

		private IReflectClass ReflectClassFromElementsEntry(Transaction a_trans, int elements
			)
		{
			if (elements != Const4.IGNORE_ID)
			{
				bool primitive = false;
				int classID = -elements;
				ClassMetadata classMetadata = a_trans.Stream().ClassMetadataForId(classID);
				if (classMetadata != null)
				{
					return (primitive ? Handlers4.PrimitiveClassReflector(classMetadata) : classMetadata
						.ClassReflector());
				}
			}
			return i_handler.ClassReflector();
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

		internal virtual void WriteClass(object a_object, StatefulBuffer a_bytes)
		{
			int yapClassID = 0;
			IReflector reflector = a_bytes.GetTransaction().Reflector();
			IReflectClass claxx = _reflectArray.GetComponentType(reflector.ForObject(a_object
				));
			bool primitive = false;
			ObjectContainerBase stream = a_bytes.GetStream();
			if (primitive)
			{
				claxx = stream.i_handlers.HandlerForClass(stream, claxx).ClassReflector();
			}
			ClassMetadata yc = stream.ProduceClassMetadata(claxx);
			if (yc != null)
			{
				yapClassID = yc.GetID();
			}
			if (yapClassID == 0)
			{
				yapClassID = -Const4.IGNORE_ID;
			}
			else
			{
				if (primitive)
				{
					yapClassID -= Const4.PRIMITIVE;
				}
			}
			a_bytes.WriteInt(-yapClassID);
		}

		public sealed override object Write(MarshallerFamily mf, object a_object, bool topLevel
			, StatefulBuffer a_bytes, bool withIndirection, bool restoreLinkOffset)
		{
			return mf._array.WriteNew(this, a_object, restoreLinkOffset, a_bytes);
		}

		public virtual void WriteNew1(object obj, StatefulBuffer writer)
		{
			WriteClass(obj, writer);
			int elements = _reflectArray.GetLength(obj);
			writer.WriteInt(elements);
			if (HandleAsByteArray(obj))
			{
				writer.Append((byte[])obj);
			}
			else
			{
				for (int i = 0; i < elements; i++)
				{
					i_handler.Write(MarshallerFamily.Current(), _reflectArray.Get(obj, i), false, writer
						, true, true);
				}
			}
		}

		public override IComparable4 PrepareComparison(object obj)
		{
			i_handler.PrepareComparison(obj);
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
				if (i_handler.CompareTo(compareWith[j]) == 0)
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
				if (i_handler.CompareTo(compareWith[j]) > 0)
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
				if (i_handler.CompareTo(compareWith[j]) < 0)
				{
					return true;
				}
			}
			return false;
		}

		public sealed override void Defrag(MarshallerFamily mf, ReaderPair readers, bool 
			redirect)
		{
			if (Handlers4.HandlesSimple(i_handler))
			{
				readers.IncrementOffset(LinkLength());
			}
			else
			{
				mf._array.DefragIDs(this, readers);
			}
		}

		public virtual void Defrag1(MarshallerFamily mf, ReaderPair readers)
		{
			int elements = ReadElementsDefrag(readers);
			for (int i = 0; i < elements; i++)
			{
				i_handler.Defrag(mf, readers, true);
			}
		}

		protected virtual int ReadElementsDefrag(ReaderPair readers)
		{
			int elements = readers.Source().ReadInt();
			readers.Target().WriteInt(MapElementsEntry(elements, readers.Mapping()));
			if (elements < 0)
			{
				elements = readers.ReadInt();
			}
			return elements;
		}
	}
}
