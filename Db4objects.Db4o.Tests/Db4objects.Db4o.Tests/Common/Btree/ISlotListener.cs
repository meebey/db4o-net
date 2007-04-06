using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Tests.Common.Btree
{
	/// <exclude></exclude>
	public interface ISlotListener
	{
		void OnFree(Slot slot);
	}
}
