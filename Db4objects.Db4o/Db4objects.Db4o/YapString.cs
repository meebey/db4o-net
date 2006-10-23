namespace Db4objects.Db4o
{
	/// <summary>
	/// YapString
	/// Legacy rename for C# obfuscator production trouble
	/// </summary>
	/// <exclude></exclude>
	public sealed class YapString : Db4objects.Db4o.YapIndependantType
	{
		public Db4objects.Db4o.YapStringIO i_stringIo;

		public YapString(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapStringIO stringIO
			) : base(stream)
		{
			i_stringIo = stringIO;
		}

		public override bool CanHold(Db4objects.Db4o.Reflect.IReflectClass claxx)
		{
			return claxx.Equals(ClassReflector());
		}

		public override void CascadeActivation(Db4objects.Db4o.Transaction a_trans, object
			 a_object, int a_depth, bool a_activate)
		{
		}

		public override Db4objects.Db4o.Reflect.IReflectClass ClassReflector()
		{
			return _stream.i_handlers.ICLASS_STRING;
		}

		public override object ComparableObject(Db4objects.Db4o.Transaction a_trans, object
			 a_object)
		{
			if (a_object == null)
			{
				return null;
			}
			if (a_object is Db4objects.Db4o.YapReader)
			{
				return a_object;
			}
			Db4objects.Db4o.Inside.Slots.Slot s = (Db4objects.Db4o.Inside.Slots.Slot)a_object;
			return a_trans.Stream().ReadReaderByAddress(s._address, s._length);
		}

		public override void DeleteEmbedded(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapWriter a_bytes)
		{
			int address = a_bytes.ReadInt();
			int length = a_bytes.ReadInt();
			if (address > 0 && !mf._string.InlinedStrings())
			{
				a_bytes.GetTransaction().SlotFreeOnCommit(address, address, length);
			}
		}

		public override bool Equals(Db4objects.Db4o.ITypeHandler4 a_dataType)
		{
			return (this == a_dataType);
		}

		public override int GetID()
		{
			return 9;
		}

		internal byte GetIdentifier()
		{
			return Db4objects.Db4o.YapConst.YAPSTRING;
		}

		public override Db4objects.Db4o.YapClass GetYapClass(Db4objects.Db4o.YapStream a_stream
			)
		{
			return a_stream.i_handlers.i_yapClasses[GetID() - 1];
		}

		public override object IndexEntryToObject(Db4objects.Db4o.Transaction trans, object
			 indexEntry)
		{
			try
			{
				return Db4objects.Db4o.Inside.Marshall.StringMarshaller.ReadShort(_stream, (Db4objects.Db4o.YapReader
					)indexEntry);
			}
			catch (Db4objects.Db4o.CorruptionException e)
			{
			}
			return null;
		}

		public override bool IndexNullHandling()
		{
			return true;
		}

		public override int IsSecondClass()
		{
			return Db4objects.Db4o.YapConst.YES;
		}

		public override void CalculateLengths(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 header, bool topLevel, object obj, bool withIndirection)
		{
			Db4objects.Db4o.Inside.Marshall.MarshallerFamily.Current()._string.CalculateLengths
				(trans, header, topLevel, obj, withIndirection);
		}

		public override object Read(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, 
			Db4objects.Db4o.YapWriter a_bytes, bool redirect)
		{
			return mf._string.ReadFromParentSlot(a_bytes.GetStream(), a_bytes, redirect);
		}

		public override Db4objects.Db4o.ITypeHandler4 ReadArrayHandler(Db4objects.Db4o.Transaction
			 a_trans, Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.YapReader[]
			 a_bytes)
		{
			return null;
		}

		public override void ReadCandidates(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapReader a_bytes, Db4objects.Db4o.QCandidates a_candidates
			)
		{
		}

		public override Db4objects.Db4o.QCandidate ReadSubCandidate(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapReader reader, Db4objects.Db4o.QCandidates candidates, bool
			 withIndirection)
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
					return new Db4objects.Db4o.QCandidate(candidates, obj, 0, true);
				}
			}
			catch (Db4objects.Db4o.CorruptionException e)
			{
			}
			return null;
		}

		/// <summary>This readIndexEntry method reads from the parent slot.</summary>
		/// <remarks>
		/// This readIndexEntry method reads from the parent slot.
		/// TODO: Consider renaming methods in Indexable4 and Typhandler4 to make direction clear.
		/// </remarks>
		public override object ReadIndexEntry(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapWriter a_writer)
		{
			return mf._string.ReadIndexEntry(a_writer);
		}

		/// <summary>This readIndexEntry method reads from the actual index in the file.</summary>
		/// <remarks>
		/// This readIndexEntry method reads from the actual index in the file.
		/// TODO: Consider renaming methods in Indexable4 and Typhandler4 to make direction clear.
		/// </remarks>
		public override object ReadIndexEntry(Db4objects.Db4o.YapReader reader)
		{
			Db4objects.Db4o.Inside.Slots.Slot s = new Db4objects.Db4o.Inside.Slots.Slot(reader
				.ReadInt(), reader.ReadInt());
			if (IsInvalidSlot(s))
			{
				return null;
			}
			return s;
		}

		private bool IsInvalidSlot(Db4objects.Db4o.Inside.Slots.Slot slot)
		{
			return (slot._address == 0) && (slot._length == 0);
		}

		public override object ReadQuery(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, bool withRedirection, Db4objects.Db4o.YapReader a_reader, bool a_toArray)
		{
			if (!withRedirection)
			{
				return mf._string.Read(a_trans.Stream(), a_reader);
			}
			Db4objects.Db4o.YapReader reader = mf._string.ReadSlotFromParentSlot(a_trans.Stream
				(), a_reader);
			if (a_toArray)
			{
				if (reader != null)
				{
					return mf._string.ReadFromOwnSlot(a_trans.Stream(), reader);
				}
			}
			return reader;
		}

		internal void SetStringIo(Db4objects.Db4o.YapStringIO a_io)
		{
			i_stringIo = a_io;
		}

		public override bool SupportsIndex()
		{
			return true;
		}

		public override void WriteIndexEntry(Db4objects.Db4o.YapReader writer, object entry
			)
		{
			if (entry == null)
			{
				writer.WriteInt(0);
				writer.WriteInt(0);
				return;
			}
			if (entry is Db4objects.Db4o.YapWriter)
			{
				Db4objects.Db4o.YapWriter entryAsWriter = (Db4objects.Db4o.YapWriter)entry;
				writer.WriteInt(entryAsWriter.GetAddress());
				writer.WriteInt(entryAsWriter.GetLength());
				return;
			}
			if (entry is Db4objects.Db4o.Inside.Slots.Slot)
			{
				Db4objects.Db4o.Inside.Slots.Slot s = (Db4objects.Db4o.Inside.Slots.Slot)entry;
				writer.WriteInt(s._address);
				writer.WriteInt(s._length);
				return;
			}
			throw new System.ArgumentException();
		}

		public override object WriteNew(Db4objects.Db4o.Inside.Marshall.MarshallerFamily 
			mf, object a_object, bool topLevel, Db4objects.Db4o.YapWriter a_bytes, bool withIndirection
			, bool restoreLinkeOffset)
		{
			return mf._string.WriteNew(a_object, topLevel, a_bytes, withIndirection);
		}

		internal void WriteShort(string a_string, Db4objects.Db4o.YapReader a_bytes)
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
			return Db4objects.Db4o.YapConst.TYPE_SIMPLE;
		}

		private Db4objects.Db4o.YapReader i_compareTo;

		private Db4objects.Db4o.YapReader Val(object obj)
		{
			if (obj is Db4objects.Db4o.YapReader)
			{
				return (Db4objects.Db4o.YapReader)obj;
			}
			if (obj is string)
			{
				return Db4objects.Db4o.Inside.Marshall.StringMarshaller.WriteShort(_stream, (string
					)obj);
			}
			if (obj is Db4objects.Db4o.Inside.Slots.Slot)
			{
				Db4objects.Db4o.Inside.Slots.Slot s = (Db4objects.Db4o.Inside.Slots.Slot)obj;
				return _stream.ReadReaderByAddress(s._address, s._length);
			}
			return null;
		}

		public override void PrepareComparison(Db4objects.Db4o.Transaction a_trans, object
			 obj)
		{
			i_compareTo = (Db4objects.Db4o.YapReader)obj;
		}

		public override Db4objects.Db4o.IYapComparable PrepareComparison(object obj)
		{
			if (obj == null)
			{
				i_compareTo = null;
				return Db4objects.Db4o.Null.INSTANCE;
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
		internal int Compare(Db4objects.Db4o.YapReader a_compare, Db4objects.Db4o.YapReader
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

		internal static int Compare(byte[] compare, byte[] with)
		{
			int min = compare.Length < with.Length ? compare.Length : with.Length;
			int start = Db4objects.Db4o.YapConst.INT_LENGTH;
			for (int i = start; i < min; i++)
			{
				if (compare[i] != with[i])
				{
					return with[i] - compare[i];
				}
			}
			return with.Length - compare.Length;
		}

		public override void DefragIndexEntry(Db4objects.Db4o.ReaderPair readers)
		{
			readers.CopyID(false, true);
			readers.IncrementIntSize();
		}

		public override void Defrag(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, 
			Db4objects.Db4o.ReaderPair readers, bool redirect)
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
