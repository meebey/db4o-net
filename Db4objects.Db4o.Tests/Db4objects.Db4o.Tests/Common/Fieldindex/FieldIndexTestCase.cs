using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Btree;
using Db4objects.Db4o.Tests.Common.Fieldindex;
using Db4objects.Db4o.Tests.Common.Foundation;

namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class FieldIndexTestCase : FieldIndexTestCaseBase
	{
		private static readonly int[] FOOS = new int[] { 3, 7, 9, 4 };

		public static void Main(string[] arguments)
		{
			new FieldIndexTestCase().RunSolo();
		}

		protected override void Configure(IConfiguration config)
		{
			base.Configure(config);
		}

		protected override void Store()
		{
			StoreItems(FOOS);
		}

		public virtual void TestTraverseValues()
		{
			IStoredField field = YapField();
			ExpectingVisitor expectingVisitor = new ExpectingVisitor(IntArrays4.ToObjectArray
				(FOOS));
			field.TraverseValues(expectingVisitor);
			expectingVisitor.AssertExpectations();
		}

		public virtual void TestAllThere()
		{
			for (int i = 0; i < FOOS.Length; i++)
			{
				IQuery q = CreateQuery(FOOS[i]);
				IObjectSet objectSet = q.Execute();
				Assert.AreEqual(1, objectSet.Size());
				FieldIndexItem fii = (FieldIndexItem)objectSet.Next();
				Assert.AreEqual(FOOS[i], fii.foo);
			}
		}

		public virtual void TestAccessingBTree()
		{
			BTree bTree = YapField().GetIndex(Trans());
			Assert.IsNotNull(bTree);
			ExpectKeysSearch(bTree, FOOS);
		}

		private void ExpectKeysSearch(BTree btree, int[] values)
		{
			int lastValue = int.MinValue;
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] != lastValue)
				{
					ExpectingVisitor expectingVisitor = BTreeAssert.CreateExpectingVisitor(values[i], 
						IntArrays4.Occurences(values, values[i]));
					IBTreeRange range = FieldIndexKeySearch(Trans(), btree, values[i]);
					BTreeAssert.TraverseKeys(range, new _AnonymousInnerClass64(this, expectingVisitor
						));
					expectingVisitor.AssertExpectations();
					lastValue = values[i];
				}
			}
		}

		private sealed class _AnonymousInnerClass64 : IVisitor4
		{
			public _AnonymousInnerClass64(FieldIndexTestCase _enclosing, ExpectingVisitor expectingVisitor
				)
			{
				this._enclosing = _enclosing;
				this.expectingVisitor = expectingVisitor;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Btree.FieldIndexKey fik = (Db4objects.Db4o.Internal.Btree.FieldIndexKey
					)obj;
				expectingVisitor.Visit(fik.Value());
			}

			private readonly FieldIndexTestCase _enclosing;

			private readonly ExpectingVisitor expectingVisitor;
		}

		private Db4objects.Db4o.Internal.Btree.FieldIndexKey FieldIndexKey(int integerPart
			, object composite)
		{
			return new Db4objects.Db4o.Internal.Btree.FieldIndexKey(integerPart, composite);
		}

		private IBTreeRange FieldIndexKeySearch(Transaction trans, BTree btree, object key
			)
		{
			BTreeNodeSearchResult start = btree.SearchLeaf(trans, FieldIndexKey(0, key), SearchTarget
				.LOWEST);
			BTreeNodeSearchResult end = btree.SearchLeaf(trans, FieldIndexKey(int.MaxValue, key
				), SearchTarget.LOWEST);
			return start.CreateIncludingRange(end);
		}

		private FieldMetadata YapField()
		{
			IReflectClass claxx = Stream().Reflector().ForObject(new FieldIndexItem());
			ClassMetadata yc = Stream().ClassMetadataForReflectClass(claxx);
			FieldMetadata yf = yc.FieldMetadataForName("foo");
			return yf;
		}
	}
}
