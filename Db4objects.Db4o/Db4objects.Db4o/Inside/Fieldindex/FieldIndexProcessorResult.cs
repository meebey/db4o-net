namespace Db4objects.Db4o.Inside.Fieldindex
{
	public class FieldIndexProcessorResult
	{
		public static readonly Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult
			 NO_INDEX_FOUND = new Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult
			(null);

		public static readonly Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult
			 FOUND_INDEX_BUT_NO_MATCH = new Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult
			(null);

		public readonly Db4objects.Db4o.TreeInt found;

		public FieldIndexProcessorResult(Db4objects.Db4o.TreeInt found_)
		{
			found = found_;
		}
	}
}
