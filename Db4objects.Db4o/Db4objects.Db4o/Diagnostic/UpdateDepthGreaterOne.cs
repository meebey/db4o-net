namespace Db4objects.Db4o.Diagnostic
{
	/// <summary>Diagnostic, if update depth greater than 1.</summary>
	/// <remarks>Diagnostic, if update depth greater than 1.</remarks>
	public class UpdateDepthGreaterOne : Db4objects.Db4o.Diagnostic.DiagnosticBase
	{
		private readonly int _depth;

		public UpdateDepthGreaterOne(int depth)
		{
			_depth = depth;
		}

		public override object Reason()
		{
			return "Db4o.configure().updateDepth(" + _depth + ")";
		}

		public override string Problem()
		{
			return "A global update depth greater than 1 is not recommended";
		}

		public override string Solution()
		{
			return "Increasing the global update depth to a value greater than 1 is only recommended for"
				 + " testing, not for production use. If individual deep updates are needed, consider using"
				 + " ExtObjectContainer#set(object, depth) and make sure to profile the performance of each call.";
		}
	}
}
