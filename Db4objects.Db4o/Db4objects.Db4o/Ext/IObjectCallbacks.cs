namespace Db4objects.Db4o.Ext
{
	/// <summary>callback methods.</summary>
	/// <remarks>
	/// callback methods.
	/// <br /><br /><b>Examples: ../com/db4o/samples/callbacks.</b><br /><br />
	/// This interface only serves as a lists of all available callback methods.
	/// Every method is called individually, independant of implementing this interface.<br /><br />
	/// <b>Using callbacks</b><br />
	/// Simply implement one or more of the listed methods in your application classes to
	/// do tasks before activation, deactivation, delete, new or update, to cancel the
	/// action about to be performed and to respond to the performed task.
	/// <br /><br />Callback methods are typically used for:
	/// <br />- cascaded delete
	/// <br />- cascaded update
	/// <br />- cascaded activation
	/// <br />- restoring transient members on instantiation
	/// <br /><br />Callback methods follow regular calling conventions. Methods in superclasses
	/// need to be called explicitely.
	/// <br /><br />All method calls are implemented to occur only once, upon one event.
	/// </remarks>
	public interface IObjectCallbacks
	{
		/// <summary>called before an Object is activated.</summary>
		/// <remarks>called before an Object is activated.</remarks>
		/// <param name="container">the <code>ObjectContainer</code> the object is stored in.
		/// 	</param>
		/// <returns>false to prevent activation.</returns>
		bool ObjectCanActivate(Db4objects.Db4o.IObjectContainer container);

		/// <summary>called before an Object is deactivated.</summary>
		/// <remarks>called before an Object is deactivated.</remarks>
		/// <param name="container">the <code>ObjectContainer</code> the object is stored in.
		/// 	</param>
		/// <returns>false to prevent deactivation.</returns>
		bool ObjectCanDeactivate(Db4objects.Db4o.IObjectContainer container);

		/// <summary>called before an Object is deleted.</summary>
		/// <remarks>
		/// called before an Object is deleted.
		/// <br /><br />In a client/server setup this callback method will be executed on
		/// the server.
		/// </remarks>
		/// <param name="container">the <code>ObjectContainer</code> the object is stored in.
		/// 	</param>
		/// <returns>false to prevent the object from being deleted.</returns>
		bool ObjectCanDelete(Db4objects.Db4o.IObjectContainer container);

		/// <summary>called before an Object is stored the first time.</summary>
		/// <remarks>called before an Object is stored the first time.</remarks>
		/// <param name="container">the <code>ObjectContainer</code> is about to be stored to.
		/// 	</param>
		/// <returns>false to prevent the object from being stored.</returns>
		bool ObjectCanNew(Db4objects.Db4o.IObjectContainer container);

		/// <summary>called before a persisted Object is updated.</summary>
		/// <remarks>called before a persisted Object is updated.</remarks>
		/// <param name="container">the <code>ObjectContainer</code> the object is stored in.
		/// 	</param>
		/// <returns>false to prevent the object from being updated.</returns>
		bool ObjectCanUpdate(Db4objects.Db4o.IObjectContainer container);

		/// <summary>called upon activation of an object.</summary>
		/// <remarks>called upon activation of an object.</remarks>
		/// <param name="container">the <code>ObjectContainer</code> the object is stored in.
		/// 	</param>
		void ObjectOnActivate(Db4objects.Db4o.IObjectContainer container);

		/// <summary>called upon deactivation of an object.</summary>
		/// <remarks>called upon deactivation of an object.</remarks>
		/// <param name="container">the <code>ObjectContainer</code> the object is stored in.
		/// 	</param>
		void ObjectOnDeactivate(Db4objects.Db4o.IObjectContainer container);

		/// <summary>called after an object was deleted.</summary>
		/// <remarks>
		/// called after an object was deleted.
		/// <br /><br />In a client/server setup this callback method will be executed on
		/// the server.
		/// </remarks>
		/// <param name="container">the <code>ObjectContainer</code> the object was stored in.
		/// 	</param>
		void ObjectOnDelete(Db4objects.Db4o.IObjectContainer container);

		/// <summary>called after a new object was stored.</summary>
		/// <remarks>called after a new object was stored.</remarks>
		/// <param name="container">the <code>ObjectContainer</code> the object is stored to.
		/// 	</param>
		void ObjectOnNew(Db4objects.Db4o.IObjectContainer container);

		/// <summary>called after an object was updated.</summary>
		/// <remarks>called after an object was updated.</remarks>
		/// <param name="container">the <code>ObjectContainer</code> the object is stored in.
		/// 	</param>
		void ObjectOnUpdate(Db4objects.Db4o.IObjectContainer container);
	}
}
