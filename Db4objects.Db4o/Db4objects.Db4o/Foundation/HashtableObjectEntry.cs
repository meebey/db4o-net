namespace Db4objects.Db4o.Foundation
{
	internal class HashtableObjectEntry : Db4objects.Db4o.Foundation.HashtableIntEntry
	{
		internal object i_objectKey;

		internal HashtableObjectEntry(int a_hash, object a_key, object a_object) : base(a_hash
			, a_object)
		{
			i_objectKey = a_key;
		}

		internal HashtableObjectEntry(object a_key, object a_object) : base(a_key.GetHashCode
			(), a_object)
		{
			i_objectKey = a_key;
		}

		protected HashtableObjectEntry() : base()
		{
		}

		public override void AcceptKeyVisitor(Db4objects.Db4o.Foundation.IVisitor4 visitor
			)
		{
			visitor.Visit(i_objectKey);
		}

		public override object DeepClone(object obj)
		{
			return DeepCloneInternal(new Db4objects.Db4o.Foundation.HashtableObjectEntry(), obj
				);
		}

		protected override Db4objects.Db4o.Foundation.HashtableIntEntry DeepCloneInternal
			(Db4objects.Db4o.Foundation.HashtableIntEntry entry, object obj)
		{
			((Db4objects.Db4o.Foundation.HashtableObjectEntry)entry).i_objectKey = i_objectKey;
			return base.DeepCloneInternal(entry, obj);
		}

		public virtual bool HasKey(object key)
		{
			return i_objectKey.Equals(key);
		}

		public override bool SameKeyAs(Db4objects.Db4o.Foundation.HashtableIntEntry other
			)
		{
			return other is Db4objects.Db4o.Foundation.HashtableObjectEntry ? HasKey(((Db4objects.Db4o.Foundation.HashtableObjectEntry
				)other).i_objectKey) : false;
		}
	}
}
