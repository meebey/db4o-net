namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class KeySpec
	{
		private readonly object _defaultValue;

		public KeySpec(byte defaultValue)
		{
			_defaultValue = defaultValue;
		}

		public KeySpec(int defaultValue)
		{
			_defaultValue = defaultValue;
		}

		public KeySpec(bool defaultValue)
		{
			_defaultValue = defaultValue;
		}

		public KeySpec(object defaultValue)
		{
			_defaultValue = defaultValue;
		}

		public virtual object DefaultValue()
		{
			return _defaultValue;
		}
	}
}
