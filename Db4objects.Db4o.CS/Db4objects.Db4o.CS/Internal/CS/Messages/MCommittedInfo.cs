/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.IO;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MCommittedInfo : MsgD, IClientSideMessage
	{
		public virtual MCommittedInfo Encode(CallbackObjectInfoCollections callbackInfo)
		{
			byte[] bytes = EncodeInfo(callbackInfo);
			MCommittedInfo committedInfo = (MCommittedInfo)GetWriterForLength(Transaction(), 
				bytes.Length);
			committedInfo._payLoad.Append(bytes);
			return committedInfo;
		}

		private byte[] EncodeInfo(CallbackObjectInfoCollections callbackInfo)
		{
			ByteArrayOutputStream os = new ByteArrayOutputStream();
			EncodeObjectInfoCollection(os, callbackInfo.added, new MCommittedInfo.InternalIDEncoder
				(this));
			EncodeObjectInfoCollection(os, callbackInfo.deleted, new MCommittedInfo.FrozenObjectInfoEncoder
				(this));
			EncodeObjectInfoCollection(os, callbackInfo.updated, new MCommittedInfo.InternalIDEncoder
				(this));
			return os.ToByteArray();
		}

		private sealed class FrozenObjectInfoEncoder : MCommittedInfo.IObjectInfoEncoder
		{
			public void Encode(ByteArrayOutputStream os, IObjectInfo info)
			{
				this._enclosing.WriteLong(os, info.GetInternalID());
				long sourceDatabaseId = ((FrozenObjectInfo)info).SourceDatabaseId(this._enclosing
					.Transaction());
				this._enclosing.WriteLong(os, sourceDatabaseId);
				this._enclosing.WriteLong(os, ((FrozenObjectInfo)info).UuidLongPart());
				this._enclosing.WriteLong(os, info.GetVersion());
			}

			public IObjectInfo Decode(ByteArrayInputStream @is)
			{
				long id = this._enclosing.ReadLong(@is);
				if (id == -1)
				{
					return null;
				}
				long sourceDatabaseId = this._enclosing.ReadLong(@is);
				Db4oDatabase sourceDatabase = null;
				if (sourceDatabaseId > 0)
				{
					sourceDatabase = (Db4oDatabase)this._enclosing.Stream().GetByID(this._enclosing.Transaction
						(), sourceDatabaseId);
				}
				long uuidLongPart = this._enclosing.ReadLong(@is);
				long version = this._enclosing.ReadLong(@is);
				return new FrozenObjectInfo(null, id, sourceDatabase, uuidLongPart, version);
			}

			internal FrozenObjectInfoEncoder(MCommittedInfo _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly MCommittedInfo _enclosing;
		}

		private sealed class InternalIDEncoder : MCommittedInfo.IObjectInfoEncoder
		{
			public void Encode(ByteArrayOutputStream os, IObjectInfo info)
			{
				this._enclosing.WriteLong(os, info.GetInternalID());
			}

			public IObjectInfo Decode(ByteArrayInputStream @is)
			{
				long id = this._enclosing.ReadLong(@is);
				if (id == -1)
				{
					return null;
				}
				return new LazyObjectReference(this._enclosing.Transaction(), (int)id);
			}

			internal InternalIDEncoder(MCommittedInfo _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly MCommittedInfo _enclosing;
		}

		internal interface IObjectInfoEncoder
		{
			void Encode(ByteArrayOutputStream os, IObjectInfo info);

			IObjectInfo Decode(ByteArrayInputStream @is);
		}

		private void EncodeObjectInfoCollection(ByteArrayOutputStream os, IObjectInfoCollection
			 collection, MCommittedInfo.IObjectInfoEncoder encoder)
		{
			IEnumerator iter = collection.GetEnumerator();
			while (iter.MoveNext())
			{
				IObjectInfo obj = (IObjectInfo)iter.Current;
				encoder.Encode(os, obj);
			}
			WriteLong(os, -1);
		}

		public virtual CallbackObjectInfoCollections Decode()
		{
			ByteArrayInputStream @is = new ByteArrayInputStream(_payLoad._buffer);
			IObjectInfoCollection added = DecodeObjectInfoCollection(@is, new MCommittedInfo.InternalIDEncoder
				(this));
			IObjectInfoCollection deleted = DecodeObjectInfoCollection(@is, new MCommittedInfo.FrozenObjectInfoEncoder
				(this));
			IObjectInfoCollection updated = DecodeObjectInfoCollection(@is, new MCommittedInfo.InternalIDEncoder
				(this));
			return new CallbackObjectInfoCollections(added, updated, deleted);
		}

		private IObjectInfoCollection DecodeObjectInfoCollection(ByteArrayInputStream @is
			, MCommittedInfo.IObjectInfoEncoder encoder)
		{
			Collection4 collection = new Collection4();
			while (true)
			{
				IObjectInfo info = encoder.Decode(@is);
				if (null == info)
				{
					break;
				}
				collection.Add(info);
			}
			return new ObjectInfoCollectionImpl(collection);
		}

		private void WriteLong(ByteArrayOutputStream os, long l)
		{
			for (int i = 0; i < 64; i += 8)
			{
				os.Write((int)(l >> i));
			}
		}

		private long ReadLong(ByteArrayInputStream @is)
		{
			long l = 0;
			for (int i = 0; i < 64; i += 8)
			{
				l += ((long)(@is.Read())) << i;
			}
			return l;
		}

		public virtual bool ProcessAtClient()
		{
			CallbackObjectInfoCollections callbackInfos = Decode();
			new Thread(new _IRunnable_125(this, callbackInfos)).Start();
			return true;
		}

		private sealed class _IRunnable_125 : IRunnable
		{
			public _IRunnable_125(MCommittedInfo _enclosing, CallbackObjectInfoCollections callbackInfos
				)
			{
				this._enclosing = _enclosing;
				this.callbackInfos = callbackInfos;
			}

			public void Run()
			{
				if (this._enclosing.Stream().IsClosed())
				{
					return;
				}
				this._enclosing.Stream().Callbacks().CommitOnCompleted(this._enclosing.Transaction
					(), callbackInfos);
			}

			private readonly MCommittedInfo _enclosing;

			private readonly CallbackObjectInfoCollections callbackInfos;
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected virtual void WriteByteArray(ByteArrayOutputStream os, byte[] signaturePart
			)
		{
			WriteLong(os, signaturePart.Length);
			os.Write(signaturePart);
		}
	}
}
