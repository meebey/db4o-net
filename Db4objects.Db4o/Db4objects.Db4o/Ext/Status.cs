namespace Db4objects.Db4o.Ext
{
	/// <summary>Static constants to describe the status of objects.</summary>
	/// <remarks>Static constants to describe the status of objects.</remarks>
	public class Status
	{
		public const double UNUSED = -1.0;

		public const double AVAILABLE = -2.0;

		public const double QUEUED = -3.0;

		public const double COMPLETED = -4.0;

		public const double PROCESSING = -5.0;

		public const double ERROR = -99.0;
	}
}
