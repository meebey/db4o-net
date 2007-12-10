/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public abstract class MockMarshallingContext
	{
		private readonly IObjectContainer _objectContainer;

		internal readonly Db4objects.Db4o.Internal.Buffer _header;

		internal readonly Db4objects.Db4o.Internal.Buffer _payLoad;

		protected Db4objects.Db4o.Internal.Buffer _current;

		public MockMarshallingContext(IObjectContainer objectContainer)
		{
			_objectContainer = objectContainer;
			_header = new Db4objects.Db4o.Internal.Buffer(1000);
			_payLoad = new Db4objects.Db4o.Internal.Buffer(1000);
			_current = _header;
		}

		public virtual IWriteBuffer NewBuffer(int length)
		{
			return new Db4objects.Db4o.Internal.Buffer(length);
		}

		public virtual IObjectContainer ObjectContainer()
		{
			return _objectContainer;
		}

		public virtual void UseVariableLength()
		{
			_current = _payLoad;
		}

		public virtual byte ReadByte()
		{
			return _current.ReadByte();
		}

		public virtual void ReadBytes(byte[] bytes)
		{
			_current.ReadBytes(bytes);
		}

		public virtual int ReadInt()
		{
			return _current.ReadInt();
		}

		public virtual long ReadLong()
		{
			return _current.ReadLong();
		}

		public virtual void WriteByte(byte b)
		{
			_current.WriteByte(b);
		}

		public virtual void WriteInt(int i)
		{
			_current.WriteInt(i);
		}

		public virtual void WriteLong(long l)
		{
			_current.WriteLong(l);
		}

		public virtual void WriteBytes(byte[] bytes)
		{
			_current.WriteBytes(bytes);
		}

		public virtual object ReadObject()
		{
			int id = ReadInt();
			object obj = Container().GetByID(Transaction(), id);
			ObjectContainer().Activate(obj, int.MaxValue);
			return obj;
		}

		public virtual void WriteObject(object obj)
		{
			int id = Container().SetInternal(Transaction(), obj, false);
			WriteInt(id);
		}

		public virtual Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return Container().Transaction();
		}

		protected virtual ObjectContainerBase Container()
		{
			return ((IInternalObjectContainer)_objectContainer).Container();
		}

		public virtual int Offset()
		{
			return _current.Offset();
		}

		public virtual void Seek(int offset)
		{
			_current.Seek(offset);
		}
	}
}
