/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;

namespace Db4objects.Db4o.Defragment
{
	/// <summary>
	/// First step in the defragmenting process: Allocates pointer slots in the target file for
	/// each ID (but doesn't fill them in, yet) and registers the mapping from source pointer address
	/// to target pointer address.
	/// </summary>
	/// <remarks>
	/// First step in the defragmenting process: Allocates pointer slots in the target file for
	/// each ID (but doesn't fill them in, yet) and registers the mapping from source pointer address
	/// to target pointer address.
	/// </remarks>
	/// <exclude></exclude>
	public sealed class FirstPassCommand : IPassCommand
	{
		private IDMappingCollector _collector = new IDMappingCollector();

		public void ProcessClass(DefragmentServicesImpl context, ClassMetadata classMetadata
			, int id, int classIndexID)
		{
			_collector.CreateIDMapping(context, id, true);
			classMetadata.ForEachField(new _IProcedure4_23(this, context));
		}

		private sealed class _IProcedure4_23 : IProcedure4
		{
			public _IProcedure4_23(FirstPassCommand _enclosing, DefragmentServicesImpl context
				)
			{
				this._enclosing = _enclosing;
				this.context = context;
			}

			public void Apply(object arg)
			{
				FieldMetadata field = (FieldMetadata)arg;
				if (!field.IsVirtual() && field.HasIndex())
				{
					this._enclosing.ProcessBTree(context, field.GetIndex(context.SystemTrans()));
				}
			}

			private readonly FirstPassCommand _enclosing;

			private readonly DefragmentServicesImpl context;
		}

		public void ProcessObjectSlot(DefragmentServicesImpl context, ClassMetadata yapClass
			, int sourceID)
		{
			_collector.CreateIDMapping(context, sourceID, false);
		}

		/// <exception cref="Db4objects.Db4o.CorruptionException"></exception>
		public void ProcessClassCollection(DefragmentServicesImpl context)
		{
			_collector.CreateIDMapping(context, context.SourceClassCollectionID(), false);
		}

		public void ProcessBTree(DefragmentServicesImpl context, BTree btree)
		{
			context.RegisterBTreeIDs(btree, _collector);
		}

		public void Flush(DefragmentServicesImpl context)
		{
			_collector.Flush(context);
		}
	}
}
