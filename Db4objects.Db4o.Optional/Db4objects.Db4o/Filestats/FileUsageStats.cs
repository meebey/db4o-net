/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System.Text;
using Db4objects.Db4o.Filestats;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Filestats
{
	/// <exclude></exclude>
	public class FileUsageStats
	{
		private TreeStringObject _classUsageStats = null;

		private long _fileSize;

		private readonly long _fileHeader;

		private readonly long _freespace;

		private readonly long _idSystem;

		private readonly long _classMetadata;

		private readonly long _freespaceUsage;

		private readonly ISlotMap _slots;

		public FileUsageStats(long fileSize, long fileHeader, long idSystem, long freespace
			, long classMetadata, long freespaceUsage, ISlotMap slots)
		{
			_fileSize = fileSize;
			_fileHeader = fileHeader;
			_idSystem = idSystem;
			_freespace = freespace;
			_classMetadata = classMetadata;
			_freespaceUsage = freespaceUsage;
			_slots = slots;
		}

		public virtual void AddClassStats(ClassUsageStats classStats)
		{
			_classUsageStats = ((TreeStringObject)Tree.Add(_classUsageStats, new TreeStringObject
				(classStats.ClassName(), classStats)));
		}

		public virtual long FileHeader()
		{
			return _fileHeader;
		}

		public virtual long Freespace()
		{
			return _freespace;
		}

		public virtual long IdSystem()
		{
			return _idSystem;
		}

		public virtual long ClassMetadata()
		{
			return _classMetadata;
		}

		public virtual long FreespaceUsage()
		{
			return _freespaceUsage;
		}

		public virtual long FileSize()
		{
			return _fileSize;
		}

		public virtual void AddSlot(Slot slot)
		{
			_slots.Add(slot);
		}

		public virtual long TotalUsage()
		{
			LongByRef total = new LongByRef(_fileHeader + _freespace + _idSystem + _classMetadata
				 + _freespaceUsage);
			Tree.Traverse(_classUsageStats, new _IVisitor4_65(total));
			return total.value;
		}

		private sealed class _IVisitor4_65 : IVisitor4
		{
			public _IVisitor4_65(LongByRef total)
			{
				this.total = total;
			}

			public void Visit(object node)
			{
				total.value += ((ClassUsageStats)((TreeStringObject)node)._value).TotalUsage();
			}

			private readonly LongByRef total;
		}

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			Tree.Traverse(_classUsageStats, new _IVisitor4_75(str));
			str.Append("\n");
			str.Append(FileUsageStatsUtil.FormatLine("File header", FileHeader()));
			str.Append(FileUsageStatsUtil.FormatLine("Freespace", Freespace()));
			str.Append(FileUsageStatsUtil.FormatLine("ID system", IdSystem()));
			str.Append(FileUsageStatsUtil.FormatLine("Class metadata", ClassMetadata()));
			str.Append(FileUsageStatsUtil.FormatLine("Freespace usage", FreespaceUsage()));
			str.Append("\n");
			long totalUsage = TotalUsage();
			str.Append(FileUsageStatsUtil.FormatLine("Total", totalUsage));
			str.Append(FileUsageStatsUtil.FormatLine("Unaccounted", FileSize() - totalUsage));
			str.Append(FileUsageStatsUtil.FormatLine("File", FileSize()));
			str.Append(_slots);
			return str.ToString();
		}

		private sealed class _IVisitor4_75 : IVisitor4
		{
			public _IVisitor4_75(StringBuilder str)
			{
				this.str = str;
			}

			public void Visit(object node)
			{
				ClassUsageStats classStats = ((ClassUsageStats)((TreeStringObject)node)._value);
				str.Append(classStats.ClassName()).Append("\n");
				str.Append(FileUsageStatsUtil.FormatLine("Slots", classStats.SlotUsage()));
				str.Append(FileUsageStatsUtil.FormatLine("Class index", classStats.ClassIndexUsage
					()));
				str.Append(FileUsageStatsUtil.FormatLine("Field indices", classStats.FieldIndexUsage
					()));
				if (classStats.MiscUsage() > 0)
				{
					str.Append(FileUsageStatsUtil.FormatLine("Misc", classStats.MiscUsage()));
				}
				str.Append(FileUsageStatsUtil.FormatLine("Total", classStats.TotalUsage()));
			}

			private readonly StringBuilder str;
		}

		public virtual ClassUsageStats ClassStats(string name)
		{
			TreeStringObject found = (TreeStringObject)Tree.Find(_classUsageStats, new TreeStringObject
				(name, null));
			return found == null ? null : ((ClassUsageStats)found._value);
		}
	}
}
