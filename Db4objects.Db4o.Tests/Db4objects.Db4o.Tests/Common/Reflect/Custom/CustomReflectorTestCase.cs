/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Reflect.Custom;
using Sharpen;

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
	/// See CustomReflector, CustomClassRepository, CustomClass, CustomField and CustomUidField
	/// for details.
	/// </remarks>
	public class CustomReflectorTestCase : ITestCase, ITestLifeCycle
	{
		private static readonly string CAT_CLASS = "Cat";

		private static readonly string[] CAT_FIELD_NAMES = new string[] { "name", "troubleMakingScore"
			 };

		private static readonly string[] CAT_FIELD_TYPES = new string[] { "string", "int"
			 };

		private static readonly string PERSON_CLASS = "Person";

		private static readonly string[] PERSON_FIELD_NAMES = new string[] { "name" };

		private static readonly string[] PERSON_FIELD_TYPES = new string[] { "string" };

		private static readonly PersistentEntry[] CAT_ENTRIES = new PersistentEntry[] { new 
			PersistentEntry(CAT_CLASS, "0", new object[] { "Biro-Biro", 9 }), new PersistentEntry
			(CAT_CLASS, "1", new object[] { "Samira", 4 }), new PersistentEntry(CAT_CLASS, "2"
			, new object[] { "Ivo", 2 }) };

		private static readonly PersistentEntry[] PERSON_ENTRIES = new PersistentEntry[] 
			{ new PersistentEntry(PERSON_CLASS, "10", new object[] { "Eric Idle" }), new PersistentEntry
			(PERSON_CLASS, "11", new object[] { "John Cleese" }) };

		internal PersistenceContext _context;

		internal Db4oPersistenceProvider _provider;

		public virtual void SetUp()
		{
			Purge();
			InitializeContext();
			InitializeProvider();
			CreateEntryClass(CAT_CLASS, CAT_FIELD_NAMES, CAT_FIELD_TYPES);
			CreateIndex(CAT_CLASS, CAT_FIELD_NAMES[0]);
			RestartProvider();
			CreateEntryClass(PERSON_CLASS, PERSON_FIELD_NAMES, PERSON_FIELD_TYPES);
			RestartProvider();
			InsertEntries();
			RestartProvider();
		}

		public virtual void TestUpdate()
		{
			PersistentEntry entry = new PersistentEntry(CAT_CLASS, CAT_ENTRIES[0].uid, new object
				[] { "Birinho", 10 });
			Update(entry);
			RestartProvider();
			PersistentEntry[] expected = Copy(CAT_ENTRIES);
			expected[0] = entry;
			AssertEntries(expected, SelectAll(CAT_CLASS));
		}

		public virtual void TestSelectAll()
		{
			AssertEntries(PERSON_ENTRIES, SelectAll(PERSON_CLASS));
			AssertEntries(CAT_ENTRIES, SelectAll(CAT_CLASS));
		}

		public virtual void TestSelectByField()
		{
			ExerciseSelectByField(CAT_ENTRIES, CAT_FIELD_NAMES);
			ExerciseSelectByField(PERSON_ENTRIES, PERSON_FIELD_NAMES);
		}

		public virtual void TestSelectByFields()
		{
			PersistentEntry existing = CAT_ENTRIES[0];
			PersistentEntry newEntry = new PersistentEntry(CAT_CLASS, existing.uid, new object
				[] { existing.fieldValues[0], 10 });
			Insert(newEntry);
			IEnumerator found = SelectByField(existing.className, CAT_FIELD_NAMES[0], existing
				.fieldValues[0]);
			AssertEntries(new PersistentEntry[] { existing, newEntry }, found);
			AssertSingleEntry(existing, Select(existing.className, CAT_FIELD_NAMES, existing.
				fieldValues));
			AssertSingleEntry(newEntry, Select(newEntry.className, CAT_FIELD_NAMES, newEntry.
				fieldValues));
		}

		public virtual void TestDropIndex()
		{
			DropIndex(CAT_CLASS, CAT_FIELD_NAMES[0]);
			Db4objects.Db4o.Internal.FieldMetadata field = FieldMetadata(CAT_CLASS, CAT_FIELD_NAMES
				[0]);
			Assert.IsFalse(field.HasIndex());
		}

		public virtual void TestFieldIndex()
		{
			Db4objects.Db4o.Internal.FieldMetadata field0 = FieldMetadata(CAT_CLASS, CAT_FIELD_NAMES
				[0]);
			Assert.IsTrue(field0.HasIndex());
			Db4objects.Db4o.Internal.FieldMetadata field1 = FieldMetadata(CAT_CLASS, CAT_FIELD_NAMES
				[1]);
			Assert.IsFalse(field1.HasIndex());
		}

		private Db4objects.Db4o.Internal.FieldMetadata FieldMetadata(string className, string
			 fieldName)
		{
			ClassMetadata meta = ClassMetadataForName(className);
			Db4objects.Db4o.Internal.FieldMetadata field0 = meta.FieldMetadataForName(fieldName
				);
			return field0;
		}

		private void Update(PersistentEntry entry)
		{
			_provider.Update(_context, entry);
		}

		private void AssertEntries(PersistentEntry[] expected, IEnumerator actual)
		{
			Collection4 checklist = new Collection4(actual);
			Assert.AreEqual(expected.Length, checklist.Size());
			for (int i = 0; i < expected.Length; ++i)
			{
				PersistentEntry e = expected[i];
				PersistentEntry a = EntryByUid(checklist.GetEnumerator(), e.uid);
				if (a != null)
				{
					AssertEqualEntries(e, a);
					checklist.Remove(a);
				}
			}
			Assert.IsTrue(checklist.IsEmpty(), checklist.ToString());
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

		private ClassMetadata ClassMetadataForName(string className)
		{
			ObjectContainerBase container = (ObjectContainerBase)_provider.DataContainer(_context
				);
			return container.ClassMetadataForReflectClass(container.Reflector().ForName(className
				));
		}

		private void ExerciseSelectByField(PersistentEntry[] entries, string[] fieldNames
			)
		{
			for (int i = 0; i < entries.Length; ++i)
			{
				ExerciseSelectByField(entries[i], fieldNames);
			}
		}

		private void ExerciseSelectByField(PersistentEntry expected, string[] fieldNames)
		{
			for (int i = 0; i < fieldNames.Length; ++i)
			{
				IEnumerator found = SelectByField(expected.className, fieldNames[i], expected.fieldValues
					[i]);
				AssertSingleEntry(expected, found);
			}
		}

		private void AssertSingleEntry(PersistentEntry expected, IEnumerator found)
		{
			Assert.IsTrue(found.MoveNext(), "Expecting entry '" + expected + "'");
			PersistentEntry actual = (PersistentEntry)found.Current;
			AssertEqualEntries(expected, actual);
			Assert.IsFalse(found.MoveNext(), "Expecting only '" + expected + "'");
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
			InsertEntries(CAT_ENTRIES);
			InsertEntries(PERSON_ENTRIES);
		}

		private void InsertEntries(PersistentEntry[] entries)
		{
			PersistentEntry entry = new PersistentEntry(null, null, null);
			for (int i = 0; i < entries.Length; ++i)
			{
				entry.className = entries[i].className;
				entry.uid = entries[i].uid;
				entry.fieldValues = entries[i].fieldValues;
				Insert(entry);
			}
		}

		private void AssertEqualEntries(PersistentEntry expected, PersistentEntry actual)
		{
			Assert.AreEqual(expected.className, actual.className);
			Assert.AreEqual(expected.uid, actual.uid);
			ArrayAssert.AreEqual(expected.fieldValues, actual.fieldValues);
		}

		private IEnumerator SelectByField(string className, string fieldName, object fieldValue
			)
		{
			return Select(className, new string[] { fieldName }, new object[] { fieldValue });
		}

		private IEnumerator Select(string className, string[] fieldNames, object[] fieldValues
			)
		{
			return Select(new PersistentEntryTemplate(className, fieldNames, fieldValues));
		}

		private IEnumerator SelectAll(string className)
		{
			return Select(className, new string[0], new object[0]);
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

		private void DropIndex(string className, string fieldName)
		{
			_provider.DropIndex(_context, className, fieldName);
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
			if (_provider != null)
			{
				_provider.CloseContext(_context);
			}
			if (purge)
			{
				Purge();
			}
			_provider = null;
		}

		internal virtual void Purge()
		{
			new Db4oPersistenceProvider().Purge(DataFile());
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

		private PersistentEntry[] Copy(PersistentEntry[] entries)
		{
			PersistentEntry[] clone = new PersistentEntry[entries.Length];
			System.Array.Copy(entries, 0, clone, 0, clone.Length);
			return clone;
		}
	}
}
