namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class QEEqual : Db4objects.Db4o.QEAbstract
	{
		public override void IndexBitMap(bool[] bits)
		{
			bits[Db4objects.Db4o.QE.EQUAL] = true;
		}
	}
}
