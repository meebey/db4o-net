namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class QEStartsWith : Db4objects.Db4o.QEStringCmp
	{
		public QEStartsWith(bool caseSensitive_) : base(caseSensitive_)
		{
		}

		protected override bool CompareStrings(string candidate, string constraint)
		{
			return candidate.IndexOf(constraint) == 0;
		}
	}
}
