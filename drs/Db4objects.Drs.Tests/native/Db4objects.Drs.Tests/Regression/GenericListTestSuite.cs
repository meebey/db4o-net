using System;
using System.Collections;
using System.Collections.Generic;
using Db4oUnit.Fixtures;

namespace Db4objects.Drs.Tests.Regression
{
	class GenericListTestSuite : FixtureBasedTestSuite
	{
		public override Type[] TestUnits()
		{
			return new Type[]
				{
					typeof(GenericListTestCase)
				};
		}

		public override IFixtureProvider[] FixtureProviders()
		{	
			return new IFixtureProvider[]
				{
					new SubjectFixtureProvider(GenerateLists()),
				};
		}

		private IEnumerable GenerateLists()
		{
			IEnumerable<string> tenStrings = GenerateStrings(10);
			yield return new List<int>();
			yield return new List<string>(tenStrings);
			yield return new ArrayList();
			yield return new ArrayList(new List<string>(tenStrings));
			yield return new LinkedList<string>(tenStrings);
			yield return new LinkedList<string>();
			yield return new LinkedList<int>(Range(0, 10));
		}

		private static IEnumerable<int> Range(int begin, int end)
		{
			for (int i=begin; i<end; ++i)
			{
				yield return i;
			}
		}

		private static IEnumerable<string> GenerateStrings(int count)
		{
			if (count < 0) throw new ArgumentOutOfRangeException("count");
			for (int i = 0; i < count; i++)
			{
				yield return "string " + i;
			}
		}
	}
}