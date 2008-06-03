/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Tests.Jre5.Collections.Typehandler
{
	//SHA: Changed from insterface to abstract class
	public abstract class IItemFactory
	{
		internal static string ListFieldName = "_list";

		public abstract object NewItem();

		public abstract Type ItemClass();

		public abstract Type ListClass();
	}
}
