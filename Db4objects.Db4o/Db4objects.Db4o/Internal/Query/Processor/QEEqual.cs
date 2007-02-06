namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QEEqual : Db4objects.Db4o.Internal.Query.Processor.QEAbstract
	{
		public override void IndexBitMap(bool[] bits)
		{
			bits[Db4objects.Db4o.Internal.Query.Processor.QE.EQUAL] = true;
		}
	}
}
