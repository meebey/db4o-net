/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.TA.Tests.Collections.Internal
{
	public class Page : IActivatable
	{
		public const int PAGESIZE = 100;

		private object[] _data = new object[PAGESIZE];

		private int _top = 0;

		private int _pageIndex;

		[System.NonSerialized]
		private bool _dirty = false;

		[System.NonSerialized]
		internal IActivator _activator;

		public Page(int pageIndex)
		{
			_pageIndex = pageIndex;
		}

		public virtual bool Add(object obj)
		{
			Activate();
			_dirty = true;
			_data[_top++] = obj;
			return true;
		}

		public virtual int Size()
		{
			Activate();
			return _top;
		}

		public virtual object Get(int indexInPage)
		{
			Activate();
			_dirty = true;
			return _data[indexInPage];
		}

		public virtual bool IsDirty()
		{
			return _dirty;
		}

		public virtual void SetDirty(bool dirty)
		{
			_dirty = dirty;
		}

		public virtual int GetPageIndex()
		{
			Activate();
			return _pageIndex;
		}

		public virtual bool AtCapacity()
		{
			return Capacity() == 0;
		}

		public virtual int Capacity()
		{
			Activate();
			return Db4objects.Db4o.TA.Tests.Collections.Internal.Page.PAGESIZE - Size();
		}

		public virtual void Bind(IActivator activator)
		{
			if (null != _activator)
			{
				throw new InvalidOperationException();
			}
			_activator = activator;
		}

		private void Activate()
		{
			if (_activator == null)
			{
				return;
			}
			_activator.Activate();
		}
	}
}
