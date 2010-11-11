/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Inside
{
	public interface IReadonlyReplicationProviderSignature
	{
		long GetId();

		byte[] GetSignature();

		long GetCreated();
	}
}
