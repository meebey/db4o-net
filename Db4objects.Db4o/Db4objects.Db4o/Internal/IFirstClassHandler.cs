/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IFirstClassHandler
	{
		void CascadeActivation(Transaction trans, object obj, int depth, bool activate);

		/// <exception cref="Db4oIOException"></exception>
		void ReadCandidates(int handlerVersion, Db4objects.Db4o.Internal.Buffer buffer, QCandidates
			 candidates);

		ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer[]
			 a_bytes);
	}
}
