/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Fieldindex;

namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class StringFieldIndexTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		public class FieldIndexItem
		{
			public FieldIndexItem(string foo)
			{
				_foo = foo;
			}

			public string _foo;

			public virtual string GetFoo()
			{
				return _foo;
			}
		}

		public class ExpectedVisitor : IVisitor4
		{
			public string[] _values;

			public int _position;

			public ExpectedVisitor(int length)
			{
				_values = new string[length];
				_position = 0;
			}

			public virtual void Visit(object obj)
			{
				_values[_position++] = (string)obj;
			}

			public virtual string[] GetValues()
			{
				return _values;
			}
		}

		private static string[] _fooValues = new string[] { "Andrew", "Richard" };

		public static void Main(string[] args)
		{
			new StringFieldIndexTestCase().RunSolo();
		}

		protected override void Configure(IConfiguration config)
		{
			IndexField(config, typeof(StringFieldIndexTestCase.FieldIndexItem), "_foo");
		}

		//$NON-NLS-1$
		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			for (int i = 0; i < _fooValues.Length; i++)
			{
				StringFieldIndexTestCase.FieldIndexItem item = new StringFieldIndexTestCase.FieldIndexItem
					(_fooValues[i]);
				Store(item);
			}
		}

		public virtual void TestTraverseValues()
		{
			IStoredField field = YapField();
			StringFieldIndexTestCase.ExpectedVisitor visitor = new StringFieldIndexTestCase.ExpectedVisitor
				(2);
			field.TraverseValues(visitor);
			for (int i = 0; i < _fooValues.Length; i++)
			{
				Assert.AreEqual(_fooValues[i], visitor.GetValues()[i]);
			}
		}

		private IStoredField YapField()
		{
			return ClassMetadataFor(typeof(StringFieldIndexTestCase.FieldIndexItem)).FieldMetadataForName
				("_foo");
		}
	}
}
