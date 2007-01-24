namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Collection4Iterator : Db4objects.Db4o.Foundation.Iterator4Impl
	{
		private readonly Db4objects.Db4o.Foundation.Collection4 _collection;

		private readonly int _initialVersion;

		public Collection4Iterator(Db4objects.Db4o.Foundation.Collection4 collection, Db4objects.Db4o.Foundation.List4
			 first) : base(first)
		{
			_collection = collection;
			_initialVersion = CurrentVersion();
		}

		public override bool MoveNext()
		{
			Validate();
			return base.MoveNext();
		}

		public override object Current
		{
			get
			{
				Validate();
				return base.Current;
			}
		}

		private void Validate()
		{
			if (_initialVersion != CurrentVersion())
			{
				throw new Db4objects.Db4o.Foundation.InvalidIteratorException();
			}
		}

		private int CurrentVersion()
		{
			return _collection.Version();
		}
	}
}
