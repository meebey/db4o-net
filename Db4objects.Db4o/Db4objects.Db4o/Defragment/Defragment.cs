/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Defragment
{
	/// <summary>defragments database files.</summary>
	/// <remarks>
	/// defragments database files.
	/// <br /><br />db4o structures storage inside database files as free and occupied slots, very
	/// much like a file system - and just like a file system it can be fragmented.<br /><br />
	/// The simplest way to defragment a database file:<br /><br />
	/// <code>Defragment.defrag("sample.yap");</code><br /><br />
	/// This will move the file to "sample.yap.backup", then create a defragmented
	/// version of this file in the original position, using a temporary file
	/// "sample.yap.mapping". If the backup file already exists, this will throw an
	/// exception and no action will be taken.<br /><br />
	/// For more detailed configuration of the defragmentation process, provide a
	/// DefragmentConfig instance:<br /><br />
	/// <code>DefragmentConfig config=new DefragmentConfig("sample.yap","sample.bap",new BTreeIDMapping("sample.map"));<br />
	/// config.forceBackupDelete(true);<br />
	/// config.storedClassFilter(new AvailableClassFilter());<br />
	/// config.db4oConfig(db4oConfig);<br />
	/// Defragment.defrag(config);</code><br /><br />
	/// This will move the file to "sample.bap", then create a defragmented version
	/// of this file in the original position, using a temporary file "sample.map" for BTree mapping.
	/// If the backup file already exists, it will be deleted. The defragmentation
	/// process will skip all classes that have instances stored within the yap file,
	/// but that are not available on the class path (through the current
	/// classloader). Custom db4o configuration options are read from the
	/// <see cref="IConfiguration">IConfiguration</see>
	/// passed as db4oConfig.
	/// <strong>Note:</strong> For some specific, non-default configuration settings like
	/// UUID generation, etc., you <strong>must</strong> pass an appropriate db4o configuration,
	/// just like you'd use it within your application for normal database operation.
	/// </remarks>
	public class Defragment
	{
		/// <summary>
		/// Renames the file at the given original path to a backup file and then
		/// builds a defragmented version of the file in the original place.
		/// </summary>
		/// <remarks>
		/// Renames the file at the given original path to a backup file and then
		/// builds a defragmented version of the file in the original place.
		/// </remarks>
		/// <param name="origPath">The path to the file to be defragmented.</param>
		/// <exception cref="IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(string origPath)
		{
			Defrag(new DefragmentConfig(origPath), new Defragment.NullListener());
		}

		/// <summary>
		/// Renames the file at the given original path to the given backup file and
		/// then builds a defragmented version of the file in the original place.
		/// </summary>
		/// <remarks>
		/// Renames the file at the given original path to the given backup file and
		/// then builds a defragmented version of the file in the original place.
		/// </remarks>
		/// <param name="origPath">The path to the file to be defragmented.</param>
		/// <param name="backupPath">The path to the backup file to be created.</param>
		/// <exception cref="IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(string origPath, string backupPath)
		{
			Defrag(new DefragmentConfig(origPath, backupPath), new Defragment.NullListener());
		}

		/// <summary>
		/// Renames the file at the configured original path to the configured backup
		/// path and then builds a defragmented version of the file in the original
		/// place.
		/// </summary>
		/// <remarks>
		/// Renames the file at the configured original path to the configured backup
		/// path and then builds a defragmented version of the file in the original
		/// place.
		/// </remarks>
		/// <param name="config">The configuration for this defragmentation run.</param>
		/// <exception cref="IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(DefragmentConfig config)
		{
			Defrag(config, new Defragment.NullListener());
		}

		/// <summary>
		/// Renames the file at the configured original path to the configured backup
		/// path and then builds a defragmented version of the file in the original
		/// place.
		/// </summary>
		/// <remarks>
		/// Renames the file at the configured original path to the configured backup
		/// path and then builds a defragmented version of the file in the original
		/// place.
		/// </remarks>
		/// <param name="config">The configuration for this defragmentation run.</param>
		/// <param name="listener">
		/// A listener for status notifications during the defragmentation
		/// process.
		/// </param>
		/// <exception cref="IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(DefragmentConfig config, IDefragmentListener listener)
		{
			Sharpen.IO.File backupFile = new Sharpen.IO.File(config.BackupPath());
			if (backupFile.Exists())
			{
				if (!config.ForceBackupDelete())
				{
					throw new IOException("Could not use '" + config.BackupPath() + "' as backup path - file exists."
						);
				}
				backupFile.Delete();
			}
			System.IO.File.Move(config.OrigPath(), config.BackupPath());
			if (config.FileNeedsUpgrade())
			{
				UpgradeFile(config);
			}
			DefragContextImpl context = new DefragContextImpl(config, listener);
			int newClassCollectionID = 0;
			int targetIdentityID = 0;
			int targetUuidIndexID = 0;
			try
			{
				FirstPass(context, config);
				SecondPass(context, config);
				DefragUnindexed(context);
				newClassCollectionID = context.MappedID(context.SourceClassCollectionID());
				context.TargetClassCollectionID(newClassCollectionID);
				int sourceIdentityID = context.DatabaseIdentityID(DefragContextImpl.SOURCEDB);
				targetIdentityID = context.MappedID(sourceIdentityID, 0);
				targetUuidIndexID = context.MappedID(context.SourceUuidIndexID(), 0);
			}
			catch (CorruptionException exc)
			{
				Sharpen.Runtime.PrintStackTrace(exc);
			}
			finally
			{
				context.Close();
			}
			if (targetIdentityID > 0)
			{
				SetIdentity(config.OrigPath(), targetIdentityID, targetUuidIndexID, config.BlockSize
					());
			}
			else
			{
				listener.NotifyDefragmentInfo(new DefragmentInfo("No database identity found in original file."
					));
			}
		}

		private static void UpgradeFile(DefragmentConfig config)
		{
			File4.Copy(config.BackupPath(), config.TempPath());
			IConfiguration db4oConfig = (IConfiguration)((Config4Impl)config.Db4oConfig()).DeepClone
				(null);
			db4oConfig.AllowVersionUpdates(true);
			IObjectContainer db = Db4oFactory.OpenFile(db4oConfig, config.TempPath());
			db.Close();
		}

		private static void DefragUnindexed(DefragContextImpl context)
		{
			IEnumerator unindexedIDs = context.UnindexedIDs();
			while (unindexedIDs.MoveNext())
			{
				int origID = ((int)unindexedIDs.Current);
				ReaderPair.ProcessCopy(context, origID, new _ISlotCopyHandler_168(), true);
			}
		}

		private sealed class _ISlotCopyHandler_168 : ISlotCopyHandler
		{
			public _ISlotCopyHandler_168()
			{
			}

			public void ProcessCopy(ReaderPair readers)
			{
				ClassMetadata.DefragObject(readers);
			}
		}

		private static void SetIdentity(string targetFile, int targetIdentityID, int targetUuidIndexID
			, int blockSize)
		{
			LocalObjectContainer targetDB = (LocalObjectContainer)Db4oFactory.OpenFile(DefragmentConfig
				.VanillaDb4oConfig(blockSize), targetFile);
			try
			{
				Db4oDatabase identity = (Db4oDatabase)targetDB.GetByID(targetIdentityID);
				targetDB.SetIdentity(identity);
				targetDB.SystemData().UuidIndexId(targetUuidIndexID);
			}
			finally
			{
				targetDB.Close();
			}
		}

		private static void FirstPass(DefragContextImpl context, DefragmentConfig config)
		{
			Pass(context, config, new FirstPassCommand());
		}

		private static void SecondPass(DefragContextImpl context, DefragmentConfig config
			)
		{
			Pass(context, config, new SecondPassCommand(config.ObjectCommitFrequency()));
		}

		private static void Pass(DefragContextImpl context, DefragmentConfig config, IPassCommand
			 command)
		{
			command.ProcessClassCollection(context);
			IStoredClass[] classes = context.StoredClasses(DefragContextImpl.SOURCEDB);
			for (int classIdx = 0; classIdx < classes.Length; classIdx++)
			{
				ClassMetadata yapClass = (ClassMetadata)classes[classIdx];
				if (!config.StoredClassFilter().Accept(yapClass))
				{
					continue;
				}
				ProcessYapClass(context, yapClass, command);
				command.Flush(context);
				if (config.ObjectCommitFrequency() > 0)
				{
					context.TargetCommit();
				}
			}
			BTree uuidIndex = context.SourceUuidIndex();
			if (uuidIndex != null)
			{
				command.ProcessBTree(context, uuidIndex);
			}
			command.Flush(context);
			context.TargetCommit();
		}

		private static void ProcessYapClass(DefragContextImpl context, ClassMetadata curClass
			, IPassCommand command)
		{
			ProcessClassIndex(context, curClass, command);
			if (!ParentHasIndex(curClass))
			{
				ProcessObjectsForYapClass(context, curClass, command);
			}
			ProcessYapClassAndFieldIndices(context, curClass, command);
		}

		private static bool ParentHasIndex(ClassMetadata curClass)
		{
			ClassMetadata parentClass = curClass.i_ancestor;
			while (parentClass != null)
			{
				if (parentClass.HasIndex())
				{
					return true;
				}
				parentClass = parentClass.i_ancestor;
			}
			return false;
		}

		private static void ProcessObjectsForYapClass(DefragContextImpl context, ClassMetadata
			 curClass, IPassCommand command)
		{
			bool withStringIndex = WithFieldIndex(curClass);
			context.TraverseAll(curClass, new _IVisitor4_261(command, context, curClass, withStringIndex
				));
		}

		private sealed class _IVisitor4_261 : IVisitor4
		{
			public _IVisitor4_261(IPassCommand command, DefragContextImpl context, ClassMetadata
				 curClass, bool withStringIndex)
			{
				this.command = command;
				this.context = context;
				this.curClass = curClass;
				this.withStringIndex = withStringIndex;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				try
				{
					command.ProcessObjectSlot(context, curClass, id, withStringIndex);
				}
				catch (CorruptionException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
				catch (IOException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}

			private readonly IPassCommand command;

			private readonly DefragContextImpl context;

			private readonly ClassMetadata curClass;

			private readonly bool withStringIndex;
		}

		private static bool WithFieldIndex(ClassMetadata clazz)
		{
			IEnumerator fieldIter = clazz.Fields();
			while (fieldIter.MoveNext())
			{
				FieldMetadata curField = (FieldMetadata)fieldIter.Current;
				if (curField.HasIndex() && (curField.GetHandler() is StringHandler))
				{
					return true;
				}
			}
			return false;
		}

		private static void ProcessYapClassAndFieldIndices(DefragContextImpl context, ClassMetadata
			 curClass, IPassCommand command)
		{
			int sourceClassIndexID = 0;
			int targetClassIndexID = 0;
			if (curClass.HasIndex())
			{
				sourceClassIndexID = curClass.Index().Id();
				targetClassIndexID = context.MappedID(sourceClassIndexID, -1);
			}
			command.ProcessClass(context, curClass, curClass.GetID(), targetClassIndexID);
		}

		private static void ProcessClassIndex(DefragContextImpl context, ClassMetadata curClass
			, IPassCommand command)
		{
			if (curClass.HasIndex())
			{
				BTreeClassIndexStrategy indexStrategy = (BTreeClassIndexStrategy)curClass.Index();
				BTree btree = indexStrategy.Btree();
				command.ProcessBTree(context, btree);
			}
		}

		internal class NullListener : IDefragmentListener
		{
			public virtual void NotifyDefragmentInfo(DefragmentInfo info)
			{
			}
		}
	}
}
