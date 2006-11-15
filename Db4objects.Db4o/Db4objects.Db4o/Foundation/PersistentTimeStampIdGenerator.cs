namespace Db4objects.Db4o.Foundation
{
	public class PersistentTimeStampIdGenerator
	{
		private bool _dirty;

		private readonly Db4objects.Db4o.Foundation.TimeStampIdGenerator _generator = new 
			Db4objects.Db4o.Foundation.TimeStampIdGenerator();

		public virtual long Next()
		{
			_dirty = true;
			return _generator.Generate();
		}

		public virtual void SetMinimumNext(long val)
		{
			if (_generator.SetMinimumNext(val))
			{
				_dirty = true;
			}
		}

		public virtual long LastTimeStampId()
		{
			return _generator.Last();
		}

		public virtual bool IsDirty()
		{
			return _dirty;
		}

		public virtual void SetClean()
		{
			_dirty = false;
		}
	}
}
