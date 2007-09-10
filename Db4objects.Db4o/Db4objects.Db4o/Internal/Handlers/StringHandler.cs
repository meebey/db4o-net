/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public abstract class StringHandler : BuiltinTypeHandler, IIndexableTypeHandler
	{
		private LatinStringIO _stringIO;

		public StringHandler(ObjectContainerBase container, LatinStringIO stringIO) : base
			(container)
		{
			_stringIO = stringIO;
		}

		protected StringHandler(ITypeHandler4 template) : this(((Db4objects.Db4o.Internal.Handlers.StringHandler
			)template).Container(), ((Db4objects.Db4o.Internal.Handlers.StringHandler)template
			).StringIO())
		{
		}

		public override void CascadeActivation(Transaction a_trans, object a_object, int 
			a_depth, bool a_activate)
		{
		}

		public override IReflectClass ClassReflector()
		{
			return Container()._handlers.ICLASS_STRING;
		}

		public override void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer buffer)
		{
			Slot slot = buffer.ReadSlot();
			if (slot.Address() > 0 && !mf._string.InlinedStrings())
			{
				buffer.GetTransaction().SlotFreeOnCommit(slot.Address(), slot);
			}
		}

		public override int GetID()
		{
			return 9;
		}

		internal virtual byte GetIdentifier()
		{
			return Const4.YAPSTRING;
		}

		public virtual object IndexEntryToObject(Transaction trans, object indexEntry)
		{
			if (indexEntry is Slot)
			{
				Slot slot = (Slot)indexEntry;
				indexEntry = Container().BufferByAddress(slot.Address(), slot.Length());
			}
			try
			{
				return StringMarshaller.ReadShort(Container(), (Db4objects.Db4o.Internal.Buffer)indexEntry
					);
			}
			catch (CorruptionException)
			{
			}
			return null;
		}

		public override object Read(MarshallerFamily mf, StatefulBuffer a_bytes, bool redirect
			)
		{
			return mf._string.ReadFromParentSlot(a_bytes.GetStream(), a_bytes, redirect);
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
					obj = mf._string.Read(Container(), reader);
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
		public virtual object ReadIndexEntry(MarshallerFamily mf, StatefulBuffer a_writer
			)
		{
			return mf._string.ReadIndexEntry(a_writer);
		}

		/// <summary>This readIndexEntry method reads from the actual index in the file.</summary>
		/// <remarks>
		/// This readIndexEntry method reads from the actual index in the file.
		/// TODO: Consider renaming methods in Indexable4 and Typhandler4 to make direction clear.
		/// </remarks>
		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer reader)
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
				return mf._string.Read(a_trans.Container(), a_reader);
			}
			Db4objects.Db4o.Internal.Buffer reader = mf._string.ReadSlotFromParentSlot(a_trans
				.Container(), a_reader);
			if (a_toArray)
			{
				return mf._string.ReadFromOwnSlot(a_trans.Container(), reader);
			}
			return reader;
		}

		public virtual void SetStringIo(LatinStringIO a_io)
		{
			_stringIO = a_io;
		}

		public virtual void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer writer, object
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
				writer.WriteInt(entryAsWriter.Length());
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

		public void WriteShort(string a_string, Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			if (a_string == null)
			{
				a_bytes.WriteInt(0);
			}
			else
			{
				a_bytes.WriteInt(a_string.Length);
				_stringIO.Write(a_bytes, a_string);
			}
		}

		private Db4objects.Db4o.Internal.Buffer i_compareTo;

		private Db4objects.Db4o.Internal.Buffer Val(object obj)
		{
			return Val(obj, Container());
		}

		public virtual Db4objects.Db4o.Internal.Buffer Val(object obj, ObjectContainerBase
			 oc)
		{
			if (obj is Db4objects.Db4o.Internal.Buffer)
			{
				return (Db4objects.Db4o.Internal.Buffer)obj;
			}
			if (obj is string)
			{
				return StringMarshaller.WriteShort(Container(), (string)obj);
			}
			if (obj is Slot)
			{
				Slot s = (Slot)obj;
				return oc.BufferByAddress(s.Address(), s.Length());
			}
			return null;
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

		public virtual void DefragIndexEntry(BufferPair readers)
		{
			readers.CopyID(false, true);
			readers.IncrementIntSize();
		}

		public override void Defrag(MarshallerFamily mf, BufferPair readers, bool redirect
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

		public abstract override object Read(IReadContext context);

		public override void Write(IWriteContext context, object obj)
		{
			string str = (string)obj;
			context.WriteInt(str.Length);
			StringIo(context).Write(context, str);
		}

		protected virtual LatinStringIO StringIo(IContext context)
		{
			IInternalObjectContainer objectContainer = (IInternalObjectContainer)context.ObjectContainer
				();
			LatinStringIO stringIO = objectContainer.Container().StringIO();
			return stringIO;
		}

		public virtual LatinStringIO StringIO()
		{
			return _stringIO;
		}

		protected virtual string ReadString(IReadContext context, IReadBuffer buffer)
		{
			string str = InternalRead(context, buffer);
			return str;
		}

		private string InternalRead(IReadContext context, IReadBuffer buffer)
		{
			int length = buffer.ReadInt();
			if (length > 0)
			{
				return Intern(context, StringIO().Read(buffer, length));
			}
			return string.Empty;
		}

		protected virtual string Intern(IReadContext context, string str)
		{
			if (context.ObjectContainer().Ext().Configure().InternStrings())
			{
				return string.Intern(str);
			}
			return str;
		}
	}
}
