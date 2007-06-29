/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Reflect.Custom;

namespace Db4objects.Db4o.Tests.Common.Reflect.Custom
{
	/// <summary>
	/// This test case serves two purposes:
	/// 1) testing the reflector API
	/// 2) documenting a common use case for the reflector API which is adapting an external
	/// data model to db4o's internal OO based mechanism.
	/// </summary>
	/// <remarks>
	/// This test case serves two purposes:
	/// 1) testing the reflector API
	/// 2) documenting a common use case for the reflector API which is adapting an external
	/// data model to db4o's internal OO based mechanism.
	/// </remarks>
	public class CustomReflectorTestCase : ITestCase, ITestLifeCycle
	{
		private static readonly string CLASS_NAME = "Cat";

		private static readonly string[] FIELD_NAMES = new string[] { "name", "troubleMakingScore"
			 };

		private static readonly string[] FIELD_TYPES = new string[] { "string", "int" };

		private static readonly PersistentEntry[] ENTRIES = new PersistentEntry[] { new PersistentEntry
			(CLASS_NAME, "0", new object[] { "Biro-Biro", 9 }), new PersistentEntry(CLASS_NAME
			, "1", new object[] { "Samira", 4 }), new PersistentEntry(CLASS_NAME, "2", new object
			[] { "Ivo", 2 }) };

		internal PersistenceContext _context;

		internal IPersistenceProvider _provider;

		public virtual void SetUp()
		{
			InitializeContext();
			InitializeProvider();
			CreateEntryClass(CLASS_NAME, FIELD_NAMES, FIELD_TYPES);
			CreateIndex(CLASS_NAME, FIELD_NAMES[0]);
			InsertEntries();
		}

		public virtual void _testSelectAll()
		{
			Collection4 all = new Collection4(SelectAll());
			Assert.AreEqual(ENTRIES.Length, all.Size());
			for (int i = 0; i < ENTRIES.Length; ++i)
			{
				PersistentEntry expected = ENTRIES[i];
				PersistentEntry actual = EntryByUid(all.GetEnumerator(), expected.uid);
				if (actual != null)
				{
					AssertEqualEntries(expected, actual);
					all.Remove(actual);
				}
			}
			Assert.IsTrue(all.IsEmpty(), all.ToString());
		}

		private PersistentEntry EntryByUid(IEnumerator iterator, object uid)
		{
			while (iterator.MoveNext())
			{
				PersistentEntry e = (PersistentEntry)iterator.Current;
				if (uid.Equals(e.uid))
				{
					return e;
				}
			}
			return null;
		}

		public virtual void _testSelectByField()
		{
			PersistentEntry expected = ENTRIES[1];
			IEnumerator found = SelectByField(FIELD_NAMES[0], expected.fieldValues[0]);
			Assert.IsTrue(found.MoveNext(), "Expecting entry '" + expected + "'");
			PersistentEntry actual = (PersistentEntry)found.Current;
			AssertEqualEntries(expected, actual);
		}

		private void InitializeContext()
		{
			_context = new PersistenceContext(DataFile());
		}

		private void InitializeProvider()
		{
			_provider = new Db4oPersistenceProvider();
			_provider.InitContext(_context);
		}

		private void InsertEntries()
		{
			PersistentEntry entry = new PersistentEntry(CLASS_NAME, null, null);
			for (int i = 0; i < ENTRIES.Length; ++i)
			{
				entry.uid = ENTRIES[i].uid;
				entry.fieldValues = ENTRIES[i].fieldValues;
				Insert(entry);
			}
		}

		private void AssertEqualEntries(PersistentEntry expected, PersistentEntry actual)
		{
			Assert.AreEqual(expected.className, actual.className);
			Assert.AreEqual(expected.uid, actual.uid);
			ArrayAssert.AreEqual(expected.fieldValues, actual.fieldValues);
		}

		private IEnumerator SelectByField(string fieldName, object fieldValue)
		{
			return Select(new PersistentEntryTemplate(CLASS_NAME, new string[] { fieldName }, 
				new object[] { fieldValue }));
		}

		private IEnumerator SelectAll()
		{
			return Select(new PersistentEntryTemplate(CLASS_NAME, new string[0], new object[0
				]));
		}

		private IEnumerator Select(PersistentEntryTemplate template)
		{
			return _provider.Select(_context, template);
		}

		private void Insert(PersistentEntry entry)
		{
			_provider.Insert(_context, entry);
		}

		private void CreateIndex(string className, string fieldName)
		{
			_provider.CreateIndex(_context, className, fieldName);
		}

		private void CreateEntryClass(string className, string[] fieldNames, string[] fieldTypes
			)
		{
			_provider.CreateEntryClass(_context, className, fieldNames, fieldTypes);
		}

		public virtual void TearDown()
		{
			ShutdownProvider(true);
			_context = null;
		}

		private void ShutdownProvider(bool purge)
		{
			_provider.CloseContext(_context, purge);
			_provider = null;
		}

		internal virtual void RestartProvider()
		{
			ShutdownProvider(false);
			InitializeProvider();
		}

		private string DataFile()
		{
			return Path.Combine(Path.GetTempPath(), "CustomReflector.db4o");
		}
	}
}
