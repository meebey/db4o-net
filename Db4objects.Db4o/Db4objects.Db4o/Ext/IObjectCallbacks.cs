/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;

namespace Db4objects.Db4o.Ext
{
	/// <summary>callback methods.</summary>
	/// <remarks>
	/// callback methods.
	/// &lt;br&gt;&lt;br&gt;
	/// This interface only serves as a list of all available callback methods.
	/// Every method is called individually, independantly of implementing this interface.&lt;br&gt;&lt;br&gt;
	/// &lt;b&gt;Using callbacks&lt;/b&gt;&lt;br&gt;
	/// Simply implement one or more of the listed methods in your application classes to
	/// do tasks before activation, deactivation, delete, new or update, to cancel the
	/// action about to be performed and to respond to the performed task.
	/// &lt;br&gt;&lt;br&gt;Callback methods are typically used for:
	/// &lt;br&gt;- cascaded delete
	/// &lt;br&gt;- cascaded update
	/// &lt;br&gt;- cascaded activation
	/// &lt;br&gt;- restoring transient members on instantiation
	/// &lt;br&gt;&lt;br&gt;Callback methods follow regular calling conventions. Methods in superclasses
	/// need to be called explicitely.
	/// &lt;br&gt;&lt;br&gt;All method calls are implemented to occur only once, upon one event.
	/// </remarks>
	public interface IObjectCallbacks
	{
		/// <summary>called before an Object is activated.</summary>
		/// <remarks>called before an Object is activated.</remarks>
		/// <param name="container">the &lt;code&gt;ObjectContainer&lt;/code&gt; the object is stored in.
		/// 	</param>
		/// <returns>false to prevent activation.</returns>
		bool ObjectCanActivate(IObjectContainer container);

		/// <summary>called before an Object is deactivated.</summary>
		/// <remarks>called before an Object is deactivated.</remarks>
		/// <param name="container">the &lt;code&gt;ObjectContainer&lt;/code&gt; the object is stored in.
		/// 	</param>
		/// <returns>false to prevent deactivation.</returns>
		bool ObjectCanDeactivate(IObjectContainer container);

		/// <summary>called before an Object is deleted.</summary>
		/// <remarks>
		/// called before an Object is deleted.
		/// &lt;br&gt;&lt;br&gt;In a client/server setup this callback method will be executed on
		/// the server.
		/// </remarks>
		/// <param name="container">the &lt;code&gt;ObjectContainer&lt;/code&gt; the object is stored in.
		/// 	</param>
		/// <returns>false to prevent the object from being deleted.</returns>
		bool ObjectCanDelete(IObjectContainer container);

		/// <summary>called before an Object is stored the first time.</summary>
		/// <remarks>called before an Object is stored the first time.</remarks>
		/// <param name="container">the &lt;code&gt;ObjectContainer&lt;/code&gt; is about to be stored to.
		/// 	</param>
		/// <returns>false to prevent the object from being stored.</returns>
		bool ObjectCanNew(IObjectContainer container);

		/// <summary>called before a persisted Object is updated.</summary>
		/// <remarks>called before a persisted Object is updated.</remarks>
		/// <param name="container">the &lt;code&gt;ObjectContainer&lt;/code&gt; the object is stored in.
		/// 	</param>
		/// <returns>false to prevent the object from being updated.</returns>
		bool ObjectCanUpdate(IObjectContainer container);

		/// <summary>called upon activation of an object.</summary>
		/// <remarks>called upon activation of an object.</remarks>
		/// <param name="container">the &lt;code&gt;ObjectContainer&lt;/code&gt; the object is stored in.
		/// 	</param>
		void ObjectOnActivate(IObjectContainer container);

		/// <summary>called upon deactivation of an object.</summary>
		/// <remarks>called upon deactivation of an object.</remarks>
		/// <param name="container">the &lt;code&gt;ObjectContainer&lt;/code&gt; the object is stored in.
		/// 	</param>
		void ObjectOnDeactivate(IObjectContainer container);

		/// <summary>called after an object was deleted.</summary>
		/// <remarks>
		/// called after an object was deleted.
		/// &lt;br&gt;&lt;br&gt;In a client/server setup this callback method will be executed on
		/// the server.
		/// </remarks>
		/// <param name="container">the &lt;code&gt;ObjectContainer&lt;/code&gt; the object was stored in.
		/// 	</param>
		void ObjectOnDelete(IObjectContainer container);

		/// <summary>called after a new object was stored.</summary>
		/// <remarks>called after a new object was stored.</remarks>
		/// <param name="container">the &lt;code&gt;ObjectContainer&lt;/code&gt; the object is stored to.
		/// 	</param>
		void ObjectOnNew(IObjectContainer container);

		/// <summary>called after an object was updated.</summary>
		/// <remarks>called after an object was updated.</remarks>
		/// <param name="container">the &lt;code&gt;ObjectContainer&lt;/code&gt; the object is stored in.
		/// 	</param>
		void ObjectOnUpdate(IObjectContainer container);
	}
}
