/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.References;

namespace Db4objects.Db4o.Internal
{
	/// <summary>TODO: Check if all time-consuming stuff is overridden!</summary>
	internal class TransactionObjectCarrier : LocalTransaction
	{
		internal TransactionObjectCarrier(ObjectContainerBase container, Transaction parentTransaction
			, IReferenceSystem referenceSystem) : base(container, parentTransaction, referenceSystem
			)
		{
		}

		public override void Commit()
		{
		}

		// do nothing
		internal override bool SupportsVirtualFields()
		{
			return false;
		}
	}
}
