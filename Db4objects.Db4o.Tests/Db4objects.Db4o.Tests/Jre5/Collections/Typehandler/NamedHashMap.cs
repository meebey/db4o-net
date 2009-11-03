/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System.Collections;

namespace Db4objects.Db4o.Tests.Jre5.Collections.Typehandler
{
	[System.Serializable]
	public class NamedHashMap : Hashtable
	{
		public string name;
	}
}
