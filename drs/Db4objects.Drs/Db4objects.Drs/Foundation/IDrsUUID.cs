/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Foundation
{
	public interface IDrsUUID
	{
		long GetLongPart();

		byte[] GetSignaturePart();
	}
}
