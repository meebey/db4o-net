namespace Db4objects.Db4o.Config
{
	/// <summary>
	/// Wildcard Alias functionality to create aliases for packages,
	/// namespaces or multiple similar named classes.
	/// </summary>
	/// <remarks>
	/// Wildcard Alias functionality to create aliases for packages,
	/// namespaces or multiple similar named classes. One single '*'
	/// wildcard character is supported in the names.
	/// <br /><br />See
	/// <see cref="Db4objects.Db4o.Config.IAlias">Db4objects.Db4o.Config.IAlias</see>
	/// for concrete examples.
	/// </remarks>
	public class WildcardAlias : Db4objects.Db4o.Config.IAlias
	{
		private readonly Db4objects.Db4o.Config.WildcardAlias.WildcardPattern _storedPattern;

		private readonly Db4objects.Db4o.Config.WildcardAlias.WildcardPattern _runtimePattern;

		public WildcardAlias(string storedPattern, string runtimePattern)
		{
			if (null == storedPattern)
			{
				throw new System.ArgumentNullException("storedPattern");
			}
			if (null == runtimePattern)
			{
				throw new System.ArgumentNullException("runtimePattern");
			}
			_storedPattern = new Db4objects.Db4o.Config.WildcardAlias.WildcardPattern(storedPattern
				);
			_runtimePattern = new Db4objects.Db4o.Config.WildcardAlias.WildcardPattern(runtimePattern
				);
		}

		/// <summary>resolving is done through simple pattern matching</summary>
		public virtual string Resolve(string runtimeType)
		{
			string match = _runtimePattern.Matches(runtimeType);
			return match != null ? _storedPattern.Inject(match) : null;
		}

		internal class WildcardPattern
		{
			private string _head;

			private string _tail;

			public WildcardPattern(string pattern)
			{
				string[] parts = Split(pattern);
				_head = parts[0];
				_tail = parts[1];
			}

			public virtual string Inject(string s)
			{
				return _head + s + _tail;
			}

			public virtual string Matches(string s)
			{
				if (!s.StartsWith(_head) || !s.EndsWith(_tail))
				{
					return null;
				}
				return Sharpen.Runtime.Substring(s, _head.Length, s.Length - _tail.Length);
			}

			private void InvalidPattern()
			{
				throw new System.ArgumentException("only one '*' character");
			}

			internal virtual string[] Split(string pattern)
			{
				int index = pattern.IndexOf('*');
				if (-1 == index || index != pattern.LastIndexOf('*'))
				{
					InvalidPattern();
				}
				return new string[] { Sharpen.Runtime.Substring(pattern, 0, index), Sharpen.Runtime.Substring
					(pattern, index + 1) };
			}
		}
	}
}
