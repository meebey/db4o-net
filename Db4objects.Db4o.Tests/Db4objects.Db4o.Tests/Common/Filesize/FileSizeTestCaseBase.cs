/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Filesize;

namespace Db4objects.Db4o.Tests.Common.Filesize
{
	public abstract class FileSizeTestCaseBase : AbstractDb4oTestCase
	{
		public class Item
		{
			public int _int;
		}

		protected virtual int FileSize()
		{
			LocalObjectContainer localContainer = Fixture().FileSession();
			IoAdaptedObjectContainer container = (IoAdaptedObjectContainer)localContainer;
			container.SyncFiles();
			long length = new Sharpen.IO.File(container.FileName()).Length();
			return (int)length;
		}

		protected virtual void ProduceSomeFreeSpace()
		{
			IFreespaceManager fm = Container().FreespaceManager();
			int length = 300;
			Slot slot = Container().GetSlot(length);
			Db4objects.Db4o.Internal.Buffer buffer = new Db4objects.Db4o.Internal.Buffer(length
				);
			Container().WriteBytes(buffer, slot.Address(), 0);
			fm.Free(slot);
		}

		protected virtual void StoreSomeItems()
		{
			for (int i = 0; i < 3; i++)
			{
				Store(new FileSizeTestCaseBase.Item());
			}
			Db().Commit();
		}

		protected virtual LocalObjectContainer Container()
		{
			return Fixture().FileSession();
		}
	}
}
