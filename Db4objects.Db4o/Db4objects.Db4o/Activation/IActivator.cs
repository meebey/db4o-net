/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Activation
{
	/// <summary>
	/// Activator interface.<br />
	/// <br /><br />
	/// <see cref="IActivatable">IActivatable</see>
	/// objects need to have a reference to
	/// an Activator implementation, which is called
	/// by Transparent Activation, when a request is received to
	/// activate the host object.
	/// </summary>
	/// <seealso><a href="http://developer.db4o.com/resources/view.aspx/reference/Object_Lifecycle/Activation/Transparent_Activation_Framework">Transparent Activation framework.</a>
	/// 	</seealso>
	public interface IActivator
	{
		/// <summary>Method to be called to activate the host object.</summary>
		/// <remarks>Method to be called to activate the host object.</remarks>
		/// <param name="purpose">
		/// for which purpose is the object being activated?
		/// <see cref="ActivationPurpose.Write">ActivationPurpose.Write</see>
		/// will cause the object
		/// to be saved on the next
		/// <see cref="IObjectContainer.Commit">IObjectContainer.Commit</see>
		/// operation.
		/// </param>
		void Activate(ActivationPurpose purpose);
	}
}
