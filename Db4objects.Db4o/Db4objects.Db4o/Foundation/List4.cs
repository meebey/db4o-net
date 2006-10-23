namespace Db4objects.Db4o.Foundation
{
	/// <summary>elements in linked list Collection4</summary>
	/// <exclude></exclude>
	public sealed class List4 : Db4objects.Db4o.Types.IUnversioned
	{
		/// <summary>next element in list</summary>
		public Db4objects.Db4o.Foundation.List4 _next;

		/// <summary>carried object</summary>
		public object _element;

		public List4()
		{
		}

		public List4(object element)
		{
			_element = element;
		}

		public List4(Db4objects.Db4o.Foundation.List4 next, object element)
		{
			_next = next;
			_element = element;
		}
	}
}
