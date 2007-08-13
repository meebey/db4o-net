/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	public abstract class StringMarshaller
	{
		public abstract bool InlinedStrings();

		public abstract void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, bool topLevel, object obj, bool withIndirection);

		protected int LinkLength()
		{
			return Const4.INT_LENGTH + Const4.ID_LENGTH;
		}

		public abstract object WriteNew(object a_object, bool topLevel, StatefulBuffer a_bytes
			, bool redirect);

		public string Read(ObjectContainerBase stream, Db4objects.Db4o.Internal.Buffer reader
			)
		{
			if (reader == null)
			{
				return null;
			}
			string ret = ReadShort(stream, reader);
			return ret;
		}

		public virtual string ReadFromOwnSlot(ObjectContainerBase stream, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			try
			{
				return Read(stream, reader);
			}
			catch (Exception e)
			{
				if (Deploy.debug || Debug.atHome)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
			return string.Empty;
		}

		public virtual string ReadFromParentSlot(ObjectContainerBase stream, Db4objects.Db4o.Internal.Buffer
			 reader, bool redirect)
		{
			if (redirect)
			{
				return Read(stream, ReadSlotFromParentSlot(stream, reader));
			}
			return Read(stream, reader);
		}

		public abstract Db4objects.Db4o.Internal.Buffer ReadIndexEntry(StatefulBuffer parentSlot
			);

		public static string ReadShort(ObjectContainerBase stream, Db4objects.Db4o.Internal.Buffer
			 bytes)
		{
			return ReadShort(stream.StringIO(), stream.ConfigImpl().InternStrings(), bytes);
		}

		public static string ReadShort(LatinStringIO io, bool internStrings, Db4objects.Db4o.Internal.Buffer
			 bytes)
		{
			int length = bytes.ReadInt();
			if (length > Const4.MAXIMUM_BLOCK_SIZE)
			{
				throw new CorruptionException();
			}
			if (length > 0)
			{
				string str = io.Read(bytes, length);
				return str;
			}
			return string.Empty;
		}

		public abstract Db4objects.Db4o.Internal.Buffer ReadSlotFromParentSlot(ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.Buffer reader);

		public static Db4objects.Db4o.Internal.Buffer WriteShort(ObjectContainerBase stream
			, string str)
		{
			Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(stream
				.StringIO().Length(str));
			WriteShort(stream, str, reader);
			return reader;
		}

		public static void WriteShort(ObjectContainerBase stream, string str, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			int length = str.Length;
			reader.WriteInt(length);
			stream.StringIO().Write(reader, str);
		}

		public abstract void Defrag(ISlotBuffer reader);
	}
}
