/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QEContains : Db4objects.Db4o.Internal.Query.Processor.QEStringCmp
	{
		/// <summary>for C/S messaging only</summary>
		public QEContains()
		{
		}

		public QEContains(bool caseSensitive_) : base(caseSensitive_)
		{
		}

		protected override bool CompareStrings(string candidate, string constraint)
		{
			return candidate.IndexOf(constraint) > -1;
		}
	}
}
