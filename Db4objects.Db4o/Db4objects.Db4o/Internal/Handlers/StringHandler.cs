/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class StringHandler : VariableLengthTypeHandler, IIndexableTypeHandler, IBuiltinTypeHandler
	{
		public StringHandler(ObjectContainerBase container) : base(container)
		{
		}

		protected StringHandler(ITypeHandler4 template) : this(((Db4objects.Db4o.Internal.Handlers.StringHandler
			)template).Container())
		{
		}

		public virtual IReflectClass ClassReflector()
		{
			return Container()._handlers.ICLASS_STRING;
		}

		public override void Delete(IDeleteContext context)
		{
			context.ReadSlot();
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
			return ReadStringNoDebug(trans.Context(), (IReadBuffer)indexEntry);
		}

		/// <summary>This readIndexEntry method reads from the parent slot.</summary>
		/// <remarks>
		/// This readIndexEntry method reads from the parent slot.
		/// TODO: Consider renaming methods in Indexable4 and Typhandler4 to make direction clear.
		/// </remarks>
		/// <exception cref="CorruptionException">CorruptionException</exception>
		/// <exception cref="Db4oIOException"></exception>
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
		public virtual object ReadIndexEntry(BufferImpl reader)
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

		public virtual void WriteIndexEntry(BufferImpl writer, object entry)
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

		public void WriteShort(Transaction trans, string str, BufferImpl buffer)
		{
			if (str == null)
			{
				buffer.WriteInt(0);
			}
			else
			{
				buffer.WriteInt(str.Length);
				trans.Container().Handlers().StringIO().Write(buffer, str);
			}
		}

		private BufferImpl Val(object obj)
		{
			return Val(obj, Container());
		}

		public virtual BufferImpl Val(object obj, ObjectContainerBase oc)
		{
			if (obj is BufferImpl)
			{
				return (BufferImpl)obj;
			}
			if (obj is string)
			{
				return WriteToBuffer((IInternalObjectContainer)oc, (string)obj);
			}
			if (obj is Slot)
			{
				Slot s = (Slot)obj;
				return oc.BufferByAddress(s.Address(), s.Length());
			}
			return null;
		}

		/// <summary>
		/// returns: -x for left is greater and +x for right is greater
		/// FIXME: The returned value is the wrong way around.
		/// </summary>
		/// <remarks>
		/// returns: -x for left is greater and +x for right is greater
		/// FIXME: The returned value is the wrong way around.
		/// TODO: You will need collators here for different languages.
		/// </remarks>
		internal int Compare(BufferImpl a_compare, BufferImpl a_with)
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

		public virtual void DefragIndexEntry(DefragmentContextImpl context)
		{
			context.CopyID(false, true);
			context.IncrementIntSize();
		}

		public override void Write(IWriteContext context, object obj)
		{
			InternalWrite((IInternalObjectContainer)context.ObjectContainer(), context, (string
				)obj);
		}

		protected static void InternalWrite(IInternalObjectContainer objectContainer, IWriteBuffer
			 buffer, string str)
		{
			buffer.WriteInt(str.Length);
			StringIo(objectContainer).Write(buffer, str);
		}

		public static BufferImpl WriteToBuffer(IInternalObjectContainer container, string
			 str)
		{
			BufferImpl buffer = new BufferImpl(StringIo(container).Length(str));
			InternalWrite(container, buffer, str);
			return buffer;
		}

		protected static LatinStringIO StringIo(IContext context)
		{
			return StringIo((IInternalObjectContainer)context.ObjectContainer());
		}

		protected static LatinStringIO StringIo(IInternalObjectContainer objectContainer)
		{
			return objectContainer.Container().StringIO();
		}

		public static string ReadString(IContext context, IReadBuffer buffer)
		{
			string str = ReadStringNoDebug(context, buffer);
			return str;
		}

		public static string ReadStringNoDebug(IContext context, IReadBuffer buffer)
		{
			int length = buffer.ReadInt();
			if (length > 0)
			{
				return Intern(context, StringIo(context).Read(buffer, length));
			}
			return string.Empty;
		}

		protected static string Intern(IContext context, string str)
		{
			if (context.ObjectContainer().Ext().Configure().InternStrings())
			{
				return string.Intern(str);
			}
			return str;
		}

		public override object Read(IReadContext context)
		{
			return ReadString(context, context);
		}

		public override void Defragment(IDefragmentContext context)
		{
			context.IncrementOffset(LinkLength());
		}

		public override IPreparedComparison NewPrepareCompare(object obj)
		{
			BufferImpl sourceBuffer = Val(obj);
			return new _IPreparedComparison_231(this, sourceBuffer);
		}

		private sealed class _IPreparedComparison_231 : IPreparedComparison
		{
			public _IPreparedComparison_231(StringHandler _enclosing, BufferImpl sourceBuffer
				)
			{
				this._enclosing = _enclosing;
				this.sourceBuffer = sourceBuffer;
			}

			public int CompareTo(object target)
			{
				BufferImpl targetBuffer = this._enclosing.Val(target);
				return -this._enclosing.Compare(sourceBuffer, targetBuffer);
			}

			private readonly StringHandler _enclosing;

			private readonly BufferImpl sourceBuffer;
		}
	}
}
