namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <summary>QQuery is the users hook on our graph.</summary>
	/// <remarks>
	/// QQuery is the users hook on our graph.
	/// A QQuery is defined by it's constraints.
	/// </remarks>
	/// <exclude></exclude>
	public class QQuery : Db4objects.Db4o.Internal.Query.Processor.QQueryBase, Db4objects.Db4o.Query.IQuery
	{
		public QQuery(Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Internal.Query.Processor.QQuery
			 a_parent, string a_field) : base(a_trans, a_parent, a_field)
		{
		}

		public QQuery() : base()
		{
		}
	}
}
