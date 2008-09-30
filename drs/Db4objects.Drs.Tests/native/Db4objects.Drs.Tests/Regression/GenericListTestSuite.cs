/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Foundation;
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
			yield return new List<Container>(GenerateContainers(tenStrings));
		}

		private IEnumerable<Container> GenerateContainers(IEnumerable<string> names)
		{
			foreach (string name in names)
			{
				yield return new Container(name, null);
			}
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