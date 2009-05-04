/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Common.Freespace
{
	public abstract class FileSizeTestCaseBase : AbstractDb4oTestCase, IOptOutTA
	{
		protected virtual int DatabaseFileSize()
		{
			LocalObjectContainer localContainer = Fixture().FileSession();
			localContainer.SyncFiles();
			long length = new Sharpen.IO.File(localContainer.FileName()).Length();
			return (int)length;
		}
	}
}
