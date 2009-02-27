/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Freespace;

namespace Db4objects.Db4o.Tests.Common.Freespace
{
	public class FreespaceManagerTypeChangeSlotCountTestCase : ITestCase
	{
		private const int Size = 10000;

		private LocalObjectContainer _container;

		private IClosure4 _currentConfig;

		private string _fileName;

		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(FreespaceManagerTypeChangeSlotCountTestCase)).Run();
		}

		public FreespaceManagerTypeChangeSlotCountTestCase()
		{
			_fileName = Path.GetTempFileName();
			File4.Delete(_fileName);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMigrateFromRamToBTree()
		{
			CreateDatabaseUsingRamManager();
			MigrateToBTree();
			Reopen();
			CreateFreeSpace();
			IList initialSlots = GetSlots(_container.FreespaceManager());
			Reopen();
			IList currentSlots = GetSlots(_container.FreespaceManager());
			Assert.AreEqual(initialSlots, currentSlots);
			_container.Close();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMigrateFromBTreeToRam()
		{
			CreateDatabaseUsingBTreeManager();
			MigrateToRam();
			CreateFreeSpace();
			IList initialSlots = GetSlots(_container.FreespaceManager());
			Reopen();
			Assert.AreEqual(initialSlots, GetSlots(_container.FreespaceManager()));
			_container.Close();
		}

		private void Reopen()
		{
			_container.Close();
			Open();
		}

		private void CreateDatabaseUsingRamManager()
		{
			ConfigureRamManager();
			Open();
		}

		private void CreateDatabaseUsingBTreeManager()
		{
			ConfigureBTreeManager();
			Open();
		}

		private void Open()
		{
			_container = (LocalObjectContainer)Db4oFactory.OpenFile(((IConfiguration)_currentConfig
				.Run()), _fileName);
		}

		private void CreateFreeSpace()
		{
			Slot slot = _container.GetSlot(Size);
			_container.Free(slot);
		}

		/// <exception cref="System.Exception"></exception>
		private void MigrateToBTree()
		{
			_container.Close();
			ConfigureBTreeManager();
			Open();
		}

		private void ConfigureBTreeManager()
		{
			_currentConfig = new _IClosure4_90();
		}

		private sealed class _IClosure4_90 : IClosure4
		{
			public _IClosure4_90()
			{
			}

			public object Run()
			{
				IConfiguration config = Db4oFactory.NewConfiguration();
				config.Freespace().UseBTreeSystem();
				return config;
			}
		}

		/// <exception cref="System.Exception"></exception>
		private void MigrateToRam()
		{
			_container.Close();
			ConfigureRamManager();
			Open();
		}

		private void ConfigureRamManager()
		{
			_currentConfig = new _IClosure4_105();
		}

		private sealed class _IClosure4_105 : IClosure4
		{
			public _IClosure4_105()
			{
			}

			public object Run()
			{
				IConfiguration config = Db4oFactory.NewConfiguration();
				config.Freespace().UseRamSystem();
				return config;
			}
		}

		private IList GetSlots(IFreespaceManager freespaceManager)
		{
			IList retVal = new ArrayList();
			freespaceManager.Traverse(new _IVisitor4_114(retVal));
			return retVal;
		}

		private sealed class _IVisitor4_114 : IVisitor4
		{
			public _IVisitor4_114(IList retVal)
			{
				this.retVal = retVal;
			}

			public void Visit(object obj)
			{
				retVal.Add(obj);
			}

			private readonly IList retVal;
		}
	}
}
