namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public abstract class Config4Abstract
	{
		protected Db4objects.Db4o.Foundation.KeySpecHashtable4 _config;

		private static readonly Db4objects.Db4o.Foundation.KeySpec CASCADE_ON_ACTIVATE = 
			new Db4objects.Db4o.Foundation.KeySpec(Db4objects.Db4o.YapConst.DEFAULT);

		private static readonly Db4objects.Db4o.Foundation.KeySpec CASCADE_ON_DELETE = new 
			Db4objects.Db4o.Foundation.KeySpec(Db4objects.Db4o.YapConst.DEFAULT);

		private static readonly Db4objects.Db4o.Foundation.KeySpec CASCADE_ON_UPDATE = new 
			Db4objects.Db4o.Foundation.KeySpec(Db4objects.Db4o.YapConst.DEFAULT);

		private static readonly Db4objects.Db4o.Foundation.KeySpec NAME = new Db4objects.Db4o.Foundation.KeySpec
			(null);

		public Config4Abstract() : this(new Db4objects.Db4o.Foundation.KeySpecHashtable4(
			10))
		{
		}

		protected Config4Abstract(Db4objects.Db4o.Foundation.KeySpecHashtable4 config)
		{
			_config = (Db4objects.Db4o.Foundation.KeySpecHashtable4)config.DeepClone(this);
		}

		public virtual void CascadeOnActivate(bool flag)
		{
			PutThreeValued(CASCADE_ON_ACTIVATE, flag);
		}

		public virtual void CascadeOnDelete(bool flag)
		{
			PutThreeValued(CASCADE_ON_DELETE, flag);
		}

		public virtual void CascadeOnUpdate(bool flag)
		{
			PutThreeValued(CASCADE_ON_UPDATE, flag);
		}

		protected virtual void PutThreeValued(Db4objects.Db4o.Foundation.KeySpec spec, bool
			 flag)
		{
			_config.Put(spec, flag ? Db4objects.Db4o.YapConst.YES : Db4objects.Db4o.YapConst.
				NO);
		}

		public virtual int CascadeOnActivate()
		{
			return Cascade(CASCADE_ON_ACTIVATE);
		}

		public virtual int CascadeOnDelete()
		{
			return Cascade(CASCADE_ON_DELETE);
		}

		public virtual int CascadeOnUpdate()
		{
			return Cascade(CASCADE_ON_UPDATE);
		}

		private int Cascade(Db4objects.Db4o.Foundation.KeySpec spec)
		{
			return _config.GetAsInt(spec);
		}

		internal abstract string ClassName();

		/// <summary>Will raise an exception if argument class doesn't match this class - violates equals() contract in favor of failing fast.
		/// 	</summary>
		/// <remarks>Will raise an exception if argument class doesn't match this class - violates equals() contract in favor of failing fast.
		/// 	</remarks>
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (null == obj)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				Db4objects.Db4o.Inside.Exceptions4.ShouldNeverHappen();
			}
			return GetName().Equals(((Db4objects.Db4o.Config4Abstract)obj).GetName());
		}

		public override int GetHashCode()
		{
			return GetName().GetHashCode();
		}

		public virtual string GetName()
		{
			return _config.GetAsString(NAME);
		}

		protected virtual void SetName(string name)
		{
			_config.Put(NAME, name);
		}
	}
}
