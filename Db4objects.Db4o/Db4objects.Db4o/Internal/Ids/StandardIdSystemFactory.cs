/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Ids;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class StandardIdSystemFactory
	{
		public const byte Legacy = 0;

		public const byte PointerBased = 1;

		public const byte Default = PointerBased;

		public const byte Btree = 2;

		public const byte InMemory = 3;

		public const byte Custom = 4;

		public static IIdSystem NewInstance(LocalObjectContainer localContainer)
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

				case InMemory:
				{
					return new InMemoryIdSystem(localContainer);
				}

				case Custom:
				{
					IIdSystemFactory customIdSystemFactory = localContainer.ConfigImpl.CustomIdSystemFactory
						();
					if (customIdSystemFactory == null)
					{
						throw new Db4oFatalException("Custom IdSystem configured but no factory was found. See IdSystemConfiguration#useCustomSystem()"
							);
					}
					return customIdSystemFactory.NewInstance(localContainer, idSystemId);
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
