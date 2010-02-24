/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Ids;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class GlobalIdSystemFactory
	{
		public const byte Legacy = 0;

		public const byte PointerBased = 1;

		public const byte Default = PointerBased;

		public const byte Btree = 2;

		public static IGlobalIdSystem CreateNew(LocalObjectContainer localContainer)
		{
			byte idSystemType = localContainer.SystemData().IdSystemType();
			int idSystemId = localContainer.SystemData().IdSystemID();
			switch (idSystemType)
			{
				case Legacy:
				{
					return new PointerBasedIdSystem(localContainer);
				}

				case PointerBased:
				{
					return new PointerBasedIdSystem(localContainer);
				}

				case Btree:
				{
					return new BTreeIdSystem(idSystemId);
				}

				default:
				{
					return new PointerBasedIdSystem(localContainer);
					break;
				}
			}
		}
	}
}
