namespace Db4objects.Db4o.Inside.Mapping
{
	/// <exclude></exclude>
	public class MappedIDPair
	{
		private int _orig;

		private int _mapped;

		private bool _seen;

		public MappedIDPair(int orig, int mapped) : this(orig, mapped, false)
		{
		}

		public MappedIDPair(int orig, int mapped, bool seen)
		{
			_orig = orig;
			_mapped = mapped;
			_seen = seen;
		}

		public virtual int Orig()
		{
			return _orig;
		}

		public virtual int Mapped()
		{
			return _mapped;
		}

		public virtual bool Seen()
		{
			return _seen;
		}

		public virtual void Seen(bool seen)
		{
			_seen = seen;
		}

		public override string ToString()
		{
			return _orig + "->" + _mapped + "(" + _seen + ")";
		}
	}
}
