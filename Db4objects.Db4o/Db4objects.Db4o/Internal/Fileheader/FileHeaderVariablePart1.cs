/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Slots;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeaderVariablePart1
	{
		private const int Length = 1 + (Const4.IntLength * 4) + Const4.LongLength + Const4
			.AddedLength;

		protected readonly LocalObjectContainer _container;

		protected readonly SystemData _systemData;

		private int _id;

		public FileHeaderVariablePart1(LocalObjectContainer container, int id, SystemData
			 systemData)
		{
			// The variable part format is:
			// (int) converter version
			// (byte) freespace system used
			// (int)  freespace address
			// (int) identity ID
			// (long) versionGenerator
			// (int) uuid index ID
			_id = id;
			_container = container;
			_systemData = systemData;
		}

		public virtual byte GetIdentifier()
		{
			return Const4.Header;
		}

		public virtual int OwnLength()
		{
			return Length;
		}

		public virtual void ReadThis(ByteArrayBuffer buffer)
		{
			_systemData.ConverterVersion(buffer.ReadInt());
			_systemData.FreespaceSystem(buffer.ReadByte());
			_systemData.FreespaceAddress(buffer.ReadInt());
			_systemData.IdentityId(buffer.ReadInt());
			_systemData.LastTimeStampID(buffer.ReadLong());
			_systemData.UuidIndexId(buffer.ReadInt());
		}

		public virtual void WriteThis(ByteArrayBuffer buffer)
		{
			buffer.WriteInt(_systemData.ConverterVersion());
			buffer.WriteByte(_systemData.FreespaceSystem());
			buffer.WriteInt(_systemData.FreespaceAddress());
			Db4oDatabase identity = _systemData.Identity();
			buffer.WriteInt(identity == null ? 0 : identity.GetID(SystemTransaction()));
			buffer.WriteLong(_systemData.LastTimeStampID());
			buffer.WriteInt(_systemData.UuidIndexId());
		}

		private Transaction SystemTransaction()
		{
			return _container.SystemTransaction();
		}

		public virtual void ReadIdentity(LocalTransaction trans)
		{
			LocalObjectContainer file = trans.LocalContainer();
			Db4oDatabase identity = Debug4.staticIdentity ? Db4oDatabase.StaticIdentity : (Db4oDatabase
				)file.GetByID(trans, _systemData.IdentityId());
			if (null != identity)
			{
				// TODO: what?
				file.Activate(trans, identity, new FixedActivationDepth(2));
				_systemData.Identity(identity);
			}
		}

		public virtual IRunnable Commit()
		{
			int length = _container.BlockConverter().BlockAlignedBytes(OwnLength());
			if (_id == 0)
			{
				_id = _container.AllocatePointerSlot();
			}
			Slot committedSlot = _container.ReadPointerSlot(_id);
			Slot newSlot = AllocateSlot(length);
			ByteArrayBuffer buffer = new ByteArrayBuffer(length);
			WriteThis(buffer);
			_container.WriteEncrypt(buffer, newSlot.Address(), 0);
			return new _IRunnable_100(this, newSlot, committedSlot);
		}

		private sealed class _IRunnable_100 : IRunnable
		{
			public _IRunnable_100(FileHeaderVariablePart1 _enclosing, Slot newSlot, Slot committedSlot
				)
			{
				this._enclosing = _enclosing;
				this.newSlot = newSlot;
				this.committedSlot = committedSlot;
			}

			public void Run()
			{
				// FIXME: This is not transactional !!!
				this._enclosing._container.WritePointer(this._enclosing._id, newSlot);
				if (committedSlot == null || committedSlot.IsNull())
				{
					return;
				}
				this._enclosing._container.FreespaceManager().FreeTransactionLogSlot(committedSlot
					);
			}

			private readonly FileHeaderVariablePart1 _enclosing;

			private readonly Slot newSlot;

			private readonly Slot committedSlot;
		}

		private Slot AllocateSlot(int length)
		{
			Slot reusedSlot = _container.FreespaceManager().AllocateTransactionLogSlot(length
				);
			if (reusedSlot != null)
			{
				return reusedSlot;
			}
			return _container.AppendBytes(length);
		}

		public virtual int Id()
		{
			return _id;
		}

		public virtual void Read()
		{
			Slot slot = _container.ReadPointerSlot(_id);
			ByteArrayBuffer buffer = _container.ReadBufferBySlot(slot);
			ReadThis(buffer);
		}
	}
}
