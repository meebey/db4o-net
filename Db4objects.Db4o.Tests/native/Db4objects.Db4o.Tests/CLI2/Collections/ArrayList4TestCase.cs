using System;
using System.Collections.Generic;
using System.Threading;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI2.Collections
{
    internal class ArrayList4TestCase : ITestLifeCycle
    {
        private delegate void AssertDelegate(int index, int value);

        private const int MULTIPLIER = 3;
        private const int OFFSET = 10000;

        #region ITestLifeCycle Members

        public void SetUp()
        {
        }

        public void TearDown()
        {
        }

        #endregion

        public void TestLowerBound()
        {
            Assert.Expect(
                typeof (ArgumentOutOfRangeException),
                new CodeBlockRunner<int>(
                        delegate(int len)
                        {
                            IList<int> list = CreateArrayList(len);
                            int i = list[-1];
                        },
                        10));
        }

        public void TestUpperBound()
        {
            // 
            Assert.Expect(
                typeof(ArgumentOutOfRangeException),
                new CodeBlockRunner<int>(
                        delegate(int len)
                        {
                            IList<int> list = CreateArrayList(len);
                            int i = list[list.Count + 1];
                        },
                        10));
        }

        public void TestItems()
        {
            IList<int> list = CreateArrayListAndAssertValues(10);
            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(i*3, list[i]);
            }
        }

        public void TestAddItems()
        {
            IList<int> list = CreateArrayListAndAssertValues(10);
            int index = list.Count;
            for (int i = 0; i < 3; i++)
            {
                list.Add(ValueForIndex(index++));
            }

            AssertArrayListValues(list);
            Assert.AreEqual(index, list.Count);
        }

        public void TestIsReadyOnly()
        {
            IList<int> list = CreateArrayList(10);
            Assert.IsFalse(list.IsReadOnly);
        }

        public void TestClear()
        {
            IList<int> list = CreateArrayListAndAssertValues(10);
            list.Clear();
            Assert.AreEqual(0, list.Count);
        }

        public void TestContains()
        {
            IList<int> list = CreateArrayList(10);

            ForEach(
                list,
                delegate(int index, int value) { Assert.IsTrue(list.Contains(value)); });

            Assert.IsFalse(list.Contains(-1));
            Assert.IsFalse(list.Contains(ValueForIndex(list.Count) + 1));
        }

        public void TestCopyTo()
        {
            IList<int> list = CreateArrayList(10);
            AssertArrayListValues(list);

            int[] backup = new int[list.Count];
            list.CopyTo(backup, 0);

            AssertAreEqual(backup, list, 0, 0, backup.Length);

            backup = new int[list.Count + 1];
            backup[0] = 0xCC;
            list.CopyTo(backup, 1);
            Assert.AreEqual(0xCC, backup[0]);
            AssertAreEqual(backup, list, 1, 0, backup.Length - 1);

            backup = new int[list.Count + 2];
            backup[0] = 0xDE;
            backup[1] = 0xAD;
            list.CopyTo(backup, 2);
            Assert.AreEqual(0xDE, backup[0]);
            Assert.AreEqual(0xAD, backup[1]);
            AssertAreEqual(backup, list, 2, 0, list.Count);
        }

        private static void AssertAreEqual(int[] array, IList<int> list, int arrayStartIndex, int listIndex, int count)
        {
            Assert.AreEqual(array.Length - arrayStartIndex, list.Count - listIndex);
            for (int i = arrayStartIndex; i < array.Length; i++)
            {
                Assert.AreEqual(array[i], list[listIndex++]);
            }
        }

        public void TestCopyToWithInvalidSize()
        {
            Assert.Expect(
                typeof (ArgumentException),
                new CodeBlockRunner<int>(
                    delegate(int len)
                    {
                        IList<int> list = CreateArrayListAndAssertValues(len);

                        int[] backup = new int[list.Count - 1];
                        list.CopyTo(backup, 0);
                    },
                    10));
        }

        public void TestCopyToWithNullTarget()
        {
            Assert.Expect(
                typeof(ArgumentNullException),
                new CodeBlockRunner<int>(
                        delegate(int len)
                        {
                            IList<int> list = CreateArrayListAndAssertValues(len);
                            list.CopyTo(null, 0);
                        },
                        10)
                );
        }

        public void TestCopyToInvalidIndex()
        {
            Assert.Expect(
                typeof (ArgumentException),
                new CodeBlockRunner<int>(
                    delegate(int len)
                        {
                            IList<int> list = CreateArrayListAndAssertValues(len);

                            int[] backup = new int[len];
                            list.CopyTo(backup, backup.Length + 1);
                        },
                    10
                    ));
        }

        //TODO: Check the documentation
        public void TestRemove()
        {
            int size = 10;
            IList<int> list = CreateArrayListAndAssertValues(size);

            Assert.IsFalse(list.Remove(ValueForIndex(-1)));
            Assert.IsFalse(list.Remove(ValueForIndex(size + 1)));
            Assert.AreEqual(size, list.Count);

            RemoveAndAssert(list, ref size, 0);
            RemoveAndAssert(list, ref size, size);
        }

        public void TestIndexOf()
        {
            int size = 10;
            IList<int> list = CreateArrayListAndAssertValues(size);

            Assert.AreEqual(-1, list.IndexOf(ValueForIndex(-1)));

            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(i, list.IndexOf(list[i]));
            }

            Assert.AreEqual(-1, list.IndexOf(ValueForIndex(list.Count + 1)));

            Assert.AreEqual(0, list.IndexOf(ValueForIndex(0)));
            Assert.AreEqual(5, list.IndexOf(ValueForIndex(5)));
            Assert.AreEqual(-1, list.IndexOf(ValueForIndex(-8)));
        }

        public void TestInsert()
        {
            int size = 10;
            IList<int> list = CreateArrayListAndAssertValues(size);
            InsertInList(list, 5);
            
            Assert.AreEqual(5, list.IndexOf(ValueWithOffsetForIndex(5)));
            Assert.AreEqual(6, list.IndexOf(ValueForIndex(5))); // index 5 was moved to 6
        }

        private static void InsertInList(IList<int> list, int index)
        {
            list.Insert(index, ValueWithOffsetForIndex(index));
        }

        private static int ValueWithOffsetForIndex(int index)
        {
            return ValueForIndex(index) + OFFSET;
        }

        private static void RemoveAndAssert(IList<int> list, ref int size, int index)
        {
            Assert.IsTrue(list.Remove(ValueForIndex(index)));
            Assert.AreEqual(--size, list.Count);
            AssertArrayListValuesOffset(list, 1);
        }

        private static void AssertArrayListValues(IList<int> list)
        {
            AssertArrayListValuesOffset(list, 0);
        }

        private static void AssertArrayListValuesOffset(IList<int> list, int offset)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(ValueForIndex(i + offset), list[i]);
            }
        }

        private static int ValueForIndex(int index)
        {
            return index*MULTIPLIER;
        }

        private static IList<int> CreateArrayListAndAssertValues(int size)
        {
            IList<int> list = new List<int>(NewListValues(size));
            Assert.AreEqual(size, list.Count);

            return list;
        }

        private static IEnumerable<int> NewListValues(int max)
        {
            for (int i = 0; i < max; i++)
            {
                yield return i*MULTIPLIER;
            }
        }

        private static IList<int> CreateArrayList(int count)
        {
            List<int> list = new List<int>(count);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = ValueForIndex(i);
            }

            return list;
        }

        private static void ForEach(IList<int> list, AssertDelegate method)
        {
            for (int i = 0; i < list.Count; i++)
            {
                method(i, list[i]);
            }
        }
    }

    class CodeBlockRunner<T> : ICodeBlock
    {
        private readonly Action<T> method;
        private readonly T value;

        internal CodeBlockRunner(Action<T> method, T value)
        {
            this.method = method;
            this.value = value;
        }

        public void Run()
        {
            method(value);
        }
    }
}