namespace Db4objects.Db4o.Types
{
	/// <summary>marks objects as second class objects.</summary>
	/// <remarks>
	/// marks objects as second class objects.
	/// <br /><br />Currently this interface is for internal use only
	/// to help discard com.db4o.config.Entry objects in the
	/// Defragment process.
	/// <br /><br />For future versions this interface is planned to
	/// mark objects that:<br />
	/// - are not to be held in the reference mechanism<br />
	/// - should always be activated with their parent objects<br />
	/// - should always be deleted with their parent objects<br />
	/// - should always be deleted if they are not referenced any
	/// longer.<br />
	/// </remarks>
	public interface ISecondClass : Db4objects.Db4o.Types.IDb4oType
	{
	}
}
