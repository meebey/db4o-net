/* Copyright (C) 2009  Versant Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Query;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.Handlers
{
	public class GuidTypeHandlerTestCase : AbstractDb4oTestCase
	{
		protected const int ItemCount = 255;

		protected override void Configure(IConfiguration config)
		{
			IObjectClass objectClass = config.ObjectClass(typeof(GuidHolder));
			objectClass.ObjectField("Guid").Indexed(true);
			objectClass.CascadeOnDelete(true);
		}
		
		public void TestNonIndexedGuidQuery()
		{
			IQuery query = Db().Query();
			query.Constrain(typeof(GuidHolder));

			query.Descend("NotIndexed").Constrain(NewGuidFor(1));
			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Count);
		}

		public void TestOrderBySucceedingFieldDoesNotThrow()
		{
			IQuery query = Db().Query();
			query.Constrain(typeof (GuidHolder));
			query.Descend("Name").OrderDescending();

			query.Execute();
		}

		public void TestIndexedQuery()
		{
			DiagnosticCollector<LoadedFromClassIndex> collector = DiagnosticCollectorFor<LoadedFromClassIndex>();

			GuidHolder holder = RetrieveHolderWithId(NewGuidFor(1));
			
			Assert.IsNotNull(holder);
			Assert.AreEqual(0, collector.Diagnostics.Count, "Query should go through guid indexes");
		}

		public void TestIndexingLowLevel()
		{
			LocalObjectContainer container = Fixture().FileSession();
			ClassMetadata classMetadata = container.ClassMetadataForReflectClass(container.Reflector().ForClass(typeof(GuidHolder)));
			FieldMetadata fieldMetadata = classMetadata.FieldMetadataForName("Guid");

			Assert.IsTrue(fieldMetadata.CanLoadByIndex(), "GuidTypeHandler should be indexable.");
			BTree index = fieldMetadata.GetIndex(container.SystemTransaction());
			Assert.IsNotNull(index, "No btree index found for guid field.");
		}

		public void TestNoClassIndex()
		{
			IStoredClass storedClass = Db().StoredClass(typeof (Guid));
			Assert.AreEqual(0, storedClass.InstanceCount());
		}

		public void TestDefragment()
		{
			Defragment();
			AssertCanRetrieveAll();
		}

		public void TestDataCanBeRead()
		{
			AssertCanRetrieveAll();
		}

		private void AssertCanRetrieveAll()
		{
			IObjectSet results = NewQuery(typeof (GuidHolder)).Execute();
			Assert.AreEqual(ItemCount, results.Count);

			List<GuidHolder> holders = new List<GuidHolder>();
			ForAllHolders(delegate(GuidHolder holder) { holders.Add(holder);});

			Iterator4Assert.SameContent(holders.ToArray(), results.GetEnumerator());
		}

		public void TestUpdate()
		{
			Guid expected = NewGuidFor(1);
			GuidHolder holder = RetrieveHolderWithId(expected);
			Guid newGuid = new Guid(0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0);
			holder.Guid = newGuid;
			Store(holder);

			Reopen();
			
			AssertQuery(expected);
			AssertQuery(newGuid, new GuidHolder(holder.Name, newGuid));
		}

		public void TestDelete()
		{
			IObjectSet results = CandidatesForDeletion();
			Assert.AreEqual(2, results.Count);
			DiagnosticCollector<DeletionFailed> collector = DiagnosticCollectorFor<DeletionFailed>();
			
			DeleteObjectSet(results);
			results = CandidatesForDeletion();
			Assert.AreEqual(0, results.Count);
			Assert.AreEqual(0, collector.Diagnostics.Count, "CascadeOnDelete failed.");
		}

		private DiagnosticCollector<T> DiagnosticCollectorFor<T>()
		{
			DiagnosticCollector<T> collector = new DiagnosticCollector<T>();
			Db().Configure().Diagnostic().AddListener(collector);
			
			return collector;
		}

		private IObjectSet CandidatesForDeletion()
		{
			return RetrieveHoldersWith(NewGuidFor(0), NewGuidFor(ItemCount - 1));
		}

		private GuidHolder RetrieveHolderWithId(Guid id)
		{
			IObjectSet result = RetrieveHoldersWith(id);
			Assert.AreEqual(1, result.Count);
			GuidHolder actual = (GuidHolder) result[0];
			Assert.AreEqual(id, actual.Guid);

			return actual;
		}

		protected override void Store()
		{
			ForAllHolders(delegate(GuidHolder holder) { Store(holder); });
		}

		protected static void ForAllHolders(Action<GuidHolder> action)
		{
			for (int i = 0; i < ItemCount; i++)
			{
				action(NewHolderFor(i));
			}
		}

		protected static GuidHolder NewHolderFor(int i)
		{
			return new GuidHolder("#" + i, NewGuidFor(i));
		}

		protected static Guid NewGuidFor(int i)
		{
			return new Guid(126 + i, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)i);
		}

		protected void AssertQuery(Guid expected, params GuidHolder[] expectedHolders)
		{
			IObjectSet result = RetrieveHoldersWith(expected);

			Assert.AreEqual(expectedHolders.Length, result.Count);
			Iterator4Assert.SameContent(expectedHolders, result.GetEnumerator());
		}

		protected IObjectSet RetrieveHoldersWith(params Guid[] ids)
		{
			IConstraint lastConstraint = null;
			
			IQuery query = NewQuery(typeof (GuidHolder));
			foreach (Guid id in ids)
			{
				IConstraint constraint = query.Descend("Guid").Constrain(id);
				if (lastConstraint != null)
				{
					lastConstraint.Or(constraint);
				}

				lastConstraint = constraint;
			}


			return query.Execute();
		}
	}

	public class GuidHolder
	{
		public String NoIdea;
		public Guid Guid;
		public Guid NotIndexed;
		public GuidHolder Myself;
		public string Name;

		public GuidHolder(string name, Guid guid)
		{
			Guid = guid;
			NotIndexed = guid;
			Name = name;
			Myself = this;
		}

		public override bool Equals(object obj)
		{
			GuidHolder other = obj as GuidHolder;
			if (other == null) return false;

			return Guid.CompareTo(other.Guid) == 0 && Name.CompareTo(other.Name) == 0;
		}
	}
}
