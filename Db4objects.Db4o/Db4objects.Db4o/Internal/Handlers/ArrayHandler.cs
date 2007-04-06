using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Mapping;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class ArrayHandler : BuiltinTypeHandler
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
			object[] all = new object[_reflectArray.GetLength(a_object)];
			for (int i = all.Length - 1; i >= 0; i--)
			{
				all[i] = _reflectArray.Get(a_object, i);
			}
			return all;
		}

		public override bool CanHold(IReflectClass claxx)
		{
			return i_handler.CanHold(claxx);
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

		public override object ComparableObject(Transaction a_trans, object a_object)
		{
			throw Exceptions4.VirtualException();
		}

		public sealed override void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer a_bytes
			)
		{
			mf._array.DeleteEmbedded(this, a_bytes);
		}

		public void DeletePrimitiveEmbedded(StatefulBuffer a_bytes, PrimitiveFieldHandler
			 a_classPrimitive)
		{
			a_bytes.ReadInt();
			a_bytes.ReadInt();
			if (true)
			{
				return;
			}
		}

		public virtual int ElementCount(Transaction a_trans, ISlotReader reader)
		{
			int typeOrLength = reader.ReadInt();
			if (typeOrLength >= 0)
			{
				return typeOrLength;
			}
			return reader.ReadInt();
		}

		public sealed override bool IsEqual(ITypeHandler4 a_dataType)
		{
			if (a_dataType is Db4objects.Db4o.Internal.Handlers.ArrayHandler)
			{
				if (((Db4objects.Db4o.Internal.Handlers.ArrayHandler)a_dataType).Identifier() == 
					Identifier())
				{
					return (i_handler.Equals(((Db4objects.Db4o.Internal.Handlers.ArrayHandler)a_dataType
						).i_handler));
				}
			}
			return false;
		}

		public sealed override int GetID()
		{
			return i_handler.GetID();
		}

		public override int GetTypeID()
		{
			return i_handler.GetTypeID();
		}

		public override ClassMetadata GetClassMetadata(ObjectContainerBase a_stream)
		{
			return i_handler.GetClassMetadata(a_stream);
		}

		public virtual byte Identifier()
		{
			return Const4.YAPARRAY;
		}

		public override object IndexEntryToObject(Transaction trans, object indexEntry)
		{
			return null;
		}

		public override bool IndexNullHandling()
		{
			return i_handler.IndexNullHandling();
		}

		public override TernaryBool IsSecondClass()
		{
			return i_handler.IsSecondClass();
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

		public virtual int OwnLength(object obj)
		{
			return OwnLength();
		}

		private int OwnLength()
		{
			return Const4.OBJECT_LENGTH + Const4.INT_LENGTH * 2;
		}

		public override void PrepareComparison(Transaction a_trans, object obj)
		{
			PrepareComparison(obj);
		}

		public override IReflectClass PrimitiveClassReflector()
		{
			return i_handler.PrimitiveClassReflector();
		}

		public sealed override object Read(MarshallerFamily mf, StatefulBuffer a_bytes, bool
			 redirect)
		{
			return mf._array.Read(this, a_bytes);
		}

		public override object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer a_reader)
		{
			throw Exceptions4.VirtualException();
		}

		public sealed override object ReadQuery(Transaction a_trans, MarshallerFamily mf, 
			bool withRedirection, Db4objects.Db4o.Internal.Buffer a_reader, bool a_toArray)
		{
			return mf._array.ReadQuery(this, a_trans, a_reader);
		}

		public virtual object Read1Query(Transaction a_trans, MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 a_reader)
		{
			int[] elements = new int[1];
			object ret = ReadCreate(a_trans, a_reader, elements);
			if (ret != null)
			{
				for (int i = 0; i < elements[0]; i++)
				{
					_reflectArray.Set(ret, i, i_handler.ReadQuery(a_trans, mf, true, a_reader, true));
				}
			}
			return ret;
		}

		public virtual object Read1(MarshallerFamily mf, StatefulBuffer reader)
		{
			int[] elements = new int[1];
			object ret = ReadCreate(reader.GetTransaction(), reader, elements);
			if (ret != null)
			{
				if (i_handler.ReadArray(ret, reader))
				{
					return ret;
				}
				for (int i = 0; i < elements[0]; i++)
				{
					_reflectArray.Set(ret, i, i_handler.Read(mf, reader, true));
				}
			}
			return ret;
		}

		private object ReadCreate(Transaction a_trans, Db4objects.Db4o.Internal.Buffer a_reader
			, int[] a_elements)
		{
			IReflectClass[] clazz = new IReflectClass[1];
			a_elements[0] = ReadElementsAndClass(a_trans, a_reader, clazz);
			if (i_isPrimitive)
			{
				return _reflectArray.NewInstance(i_handler.PrimitiveClassReflector(), a_elements[
					0]);
			}
			if (clazz[0] != null)
			{
				return _reflectArray.NewInstance(clazz[0], a_elements[0]);
			}
			return null;
		}

		public override ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer[] a_bytes)
		{
			return this;
		}

		public override void ReadCandidates(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates)
		{
			mf._array.ReadCandidates(this, reader, candidates);
		}

		public virtual void Read1Candidates(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates)
		{
			int[] elements = new int[1];
			object ret = ReadCreate(candidates.i_trans, reader, elements);
			if (ret != null)
			{
				for (int i = 0; i < elements[0]; i++)
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

		internal int ReadElementsAndClass(Transaction a_trans, Db4objects.Db4o.Internal.Buffer
			 a_bytes, IReflectClass[] clazz)
		{
			int elements = a_bytes.ReadInt();
			if (elements < 0)
			{
				clazz[0] = ReflectClassFromElementsEntry(a_trans, elements);
				elements = a_bytes.ReadInt();
			}
			else
			{
				clazz[0] = i_handler.ClassReflector();
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
				ClassMetadata yc = a_trans.Stream().ClassMetadataForId(classID);
				if (yc != null)
				{
					return (primitive ? yc.PrimitiveClassReflector() : yc.ClassReflector());
				}
			}
			return i_handler.ClassReflector();
		}

		public static object[] ToArray(ObjectContainerBase a_stream, object a_object)
		{
			if (a_object != null)
			{
				IReflectClass claxx = a_stream.Reflector().ForObject(a_object);
				if (claxx.IsArray())
				{
					Db4objects.Db4o.Internal.Handlers.ArrayHandler ya;
					if (a_stream.Reflector().Array().IsNDimensional(claxx))
					{
						ya = new MultidimensionalArrayHandler(a_stream, null, false);
					}
					else
					{
						ya = new Db4objects.Db4o.Internal.Handlers.ArrayHandler(a_stream, null, false);
					}
					return ya.AllElements(a_object);
				}
			}
			return new object[0];
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

		public override void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer a_writer, object
			 a_object)
		{
			throw Exceptions4.VirtualException();
		}

		public sealed override object WriteNew(MarshallerFamily mf, object a_object, bool
			 topLevel, StatefulBuffer a_bytes, bool withIndirection, bool restoreLinkOffset)
		{
			return mf._array.WriteNew(this, a_object, restoreLinkOffset, a_bytes);
		}

		public virtual void WriteNew1(object obj, StatefulBuffer writer)
		{
			WriteClass(obj, writer);
			int elements = _reflectArray.GetLength(obj);
			writer.WriteInt(elements);
			if (!i_handler.WriteArray(obj, writer))
			{
				for (int i = 0; i < elements; i++)
				{
					i_handler.WriteNew(MarshallerFamily.Current(), _reflectArray.Get(obj, i), false, 
						writer, true, true);
				}
			}
		}

		public override IComparable4 PrepareComparison(object obj)
		{
			i_handler.PrepareComparison(obj);
			return this;
		}

		public override object Current()
		{
			return i_handler.Current();
		}

		public override int CompareTo(object a_obj)
		{
			return -1;
		}

		public override bool IsEqual(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			object[] compareWith = AllElements(obj);
			for (int j = 0; j < compareWith.Length; j++)
			{
				if (i_handler.IsEqual(compareWith[j]))
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsGreater(object obj)
		{
			object[] compareWith = AllElements(obj);
			for (int j = 0; j < compareWith.Length; j++)
			{
				if (i_handler.IsGreater(compareWith[j]))
				{
					return true;
				}
			}
			return false;
		}

		public override bool IsSmaller(object obj)
		{
			object[] compareWith = AllElements(obj);
			for (int j = 0; j < compareWith.Length; j++)
			{
				if (i_handler.IsSmaller(compareWith[j]))
				{
					return true;
				}
			}
			return false;
		}

		public override bool SupportsIndex()
		{
			return false;
		}

		public sealed override void Defrag(MarshallerFamily mf, ReaderPair readers, bool 
			redirect)
		{
			if (!(i_handler.IsSecondClass() == TernaryBool.YES))
			{
				mf._array.DefragIDs(this, readers);
			}
			else
			{
				readers.IncrementOffset(LinkLength());
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

		public override void DefragIndexEntry(ReaderPair readers)
		{
			throw Exceptions4.VirtualException();
		}
	}
}
