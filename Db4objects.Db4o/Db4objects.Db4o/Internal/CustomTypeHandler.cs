/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class CustomTypeHandler : ITypeHandler4
	{
		public virtual void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, bool topLevel, object obj, bool withIndirection)
		{
		}

		public virtual void CascadeActivation(Transaction a_trans, object a_object, int a_depth
			, bool a_activate)
		{
		}

		public virtual IReflectClass ClassReflector()
		{
			return null;
		}

		public virtual void Defrag(MarshallerFamily mf, BufferPair readers, bool redirect
			)
		{
		}

		public virtual void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer a_bytes)
		{
		}

		public virtual int GetID()
		{
			return 0;
		}

		public virtual bool HasFixedLength()
		{
			return false;
		}

		public virtual int LinkLength()
		{
			return 0;
		}

		public virtual object Read(MarshallerFamily mf, StatefulBuffer writer, bool redirect
			)
		{
			return null;
		}

		public virtual object ReadQuery(Transaction trans, MarshallerFamily mf, bool withRedirection
			, Db4objects.Db4o.Internal.Buffer reader, bool toArray)
		{
			return null;
		}

		public virtual QCandidate ReadSubCandidate(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates, bool withIndirection)
		{
			return null;
		}

		public virtual object Write(MarshallerFamily mf, object a_object, bool topLevel, 
			StatefulBuffer a_bytes, bool withIndirection, bool restoreLinkOffset)
		{
			return null;
		}

		public virtual int CompareTo(object obj)
		{
			return 0;
		}

		public virtual IComparable4 PrepareComparison(object obj)
		{
			return null;
		}
	}
}
