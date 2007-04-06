using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class Config4Abstract
	{
		protected KeySpecHashtable4 _config;

		private static readonly KeySpec CASCADE_ON_ACTIVATE = new KeySpec(TernaryBool.UNSPECIFIED
			);

		private static readonly KeySpec CASCADE_ON_DELETE = new KeySpec(TernaryBool.UNSPECIFIED
			);

		private static readonly KeySpec CASCADE_ON_UPDATE = new KeySpec(TernaryBool.UNSPECIFIED
			);

		private static readonly KeySpec NAME = new KeySpec(null);

		public Config4Abstract() : this(new KeySpecHashtable4(10))
		{
		}

		protected Config4Abstract(KeySpecHashtable4 config)
		{
			_config = (KeySpecHashtable4)config.DeepClone(this);
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

		protected virtual void PutThreeValued(KeySpec spec, bool flag)
		{
			_config.Put(spec, TernaryBool.ForBoolean(flag));
		}

		protected virtual void PutThreeValuedInt(KeySpec spec, bool flag)
		{
			_config.Put(spec, flag ? 1 : -1);
		}

		public virtual TernaryBool CascadeOnActivate()
		{
			return Cascade(CASCADE_ON_ACTIVATE);
		}

		public virtual TernaryBool CascadeOnDelete()
		{
			return Cascade(CASCADE_ON_DELETE);
		}

		public virtual TernaryBool CascadeOnUpdate()
		{
			return Cascade(CASCADE_ON_UPDATE);
		}

		private TernaryBool Cascade(KeySpec spec)
		{
			return _config.GetAsTernaryBool(spec);
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
				Exceptions4.ShouldNeverHappen();
			}
			return GetName().Equals(((Db4objects.Db4o.Internal.Config4Abstract)obj).GetName()
				);
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
