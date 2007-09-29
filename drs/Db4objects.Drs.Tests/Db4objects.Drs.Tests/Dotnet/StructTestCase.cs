/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

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
namespace Db4objects.Drs.Tests.Dotnet
{
	internal class Container
	{
		public Db4objects.Drs.Tests.Dotnet.Value value;

		public Container(Db4objects.Drs.Tests.Dotnet.Value value)
		{
			this.value = value;
		}
	}

	internal struct Value
	{
		public int value;

		public Value(int value)
		{
			this.value = value;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Db4objects.Drs.Tests.Dotnet.Value))
			{
				return false;
			}
			Db4objects.Drs.Tests.Dotnet.Value other = (Db4objects.Drs.Tests.Dotnet.Value)obj;
			return other.value == value;
		}
	}

	public class StructTestCase : Db4objects.Drs.Tests.DrsTestCase
	{
		internal Db4objects.Drs.Tests.Dotnet.Container template = new Db4objects.Drs.Tests.Dotnet.Container
			(new Db4objects.Drs.Tests.Dotnet.Value(42));

		public virtual void Test()
		{
			StoreToProviderA();
			ReplicateAllToProviderB();
		}

		internal virtual void StoreToProviderA()
		{
			Db4objects.Drs.Inside.ITestableReplicationProviderInside provider = A().Provider(
				);
			provider.StoreNew(template);
			provider.Commit();
			EnsureContent(template, provider);
		}

		internal virtual void ReplicateAllToProviderB()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureContent(template, B().Provider());
		}

		private void EnsureContent(Db4objects.Drs.Tests.Dotnet.Container container, Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 provider)
		{
			Db4objects.Db4o.IObjectSet result = provider.GetStoredObjects(container.GetType()
				);
			Db4oUnit.Assert.AreEqual(1, result.Count);
			Db4objects.Drs.Tests.Dotnet.Container c = Next(result);
			Db4oUnit.Assert.AreEqual(template.value, c.value);
		}

		private Db4objects.Drs.Tests.Dotnet.Container Next(Db4objects.Db4o.IObjectSet result
			)
		{
			System.Collections.IEnumerator iterator = result.GetEnumerator();
			if (iterator.MoveNext())
			{
				return (Db4objects.Drs.Tests.Dotnet.Container)iterator.Current;
			}
			return null;
		}
	}
}
