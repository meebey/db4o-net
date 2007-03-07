namespace Db4objects.Db4o.Foundation
{
	/// <summary>yes/no/dontknow data type</summary>
	/// <exclude></exclude>
	[System.Serializable]
	public sealed class TernaryBool
	{
		private const int NO_ID = -1;

		private const int YES_ID = 1;

		private const int UNSPECIFIED_ID = 0;

		public static readonly Db4objects.Db4o.Foundation.TernaryBool NO = new Db4objects.Db4o.Foundation.TernaryBool
			(NO_ID);

		public static readonly Db4objects.Db4o.Foundation.TernaryBool YES = new Db4objects.Db4o.Foundation.TernaryBool
			(YES_ID);

		public static readonly Db4objects.Db4o.Foundation.TernaryBool UNSPECIFIED = new Db4objects.Db4o.Foundation.TernaryBool
			(UNSPECIFIED_ID);

		private readonly int _value;

		private TernaryBool(int value)
		{
			_value = value;
		}

		public bool BooleanValue(bool defaultValue)
		{
			switch (_value)
			{
				case NO_ID:
				{
					return false;
				}

				case YES_ID:
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

		public bool Unspecified()
		{
			return this == UNSPECIFIED;
		}

		public bool DefiniteYes()
		{
			return this == YES;
		}

		public bool DefiniteNo()
		{
			return this == NO;
		}

		public static Db4objects.Db4o.Foundation.TernaryBool ForBoolean(bool value)
		{
			return (value ? YES : NO);
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
			Db4objects.Db4o.Foundation.TernaryBool tb = (Db4objects.Db4o.Foundation.TernaryBool
				)obj;
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
				case NO_ID:
				{
					return NO;
				}

				case YES_ID:
				{
					return YES;
				}

				default:
				{
					return UNSPECIFIED;
					break;
				}
			}
		}
	}
}
