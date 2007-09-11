using System;
using Db4oAdmin.Core;
using Mono.Cecil;

namespace Db4oAdmin.Tests.Core
{
	public class AcceptNoneFilter : ITypeFilter
	{
		public bool Accept(TypeDefinition typeDef)
		{
			return false;
		}
	}

    class ByFilterTestCase : SingleResourceTestCase
	{
		protected override string ResourceName
		{
			get { return "ByFilterInstrumentationSubject"; }
		}

		protected override string CommandLine
		{
			get
			{
				return "-by-filter:Db4oAdmin.Tests.Core.AcceptNoneFilter,Db4oAdmin.Tests"
					+ " -instrumentation:Db4oAdmin.Tests.Core.TraceInstrumentation,Db4oAdmin.Tests";
			}
		}
	}
}
