/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IFirstClassHandler
	{
		void CascadeActivation(Transaction trans, object obj, IActivationDepth depth);

		/// <exception cref="Db4oIOException"></exception>
		void ReadCandidates(int handlerVersion, BufferImpl buffer, QCandidates candidates
			);

		ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily mf, BufferImpl
			[] a_bytes);
	}
}
