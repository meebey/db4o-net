/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Filestats
{
	/// <exclude></exclude>
	public class ClassUsageStats
	{
		private readonly string _className;

		private readonly long _slotUsage;

		private readonly long _classIndexUsage;

		private readonly long _fieldIndexUsage;

		private readonly long _miscUsage;

		public ClassUsageStats(string className, long slotSpace, long classIndexUsage, long
			 fieldIndexUsage, long miscUsage)
		{
			_className = className;
			_slotUsage = slotSpace;
			_classIndexUsage = classIndexUsage;
			_fieldIndexUsage = fieldIndexUsage;
			_miscUsage = miscUsage;
		}

		public virtual string ClassName()
		{
			return _className;
		}

		public virtual long SlotUsage()
		{
			return _slotUsage;
		}

		public virtual long ClassIndexUsage()
		{
			return _classIndexUsage;
		}

		public virtual long FieldIndexUsage()
		{
			return _fieldIndexUsage;
		}

		public virtual long MiscUsage()
		{
			return _miscUsage;
		}

		public virtual long TotalUsage()
		{
			return _slotUsage + _classIndexUsage + _fieldIndexUsage + _miscUsage;
		}
	}
}
