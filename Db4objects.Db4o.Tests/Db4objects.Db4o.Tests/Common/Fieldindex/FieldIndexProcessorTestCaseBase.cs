namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public abstract class FieldIndexProcessorTestCaseBase : Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexTestCaseBase
	{
		public FieldIndexProcessorTestCaseBase() : base()
		{
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			base.Configure(config);
			IndexField(config, typeof(Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem)
				, "foo");
			IndexField(config, typeof(Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem)
				, "bar");
			IndexField(config, typeof(Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem)
				, "child");
		}

		protected virtual Db4objects.Db4o.Query.IQuery CreateComplexItemQuery()
		{
			return CreateQuery(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem)
				);
		}

		protected virtual Db4objects.Db4o.Inside.Fieldindex.IIndexedNode SelectBestIndex(
			Db4objects.Db4o.Query.IQuery query)
		{
			Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessor processor = CreateProcessor
				(query);
			return processor.SelectBestIndex();
		}

		protected virtual Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessor CreateProcessor
			(Db4objects.Db4o.Query.IQuery query)
		{
			Db4objects.Db4o.QCandidates candidates = GetQCandidates(query);
			return new Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessor(candidates);
		}

		private Db4objects.Db4o.QCandidates GetQCandidates(Db4objects.Db4o.Query.IQuery query
			)
		{
			Db4objects.Db4o.QQueryBase.CreateCandidateCollectionResult result = ((Db4objects.Db4o.QQuery
				)query).CreateCandidateCollection();
			Db4objects.Db4o.QCandidates candidates = (Db4objects.Db4o.QCandidates)result.candidateCollection
				._element;
			return candidates;
		}

		protected virtual void AssertComplexItemIndex(string expectedFieldIndex, Db4objects.Db4o.Inside.Fieldindex.IIndexedNode
			 node)
		{
			Db4oUnit.Assert.AreSame(ComplexItemIndex(expectedFieldIndex), node.GetIndex());
		}

		protected virtual Db4objects.Db4o.Inside.Btree.BTree FieldIndexBTree(System.Type 
			clazz, string fieldName)
		{
			return GetYapClass(clazz).GetYapField(fieldName).GetIndex(null);
		}

		private Db4objects.Db4o.YapClass GetYapClass(System.Type clazz)
		{
			return Stream().GetYapClass(GetReflectClass(clazz));
		}

		private Db4objects.Db4o.Reflect.IReflectClass GetReflectClass(System.Type clazz)
		{
			return Stream().Reflector().ForClass(clazz);
		}

		protected virtual Db4objects.Db4o.Inside.Btree.BTree ClassIndexBTree(System.Type 
			clazz)
		{
			return ((Db4objects.Db4o.Inside.Classindex.BTreeClassIndexStrategy)GetYapClass(clazz
				).Index()).Btree();
		}

		private Db4objects.Db4o.Inside.Btree.BTree ComplexItemIndex(string fieldName)
		{
			return FieldIndexBTree(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem)
				, fieldName);
		}

		protected virtual int[] MapToObjectIds(Db4objects.Db4o.Query.IQuery itemQuery, int[]
			 foos)
		{
			int[] lookingFor = Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.Clone(foos);
			int[] objectIds = new int[foos.Length];
			Db4objects.Db4o.IObjectSet set = itemQuery.Execute();
			while (set.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Fieldindex.IHasFoo item = (Db4objects.Db4o.Tests.Common.Fieldindex.IHasFoo
					)set.Next();
				for (int i = 0; i < lookingFor.Length; i++)
				{
					if (lookingFor[i] == item.GetFoo())
					{
						lookingFor[i] = -1;
						objectIds[i] = (int)Db().GetID(item);
						break;
					}
				}
			}
			int index = IndexOfNot(lookingFor, -1);
			if (-1 != index)
			{
				throw new System.ArgumentException("Foo '" + lookingFor[index] + "' not found!");
			}
			return objectIds;
		}

		public static int IndexOfNot(int[] array, int value)
		{
			for (int i = 0; i < array.Length; ++i)
			{
				if (value != array[i])
				{
					return i;
				}
			}
			return -1;
		}

		protected virtual void StoreComplexItems(int[] foos, int[] bars)
		{
			Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem last = null;
			for (int i = 0; i < foos.Length; i++)
			{
				last = new Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem(foos[i], 
					bars[i], last);
				Store(last);
			}
		}

		protected virtual void AssertTreeInt(int[] expectedValues, Db4objects.Db4o.TreeInt
			 treeInt)
		{
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor visitor = Db4objects.Db4o.Tests.Common.Btree.BTreeAssert
				.CreateExpectingVisitor(expectedValues);
			treeInt.Traverse(new _AnonymousInnerClass118(this, visitor));
			visitor.AssertExpectations();
		}

		private sealed class _AnonymousInnerClass118 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass118(FieldIndexProcessorTestCaseBase _enclosing, Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				 visitor)
			{
				this._enclosing = _enclosing;
				this.visitor = visitor;
			}

			public void Visit(object obj)
			{
				visitor.Visit(((Db4objects.Db4o.TreeInt)obj)._key);
			}

			private readonly FieldIndexProcessorTestCaseBase _enclosing;

			private readonly Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor visitor;
		}
	}
}
