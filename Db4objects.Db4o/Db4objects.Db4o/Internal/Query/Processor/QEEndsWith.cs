using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QEEndsWith : QEStringCmp
	{
		public QEEndsWith(bool caseSensitive_) : base(caseSensitive_)
		{
		}

		protected override bool CompareStrings(string candidate, string constraint)
		{
			int lastIndex = candidate.LastIndexOf(constraint);
			if (lastIndex == -1)
			{
				return false;
			}
			return lastIndex == candidate.Length - constraint.Length;
		}
	}
}
