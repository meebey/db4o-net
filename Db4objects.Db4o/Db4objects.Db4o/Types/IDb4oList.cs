/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Types
{
	/// <summary>db4o List implementation for database-aware lists.</summary>
	/// <remarks>
	/// db4o List implementation for database-aware lists.
	/// <br /><br />
	/// A <code>Db4oList</code> supplies the methods specified in java.util.List.<br /><br />
	/// All access to the list is controlled by the
	/// <see cref="Db4objects.Db4o.IObjectContainer">IObjectContainer</see>
	/// to help the
	/// programmer produce expected results with as little work as possible:<br />
	/// - newly added objects are automatically persisted.<br />
	/// - list elements are automatically activated when they are needed. The activation
	/// depth is configurable with
	/// <see cref="Db4objects.Db4o.Types.IDb4oCollection.ActivationDepth">Db4objects.Db4o.Types.IDb4oCollection.ActivationDepth
	/// 	</see>
	/// .<br />
	/// - removed objects can be deleted automatically, if the list is configured
	/// with
	/// <see cref="Db4objects.Db4o.Types.IDb4oCollection.DeleteRemoved">Db4objects.Db4o.Types.IDb4oCollection.DeleteRemoved
	/// 	</see>
	/// <br /><br />
	/// Usage:<br />
	/// - declare a <code>java.util.List</code> variable on your persistent classes.<br />
	/// - fill this variable with a method in the ObjectContainer collection factory.<br /><br />
	/// <b>Example:</b><br /><br />
	/// <code>class MyClass{<br />
	/// &nbsp;&nbsp;List myList;<br />
	/// }<br /><br />
	/// MyClass myObject = new MyClass();<br />
	/// myObject.myList = objectContainer.ext().collections().newLinkedList();
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Ext.IExtObjectContainer.Collections">Db4objects.Db4o.Ext.IExtObjectContainer.Collections
	/// 	</seealso>
	/// <decaf.ignore.implements.jdk11>List</decaf.ignore.implements.jdk11>
	[System.ObsoleteAttribute(@"since 7.0")]
	public interface IDb4oList : IDb4oCollection, IList
	{
	}
}
