/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IVersionedTypeHandler : ITypeHandler4, IDeepClone
	{
		ITypeHandler4 UnversionedTemplate();
	}
}
