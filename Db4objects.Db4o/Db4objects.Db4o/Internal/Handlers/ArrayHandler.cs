namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class ArrayHandler : Db4objects.Db4o.Internal.Handlers.BuiltinTypeHandler
	{
		public readonly Db4objects.Db4o.Internal.ITypeHandler4 i_handler;

		public readonly bool i_isPrimitive;

		public readonly Db4objects.Db4o.Reflect.IReflectArray _reflectArray;

		public ArrayHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream, Db4objects.Db4o.Internal.ITypeHandler4
			 a_handler, bool a_isPrimitive) : base(stream)
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

		public override bool CanHold(Db4objects.Db4o.Reflect.IReflectClass claxx)
		{
			return i_handler.CanHold(claxx);
		}

		public sealed override void CascadeActivation(Db4objects.Db4o.Internal.Transaction
			 a_trans, object a_object, int a_depth, bool a_activate)
		{
			if (i_handler is Db4objects.Db4o.Internal.ClassMetadata)
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

		public override Db4objects.Db4o.Reflect.IReflectClass ClassReflector()
		{
			return i_handler.ClassReflector();
		}

		public Db4objects.Db4o.Internal.TreeInt CollectIDs(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.TreeInt tree, Db4objects.Db4o.Internal.StatefulBuffer
			 reader)
		{
			return mf._array.CollectIDs(this, tree, reader);
		}

		public Db4objects.Db4o.Internal.TreeInt CollectIDs1(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Internal.TreeInt tree, Db4objects.Db4o.Internal.Buffer reader
			)
		{
			if (reader == null)
			{
				return tree;
			}
			int count = ElementCount(trans, reader);
			for (int i = 0; i < count; i++)
			{
				tree = (Db4objects.Db4o.Internal.TreeInt)Db4objects.Db4o.Foundation.Tree.Add(tree
					, new Db4objects.Db4o.Internal.TreeInt(reader.ReadInt()));
			}
			return tree;
		}

		public override object ComparableObject(Db4objects.Db4o.Internal.Transaction a_trans
			, object a_object)
		{
			throw Db4objects.Db4o.Internal.Exceptions4.VirtualException();
		}

		public sealed override void DeleteEmbedded(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.StatefulBuffer a_bytes)
		{
			mf._array.DeleteEmbedded(this, a_bytes);
		}

		public void DeletePrimitiveEmbedded(Db4objects.Db4o.Internal.StatefulBuffer a_bytes
			, Db4objects.Db4o.Internal.PrimitiveFieldHandler a_classPrimitive)
		{
			a_bytes.ReadInt();
			a_bytes.ReadInt();
			if (true)
			{
				return;
			}
		}

		public virtual int ElementCount(Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Internal.ISlotReader
			 reader)
		{
			int typeOrLength = reader.ReadInt();
			if (typeOrLength >= 0)
			{
				return typeOrLength;
			}
			return reader.ReadInt();
		}

		public sealed override bool Equals(Db4objects.Db4o.Internal.ITypeHandler4 a_dataType
			)
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

		public override Db4objects.Db4o.Internal.ClassMetadata GetYapClass(Db4objects.Db4o.Internal.ObjectContainerBase
			 a_stream)
		{
			return i_handler.GetYapClass(a_stream);
		}

		public virtual byte Identifier()
		{
			return Db4objects.Db4o.Internal.Const4.YAPARRAY;
		}

		public override object IndexEntryToObject(Db4objects.Db4o.Internal.Transaction trans
			, object indexEntry)
		{
			return null;
		}

		public override bool IndexNullHandling()
		{
			return i_handler.IndexNullHandling();
		}

		public override int IsSecondClass()
		{
			return i_handler.IsSecondClass();
		}

		public override void CalculateLengths(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes header, bool topLevel, 
			object obj, bool withIndirection)
		{
			Db4objects.Db4o.Internal.Marshall.MarshallerFamily.Current()._array.CalculateLengths
				(trans, header, this, obj, topLevel);
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
			return Db4objects.Db4o.Internal.Const4.OBJECT_LENGTH + Db4objects.Db4o.Internal.Const4
				.INT_LENGTH * 2;
		}

		public override void PrepareComparison(Db4objects.Db4o.Internal.Transaction a_trans
			, object obj)
		{
			PrepareComparison(obj);
		}

		public override Db4objects.Db4o.Reflect.IReflectClass PrimitiveClassReflector()
		{
			return i_handler.PrimitiveClassReflector();
		}

		public sealed override object Read(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.StatefulBuffer a_bytes, bool redirect)
		{
			return mf._array.Read(this, a_bytes);
		}

		public override object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer a_reader)
		{
			throw Db4objects.Db4o.Internal.Exceptions4.VirtualException();
		}

		public sealed override object ReadQuery(Db4objects.Db4o.Internal.Transaction a_trans
			, Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, bool withRedirection, Db4objects.Db4o.Internal.Buffer
			 a_reader, bool a_toArray)
		{
			return mf._array.ReadQuery(this, a_trans, a_reader);
		}

		public virtual object Read1Query(Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer a_reader)
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

		public virtual object Read1(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf
			, Db4objects.Db4o.Internal.StatefulBuffer reader)
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

		private object ReadCreate(Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Internal.Buffer
			 a_reader, int[] a_elements)
		{
			Db4objects.Db4o.Reflect.IReflectClass[] clazz = new Db4objects.Db4o.Reflect.IReflectClass
				[1];
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

		public override Db4objects.Db4o.Internal.ITypeHandler4 ReadArrayHandler(Db4objects.Db4o.Internal.Transaction
			 a_trans, Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer[]
			 a_bytes)
		{
			return this;
		}

		public override void ReadCandidates(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer reader, Db4objects.Db4o.Internal.Query.Processor.QCandidates
			 candidates)
		{
			mf._array.ReadCandidates(this, reader, candidates);
		}

		public virtual void Read1Candidates(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer reader, Db4objects.Db4o.Internal.Query.Processor.QCandidates
			 candidates)
		{
			int[] elements = new int[1];
			object ret = ReadCreate(candidates.i_trans, reader, elements);
			if (ret != null)
			{
				for (int i = 0; i < elements[0]; i++)
				{
					Db4objects.Db4o.Internal.Query.Processor.QCandidate qc = i_handler.ReadSubCandidate
						(mf, reader, candidates, true);
					if (qc != null)
					{
						candidates.AddByIdentity(qc);
					}
				}
			}
		}

		public override Db4objects.Db4o.Internal.Query.Processor.QCandidate ReadSubCandidate
			(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, Db4objects.Db4o.Internal.Query.Processor.QCandidates candidates, bool withIndirection
			)
		{
			reader.IncrementOffset(LinkLength());
			return null;
		}

		internal int ReadElementsAndClass(Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Internal.Buffer
			 a_bytes, Db4objects.Db4o.Reflect.IReflectClass[] clazz)
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
			if (Db4objects.Db4o.Debug.ExceedsMaximumArrayEntries(elements, i_isPrimitive))
			{
				return 0;
			}
			return elements;
		}

		protected int MapElementsEntry(int orig, Db4objects.Db4o.Internal.Mapping.IIDMapping
			 mapping)
		{
			if (orig >= 0 || orig == Db4objects.Db4o.Internal.Const4.IGNORE_ID)
			{
				return orig;
			}
			bool primitive = !Db4objects.Db4o.Deploy.csharp && orig < Db4objects.Db4o.Internal.Const4
				.PRIMITIVE;
			if (primitive)
			{
				orig -= Db4objects.Db4o.Internal.Const4.PRIMITIVE;
			}
			int origID = -orig;
			int mappedID = mapping.MappedID(origID);
			int mapped = -mappedID;
			if (primitive)
			{
				mapped += Db4objects.Db4o.Internal.Const4.PRIMITIVE;
			}
			return mapped;
		}

		private Db4objects.Db4o.Reflect.IReflectClass ReflectClassFromElementsEntry(Db4objects.Db4o.Internal.Transaction
			 a_trans, int elements)
		{
			if (elements != Db4objects.Db4o.Internal.Const4.IGNORE_ID)
			{
				bool primitive = false;
				int classID = -elements;
				Db4objects.Db4o.Internal.ClassMetadata yc = a_trans.Stream().GetYapClass(classID);
				if (yc != null)
				{
					return (primitive ? yc.PrimitiveClassReflector() : yc.ClassReflector());
				}
			}
			return i_handler.ClassReflector();
		}

		public static object[] ToArray(Db4objects.Db4o.Internal.ObjectContainerBase a_stream
			, object a_object)
		{
			if (a_object != null)
			{
				Db4objects.Db4o.Reflect.IReflectClass claxx = a_stream.Reflector().ForObject(a_object
					);
				if (claxx.IsArray())
				{
					Db4objects.Db4o.Internal.Handlers.ArrayHandler ya;
					if (a_stream.Reflector().Array().IsNDimensional(claxx))
					{
						ya = new Db4objects.Db4o.Internal.Handlers.MultidimensionalArrayHandler(a_stream, 
							null, false);
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

		internal virtual void WriteClass(object a_object, Db4objects.Db4o.Internal.StatefulBuffer
			 a_bytes)
		{
			int yapClassID = 0;
			Db4objects.Db4o.Reflect.IReflector reflector = a_bytes.GetTransaction().Reflector
				();
			Db4objects.Db4o.Reflect.IReflectClass claxx = _reflectArray.GetComponentType(reflector
				.ForObject(a_object));
			bool primitive = false;
			Db4objects.Db4o.Internal.ObjectContainerBase stream = a_bytes.GetStream();
			if (primitive)
			{
				claxx = stream.i_handlers.HandlerForClass(stream, claxx).ClassReflector();
			}
			Db4objects.Db4o.Internal.ClassMetadata yc = stream.ProduceYapClass(claxx);
			if (yc != null)
			{
				yapClassID = yc.GetID();
			}
			if (yapClassID == 0)
			{
				yapClassID = -Db4objects.Db4o.Internal.Const4.IGNORE_ID;
			}
			else
			{
				if (primitive)
				{
					yapClassID -= Db4objects.Db4o.Internal.Const4.PRIMITIVE;
				}
			}
			a_bytes.WriteInt(-yapClassID);
		}

		public override void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer a_writer, object
			 a_object)
		{
			throw Db4objects.Db4o.Internal.Exceptions4.VirtualException();
		}

		public sealed override object WriteNew(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, object a_object, bool topLevel, Db4objects.Db4o.Internal.StatefulBuffer a_bytes
			, bool withIndirection, bool restoreLinkOffset)
		{
			return mf._array.WriteNew(this, a_object, restoreLinkOffset, a_bytes);
		}

		public virtual void WriteNew1(object obj, Db4objects.Db4o.Internal.StatefulBuffer
			 writer)
		{
			WriteClass(obj, writer);
			int elements = _reflectArray.GetLength(obj);
			writer.WriteInt(elements);
			if (!i_handler.WriteArray(obj, writer))
			{
				for (int i = 0; i < elements; i++)
				{
					i_handler.WriteNew(Db4objects.Db4o.Internal.Marshall.MarshallerFamily.Current(), 
						_reflectArray.Get(obj, i), false, writer, true, true);
				}
			}
		}

		public override Db4objects.Db4o.Internal.IComparable4 PrepareComparison(object obj
			)
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

		public sealed override void Defrag(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.ReaderPair readers, bool redirect)
		{
			if (!(i_handler.IsSecondClass() == Db4objects.Db4o.Internal.Const4.YES))
			{
				mf._array.DefragIDs(this, readers);
			}
			else
			{
				readers.IncrementOffset(LinkLength());
			}
		}

		public virtual void Defrag1(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf
			, Db4objects.Db4o.Internal.ReaderPair readers)
		{
			int elements = ReadElementsDefrag(readers);
			for (int i = 0; i < elements; i++)
			{
				i_handler.Defrag(mf, readers, true);
			}
		}

		protected virtual int ReadElementsDefrag(Db4objects.Db4o.Internal.ReaderPair readers
			)
		{
			int elements = readers.Source().ReadInt();
			readers.Target().WriteInt(MapElementsEntry(elements, readers.Mapping()));
			if (elements < 0)
			{
				elements = readers.ReadInt();
			}
			return elements;
		}

		public override void DefragIndexEntry(Db4objects.Db4o.Internal.ReaderPair readers
			)
		{
			throw Db4objects.Db4o.Internal.Exceptions4.VirtualException();
		}
	}
}
