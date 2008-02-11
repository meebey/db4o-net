/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Defragment
{
	/// <summary>Base class for defragment ID mappings.</summary>
	/// <remarks>Base class for defragment ID mappings.</remarks>
	/// <seealso cref="Db4objects.Db4o.Defragment.Defragment">Db4objects.Db4o.Defragment.Defragment
	/// 	</seealso>
	public abstract class AbstractContextIDMapping : IContextIDMapping
	{
		private Hashtable4 _classIDs = new Hashtable4();

		public void MapIDs(int origID, int mappedID, bool isClassID)
		{
			if (isClassID)
			{
				MapClassIDs(origID, mappedID);
				return;
			}
			MapNonClassIDs(origID, mappedID);
		}

		protected virtual int MappedClassID(int origID)
		{
			object obj = _classIDs.Get(origID);
			if (obj == null)
			{
				return 0;
			}
			return ((int)obj);
		}

		private void MapClassIDs(int oldID, int newID)
		{
			_classIDs.Put(oldID, newID);
		}

		protected abstract void MapNonClassIDs(int origID, int mappedID);

		public abstract void Close();

		public abstract int MappedID(int arg1, bool arg2);

		public abstract void Open();
	}
}
