/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Internal
{
	public interface ICascadingTypeHandler : ITypeHandler4
	{
		void CascadeActivation(Transaction trans, object obj, IActivationDepth depth);
	}
}
