namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class QEContains : Db4objects.Db4o.QEStringCmp
	{
		public QEContains(bool caseSensitive_) : base(caseSensitive_)
		{
		}

		protected override bool CompareStrings(string candidate, string constraint)
		{
			return candidate.IndexOf(constraint) > -1;
		}
	}
}
