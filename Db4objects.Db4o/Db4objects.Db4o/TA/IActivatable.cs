/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.TA
{
	/// <summary>
	/// Activatable must be implemented by classes in order to support
	/// Transparent Activation.&lt;br&gt;&lt;br&gt;
	/// The Activatable interface may be added to persistent classes
	/// by hand or by using the db4o enhancer.
	/// </summary>
	/// <remarks>
	/// Activatable must be implemented by classes in order to support
	/// Transparent Activation.&lt;br&gt;&lt;br&gt;
	/// The Activatable interface may be added to persistent classes
	/// by hand or by using the db4o enhancer. For further information
	/// on the enhancer see the chapter "Enhancement" in the db4o
	/// tutorial.&lt;br&gt;&lt;br&gt;
	/// The basic idea for Transparent Activation is as follows:&lt;br&gt;
	/// Objects have an activation depth of 0, i.e. by default they are
	/// not activated at all. Whenever a method is called on such an object,
	/// the first thing to do before actually executing the method body is
	/// to activate the object to level 1, i.e. populating its direct
	/// members.&lt;br&gt;&lt;br&gt;
	/// To illustrate this approach, we will use the following simple class.&lt;br&gt;&lt;br&gt;
	/// &lt;code&gt;
	/// public class Item {&lt;br&gt;
	/// &#160;&#160;&#160;private Item _next;&lt;br&gt;&lt;br&gt;
	/// &#160;&#160;&#160;public Item(Item next) {&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;_next = next;&lt;br&gt;
	/// &#160;&#160;&#160;}&lt;br&gt;&lt;br&gt;
	/// &#160;&#160;&#160;public Item next() {&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;return _next;&lt;br&gt;
	/// &#160;&#160;&#160;}&lt;br&gt;
	/// }&lt;br&gt;&lt;br&gt;&lt;/code&gt;
	/// The basic sequence of actions to get the above scheme to work is the
	/// following:&lt;br&gt;&lt;br&gt;
	/// - Whenever an object is instantiated from db4o, the database registers
	/// an activator for this object. To enable this, the object has to implement
	/// the Activatable interface and provide the according bind(Activator)
	/// method. The default implementation of the bind method will simply store
	/// the given activator reference for later use.&lt;br&gt;&lt;br&gt;
	/// &lt;code&gt;
	/// public class Item implements Activatable {&lt;br&gt;
	/// &#160;&#160;&#160;transient Activator _activator;&lt;br&gt;&lt;br&gt;
	/// &#160;&#160;&#160;public void bind(Activator activator) {&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;if (null != _activator) {&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;throw new IllegalStateException();&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;}&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;_activator = activator;&lt;br&gt;
	/// &#160;&#160;&#160;}&lt;br&gt;&lt;br&gt;
	/// &#160;&#160;&#160;// ...&lt;br&gt;
	/// }&lt;br&gt;&lt;br&gt;&lt;/code&gt;
	/// - The first action in every method body of an activatable object should
	/// be a call to the corresponding Activator's activate() method. (Note that
	/// this is not enforced by any interface, it is rather a convention, and
	/// other implementations are possible.)&lt;br&gt;&lt;br&gt;
	/// &lt;code&gt;
	/// public class Item implements Activatable {&lt;br&gt;
	/// &#160;&#160;&#160;public void activate() {&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;if (_activator == null) return;&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;_activator.activate();&lt;br&gt;
	/// &#160;&#160;&#160;}&lt;br&gt;&lt;br&gt;
	/// &#160;&#160;&#160;public Item next() {&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;activate();&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;return _next;&lt;br&gt;
	/// &#160;&#160;&#160;}&lt;br&gt;
	/// }&lt;br&gt;&lt;br&gt;&lt;/code&gt;
	/// - The activate() method will check whether the object is already activated.
	/// If this is not the case, it will request the container to activate the
	/// object to level 1 and set the activated flag accordingly.&lt;br&gt;&lt;br&gt;
	/// To instruct db4o to actually use these hooks (i.e. to register the
	/// database when instantiating an object), TransparentActivationSupport
	/// has to be registered with the db4o configuration.&lt;br&gt;&lt;br&gt;
	/// &lt;code&gt;
	/// Configuration config = ...&lt;br&gt;
	/// config.add(new TransparentActivationSupport());&lt;br&gt;&lt;br&gt;
	/// &lt;/code&gt;
	/// </remarks>
	public interface IActivatable
	{
		/// <summary>called by db4o upon instantiation.</summary>
		/// <remarks>
		/// called by db4o upon instantiation.
		/// &lt;br&gt;&lt;br&gt;The recommended implementation of this method is to store
		/// the passed
		/// <see cref="IActivator">IActivator</see>
		/// in a transient field of the object.
		/// </remarks>
		/// <param name="activator">the Activator</param>
		void Bind(IActivator activator);

		/// <summary>should be called by every reading field access of an object.</summary>
		/// <remarks>
		/// should be called by every reading field access of an object.
		/// &lt;br&gt;&lt;br&gt;The recommended implementation of this method is to call
		/// <see cref="IActivator.Activate">IActivator.Activate</see>
		/// on the
		/// <see cref="IActivator">IActivator</see>
		/// that was
		/// previously passed to
		/// <see cref="IActivatable.Bind">IActivatable.Bind</see>
		/// .
		/// </remarks>
		/// <param name="purpose">TODO</param>
		void Activate(ActivationPurpose purpose);
	}
}
