/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Db4o
{
	internal class Db4oSignatureMap
	{
		private readonly Db4objects.Db4o.Internal.ObjectContainerBase _stream;

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _identities;

		internal Db4oSignatureMap(Db4objects.Db4o.Internal.ObjectContainerBase stream)
		{
			_stream = stream;
			_identities = new Db4objects.Db4o.Foundation.Hashtable4();
		}

		internal virtual Db4objects.Db4o.Ext.Db4oDatabase Produce(byte[] signature, long 
			creationTime)
		{
			Db4objects.Db4o.Ext.Db4oDatabase db = (Db4objects.Db4o.Ext.Db4oDatabase)_identities
				.Get(signature);
			if (db != null)
			{
				return db;
			}
			db = new Db4objects.Db4o.Ext.Db4oDatabase(signature, creationTime);
			db.Bind(_stream.Transaction());
			_identities.Put(signature, db);
			return db;
		}

		public virtual void Put(Db4objects.Db4o.Ext.Db4oDatabase db)
		{
			Db4objects.Db4o.Ext.Db4oDatabase existing = (Db4objects.Db4o.Ext.Db4oDatabase)_identities
				.Get(db.GetSignature());
			if (existing == null)
			{
				_identities.Put(db.GetSignature(), db);
			}
		}
	}
}
