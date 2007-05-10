using Db4objects.Db4o;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.TA.Internal;
using Db4objects.Db4o.TA.Tests.Collections.Internal;
using Sharpen;

namespace Db4objects.Db4o.TA.Tests.Collections.Internal
{
	/// <summary>Shared implementation for a paged collection.</summary>
	/// <remarks>Shared implementation for a paged collection.</remarks>
	public class PagedBackingStore : IActivatable
	{
		public const int INITIAL_PAGE_COUNT = 16;

		private Page[] _pages = new Page[INITIAL_PAGE_COUNT];

		private int _top = 0;

		[System.NonSerialized]
		internal Activator _activator;

		public PagedBackingStore()
		{
			AddNewPage();
		}

		public virtual bool Add(object item)
		{
			Activate();
			return GetPageForAdd().Add(item);
		}

		public virtual int Size()
		{
			Activate();
			return _top * Page.PAGESIZE - LastPage().Capacity();
		}

		public virtual object Get(int itemIndex)
		{
			Activate();
			Page page = PageHolding(itemIndex);
			return page.Get(IndexInPage(itemIndex));
		}

		private Page LastPage()
		{
			return (Page)_pages[_top - 1];
		}

		private Page GetPageForAdd()
		{
			Page lastPage = LastPage();
			if (lastPage.AtCapacity())
			{
				lastPage = AddNewPage();
			}
			return lastPage;
		}

		private Page AddNewPage()
		{
			Page page = new Page(_top);
			if (_top == _pages.Length)
			{
				GrowPages();
			}
			_pages[_top] = page;
			_top++;
			return page;
		}

		private void GrowPages()
		{
			Page[] grown = new Page[_pages.Length * 2];
			System.Array.Copy(_pages, 0, grown, 0, _pages.Length);
			_pages = grown;
		}

		/// <summary>Will return the page that holds the index passed in.</summary>
		/// <remarks>
		/// Will return the page that holds the index passed in.
		/// For example, if pagesize == 100 and index == 525, then this will return page 5.
		/// </remarks>
		/// <param name="itemIndex"></param>
		/// <returns></returns>
		private Page PageHolding(int itemIndex)
		{
			return (Page)_pages[PageIndex(itemIndex)];
		}

		private int PageIndex(int itemIndex)
		{
			return itemIndex / Page.PAGESIZE;
		}

		private int IndexInPage(int itemIndex)
		{
			return itemIndex % Page.PAGESIZE;
		}

		public virtual void Bind(IObjectContainer container)
		{
			if (null != _activator)
			{
				_activator.AssertCompatible(container);
				return;
			}
			_activator = new Activator(container, this);
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
