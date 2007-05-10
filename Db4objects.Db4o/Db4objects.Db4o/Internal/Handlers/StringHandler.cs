using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public sealed class StringHandler : BuiltinTypeHandler
	{
		public LatinStringIO i_stringIo;

		public StringHandler(ObjectContainerBase stream, LatinStringIO stringIO) : base(stream
			)
		{
			i_stringIo = stringIO;
		}

		public override bool CanHold(IReflectClass claxx)
		{
			return claxx.Equals(ClassReflector());
		}

		public override void CascadeActivation(Transaction a_trans, object a_object, int 
			a_depth, bool a_activate)
		{
		}

		public override IReflectClass ClassReflector()
		{
			return _stream.i_handlers.ICLASS_STRING;
		}

		public override object ComparableObject(Transaction trans, object obj)
		{
			return Val(obj, trans.Stream());
		}

		public override void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer buffer)
		{
			Slot slot = buffer.ReadSlot();
			if (slot.Address() > 0 && !mf._string.InlinedStrings())
			{
				buffer.GetTransaction().SlotFreeOnCommit(slot.Address(), slot);
			}
		}

		public override bool IsEqual(ITypeHandler4 a_dataType)
		{
			return (this == a_dataType);
		}

		public override int GetID()
		{
			return 9;
		}

		internal byte GetIdentifier()
		{
			return Const4.YAPSTRING;
		}

		public override ClassMetadata GetClassMetadata(ObjectContainerBase a_stream)
		{
			return a_stream.i_handlers.PrimitiveClassById(GetID());
		}

		public override object IndexEntryToObject(Transaction trans, object indexEntry)
		{
			try
			{
				return StringMarshaller.ReadShort(_stream, (Db4objects.Db4o.Internal.Buffer)indexEntry
					);
			}
			catch (CorruptionException)
			{
			}
			return null;
		}

		public override bool IndexNullHandling()
		{
			return true;
		}

		public override TernaryBool IsSecondClass()
		{
			return TernaryBool.YES;
		}

		public override void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, bool topLevel, object obj, bool withIndirection)
		{
			MarshallerFamily.Current()._string.CalculateLengths(trans, header, topLevel, obj, 
				withIndirection);
		}

		public override object Read(MarshallerFamily mf, StatefulBuffer a_bytes, bool redirect
			)
		{
			return mf._string.ReadFromParentSlot(a_bytes.GetStream(), a_bytes, redirect);
		}

		public override ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer[] a_bytes)
		{
			return null;
		}

		public override void ReadCandidates(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 a_bytes, QCandidates a_candidates)
		{
		}

		public override QCandidate ReadSubCandidate(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates, bool withIndirection)
		{
			try
			{
				object obj = null;
				if (withIndirection)
				{
					obj = ReadQuery(candidates.i_trans, mf, withIndirection, reader, true);
				}
				else
				{
					obj = mf._string.Read(_stream, reader);
				}
				if (obj != null)
				{
					return new QCandidate(candidates, obj, 0, true);
				}
			}
			catch (CorruptionException)
			{
			}
			return null;
		}

		/// <summary>This readIndexEntry method reads from the parent slot.</summary>
		/// <remarks>
		/// This readIndexEntry method reads from the parent slot.
		/// TODO: Consider renaming methods in Indexable4 and Typhandler4 to make direction clear.
		/// </remarks>
		/// <exception cref="CorruptionException">CorruptionException</exception>
		public override object ReadIndexEntry(MarshallerFamily mf, StatefulBuffer a_writer
			)
		{
			return mf._string.ReadIndexEntry(a_writer);
		}

		/// <summary>This readIndexEntry method reads from the actual index in the file.</summary>
		/// <remarks>
		/// This readIndexEntry method reads from the actual index in the file.
		/// TODO: Consider renaming methods in Indexable4 and Typhandler4 to make direction clear.
		/// </remarks>
		public override object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer reader)
		{
			Slot s = new Slot(reader.ReadInt(), reader.ReadInt());
			if (IsInvalidSlot(s))
			{
				return null;
			}
			return s;
		}

		private bool IsInvalidSlot(Slot slot)
		{
			return (slot.Address() == 0) && (slot.Length() == 0);
		}

		public override object ReadQuery(Transaction a_trans, MarshallerFamily mf, bool withRedirection
			, Db4objects.Db4o.Internal.Buffer a_reader, bool a_toArray)
		{
			if (!withRedirection)
			{
				return mf._string.Read(a_trans.Stream(), a_reader);
			}
			Db4objects.Db4o.Internal.Buffer reader = mf._string.ReadSlotFromParentSlot(a_trans
				.Stream(), a_reader);
			if (a_toArray)
			{
				return mf._string.ReadFromOwnSlot(a_trans.Stream(), reader);
			}
			return reader;
		}

		public void SetStringIo(LatinStringIO a_io)
		{
			i_stringIo = a_io;
		}

		public override bool SupportsIndex()
		{
			return true;
		}

		public override void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer writer, object
			 entry)
		{
			if (entry == null)
			{
				writer.WriteInt(0);
				writer.WriteInt(0);
				return;
			}
			if (entry is StatefulBuffer)
			{
				StatefulBuffer entryAsWriter = (StatefulBuffer)entry;
				writer.WriteInt(entryAsWriter.GetAddress());
				writer.WriteInt(entryAsWriter.GetLength());
				return;
			}
			if (entry is Slot)
			{
				Slot s = (Slot)entry;
				writer.WriteInt(s.Address());
				writer.WriteInt(s.Length());
				return;
			}
			throw new ArgumentException();
		}

		public override object WriteNew(MarshallerFamily mf, object a_object, bool topLevel
			, StatefulBuffer a_bytes, bool withIndirection, bool restoreLinkeOffset)
		{
			return mf._string.WriteNew(a_object, topLevel, a_bytes, withIndirection);
		}

		public void WriteShort(string a_string, Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			if (a_string == null)
			{
				a_bytes.WriteInt(0);
			}
			else
			{
				a_bytes.WriteInt(a_string.Length);
				i_stringIo.Write(a_bytes, a_string);
			}
		}

		public override int GetTypeID()
		{
			return Const4.TYPE_SIMPLE;
		}

		private Db4objects.Db4o.Internal.Buffer i_compareTo;

		private Db4objects.Db4o.Internal.Buffer Val(object obj)
		{
			return Val(obj, _stream);
		}

		private Db4objects.Db4o.Internal.Buffer Val(object obj, ObjectContainerBase oc)
		{
			if (obj is Db4objects.Db4o.Internal.Buffer)
			{
				return (Db4objects.Db4o.Internal.Buffer)obj;
			}
			if (obj is string)
			{
				return StringMarshaller.WriteShort(_stream, (string)obj);
			}
			if (obj is Slot)
			{
				Slot s = (Slot)obj;
				return oc.BufferByAddress(s.Address(), s.Length());
			}
			return null;
		}

		public override void PrepareComparison(Transaction a_trans, object obj)
		{
			i_compareTo = (Db4objects.Db4o.Internal.Buffer)obj;
		}

		public override IComparable4 PrepareComparison(object obj)
		{
			if (obj == null)
			{
				i_compareTo = null;
				return Null.INSTANCE;
			}
			i_compareTo = Val(obj);
			return this;
		}

		public override object Current()
		{
			return i_compareTo;
		}

		public override int CompareTo(object obj)
		{
			if (i_compareTo == null)
			{
				if (obj == null)
				{
					return 0;
				}
				return 1;
			}
			return Compare(i_compareTo, Val(obj));
		}

		public override bool IsEqual(object obj)
		{
			if (i_compareTo == null)
			{
				return obj == null;
			}
			return i_compareTo.ContainsTheSame(Val(obj));
		}

		public override bool IsGreater(object obj)
		{
			if (i_compareTo == null)
			{
				return obj != null;
			}
			return Compare(i_compareTo, Val(obj)) > 0;
		}

		public override bool IsSmaller(object obj)
		{
			if (i_compareTo == null)
			{
				return false;
			}
			return Compare(i_compareTo, Val(obj)) < 0;
		}

		/// <summary>
		/// returns: -x for left is greater and +x for right is greater
		/// TODO: You will need collators here for different languages.
		/// </summary>
		/// <remarks>
		/// returns: -x for left is greater and +x for right is greater
		/// TODO: You will need collators here for different languages.
		/// </remarks>
		internal int Compare(Db4objects.Db4o.Internal.Buffer a_compare, Db4objects.Db4o.Internal.Buffer
			 a_with)
		{
			if (a_compare == null)
			{
				if (a_with == null)
				{
					return 0;
				}
				return 1;
			}
			if (a_with == null)
			{
				return -1;
			}
			return Compare(a_compare._buffer, a_with._buffer);
		}

		public static int Compare(byte[] compare, byte[] with)
		{
			int min = compare.Length < with.Length ? compare.Length : with.Length;
			int start = Const4.INT_LENGTH;
			for (int i = start; i < min; i++)
			{
				if (compare[i] != with[i])
				{
					return with[i] - compare[i];
				}
			}
			return with.Length - compare.Length;
		}

		public override void DefragIndexEntry(ReaderPair readers)
		{
			readers.CopyID(false, true);
			readers.IncrementIntSize();
		}

		public override void Defrag(MarshallerFamily mf, ReaderPair readers, bool redirect
			)
		{
			if (!redirect)
			{
				readers.IncrementOffset(LinkLength());
			}
			else
			{
				mf._string.Defrag(readers);
			}
		}
	}
}
