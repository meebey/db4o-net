/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Activation;

namespace Db4objects.Db4o.TA
{
	/// <summary>
	/// The basic idea for this initial TA implementation is very simple:
	/// Objects have an activation depth of 0, i.e.
	/// </summary>
	/// <remarks>
	/// The basic idea for this initial TA implementation is very simple:
	/// Objects have an activation depth of 0, i.e. by default they are
	/// not activated at all. Whenever a method is called on such an object,
	/// the first thing to do before actually executing the method body is
	/// to activate the object to level 1, i.e. populating its direct
	/// members.
	/// To illustrate this approach, we will use the following simple class.
	/// public class Item {
	/// private Item _next;
	/// public Item(Item next) {
	/// _next = next;
	/// }
	/// public Item next() {
	/// return _next;
	/// }
	/// }
	/// The basic sequence of actions to get the above scheme to work is the
	/// following:
	/// - Whenever an object is instantiated from db4o, the database registers
	/// an activator for this object. To enable this, the object has to implement
	/// the Activatable interface and provide the according bind(Activator)
	/// method. The default implementation of the bind method will simply store
	/// the given activator reference for later use.
	/// public class Item implements Activatable {
	/// transient Activator _activator;
	/// public void bind(Activator activator) {
	/// if (null != _activator) {
	/// throw new IllegalStateException();
	/// }
	/// _activator = activator;
	/// }
	/// // ...
	/// }
	/// - The first action in every method body of an activatable object should
	/// be a call to the corresponding Activator's activate() method. (Note that
	/// this is not enforced by any interface, it is rather a convention, and
	/// other implementations are possible.)
	/// public class Item implements Activatable {
	/// protected void activate() {
	/// if (_activator == null) return;
	/// _activator.activate();
	/// }
	/// public Item next() {
	/// activate();
	/// return _next;
	/// }
	/// }
	/// - The activate() method will check whether the object is already activated.
	/// If this is not the case, it will request the container to activate the
	/// object to level 1 and set the activated flag accordingly.
	/// To instruct db4o to actually use these hooks (i.e. to register the
	/// database when instantiating an object), TransparentActivationSupport
	/// has to be registered with the db4o configuration.
	/// Configuration config = ...
	/// config.add(new TransparentActivationSupport());
	/// This basic implementation may still be subject to changes. For example,
	/// the "one activator per object" policy could be dropped in favor of a
	/// (probably Map based) per-container registry of activatable objects, or
	/// by reusing existing db4o internal metadata (like ObjectReference) for
	/// TA purposes.
	/// </remarks>
	public interface IActivatable
	{
		void Bind(IActivator activator);
	}
}
