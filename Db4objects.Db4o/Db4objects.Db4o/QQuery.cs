namespace Db4objects.Db4o
{
	/// <summary>QQuery is the users hook on our graph.</summary>
	/// <remarks>
	/// QQuery is the users hook on our graph.
	/// A QQuery is defined by it's constraints.
	/// </remarks>
	/// <exclude></exclude>
	public class QQuery : Db4objects.Db4o.QQueryBase, Db4objects.Db4o.Query.IQuery
	{
		internal QQuery(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.QQuery a_parent
			, string a_field) : base(a_trans, a_parent, a_field)
		{
		}

		public QQuery()
		{
		}
	}
}
