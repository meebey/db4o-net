/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	internal class WriteUpdateProcessor
	{
		private readonly Db4objects.Db4o.Internal.LocalTransaction _localTransaction;

		private readonly int _id;

		private readonly ClassMetadata _clazz;

		private readonly ArrayType _typeInfo;

		private int _cascade;

		public WriteUpdateProcessor(Db4objects.Db4o.Internal.LocalTransaction localTransaction
			, int id, ClassMetadata clazz, ArrayType typeInfo, int cascade)
		{
			_localTransaction = localTransaction;
			_id = id;
			_clazz = clazz;
			_typeInfo = typeInfo;
			_cascade = cascade;
		}

		public virtual void Run()
		{
			_localTransaction.CheckSynchronization();
			if (DTrace.enabled)
			{
				DTrace.WriteUpdateAdjustIndexes.Log(_id);
			}
			if (AlreadyHandled())
			{
				return;
			}
			Slot slot = _localTransaction.GetCurrentSlotOfID(_id);
			if (HandledAsReAdd(slot))
			{
				return;
			}
			if (HandledWithNoChildIndexModification(slot))
			{
				return;
			}
			StatefulBuffer objectBytes = (StatefulBuffer)Container().ReadReaderOrWriterBySlot
				(LocalTransaction(), _id, false, slot);
			UpdateChildIndexes(objectBytes);
			FreeSlotOnCommit(objectBytes);
		}

		private LocalObjectContainer Container()
		{
			return _localTransaction.File();
		}

		private void FreeSlotOnCommit(StatefulBuffer objectBytes)
		{
			_localTransaction.SlotFreeOnCommit(_id, new Slot(objectBytes.GetAddress(), objectBytes
				.Length()));
		}

		private void UpdateChildIndexes(StatefulBuffer objectBytes)
		{
			ObjectHeader oh = new ObjectHeader(Container(), _clazz, objectBytes);
			DeleteInfo info = (DeleteInfo)TreeInt.Find(_localTransaction._delete, _id);
			if (info != null)
			{
				if (info._cascade > _cascade)
				{
					_cascade = info._cascade;
				}
			}
			objectBytes.SetCascadeDeletes(_cascade);
			DeleteContextImpl context = new DeleteContextImpl(objectBytes, oh, _clazz.ClassReflector
				(), null);
			_clazz.DeleteMembers(context, _typeInfo, true);
		}

		private bool HandledAsReAdd(Slot slot)
		{
			if (slot != null && slot.Address() > 0)
			{
				return false;
			}
			_clazz.AddToIndex(LocalTransaction(), _id);
			return true;
		}

		private bool AlreadyHandled()
		{
			TreeInt newNode = new TreeInt(_id);
			_localTransaction._writtenUpdateAdjustedIndexes = Tree.Add(_localTransaction._writtenUpdateAdjustedIndexes
				, newNode);
			return !newNode.WasAddedToTree();
		}

		private bool HandledWithNoChildIndexModification(Slot slot)
		{
			if (!_clazz.CanUpdateFast())
			{
				return false;
			}
			_localTransaction.SlotFreeOnCommit(_id, slot);
			return true;
		}

		private Db4objects.Db4o.Internal.LocalTransaction LocalTransaction()
		{
			return _localTransaction;
		}
	}
}
