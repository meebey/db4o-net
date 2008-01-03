/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
			ByteArrayOutputStream os = new ByteArrayOutputStream();
			EncodeObjectInfoCollection(os, callbackInfo.added);
			EncodeObjectInfoCollection(os, callbackInfo.deleted);
			EncodeObjectInfoCollection(os, callbackInfo.updated);
			byte[] bytes = os.ToByteArray();
			MCommittedInfo committedInfo = (MCommittedInfo)GetWriterForLength(Transaction(), 
				bytes.Length);
			committedInfo._payLoad.Append(bytes);
			return committedInfo;
		}

		private void EncodeObjectInfoCollection(ByteArrayOutputStream os, IObjectInfoCollection
			 collection)
		{
			IEnumerator iter = collection.GetEnumerator();
			while (iter.MoveNext())
			{
				LazyObjectReference obj = (LazyObjectReference)iter.Current;
				WriteLong(os, obj.GetInternalID());
			}
			WriteLong(os, -1);
		}

		public virtual CallbackObjectInfoCollections Decode()
		{
			CallbackObjectInfoCollections callbackInfo = CallbackObjectInfoCollections.Emtpy;
			ByteArrayInputStream @is = new ByteArrayInputStream(_payLoad._buffer);
			callbackInfo.added = DecodeObjectInfoCollection(@is);
			callbackInfo.deleted = DecodeObjectInfoCollection(@is);
			callbackInfo.updated = DecodeObjectInfoCollection(@is);
			return callbackInfo;
		}

		private IObjectInfoCollection DecodeObjectInfoCollection(ByteArrayInputStream @is
			)
		{
			Collection4 collection = new Collection4();
			while (true)
			{
				long id = ReadLong(@is);
				if (id == -1)
				{
					break;
				}
				collection.Add(new LazyObjectReference(Transaction(), (int)id));
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
			new Thread(new _IRunnable_76(this, callbackInfos)).Start();
			return true;
		}

		private sealed class _IRunnable_76 : IRunnable
		{
			public _IRunnable_76(MCommittedInfo _enclosing, CallbackObjectInfoCollections callbackInfos
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
	}
}
