/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Inside
{
	internal class ObjectStateImpl : Db4objects.Drs.IObjectState
	{
		private object _object;

		private bool _isNew;

		private bool _wasModified;

		private long _modificationDate;

		public virtual object GetObject()
		{
			return _object;
		}

		public virtual bool IsNew()
		{
			return _isNew;
		}

		public virtual bool WasModified()
		{
			return _wasModified;
		}

		public virtual long ModificationDate()
		{
			return _modificationDate;
		}

		internal virtual void SetAll(object obj, bool isNew, bool wasModified, long modificationDate
			)
		{
			_object = obj;
			_isNew = isNew;
			_wasModified = wasModified;
			_modificationDate = modificationDate;
		}

		public override string ToString()
		{
			return "ObjectStateImpl{" + "_object=" + _object + ", _isNew=" + _isNew + ", _wasModified="
				 + _wasModified + ", _modificationDate=" + _modificationDate + '}';
		}
	}
}
