/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Inside;
using Db4objects.Db4o.Types;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o
{
	internal class P2HashMap : P1Collection, IDb4oMap, ITransactionListener
	{
		protected static float FILL = 0.6F;

		[NonSerialized]
		protected int i_changes;

		[NonSerialized]
		protected bool i_dontStoreOnDeactivate;

		public P1HashElement[] i_entries;
		public int i_mask;
		public int i_maximumSize;
		public int i_size;
		public int i_type;  // 0 == default hash, 1 == ID hash

		[NonSerialized]
		internal P1HashElement[] i_table;

		public int i_tableSize;

		internal P2HashMap()
			: base()
		{
		}

		internal P2HashMap(int a_size)
			: base()
		{
			a_size = (int)(a_size / FILL);
			i_tableSize = 1;
			while (i_tableSize < a_size)
			{
				i_tableSize = i_tableSize << 1;
			}
			i_mask = i_tableSize - 1;
			i_maximumSize = (int)(i_tableSize * FILL);
			i_table = new P1HashElement[i_tableSize];
		}

		public void Add(Object a_key, Object a_value)
		{
			lock (this.StreamLock())
			{
				CheckActive();
				Put4(a_key, a_value);
			}
		}

		public void Clear()
		{
			lock (this.StreamLock())
			{
				CheckActive();
				if (i_size != 0)
				{
					for (int i = 0; i < i_table.Length; i++)
					{
						DeleteAllElements(i_table[i]);
						i_table[i] = null;
					}
					for (int i = 0; i < i_entries.Length; i++)
					{
						i_entries[i] = null;
					}
					i_size = 0;
					Modified();
				}
			}
		}

		public bool Contains(Object obj)
		{
			lock (this.StreamLock())
			{
				CheckActive();
				return Get4(obj) != null;
			}
		}

		public void CopyTo(Array arr, int pos)
		{
			lock (this.StreamLock())
			{
				this.CheckActive();
				P2HashMapIterator i = new P2HashMapIterator(this);
				while (i.HasNext())
				{
					Object key = i.Next();
					arr.SetValue(new DictionaryEntry(key, Get4(key)), pos++);
				}
			}
		}

		public int Count
		{
			get
			{
				lock (this.StreamLock())
				{
					CheckActive();
					return i_size;
				}
			}
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return (IDictionaryEnumerator)GetEnumerator1();
		}

		private int HashOf(Object key)
		{
			if (i_type == 1)
			{
				int id = (int)GetIDOf(key);
				if (id == 0)
				{
					Exceptions4.ThrowRuntimeException(62);
				}
				return id;
			}
			return key.GetHashCode();
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

		public ICollection Keys
		{
			get
			{
				lock (this.StreamLock())
				{
					CheckActive();
					return new P2HashMapKeySet(this);
				}
			}
		}

		public void Remove(Object obj)
		{
			lock (this.StreamLock())
			{
				this.CheckActive();
				Remove4(obj);
			}
		}

		public Object SyncRoot
		{
			get
			{
				this.CheckActive();
				return StreamLock();
			}
		}

		public Object this[object a_key]
		{
			get
			{
				lock (StreamLock())
				{
					CheckActive();
					return Get4(a_key);
				}
			}

			set
			{
				lock (this.StreamLock())
				{
					CheckActive();
					Put4(a_key, value);
				}
			}
		}

		public ICollection Values
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override int ActivationDepth()
		{
			return 2;
		}

		public override int AdjustReadDepth(int i)
		{
			return 2;
		}

		public override void CheckActive()
		{
			base.CheckActive();
			if (i_table == null)
			{
				i_table = new P1HashElement[i_tableSize];
				if (i_entries != null)
				{
					for (int i = 0; i < i_entries.Length; i++)
					{
						if (i_entries[i] != null)
						{
							i_entries[i].CheckActive();
							i_table[i_entries[i].i_position] = i_entries[i];
						}
					}
				}
				i_changes = 0;

				// FIXME: reducing the table in size can be a problem during defragment in 
				//        C/S mode on P2HashMaps that were partially stored uncommitted.

				//                if ((i_size + 1) * 10 < i_tableSize) {
				//                    i_tableSize = i_size + 1;
				//                    IncreaseSize();
				//                    Modified();
				//                }

			}
		}

		public bool ContainsValue(Object obj)
		{
			throw new NotSupportedException();
		}

		public override Object CreateDefault(Transaction transaction)
		{
			CheckActive();
			P2HashMap m4 = new P2HashMap(i_size);
			m4.SetTrans(transaction);
			P2HashMapIterator i = new P2HashMapIterator(this);
			while (i.HasNext())
			{
				Object obj1 = i.Next();
				m4.Put4(obj1, Get4(obj1));
			}
			return m4;
		}

		protected void DeleteAllElements(P1HashElement a_entry)
		{
			if (a_entry != null)
			{
				a_entry.CheckActive();
				DeleteAllElements((P1HashElement)a_entry.i_next);
				a_entry.Delete(i_deleteRemoved);
			}
		}

		protected bool Equals(P1HashElement phe, int i, Object obj)
		{
			return phe.i_hashCode == i && phe.ActivatedKey(ElementActivationDepth()).Equals(obj);
		}

		public Object Get(Object obj)
		{
			lock (this.StreamLock())
			{
				CheckActive();
				return Get4(obj);
			}
		}

		internal Object Get4(Object obj)
		{
			int hash = HashOf(obj);
			for (P1HashElement phe = i_table[hash & i_mask]; phe != null; phe = (P1HashElement)phe.i_next)
			{
				phe.CheckActive();
				if (Equals(phe, hash, obj))
				{
					return phe.ActivatedObject(ElementActivationDepth());
				}
			}
			return null;
		}

		protected override IEnumerator GetEnumerator1()
		{
			lock (this.StreamLock())
			{
				this.CheckActive();
				return new P2HashMapIterator(this);
			}
		}

		protected void IncreaseSize()
		{
			i_tableSize = i_tableSize << 1;
			i_maximumSize = (int)(i_tableSize * FILL);
			i_mask = i_tableSize - 1;
			P1HashElement[] temp = i_table;
			i_table = new P1HashElement[i_tableSize];
			for (int i = 0; i < temp.Length; i++)
			{
				Reposition(temp[i]);
			}
		}

		internal void Modified()
		{
			if (GetTrans() != null)
			{
				if (i_changes == 0)
				{
					GetTrans().AddTransactionListener(this);
				}
				i_changes++;
			}
		}

		public void PostRollback()
		{
			i_dontStoreOnDeactivate = true;
			Deactivate();
			i_dontStoreOnDeactivate = false;
		}

		public void PreCommit()
		{
			if (i_changes > 0)
			{
				Collection4 col = new Collection4();
				for (int i = 0; i < i_table.Length; i++)
				{
					if (i_table[i] != null)
					{
						i_table[i].CheckActive();
						if (i_table[i].i_position != i)
						{
							i_table[i].i_position = i;
							i_table[i].Update();
						}
						col.Add(i_table[i]);
					}
				}
				if (i_entries == null || i_entries.Length != col.Size())
				{
					i_entries = new P1HashElement[col.Size()];
				}
				int j = 0;
				foreach (object item in col)
				{
					i_entries[j++] = (P1HashElement)item;
				}
				Store(2);
			}
			i_changes = 0;
		}

		public override void PreDeactivate()
		{
			if (!i_dontStoreOnDeactivate)
			{
				PreCommit();
			}
			i_table = null;
		}

		protected Object Put4(Object a_key, Object a_value)
		{
			int hash = HashOf(a_key);
			P1HashElement entry = new P1HashElement(this.GetTrans(), null, a_key, hash, a_value);
			i_size++;
			if (i_size > i_maximumSize)
			{
				IncreaseSize();
			}
			Modified();
			int index = entry.i_hashCode & i_mask;
			P1HashElement phe = i_table[index];
			P1HashElement last = null;
			while (phe != null)
			{
				phe.CheckActive();
				if (Equals(phe, entry.i_hashCode, a_key))
				{
					i_size--;
					Object ret = phe.ActivatedObject(ElementActivationDepth());
					entry.i_next = phe.i_next;
					this.Store(entry);
					if (last != null)
					{
						last.i_next = entry;
						last.Update();
					}
					else
					{
						i_table[index] = entry;
					}
					phe.Delete(i_deleteRemoved);
					return ret;
				}
				last = phe;
				phe = (P1HashElement)phe.i_next;
			}
			entry.i_next = i_table[index];
			i_table[index] = entry;
			this.Store(entry);
			return null;
		}

		/*
		public Object Remove(Object obj) {
			lock (this.StreamLock()) {
				CheckActive();
				return Remove4(obj);
			}
		}
		*/

		internal Object Remove4(Object a_key)
		{
			int hash = HashOf(a_key);
			P1HashElement phe = i_table[hash & i_mask];
			P1HashElement last = null;
			for (; phe != null; phe = (P1HashElement)phe.i_next)
			{
				phe.CheckActive();
				if (Equals(phe, hash, a_key))
				{
					if (last != null)
					{
						last.i_next = phe.i_next;
						last.Update();
					}
					else
					{
						i_table[hash & i_mask] = (P1HashElement)phe.i_next;
					}
					Modified();
					i_size--;
					Object obj = phe.ActivatedObject(ElementActivationDepth());
					phe.Delete(i_deleteRemoved);
					return obj;
				}
				last = phe;
			}
			return null;
		}

		public void ReplicateFrom(Object obj)
		{
			CheckActive();
			if (i_entries != null)
			{
				for (int i = 0; i < i_entries.Length; i++)
				{
					if (i_entries[i] != null)
					{
						i_entries[i].Delete(false);
					}
					i_entries[i] = null;
				}
			}
			if (i_table != null)
			{
				for (int i = 0; i < i_table.Length; i++)
				{
					i_table[i] = null;
				}
			}
			i_size = 0;

			P2HashMap m4 = (P2HashMap)obj;
			m4.CheckActive();
			P2HashMapIterator it = new P2HashMapIterator(m4);
			while (it.HasNext())
			{
				Object key = it.Next();
				Put4(key, m4.Get4(key));
			}

			Modified();
		}


		protected void Reposition(P1HashElement a_entry)
		{
			if (a_entry != null)
			{
				Reposition((P1HashElement)a_entry.i_next);
				a_entry.CheckActive();
				Object oldNext = a_entry.i_next;
				a_entry.i_next = i_table[a_entry.i_hashCode & i_mask];
				if (a_entry.i_next != oldNext)
				{
					a_entry.Update();
				}
				i_table[a_entry.i_hashCode & i_mask] = a_entry;
			}
		}

		public override Object StoredTo(Transaction transaction)
		{
			if (this.GetTrans() == null)
			{
				this.SetTrans(transaction);
				Modified();
			}
			else
			{
				if (transaction != this.GetTrans())
				{
					return Replicate(GetTrans(), transaction);
				}
			}
			return this;
		}
	}
}