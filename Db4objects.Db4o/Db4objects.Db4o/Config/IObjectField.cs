/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Config
{
	/// <summary>configuration interface for fields of classes.</summary>
	/// <remarks>
	/// configuration interface for fields of classes.
	/// <br/><br/>
	/// Use the global Configuration object to configure db4o before opening an
	/// <see cref="IObjectContainer">IObjectContainer</see>
	/// .<br/><br/>
	/// <b>Example:</b><br/>
	/// <code>
	/// IConfiguration config = Db4oFactory.Configure();<br/>
	/// IObjectClass oc = config.ObjectClass("Namespace.ClassName");<br/>
	/// IObjectField of = oc.ObjectField("fieldName");
	/// of.Rename("newFieldName");
	/// of.QueryEvaluation(false);
	/// 
	/// </code>
	/// </remarks>
	public interface IObjectField
	{
		/// <summary>sets cascaded activation behaviour.</summary>
		/// <remarks>
		/// sets cascaded activation behaviour.
		/// &lt;br&gt;&lt;br&gt;
		/// Setting cascadeOnActivate to true will result in the activation
		/// of the object attribute stored in this field if the parent object
		/// is activated.
		/// &lt;br&gt;&lt;br&gt;
		/// The default setting is &lt;b&gt;false&lt;/b&gt;.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">whether activation is to be cascaded to the member object.</param>
		/// <seealso cref="IConfiguration.ActivationDepth">Why activation?</seealso>
		/// <seealso cref="IObjectClass.CascadeOnActivate">IObjectClass.CascadeOnActivate</seealso>
		/// <seealso cref="IObjectContainer.Activate">IObjectContainer.Activate</seealso>
		/// <seealso cref="IObjectCallbacks">Using callbacks</seealso>
		void CascadeOnActivate(bool flag);

		/// <summary>sets cascaded delete behaviour.</summary>
		/// <remarks>
		/// sets cascaded delete behaviour.
		/// &lt;br&gt;&lt;br&gt;
		/// Setting cascadeOnDelete to true will result in the deletion of
		/// the object attribute stored in this field on the parent object
		/// if the parent object is passed to
		/// <see cref="IObjectContainer.Delete">IObjectContainer.Delete</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;
		/// &lt;b&gt;Caution !&lt;/b&gt;&lt;br&gt;
		/// This setting will also trigger deletion of the old member object, on
		/// calls to
		/// <see cref="IObjectContainer.Set"></see>
		/// .
		/// An example of the behaviour can be found in
		/// <see cref="IObjectClass.CascadeOnDelete">IObjectClass.CascadeOnDelete</see>
		/// &lt;br&gt;&lt;br&gt;
		/// The default setting is &lt;b&gt;false&lt;/b&gt;.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">whether deletes are to be cascaded to the member object.</param>
		/// <seealso cref="IObjectClass.CascadeOnDelete">IObjectClass.CascadeOnDelete</seealso>
		/// <seealso cref="IObjectContainer.Delete">IObjectContainer.Delete</seealso>
		/// <seealso cref="IObjectCallbacks">Using callbacks</seealso>
		void CascadeOnDelete(bool flag);

		/// <summary>sets cascaded update behaviour.</summary>
		/// <remarks>
		/// sets cascaded update behaviour.
		/// &lt;br&gt;&lt;br&gt;
		/// Setting cascadeOnUpdate to true will result in the update
		/// of the object attribute stored in this field if the parent object
		/// is passed to
		/// <see cref="IObjectContainer.Set">IObjectContainer.Set</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;
		/// The default setting is &lt;b&gt;false&lt;/b&gt;.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">whether updates are to be cascaded to the member object.</param>
		/// <seealso cref="IObjectContainer.Set">IObjectContainer.Set</seealso>
		/// <seealso cref="IObjectClass.CascadeOnUpdate">IObjectClass.CascadeOnUpdate</seealso>
		/// <seealso cref="IObjectClass.UpdateDepth">IObjectClass.UpdateDepth</seealso>
		/// <seealso cref="IObjectCallbacks">Using callbacks</seealso>
		void CascadeOnUpdate(bool flag);

		/// <summary>turns indexing on or off.</summary>
		/// <remarks>
		/// turns indexing on or off.
		/// &lt;br&gt;&lt;br&gt;Field indices dramatically improve query performance but they may
		/// considerably reduce storage and update performance.&lt;br&gt;The best benchmark whether
		/// or not an index on a field achieves the desired result is the completed application
		/// - with a data load that is typical for it's use.&lt;br&gt;&lt;br&gt;This configuration setting
		/// is only checked when the
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// is opened. If the
		/// setting is set to &lt;code&gt;true&lt;/code&gt; and an index does not exist, the index will be
		/// created. If the setting is set to &lt;code&gt;false&lt;/code&gt; and an index does exist the
		/// index will be dropped.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// If this setting is applied to an open ObjectContainer it will take an effect on the next
		/// time ObjectContainer is opened.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">
		/// specify &lt;code&gt;true&lt;/code&gt; or &lt;code&gt;false&lt;/code&gt; to turn indexing on for
		/// this field
		/// </param>
		void Indexed(bool flag);

		/// <summary>renames a field of a stored class.</summary>
		/// <remarks>
		/// renames a field of a stored class.
		/// &lt;br&gt;&lt;br&gt;Use this method to refactor classes.
		/// &lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// This setting can NOT be applied to an open object container. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="newName">the new fieldname.</param>
		void Rename(string newName);

		/// <summary>toggles query evaluation.</summary>
		/// <remarks>
		/// toggles query evaluation.
		/// &lt;br&gt;&lt;br&gt;All fields are evaluated by default. Use this method to turn query
		/// evaluation off for specific fields.&lt;br&gt;&lt;br&gt;
		/// In client-server environment this setting should be used on both
		/// client and server. &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">specify &lt;code&gt;false&lt;/code&gt; to ignore this field during query evaluation.
		/// 	</param>
		void QueryEvaluation(bool flag);
	}
}
