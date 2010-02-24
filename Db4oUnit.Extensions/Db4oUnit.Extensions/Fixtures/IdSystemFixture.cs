/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal.Config;
using Db4objects.Db4o.Internal.Ids;

namespace Db4oUnit.Extensions.Fixtures
{
	public class IdSystemFixture : Db4oSolo
	{
		private readonly byte _idSystemType;

		public IdSystemFixture(byte idSystemType)
		{
			_idSystemType = idSystemType;
		}

		public IdSystemFixture()
		{
			_idSystemType = GlobalIdSystemFactory.Btree;
		}

		protected override IObjectContainer CreateDatabase(IConfiguration config)
		{
			IEmbeddedConfiguration embeddedConfiguration = Db4oLegacyConfigurationBridge.AsEmbeddedConfiguration
				(config);
			switch (_idSystemType)
			{
				case GlobalIdSystemFactory.PointerBased:
				{
					embeddedConfiguration.IdSystem.UsePointerBasedSystem();
					break;
				}

				case GlobalIdSystemFactory.Btree:
				{
					embeddedConfiguration.IdSystem.UseBTreeSystem();
					break;
				}

				default:
				{
					throw new InvalidOperationException();
					break;
				}
			}
			return base.CreateDatabase(config);
		}

		public override string Label()
		{
			string idSystemType = string.Empty;
			switch (_idSystemType)
			{
				case GlobalIdSystemFactory.PointerBased:
				{
					idSystemType = "PointerBased";
					break;
				}

				case GlobalIdSystemFactory.Btree:
				{
					idSystemType = "BTree";
					break;
				}

				default:
				{
					throw new InvalidOperationException();
					break;
				}
			}
			return "IdSystem-" + idSystemType + " " + base.Label();
		}
	}
}
