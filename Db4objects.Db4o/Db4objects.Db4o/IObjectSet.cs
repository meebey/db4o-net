/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o
{
	/// <summary>query resultset.</summary>
	/// <remarks>
	/// query resultset.
	/// <br /><br />An ObjectSet is a representation for a set of objects returned
	/// by a query.
	/// <br /><br />ObjectSet extends the system collection interfaces
	/// java.util.List/System.Collections.IList where they are available. It is
	/// recommended, never to reference ObjectSet directly in code but to use
	/// List / IList instead.
	/// <br /><br />Note that the underlying
	/// <see cref="Db4objects.Db4o.IObjectContainer">IObjectContainer</see>
	/// of an ObjectSet
	/// needs to remain open as long as an ObjectSet is used. This is necessary
	/// for lazy instantiation. The objects in an ObjectSet are only instantiated
	/// when they are actually being used by the application.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Ext.IExtObjectSet">for extended functionality.</seealso>
	/// <decaf.ignore.implements.jdk11></decaf.ignore.implements.jdk11>
	public interface IObjectSet : IList
	{
		/// <summary>returns an ObjectSet with extended functionality.</summary>
		/// <remarks>
		/// returns an ObjectSet with extended functionality.
		/// <br /><br />Every ObjectSet that db4o provides can be casted to
		/// an ExtObjectSet. This method is supplied for your convenience
		/// to work without a cast.
		/// <br /><br />The ObjectSet functionality is split to two interfaces
		/// to allow newcomers to focus on the essential methods.
		/// </remarks>
		IExtObjectSet Ext();

		/// <summary>returns <code>true</code> if the <code>ObjectSet</code> has more elements.
		/// 	</summary>
		/// <remarks>returns <code>true</code> if the <code>ObjectSet</code> has more elements.
		/// 	</remarks>
		/// <returns>
		/// boolean - <code>true</code> if the <code>ObjectSet</code> has more
		/// elements.
		/// </returns>
		bool HasNext();

		/// <summary>returns the next object in the <code>ObjectSet</code>.</summary>
		/// <remarks>
		/// returns the next object in the <code>ObjectSet</code>.
		/// <br /><br />
		/// Before returning the Object, next() triggers automatic activation of the
		/// Object with the respective
		/// <see cref="Db4objects.Db4o.Config.IConfiguration.ActivationDepth">global</see>
		/// or
		/// <see cref="Db4objects.Db4o.Config.IObjectClass.MaximumActivationDepth">class specific
		/// 	</see>
		/// setting.<br /><br />
		/// </remarks>
		/// <returns>the next object in the <code>ObjectSet</code>.</returns>
		object Next();

		/// <summary>resets the <code>ObjectSet</code> cursor before the first element.</summary>
		/// <remarks>
		/// resets the <code>ObjectSet</code> cursor before the first element.
		/// <br /><br />A subsequent call to <code>next()</code> will return the first element.
		/// </remarks>
		void Reset();
	}
}
