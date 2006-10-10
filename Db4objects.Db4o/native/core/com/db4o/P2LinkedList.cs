/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o
{
	internal class P2LinkedList : P1Collection, Db4oList
	{
		public P1ListElement i_first;

		public P1ListElement i_last;

		internal P2LinkedList()
			: base()
		{
		}

		public int Add(Object obj)
		{
			lock (StreamLock())
			{
				CheckActive();
				if (obj == null)
				{
					throw new ArgumentNullException();
				}
				Add4(obj);
				Update();
				return Size4() - 1;
			}
		}

		public void Clear()
		{
			lock (StreamLock())
			{
				CheckActive();
				P2ListElementIterator i = Iterator4();
				while (i.HasNext())
				{
					P1ListElement le = i.NextElement();
					le.Delete(i_deleteRemoved);
				}
				i_first = null;
				i_last = null;
				Update();
			}
		}

		public bool Contains(Object obj)
		{
			return IndexOf(obj) >= 0;
		}

		public void CopyTo(Array arr, int pos)
		{
			lock (StreamLock())
			{
				CheckActive();
				P2ListElementIterator i = Iterator4();
				while (i.HasNext())
				{
					P1ListElement ple = i.NextElement();
					arr.SetValue(ple.ActivatedObject(ElementActivationDepth()), pos++);
				}
			}
		}

		public int Count
		{
			get
			{
				lock (StreamLock())
				{
					CheckActive();
					return Size4();
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return GetEnumerator1();
		}

		public int IndexOf(Object obj)
		{
			lock (StreamLock())
			{
				CheckActive();
				return IndexOf4(obj);
			}
		}

		public void Insert(int pos, Object obj)
		{
			lock (StreamLock())
			{
				CheckActive();
				if (pos == 0)
				{
					i_first = new P1ListElement(GetTrans(), i_first, obj);
					Store(i_first);
					CheckLastAndUpdate(null, i_first);
				}
				else
				{
					P2ListElementIterator i = Iterator4();
					P1ListElement previous = i.Move(pos - 1);
					if (previous == null)
					{
						throw new IndexOutOfRangeException();
					}
					P1ListElement newE = new P1ListElement(GetTrans(), previous.i_next, obj);
					Store(newE);
					previous.i_next = newE;
					previous.Update();
					CheckLastAndUpdate(previous, newE);
				}
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return true;
			}
		}

		public void Remove(Object obj)
		{
			lock (StreamLock())
			{
				CheckActive();
				Remove4(obj);
			}
		}

		public void RemoveAt(int pos)
		{
			lock (StreamLock())
			{
				CheckActive();
				Remove4(pos);
			}
		}

		public Object SyncRoot
		{
			get
			{
				CheckActive();
				return StreamLock();
			}
		}

		public Object this[int index]
		{

			get
			{
				lock (StreamLock())
				{
					CheckActive();
					P1ListElement ple = Iterator4().Move(index);
					if (ple != null)
					{
						return ple.ActivatedObject(ElementActivationDepth());
					}
					return null;
				}
			}

			set
			{
				lock (StreamLock())
				{
					CheckActive();
					bool needUpdate = false;
					P1ListElement elem = null;
					P1ListElement previous = null;
					P1ListElement newElement = new P1ListElement(GetTrans(), null, value);
					if (index == 0)
					{
						previous = i_first;
						i_first = newElement;
						needUpdate = true;
					}
					else
					{
						elem = Iterator4().Move(index - 1);
						if (elem != null)
						{
							previous = elem.i_next;
						}
						else
						{
							throw new IndexOutOfRangeException();
						}
					}
					if (previous != null)
					{
						previous.CheckActive();
						newElement.i_next = previous.i_next;
						if (elem != null)
						{
							elem.i_next = newElement;
							elem.Update();
						}
						previous.Delete(i_deleteRemoved);
					}
					else
					{
						i_last = newElement;
						needUpdate = true;
					}
					if (needUpdate)
					{
						Update();
					}
				}
			}
		}

		protected bool Add4(Object obj)
		{
			if (obj != null)
			{
				P1ListElement newElement = new P1ListElement(GetTrans(), null, obj);
				Store(newElement);
				if (i_first == null)
				{
					i_first = newElement;
				}
				else
				{
					i_last.CheckActive();
					i_last.i_next = newElement;
					i_last.Update();
				}
				i_last = newElement;
				return true;
			}
			return false;
		}

		public override int AdjustReadDepth(int i)
		{
			return 1;
		}

		protected void CheckLastAndUpdate(P1ListElement a_previous, P1ListElement a_added)
		{
			if (i_last == a_previous)
			{
				i_last = a_added;
			}
			Update();
		}

		internal void CheckRemoved(P1ListElement a_previous, P1ListElement a_removed)
		{
			bool needsUpdate = false;
			if (a_removed == i_first)
			{
				i_first = a_removed.i_next;
				needsUpdate = true;
			}
			if (a_removed == i_last)
			{
				i_last = a_previous;
				needsUpdate = true;
			}
			if (needsUpdate)
			{
				Update();
			}
		}

		protected bool Contains4(Object obj)
		{
			return IndexOf4(obj) >= 0;
		}

		public override Object CreateDefault(Transaction transaction)
		{
			CheckActive();
			P2LinkedList ll = new P2LinkedList();
			ll.SetTrans(transaction);
			P2ListElementIterator i = Iterator4();
			while (i.MoveNext())
			{
				ll.Add4(i.Current);
			}
			return ll;
		}

		protected override IEnumerator GetEnumerator1()
		{
			lock (StreamLock())
			{
				CheckActive();
				return Iterator4();
			}
		}

		public override bool HasClassIndex()
		{
			return true;
		}

		protected int IndexOf4(Object obj)
		{
			int idx = 0;
			if (GetTrans() != null && (!GetTrans().Stream().Handlers().IsSecondClass(obj)))
			{
				long id = GetIDOf(obj);
				if (id > 0)
				{
					P2ListElementIterator i = Iterator4();
					while (i.HasNext())
					{
						P1ListElement le = i.NextElement();
						if (GetIDOf(le.i_object) == id)
						{
							return idx;
						}
						idx++;
					}
				}
			}
			else
			{
				P2ListElementIterator i = Iterator4();
				while (i.HasNext())
				{
					P1ListElement le = i.NextElement();
					if (le.i_object.Equals(obj))
					{
						return idx;
					}
					idx++;
				}
			}
			return -1;
		}

		protected P2ListElementIterator Iterator4()
		{
			return new P2ListElementIterator(this, i_first);
		}

		protected Object Remove4(int idx)
		{
			Object ret = null;
			P1ListElement elem = null;
			P1ListElement previous = null;
			if (idx == 0)
			{
				elem = i_first;
			}
			else
			{
				previous = Iterator4().Move(idx - 1);
				if (previous != null)
				{
					elem = previous.i_next;
				}
			}
			if (elem != null)
			{
				elem.CheckActive();
				if (previous != null)
				{
					previous.i_next = elem.i_next;
					previous.Update();
				}
				CheckRemoved(previous, elem);
				ret = elem.ActivatedObject(ElementActivationDepth());
				elem.Delete(i_deleteRemoved);
				return ret;
			}
			throw new IndexOutOfRangeException();
		}

		protected bool Remove4(Object obj)
		{
			int idx = IndexOf4(obj);
			if (idx >= 0)
			{
				Remove4(idx);
				return true;
			}
			return false;
		}

		public override void ReplicateFrom(Object obj)
		{
			CheckActive();
			P2ListElementIterator i = Iterator4();
			while (i.HasNext())
			{
				P1ListElement elem = i.NextElement();
				elem.Delete(false);
			}
			i_first = null;
			i_last = null;
			P2LinkedList l4 = (P2LinkedList)obj;
			i = l4.Iterator4();
			while (i.HasNext())
			{
				Add4(i.NextElement());
			}
			UpdateInternal();
		}

		protected int Size4()
		{
			int size = 0;
			P2ListElementIterator i = Iterator4();
			while (i.HasNext())
			{
				size++;
				i.NextElement();
			}
			return size;
		}

		public override Object StoredTo(Transaction transaction)
		{
			if (GetTrans() == null)
			{
				SetTrans(transaction);
			}
			else
			{
				if (transaction != GetTrans())
				{
					return Replicate(GetTrans(), transaction);
				}
			}
			return this;
		}
	}
}