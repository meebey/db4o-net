using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	/// <exclude></exclude>
	public class AddressKeySlotHandler : SlotHandler
	{
		public override int CompareTo(object obj)
		{
			return _current.CompareByAddress((Slot)obj);
		}
	}
}
