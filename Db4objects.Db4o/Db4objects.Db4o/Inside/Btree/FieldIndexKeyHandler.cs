namespace Db4objects.Db4o.Inside.Btree
{
	/// <exclude></exclude>
	public class FieldIndexKeyHandler : Db4objects.Db4o.Inside.IX.IIndexable4
	{
		private readonly Db4objects.Db4o.Inside.IX.IIndexable4 _valueHandler;

		private readonly Db4objects.Db4o.YInt _parentIdHandler;

		public FieldIndexKeyHandler(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.Inside.IX.IIndexable4
			 delegate_)
		{
			_parentIdHandler = new Db4objects.Db4o.IDHandler(stream);
			_valueHandler = delegate_;
		}

		public virtual object ComparableObject(Db4objects.Db4o.Transaction trans, object 
			indexEntry)
		{
			throw new System.NotImplementedException();
		}

		public virtual int LinkLength()
		{
			return _valueHandler.LinkLength() + Db4objects.Db4o.YapConst.INT_LENGTH;
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.YapReader a_reader)
		{
			int parentID = ReadParentID(a_reader);
			object objPart = _valueHandler.ReadIndexEntry(a_reader);
			if (parentID < 0)
			{
				objPart = null;
				parentID = -parentID;
			}
			return new Db4objects.Db4o.Inside.Btree.FieldIndexKey(parentID, objPart);
		}

		private int ReadParentID(Db4objects.Db4o.YapReader a_reader)
		{
			return ((int)_parentIdHandler.ReadIndexEntry(a_reader));
		}

		public virtual void WriteIndexEntry(Db4objects.Db4o.YapReader writer, object obj)
		{
			Db4objects.Db4o.Inside.Btree.FieldIndexKey composite = (Db4objects.Db4o.Inside.Btree.FieldIndexKey
				)obj;
			int parentID = composite.ParentID();
			object value = composite.Value();
			if (value == null)
			{
				parentID = -parentID;
			}
			_parentIdHandler.Write(parentID, writer);
			_valueHandler.WriteIndexEntry(writer, composite.Value());
		}

		public virtual Db4objects.Db4o.Inside.IX.IIndexable4 ValueHandler()
		{
			return _valueHandler;
		}

		public virtual Db4objects.Db4o.IYapComparable PrepareComparison(object obj)
		{
			Db4objects.Db4o.Inside.Btree.FieldIndexKey composite = (Db4objects.Db4o.Inside.Btree.FieldIndexKey
				)obj;
			_valueHandler.PrepareComparison(composite.Value());
			_parentIdHandler.PrepareComparison(composite.ParentID());
			return this;
		}

		public virtual int CompareTo(object obj)
		{
			if (null == obj)
			{
				throw new System.ArgumentNullException();
			}
			Db4objects.Db4o.Inside.Btree.FieldIndexKey composite = (Db4objects.Db4o.Inside.Btree.FieldIndexKey
				)obj;
			int delegateResult = _valueHandler.CompareTo(composite.Value());
			if (delegateResult != 0)
			{
				return delegateResult;
			}
			return _parentIdHandler.CompareTo(composite.ParentID());
		}

		public virtual bool IsEqual(object obj)
		{
			throw new System.NotImplementedException();
		}

		public virtual bool IsGreater(object obj)
		{
			throw new System.NotImplementedException();
		}

		public virtual bool IsSmaller(object obj)
		{
			throw new System.NotImplementedException();
		}

		public virtual object Current()
		{
			return new Db4objects.Db4o.Inside.Btree.FieldIndexKey(_parentIdHandler.CurrentInt
				(), _valueHandler.Current());
		}

		public virtual void DefragIndexEntry(Db4objects.Db4o.ReaderPair readers)
		{
			_parentIdHandler.DefragIndexEntry(readers);
			_valueHandler.DefragIndexEntry(readers);
		}
	}
}
