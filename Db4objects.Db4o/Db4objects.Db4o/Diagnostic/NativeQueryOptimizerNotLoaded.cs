/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Diagnostic;

namespace Db4objects.Db4o.Diagnostic
{
	public class NativeQueryOptimizerNotLoaded : DiagnosticBase
	{
		private int _reason;

		public const int NQ_NOT_PRESENT = 1;

		public const int NQ_CONSTRUCTION_FAILED = 2;

		public NativeQueryOptimizerNotLoaded(int reason)
		{
			_reason = reason;
		}

		public override string Problem()
		{
			return "Native Query Optimizer could not be loaded";
		}

		public override object Reason()
		{
			switch (_reason)
			{
				case NQ_NOT_PRESENT:
				{
					return "Native query not present.";
				}

				case NQ_CONSTRUCTION_FAILED:
				{
					return "Native query couldn't be instantiated.";
				}

				default:
				{
					return "Reason not specified.";
					break;
				}
			}
		}

		public override string Solution()
		{
			return "If you to have the native queries optimized, please check that the native query jar is present in the class-path.";
		}
	}
}
