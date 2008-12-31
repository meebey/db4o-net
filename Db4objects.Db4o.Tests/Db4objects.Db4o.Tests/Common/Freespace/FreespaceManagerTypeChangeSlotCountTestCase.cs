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
	/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
	public class FreespaceManagerTypeChangeSlotCountTestCase : ITestCase
	{
		private const int Size = 10000;

		private LocalObjectContainer _container;

		private IConfiguration _currentConfig;

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
			_container = (LocalObjectContainer)Db4oFactory.OpenFile(_currentConfig, _fileName
				);
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
			_currentConfig = Db4oFactory.NewConfiguration();
			_currentConfig.Freespace().UseBTreeSystem();
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
			_currentConfig = Db4oFactory.NewConfiguration();
			_currentConfig.Freespace().UseRamSystem();
		}

		private IList GetSlots(IFreespaceManager freespaceManager)
		{
			IList retVal = new ArrayList();
			freespaceManager.Traverse(new _IVisitor4_109(retVal));
			return retVal;
		}

		private sealed class _IVisitor4_109 : IVisitor4
		{
			public _IVisitor4_109(IList retVal)
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
