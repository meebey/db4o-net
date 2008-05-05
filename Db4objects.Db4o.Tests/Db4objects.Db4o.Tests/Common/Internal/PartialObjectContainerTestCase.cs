/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Internal;

namespace Db4objects.Db4o.Tests.Common.Internal
{
	/// <exclude></exclude>
	public class PartialObjectContainerTestCase : AbstractDb4oTestCase, IOptOutTA
	{
		public static void Main(string[] arguments)
		{
			new PartialObjectContainerTestCase().RunSolo();
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.BlockSize(8);
		}

		public virtual void TestBlocksToBytes()
		{
			int[] blocks = new int[] { 0, 1, 8, 9 };
			int[] bytes = new int[] { 0, 8, 64, 72 };
			for (int i = 0; i < blocks.Length; i++)
			{
				Assert.AreEqual(bytes[i], LocalContainer().BlocksToBytes(blocks[i]));
			}
		}

		public virtual void TestBytesToBlocks()
		{
			int[] bytes = new int[] { 0, 1, 2, 7, 8, 9, 16, 17, 799, 800, 801 };
			int[] blocks = new int[] { 0, 1, 1, 1, 1, 2, 2, 3, 100, 100, 101 };
			for (int i = 0; i < blocks.Length; i++)
			{
				Assert.AreEqual(blocks[i], LocalContainer().BytesToBlocks(bytes[i]));
			}
		}

		private ObjectContainerBase LocalContainer()
		{
			return Stream().Container();
		}
	}
}
