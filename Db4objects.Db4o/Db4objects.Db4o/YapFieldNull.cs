namespace Db4objects.Db4o
{
	internal class YapFieldNull : Db4objects.Db4o.YapField
	{
		public YapFieldNull() : base(null)
		{
		}

		internal override Db4objects.Db4o.IYapComparable PrepareComparison(object obj)
		{
			return Db4objects.Db4o.Null.INSTANCE;
		}

		internal override object Read(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf
			, Db4objects.Db4o.YapWriter a_bytes)
		{
			return null;
		}

		internal override object ReadQuery(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapReader a_reader)
		{
			return null;
		}
	}
}
