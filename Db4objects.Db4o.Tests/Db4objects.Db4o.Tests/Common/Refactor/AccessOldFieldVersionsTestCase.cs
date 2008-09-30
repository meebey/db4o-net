/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Refactor;

namespace Db4objects.Db4o.Tests.Common.Refactor
{
	public class AccessOldFieldVersionsTestCase : ITestLifeCycle
	{
		private static readonly Type OrigType = typeof(int);

		private static readonly string FieldName = "_value";

		private const int OrigValue = 42;

		private readonly string DatabasePath = Path.GetTempFileName();

		public virtual void TestRetypedField()
		{
			Type targetClazz = typeof(AccessOldFieldVersionsTestCase.RetypedFieldData);
			RenameClass(ReflectPlatform.FullyQualifiedName(targetClazz));
			AssertOriginalField(targetClazz);
		}

		private void AssertOriginalField(Type targetClazz)
		{
			WithDatabase(new _IDatabaseAction_28(targetClazz));
		}

		private sealed class _IDatabaseAction_28 : AccessOldFieldVersionsTestCase.IDatabaseAction
		{
			public _IDatabaseAction_28(Type targetClazz)
			{
				this.targetClazz = targetClazz;
			}

			public void RunWith(IObjectContainer db)
			{
				IStoredClass storedClass = db.Ext().StoredClass(targetClazz);
				IStoredField storedField = storedClass.StoredField(AccessOldFieldVersionsTestCase
					.FieldName, AccessOldFieldVersionsTestCase.OrigType);
				IObjectSet result = db.Query(targetClazz);
				Assert.AreEqual(1, result.Count);
				object obj = result.Next();
				object value = storedField.Get(obj);
				Assert.AreEqual(AccessOldFieldVersionsTestCase.OrigValue, value);
			}

			private readonly Type targetClazz;
		}

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			DeleteFile();
			WithDatabase(new _IDatabaseAction_43());
		}

		private sealed class _IDatabaseAction_43 : AccessOldFieldVersionsTestCase.IDatabaseAction
		{
			public _IDatabaseAction_43()
			{
			}

			public void RunWith(IObjectContainer db)
			{
				db.Store(new AccessOldFieldVersionsTestCase.OriginalData(AccessOldFieldVersionsTestCase
					.OrigValue));
			}
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
			DeleteFile();
		}

		private void RenameClass(string targetName)
		{
			IConfiguration config = Db4oEmbedded.NewConfiguration();
			config.ObjectClass(typeof(AccessOldFieldVersionsTestCase.OriginalData)).Rename(targetName
				);
			WithDatabase(config, new _IDatabaseAction_57());
		}

		private sealed class _IDatabaseAction_57 : AccessOldFieldVersionsTestCase.IDatabaseAction
		{
			public _IDatabaseAction_57()
			{
			}

			public void RunWith(IObjectContainer db)
			{
			}
		}

		// do nothing
		private void DeleteFile()
		{
			File4.Delete(DatabasePath);
		}

		private void WithDatabase(AccessOldFieldVersionsTestCase.IDatabaseAction action)
		{
			WithDatabase(Db4oEmbedded.NewConfiguration(), action);
		}

		private void WithDatabase(IConfiguration config, AccessOldFieldVersionsTestCase.IDatabaseAction
			 action)
		{
			IObjectContainer db = Db4oEmbedded.OpenFile(config, DatabasePath);
			try
			{
				action.RunWith(db);
			}
			finally
			{
				db.Close();
			}
		}

		private interface IDatabaseAction
		{
			void RunWith(IObjectContainer db);
		}

		public class OriginalData
		{
			public int _value;

			public OriginalData(int value)
			{
				_value = value;
			}
		}

		public class RetypedFieldData
		{
			public string _value;

			public RetypedFieldData(string value)
			{
				_value = value;
			}
		}

		public class RenamedFieldData
		{
			public int _newValue;

			public RenamedFieldData(int newValue)
			{
				_newValue = newValue;
			}
		}
	}
}
