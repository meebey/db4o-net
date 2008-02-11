/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Common.Freespace
{
	public abstract class FileSizeTestCaseBase : AbstractDb4oTestCase, IOptOutTA
	{
		protected virtual int DatabaseFileSize()
		{
			LocalObjectContainer localContainer = Fixture().FileSession();
			IoAdaptedObjectContainer container = (IoAdaptedObjectContainer)localContainer;
			container.SyncFiles();
			long length = new Sharpen.IO.File(container.FileName()).Length();
			return (int)length;
		}
	}
}
