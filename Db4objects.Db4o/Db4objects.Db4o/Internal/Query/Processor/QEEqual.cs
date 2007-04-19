using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QEEqual : QEAbstract
	{
		public override void IndexBitMap(bool[] bits)
		{
			bits[QE.EQUAL] = true;
		}
	}
}
