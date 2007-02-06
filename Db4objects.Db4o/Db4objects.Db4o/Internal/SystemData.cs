namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class SystemData
	{
		private int _classCollectionID;

		private int _converterVersion;

		private int _freespaceAddress;

		private int _freespaceID;

		private byte _freespaceSystem;

		private Db4objects.Db4o.Ext.Db4oDatabase _identity;

		private long _lastTimeStampID;

		private byte _stringEncoding;

		private int _uuidIndexId;

		public virtual int ClassCollectionID()
		{
			return _classCollectionID;
		}

		public virtual void ClassCollectionID(int id)
		{
			_classCollectionID = id;
		}

		public virtual int ConverterVersion()
		{
			return _converterVersion;
		}

		public virtual void ConverterVersion(int version)
		{
			_converterVersion = version;
		}

		public virtual int FreespaceAddress()
		{
			return _freespaceAddress;
		}

		public virtual void FreespaceAddress(int address)
		{
			_freespaceAddress = address;
		}

		public virtual int FreespaceID()
		{
			return _freespaceID;
		}

		public virtual void FreespaceID(int id)
		{
			_freespaceID = id;
		}

		public virtual byte FreespaceSystem()
		{
			return _freespaceSystem;
		}

		public virtual void FreespaceSystem(byte freespaceSystemtype)
		{
			_freespaceSystem = freespaceSystemtype;
		}

		public virtual Db4objects.Db4o.Ext.Db4oDatabase Identity()
		{
			return _identity;
		}

		public virtual void Identity(Db4objects.Db4o.Ext.Db4oDatabase identityObject)
		{
			_identity = identityObject;
		}

		public virtual long LastTimeStampID()
		{
			return _lastTimeStampID;
		}

		public virtual void LastTimeStampID(long id)
		{
			_lastTimeStampID = id;
		}

		public virtual byte StringEncoding()
		{
			return _stringEncoding;
		}

		public virtual void StringEncoding(byte encodingByte)
		{
			_stringEncoding = encodingByte;
		}

		public virtual int UuidIndexId()
		{
			return _uuidIndexId;
		}

		public virtual void UuidIndexId(int id)
		{
			_uuidIndexId = id;
		}
	}
}
