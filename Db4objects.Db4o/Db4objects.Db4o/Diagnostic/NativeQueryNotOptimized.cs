namespace Db4objects.Db4o.Diagnostic
{
	/// <summary>Diagnostic, if Native Query can not be run optimized.</summary>
	/// <remarks>Diagnostic, if Native Query can not be run optimized.</remarks>
	public class NativeQueryNotOptimized : Db4objects.Db4o.Diagnostic.DiagnosticBase
	{
		private readonly Db4objects.Db4o.Query.Predicate _predicate;

		public NativeQueryNotOptimized(Db4objects.Db4o.Query.Predicate predicate)
		{
			_predicate = predicate;
		}

		public override object Reason()
		{
			return _predicate;
		}

		public override string Problem()
		{
			return "Native Query Predicate could not be run optimized";
		}

		public override string Solution()
		{
			return "This Native Query was run by instantiating all objects of the candidate class. "
				 + "Consider simplifying the expression in the Native Query method. If you feel that "
				 + "the Native Query processor should understand your code better, you are invited to "
				 + "post yout query code to db4o forums at http://developer.db4o.com/forums";
		}
	}
}
