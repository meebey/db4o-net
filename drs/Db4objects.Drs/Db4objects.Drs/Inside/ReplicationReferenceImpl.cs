/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Drs.Inside;

namespace Db4objects.Drs.Inside
{
	public sealed class ReplicationReferenceImpl : IReplicationReference
	{
		private bool _objectIsNew;

		private readonly object _obj;

		private readonly Db4oUUID _uuid;

		private readonly long _version;

		private object _counterPart;

		private bool _markedForReplicating;

		private bool _markedForDeleting;

		public ReplicationReferenceImpl(object obj, Db4oUUID uuid, long version)
		{
			this._obj = obj;
			this._uuid = uuid;
			this._version = version;
		}

		public object Counterpart()
		{
			return _counterPart;
		}

		public sealed override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || o.GetType().BaseType != o.GetType().BaseType)
			{
				return false;
			}
			IReplicationReference that = (Db4objects.Drs.Inside.ReplicationReferenceImpl)o;
			if (_version != that.Version())
			{
				return false;
			}
			return _uuid.Equals(that.Uuid());
		}

		public sealed override int GetHashCode()
		{
			int result;
			result = _uuid.GetHashCode();
			result = 29 * result + (int)(_version ^ ((_version) >> (32 & 0x1f)));
			return result;
		}

		public bool IsCounterpartNew()
		{
			return _objectIsNew;
		}

		public bool IsMarkedForDeleting()
		{
			return _markedForDeleting;
		}

		public bool IsMarkedForReplicating()
		{
			return _markedForReplicating;
		}

		public void MarkCounterpartAsNew()
		{
			_objectIsNew = true;
		}

		public void MarkForDeleting()
		{
			_markedForDeleting = true;
		}

		public void MarkForReplicating()
		{
			_markedForReplicating = true;
		}

		public object Object()
		{
			return _obj;
		}

		public void SetCounterpart(object obj)
		{
			_counterPart = obj;
		}

		public override string ToString()
		{
			return "ReplicationReferenceImpl{" + "_objectIsNew=" + _objectIsNew + ", _obj=" +
				 _obj + ", _uuid=" + _uuid + ", _version=" + _version + ", _counterPart=" + _counterPart
				 + ", _markedForReplicating=" + _markedForReplicating + ", _markedForDeleting=" 
				+ _markedForDeleting + '}';
		}

		public Db4oUUID Uuid()
		{
			return _uuid;
		}

		public long Version()
		{
			return _version;
		}
	}
}
