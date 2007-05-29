/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.Config
{
	/// <summary>
	/// Defines a scope of applicability of a config setting.<br /><br />
	/// Some of the configuration settings can be either: <br /><br />
	/// - enabled globally; <br />
	/// - enabled individually for a specified class; <br />
	/// - disabled.<br /><br />
	/// </summary>
	/// <seealso cref="IConfiguration.GenerateUUIDs">IConfiguration.GenerateUUIDs</seealso>
	/// <seealso cref="IConfiguration.GenerateVersionNumbers">IConfiguration.GenerateVersionNumbers
	/// 	</seealso>
	[System.Serializable]
	public sealed class ConfigScope
	{
		public const int DISABLED_ID = -1;

		public const int INDIVIDUALLY_ID = 1;

		public const int GLOBALLY_ID = int.MaxValue;

		private static readonly string DISABLED_NAME = "disabled";

		private static readonly string INDIVIDUALLY_NAME = "individually";

		private static readonly string GLOBALLY_NAME = "globally";

		/// <summary>Marks a configuration feature as globally disabled.</summary>
		/// <remarks>Marks a configuration feature as globally disabled.</remarks>
		public static readonly Db4objects.Db4o.Config.ConfigScope DISABLED = new Db4objects.Db4o.Config.ConfigScope
			(DISABLED_ID, DISABLED_NAME);

		/// <summary>Marks a configuration feature as individually configurable.</summary>
		/// <remarks>Marks a configuration feature as individually configurable.</remarks>
		public static readonly Db4objects.Db4o.Config.ConfigScope INDIVIDUALLY = new Db4objects.Db4o.Config.ConfigScope
			(INDIVIDUALLY_ID, INDIVIDUALLY_NAME);

		/// <summary>Marks a configuration feature as globally enabled.</summary>
		/// <remarks>Marks a configuration feature as globally enabled.</remarks>
		public static readonly Db4objects.Db4o.Config.ConfigScope GLOBALLY = new Db4objects.Db4o.Config.ConfigScope
			(GLOBALLY_ID, GLOBALLY_NAME);

		private readonly int _value;

		private readonly string _name;

		private ConfigScope(int value, string name)
		{
			_value = value;
			_name = name;
		}

		/// <summary>
		/// Checks if the current configuration scope is globally
		/// enabled or disabled.
		/// </summary>
		/// <remarks>
		/// Checks if the current configuration scope is globally
		/// enabled or disabled.
		/// </remarks>
		/// <param name="defaultValue">- default result</param>
		/// <returns>
		/// false if disabled, true if globally enabled, default
		/// value otherwise
		/// </returns>
		public bool ApplyConfig(bool defaultValue)
		{
			switch (_value)
			{
				case DISABLED_ID:
				{
					return false;
				}

				case GLOBALLY_ID:
				{
					return true;
				}

				default:
				{
					return defaultValue;
					break;
				}
			}
		}

		[System.ObsoleteAttribute]
		public static Db4objects.Db4o.Config.ConfigScope ForID(int id)
		{
			switch (id)
			{
				case DISABLED_ID:
				{
					return DISABLED;
				}

				case INDIVIDUALLY_ID:
				{
					return INDIVIDUALLY;
				}
			}
			return GLOBALLY;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			Db4objects.Db4o.Config.ConfigScope tb = (Db4objects.Db4o.Config.ConfigScope)obj;
			return _value == tb._value;
		}

		public override int GetHashCode()
		{
			return _value;
		}

		private object ReadResolve()
		{
			switch (_value)
			{
				case DISABLED_ID:
				{
					return DISABLED;
				}

				case INDIVIDUALLY_ID:
				{
					return INDIVIDUALLY;
				}

				default:
				{
					return GLOBALLY;
					break;
				}
			}
		}

		public override string ToString()
		{
			return _name;
		}
	}
}
