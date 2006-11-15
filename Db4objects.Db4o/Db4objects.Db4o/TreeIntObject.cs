namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class TreeIntObject : Db4objects.Db4o.TreeInt
	{
		public object _object;

		public TreeIntObject(int a_key) : base(a_key)
		{
		}

		public TreeIntObject(int a_key, object a_object) : base(a_key)
		{
			_object = a_object;
		}

		public override object ShallowClone()
		{
			return ShallowCloneInternal(new Db4objects.Db4o.TreeIntObject(_key));
		}

		protected override Db4objects.Db4o.Foundation.Tree ShallowCloneInternal(Db4objects.Db4o.Foundation.Tree
			 tree)
		{
			Db4objects.Db4o.TreeIntObject tio = (Db4objects.Db4o.TreeIntObject)base.ShallowCloneInternal
				(tree);
			tio._object = _object;
			return tio;
		}

		public virtual object GetObject()
		{
			return _object;
		}

		public virtual void SetObject(object obj)
		{
			_object = obj;
		}

		public override object Read(Db4objects.Db4o.YapReader a_bytes)
		{
			int key = a_bytes.ReadInt();
			object obj = null;
			if (_object is Db4objects.Db4o.TreeInt)
			{
				obj = new Db4objects.Db4o.TreeReader(a_bytes, (Db4objects.Db4o.IReadable)_object)
					.Read();
			}
			else
			{
				obj = ((Db4objects.Db4o.IReadable)_object).Read(a_bytes);
			}
			return new Db4objects.Db4o.TreeIntObject(key, obj);
		}

		public override void Write(Db4objects.Db4o.YapReader a_writer)
		{
			a_writer.WriteInt(_key);
			if (_object == null)
			{
				a_writer.WriteInt(0);
			}
			else
			{
				if (_object is Db4objects.Db4o.TreeInt)
				{
					Db4objects.Db4o.TreeInt.Write(a_writer, (Db4objects.Db4o.TreeInt)_object);
				}
				else
				{
					((Db4objects.Db4o.IReadWriteable)_object).Write(a_writer);
				}
			}
		}

		public override int OwnLength()
		{
			if (_object == null)
			{
				return Db4objects.Db4o.YapConst.INT_LENGTH * 2;
			}
			return Db4objects.Db4o.YapConst.INT_LENGTH + ((Db4objects.Db4o.IReadable)_object)
				.ByteCount();
		}

		internal override bool VariableLength()
		{
			return true;
		}
	}
}
