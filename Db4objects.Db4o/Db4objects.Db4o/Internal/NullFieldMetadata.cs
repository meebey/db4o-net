namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class NullFieldMetadata : Db4objects.Db4o.Internal.FieldMetadata
	{
		public NullFieldMetadata() : base(null)
		{
		}

		public override Db4objects.Db4o.Internal.IComparable4 PrepareComparison(object obj
			)
		{
			return Db4objects.Db4o.Internal.Null.INSTANCE;
		}

		internal override object Read(Db4objects.Db4o.Internal.Marshall.MarshallerFamily 
			mf, Db4objects.Db4o.Internal.StatefulBuffer a_bytes)
		{
			return null;
		}

		public override object ReadQuery(Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer a_reader)
		{
			return null;
		}
	}
}
