/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Activation
{
	/// <summary>Activator interface.</summary>
	/// <remarks>
	/// Activator interface. <br />
	/// Activatable objects need to have a reference to
	/// an Activator implementation, which is called
	/// by TransparentActivation framework, when a request is
	/// received to activate the host object.
	/// </remarks>
	/// <seealso><a href="http://developer.db4o.com/resources/view.aspx/reference/Object_Lifecycle/Activation/Transparent_Activation_Framework">Transparent Activation framework.</a>
	/// 	</seealso>
	public interface IActivator
	{
		/// <summary>Method to be called to activate the host object.</summary>
		/// <remarks>Method to be called to activate the host object.</remarks>
		void Activate();
	}
}
