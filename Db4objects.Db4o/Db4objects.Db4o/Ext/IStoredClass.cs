/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>the internal representation of a stored class.</summary>
	/// <remarks>the internal representation of a stored class.</remarks>
	public interface IStoredClass
	{
		/// <summary>returns the name of this stored class.</summary>
		/// <remarks>returns the name of this stored class.</remarks>
		string GetName();

		/// <summary>returns an array of IDs of all stored object instances of this stored class.
		/// 	</summary>
		/// <remarks>returns an array of IDs of all stored object instances of this stored class.
		/// 	</remarks>
		long[] GetIDs();

		/// <summary>returns the StoredClass for the parent of the class, this StoredClass represents.
		/// 	</summary>
		/// <remarks>returns the StoredClass for the parent of the class, this StoredClass represents.
		/// 	</remarks>
		IStoredClass GetParentStoredClass();

		/// <summary>returns all stored fields of this stored class.</summary>
		/// <remarks>returns all stored fields of this stored class.</remarks>
		IStoredField[] GetStoredFields();

		/// <summary>returns true if this StoredClass has a class index.</summary>
		/// <remarks>returns true if this StoredClass has a class index.</remarks>
		bool HasClassIndex();

		/// <summary>renames this stored class.</summary>
		/// <remarks>
		/// renames this stored class.
		/// &lt;br&gt;&lt;br&gt;After renaming one or multiple classes the ObjectContainer has
		/// to be closed and reopened to allow internal caches to be refreshed.
		/// &lt;br&gt;&lt;br&gt;.NET: As the name you should provide [Classname, Assemblyname]&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="name">the new name</param>
		void Rename(string name);

		/// <summary>returns an existing stored field of this stored class.</summary>
		/// <remarks>returns an existing stored field of this stored class.</remarks>
		/// <param name="name">the name of the field</param>
		/// <param name="type">
		/// the type of the field.
		/// There are four possibilities how to supply the type:&lt;br&gt;
		/// - a Class object.  (.NET: a Type object)&lt;br&gt;
		/// - a fully qualified classname.&lt;br&gt;
		/// - any object to be used as a template.&lt;br&gt;&lt;br&gt;
		/// - null, if the first found field should be returned.
		/// </param>
		/// <returns>
		/// the
		/// <see cref="IStoredField">IStoredField</see>
		/// </returns>
		IStoredField StoredField(string name, object type);
	}
}
