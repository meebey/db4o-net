namespace Db4objects.Db4o.Internal.Marshall
{
	public abstract class StringMarshaller
	{
		public abstract bool InlinedStrings();

		public abstract void CalculateLengths(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes header, bool topLevel, 
			object obj, bool withIndirection);

		protected int LinkLength()
		{
			return Db4objects.Db4o.Internal.Const4.INT_LENGTH + Db4objects.Db4o.Internal.Const4
				.ID_LENGTH;
		}

		public abstract object WriteNew(object a_object, bool topLevel, Db4objects.Db4o.Internal.StatefulBuffer
			 a_bytes, bool redirect);

		public string Read(Db4objects.Db4o.Internal.ObjectContainerBase stream, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			if (reader == null)
			{
				return null;
			}
			string ret = ReadShort(stream, reader);
			return ret;
		}

		public virtual string ReadFromOwnSlot(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.Buffer reader)
		{
			try
			{
				return Read(stream, reader);
			}
			catch (System.Exception e)
			{
				if (Db4objects.Db4o.Deploy.debug || Db4objects.Db4o.Debug.atHome)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
			return string.Empty;
		}

		public virtual string ReadFromParentSlot(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.Buffer reader, bool redirect)
		{
			if (!redirect)
			{
				return Read(stream, reader);
			}
			return Read(stream, ReadSlotFromParentSlot(stream, reader));
		}

		public abstract Db4objects.Db4o.Internal.Buffer ReadIndexEntry(Db4objects.Db4o.Internal.StatefulBuffer
			 parentSlot);

		public static string ReadShort(Db4objects.Db4o.Internal.ObjectContainerBase stream
			, Db4objects.Db4o.Internal.Buffer bytes)
		{
			return ReadShort(stream.StringIO(), stream.ConfigImpl().InternStrings(), bytes);
		}

		public static string ReadShort(Db4objects.Db4o.Internal.LatinStringIO io, bool internStrings
			, Db4objects.Db4o.Internal.Buffer bytes)
		{
			int length = bytes.ReadInt();
			if (length > Db4objects.Db4o.Internal.Const4.MAXIMUM_BLOCK_SIZE)
			{
				throw new Db4objects.Db4o.CorruptionException();
			}
			if (length > 0)
			{
				string str = io.Read(bytes, length);
				return str;
			}
			return string.Empty;
		}

		public abstract Db4objects.Db4o.Internal.Buffer ReadSlotFromParentSlot(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.Buffer reader);

		public static Db4objects.Db4o.Internal.Buffer WriteShort(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, string str)
		{
			Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(stream
				.StringIO().Length(str));
			WriteShort(stream, str, reader);
			return reader;
		}

		public static void WriteShort(Db4objects.Db4o.Internal.ObjectContainerBase stream
			, string str, Db4objects.Db4o.Internal.Buffer reader)
		{
			int length = str.Length;
			reader.WriteInt(length);
			stream.StringIO().Write(reader, str);
		}

		public abstract void Defrag(Db4objects.Db4o.Internal.ISlotReader reader);
	}
}
