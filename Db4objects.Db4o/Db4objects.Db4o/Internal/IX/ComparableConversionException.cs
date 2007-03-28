namespace Db4objects.Db4o.Internal.IX
{
	[System.Serializable]
	public class ComparableConversionException : System.Exception
	{
		private object _unconverted;

		public ComparableConversionException(object unconverted) : this(unconverted, null
			, null)
		{
		}

		public ComparableConversionException(object unconverted, System.Exception cause) : 
			this(unconverted, null, cause)
		{
		}

		public ComparableConversionException(object unconverted, string msg, System.Exception
			 cause) : base(msg, cause)
		{
			_unconverted = unconverted;
		}

		public ComparableConversionException(object unconverted, string msg) : this(unconverted
			, msg, null)
		{
		}

		public virtual object Unconverted()
		{
			return _unconverted;
		}
	}
}
