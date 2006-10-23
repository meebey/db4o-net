namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class QEEndsWith : Db4objects.Db4o.QEStringCmp
	{
		public QEEndsWith(bool caseSensitive_) : base(caseSensitive_)
		{
		}

		protected override bool CompareStrings(string candidate, string constraint)
		{
			return candidate.LastIndexOf(constraint) == candidate.Length - constraint.Length;
		}
	}
}
