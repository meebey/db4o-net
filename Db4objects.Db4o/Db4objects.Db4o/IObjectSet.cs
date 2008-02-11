/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o
{
	/// <summary>query resultset.</summary>
	/// <remarks>
	/// query resultset.
	/// &lt;br&gt;&lt;br&gt;An ObjectSet is a representation for a set of objects returned
	/// by a query.
	/// &lt;br&gt;&lt;br&gt;ObjectSet extends the system collection interfaces
	/// java.util.List/System.Collections.IList where they are available. It is
	/// recommended, never to reference ObjectSet directly in code but to use
	/// List / IList instead.
	/// &lt;br&gt;&lt;br&gt;Note that the underlying
	/// <see cref="IObjectContainer">IObjectContainer</see>
	/// of an ObjectSet
	/// needs to remain open as long as an ObjectSet is used. This is necessary
	/// for lazy instantiation. The objects in an ObjectSet are only instantiated
	/// when they are actually being used by the application.
	/// </remarks>
	/// <seealso cref="IExtObjectSet">for extended functionality.</seealso>
	public interface IObjectSet : System.Collections.IList
	{
		/// <summary>returns an ObjectSet with extended functionality.</summary>
		/// <remarks>
		/// returns an ObjectSet with extended functionality.
		/// &lt;br&gt;&lt;br&gt;Every ObjectSet that db4o provides can be casted to
		/// an ExtObjectSet. This method is supplied for your convenience
		/// to work without a cast.
		/// &lt;br&gt;&lt;br&gt;The ObjectSet functionality is split to two interfaces
		/// to allow newcomers to focus on the essential methods.
		/// </remarks>
		IExtObjectSet Ext();

		/// <summary>returns &lt;code&gt;true&lt;/code&gt; if the &lt;code&gt;ObjectSet&lt;/code&gt; has more elements.
		/// 	</summary>
		/// <remarks>returns &lt;code&gt;true&lt;/code&gt; if the &lt;code&gt;ObjectSet&lt;/code&gt; has more elements.
		/// 	</remarks>
		/// <returns>
		/// boolean - &lt;code&gt;true&lt;/code&gt; if the &lt;code&gt;ObjectSet&lt;/code&gt; has more
		/// elements.
		/// </returns>
		bool HasNext();

		/// <summary>returns the next object in the &lt;code&gt;ObjectSet&lt;/code&gt;.</summary>
		/// <remarks>
		/// returns the next object in the &lt;code&gt;ObjectSet&lt;/code&gt;.
		/// &lt;br&gt;&lt;br&gt;
		/// Before returning the Object, next() triggers automatic activation of the
		/// Object with the respective
		/// <see cref="IConfiguration.ActivationDepth">global</see>
		/// or
		/// <see cref="IObjectClass.MaximumActivationDepth">class specific</see>
		/// setting.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <returns>the next object in the &lt;code&gt;ObjectSet&lt;/code&gt;.</returns>
		object Next();

		/// <summary>resets the &lt;code&gt;ObjectSet&lt;/code&gt; cursor before the first element.
		/// 	</summary>
		/// <remarks>
		/// resets the &lt;code&gt;ObjectSet&lt;/code&gt; cursor before the first element.
		/// &lt;br&gt;&lt;br&gt;A subsequent call to &lt;code&gt;next()&lt;/code&gt; will return the first element.
		/// </remarks>
		void Reset();

		/// <summary>returns the number of elements in the &lt;code&gt;ObjectSet&lt;/code&gt;.
		/// 	</summary>
		/// <remarks>returns the number of elements in the &lt;code&gt;ObjectSet&lt;/code&gt;.
		/// 	</remarks>
		/// <returns>the number of elements in the &lt;code&gt;ObjectSet&lt;/code&gt;.</returns>
		int Size();
	}
}
