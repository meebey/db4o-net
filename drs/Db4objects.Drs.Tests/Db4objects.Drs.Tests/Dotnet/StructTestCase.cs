/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;
using Db4objects.Drs.Tests.Dotnet;

namespace Db4objects.Drs.Tests.Dotnet
{
	internal class Container
	{
		public Value value;

		public Container(Value value)
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

	public class StructTestCase : DrsTestCase
	{
		internal Container template = new Container(new Value(42));

		public virtual void Test()
		{
			StoreToProviderA();
			ReplicateAllToProviderB();
		}

		internal virtual void StoreToProviderA()
		{
			ITestableReplicationProviderInside provider = A().Provider();
			provider.StoreNew(template);
			provider.Commit();
			EnsureContent(template, provider);
		}

		internal virtual void ReplicateAllToProviderB()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureContent(template, B().Provider());
		}

		private void EnsureContent(Container container, ITestableReplicationProviderInside
			 provider)
		{
			IObjectSet result = provider.GetStoredObjects(container.GetType());
			Assert.AreEqual(1, result.Count);
			Container c = Next(result);
			Assert.AreEqual(template.value, c.value);
		}

		private Container Next(IObjectSet result)
		{
			IEnumerator iterator = result.GetEnumerator();
			if (iterator.MoveNext())
			{
				return (Container)iterator.Current;
			}
			return null;
		}
	}
}
