/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class ReferenceSystemRegistry
	{
		private readonly Collection4 _referenceSystems = new Collection4();

		public virtual void RemoveId(int id)
		{
			RemoveReference(new _IReferenceSource_16(id));
		}

		private sealed class _IReferenceSource_16 : ReferenceSystemRegistry.IReferenceSource
		{
			public _IReferenceSource_16(int id)
			{
				this.id = id;
			}

			public ObjectReference ReferenceFrom(IReferenceSystem referenceSystem)
			{
				return referenceSystem.ReferenceForId(id);
			}

			private readonly int id;
		}

		public virtual void RemoveObject(object obj)
		{
			RemoveReference(new _IReferenceSource_24(obj));
		}

		private sealed class _IReferenceSource_24 : ReferenceSystemRegistry.IReferenceSource
		{
			public _IReferenceSource_24(object obj)
			{
				this.obj = obj;
			}

			public ObjectReference ReferenceFrom(IReferenceSystem referenceSystem)
			{
				return referenceSystem.ReferenceForObject(obj);
			}

			private readonly object obj;
		}

		public virtual void RemoveReference(ObjectReference reference)
		{
			RemoveReference(new _IReferenceSource_32(reference));
		}

		private sealed class _IReferenceSource_32 : ReferenceSystemRegistry.IReferenceSource
		{
			public _IReferenceSource_32(ObjectReference reference)
			{
				this.reference = reference;
			}

			public ObjectReference ReferenceFrom(IReferenceSystem referenceSystem)
			{
				return reference;
			}

			private readonly ObjectReference reference;
		}

		private void RemoveReference(ReferenceSystemRegistry.IReferenceSource referenceSource
			)
		{
			IEnumerator i = _referenceSystems.GetEnumerator();
			while (i.MoveNext())
			{
				IReferenceSystem referenceSystem = (IReferenceSystem)i.Current;
				ObjectReference reference = referenceSource.ReferenceFrom(referenceSystem);
				if (reference != null)
				{
					referenceSystem.RemoveReference(reference);
				}
			}
		}

		public virtual void AddReferenceSystem(IReferenceSystem referenceSystem)
		{
			_referenceSystems.Add(referenceSystem);
		}

		public virtual void RemoveReferenceSystem(IReferenceSystem referenceSystem)
		{
			_referenceSystems.Remove(referenceSystem);
		}

		private interface IReferenceSource
		{
			ObjectReference ReferenceFrom(IReferenceSystem referenceSystem);
		}
	}
}
