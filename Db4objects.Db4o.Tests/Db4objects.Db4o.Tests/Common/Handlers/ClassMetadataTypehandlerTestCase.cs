/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class ClassMetadataTypehandlerTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ClassMetadataTypehandlerTestCase().RunSolo();
		}

		public class Item
		{
			public string _name;

			public Item(string name)
			{
				_name = name;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is ClassMetadataTypehandlerTestCase.Item))
				{
					return false;
				}
				ClassMetadataTypehandlerTestCase.Item other = (ClassMetadataTypehandlerTestCase.Item
					)obj;
				return _name == null ? other._name == null : _name.Equals(other._name);
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new ClassMetadataTypehandlerTestCase.Item("stored"));
		}

		public virtual void TestHandlerForClass()
		{
			Assert.IsNotNull(ClassMetadataHandler());
		}

		public virtual void TestReadWrite()
		{
			MockWriteContext writeContext = new MockWriteContext(Db());
			ClassMetadataTypehandlerTestCase.Item expected = new ClassMetadataTypehandlerTestCase.Item
				("mock");
			ClassMetadataHandler().Write(writeContext, expected);
			MockReadContext readContext = new MockReadContext(writeContext);
			ClassMetadataTypehandlerTestCase.Item actual = (ClassMetadataTypehandlerTestCase.Item
				)ClassMetadataHandler().Read(readContext);
			Assert.AreEqual(expected, actual);
		}

		public virtual void TestCompare()
		{
			ClassMetadataTypehandlerTestCase.Item item = (ClassMetadataTypehandlerTestCase.Item
				)RetrieveOnlyInstance(typeof(ClassMetadataTypehandlerTestCase.Item));
			int id = Stream().GetID(Trans(), item);
			IPreparedComparison preparedComparison = ClassMetadataHandler().PrepareComparison
				(id);
			Assert.AreEqual(0, preparedComparison.CompareTo(id));
		}

		private ITypeHandler4 ClassMetadataHandler()
		{
			return (ITypeHandler4)Stream().FieldHandlerForClass(ItemClass());
		}

		private IReflectClass ItemClass()
		{
			return Reflector().ForClass(typeof(ClassMetadataTypehandlerTestCase.Item));
		}
	}
}
