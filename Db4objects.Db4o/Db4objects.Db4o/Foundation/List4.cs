/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Foundation
{
	/// <summary>elements in linked list Collection4</summary>
	/// <exclude></exclude>
	public sealed class List4 : IUnversioned
	{
		/// <summary>next element in list</summary>
		public Db4objects.Db4o.Foundation.List4 _next;

		/// <summary>carried object</summary>
		public object _element;

		/// <summary>db4o constructor to be able to store objects of this class</summary>
		public List4()
		{
		}

		public List4(object element)
		{
			// TODO: encapsulate field access
			_element = element;
		}

		public List4(Db4objects.Db4o.Foundation.List4 next, object element)
		{
			_next = next;
			_element = element;
		}

		internal bool Holds(object obj)
		{
			if (obj == null)
			{
				return _element == null;
			}
			return obj.Equals(_element);
		}
	}
}
