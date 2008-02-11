/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Types
{
	/// <summary>marks objects as second class objects.</summary>
	/// <remarks>
	/// marks objects as second class objects.
	/// &lt;br&gt;&lt;br&gt;Currently this interface is for internal use only
	/// to help discard com.db4o.config.Entry objects in the
	/// Defragment process.
	/// &lt;br&gt;&lt;br&gt;For future versions this interface is planned to
	/// mark objects that:&lt;br&gt;
	/// - are not to be held in the reference mechanism&lt;br&gt;
	/// - should always be activated with their parent objects&lt;br&gt;
	/// - should always be deleted with their parent objects&lt;br&gt;
	/// - should always be deleted if they are not referenced any
	/// longer.&lt;br&gt;
	/// </remarks>
	public interface ISecondClass : IDb4oType
	{
	}
}
