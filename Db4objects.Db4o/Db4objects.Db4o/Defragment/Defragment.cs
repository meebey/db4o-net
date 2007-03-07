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
	/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
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
		/// <exception cref="System.IO.IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(string origPath)
		{
			Defrag(new Db4objects.Db4o.Defragment.DefragmentConfig(origPath), new Db4objects.Db4o.Defragment.Defragment.NullListener
				());
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
		/// <exception cref="System.IO.IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(string origPath, string backupPath)
		{
			Defrag(new Db4objects.Db4o.Defragment.DefragmentConfig(origPath, backupPath), new 
				Db4objects.Db4o.Defragment.Defragment.NullListener());
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
		/// <exception cref="System.IO.IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(Db4objects.Db4o.Defragment.DefragmentConfig config)
		{
			Defrag(config, new Db4objects.Db4o.Defragment.Defragment.NullListener());
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
		/// <exception cref="System.IO.IOException">if the original file cannot be moved to the backup location
		/// 	</exception>
		public static void Defrag(Db4objects.Db4o.Defragment.DefragmentConfig config, Db4objects.Db4o.Defragment.IDefragmentListener
			 listener)
		{
			Sharpen.IO.File backupFile = new Sharpen.IO.File(config.BackupPath());
			if (backupFile.Exists())
			{
				if (!config.ForceBackupDelete())
				{
					throw new System.IO.IOException("Could not use '" + config.BackupPath() + "' as backup path - file exists."
						);
				}
				backupFile.Delete();
			}
			System.IO.File.Move(config.OrigPath(), config.BackupPath());
			if (config.FileNeedsUpgrade())
			{
				UpgradeFile(config);
			}
			Db4objects.Db4o.Defragment.DefragContextImpl context = new Db4objects.Db4o.Defragment.DefragContextImpl
				(config, listener);
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
				int sourceIdentityID = context.DatabaseIdentityID(Db4objects.Db4o.Defragment.DefragContextImpl
					.SOURCEDB);
				targetIdentityID = context.MappedID(sourceIdentityID, 0);
				targetUuidIndexID = context.MappedID(context.SourceUuidIndexID(), 0);
			}
			catch (Db4objects.Db4o.CorruptionException exc)
			{
				Sharpen.Runtime.PrintStackTrace(exc);
			}
			finally
			{
				context.Close();
			}
			if (targetIdentityID > 0)
			{
				SetIdentity(config.OrigPath(), targetIdentityID, targetUuidIndexID);
			}
			else
			{
				listener.NotifyDefragmentInfo(new Db4objects.Db4o.Defragment.DefragmentInfo("No database identity found in original file."
					));
			}
		}

		private static void UpgradeFile(Db4objects.Db4o.Defragment.DefragmentConfig config
			)
		{
			Db4objects.Db4o.Foundation.IO.File4.Copy(config.BackupPath(), config.TempPath());
			Db4objects.Db4o.Config.IConfiguration db4oConfig = (Db4objects.Db4o.Config.IConfiguration
				)((Db4objects.Db4o.Internal.Config4Impl)config.Db4oConfig()).DeepClone(null);
			db4oConfig.AllowVersionUpdates(true);
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(db4oConfig
				, config.TempPath());
			db.Close();
		}

		private static void DefragUnindexed(Db4objects.Db4o.Defragment.DefragContextImpl 
			context)
		{
			System.Collections.IEnumerator unindexedIDs = context.UnindexedIDs();
			while (unindexedIDs.MoveNext())
			{
				int origID = ((int)unindexedIDs.Current);
				Db4objects.Db4o.Internal.ReaderPair.ProcessCopy(context, origID, new _AnonymousInnerClass168
					(), true);
			}
		}

		private sealed class _AnonymousInnerClass168 : Db4objects.Db4o.Internal.ISlotCopyHandler
		{
			public _AnonymousInnerClass168()
			{
			}

			public void ProcessCopy(Db4objects.Db4o.Internal.ReaderPair readers)
			{
				Db4objects.Db4o.Internal.ClassMetadata.DefragObject(readers);
			}
		}

		private static void SetIdentity(string targetFile, int targetIdentityID, int targetUuidIndexID
			)
		{
			Db4objects.Db4o.Internal.LocalObjectContainer targetDB = (Db4objects.Db4o.Internal.LocalObjectContainer
				)Db4objects.Db4o.Db4oFactory.OpenFile(Db4objects.Db4o.Defragment.DefragmentConfig
				.VanillaDb4oConfig(), targetFile);
			try
			{
				Db4objects.Db4o.Ext.Db4oDatabase identity = (Db4objects.Db4o.Ext.Db4oDatabase)targetDB
					.GetByID(targetIdentityID);
				targetDB.SetIdentity(identity);
				targetDB.SystemData().UuidIndexId(targetUuidIndexID);
			}
			finally
			{
				targetDB.Close();
			}
		}

		private static void FirstPass(Db4objects.Db4o.Defragment.DefragContextImpl context
			, Db4objects.Db4o.Defragment.DefragmentConfig config)
		{
			Pass(context, config, new Db4objects.Db4o.Defragment.FirstPassCommand());
		}

		private static void SecondPass(Db4objects.Db4o.Defragment.DefragContextImpl context
			, Db4objects.Db4o.Defragment.DefragmentConfig config)
		{
			Pass(context, config, new Db4objects.Db4o.Defragment.SecondPassCommand(config.ObjectCommitFrequency
				()));
		}

		private static void Pass(Db4objects.Db4o.Defragment.DefragContextImpl context, Db4objects.Db4o.Defragment.DefragmentConfig
			 config, Db4objects.Db4o.Defragment.IPassCommand command)
		{
			command.ProcessClassCollection(context);
			Db4objects.Db4o.Ext.IStoredClass[] classes = context.StoredClasses(Db4objects.Db4o.Defragment.DefragContextImpl
				.SOURCEDB);
			for (int classIdx = 0; classIdx < classes.Length; classIdx++)
			{
				Db4objects.Db4o.Internal.ClassMetadata yapClass = (Db4objects.Db4o.Internal.ClassMetadata
					)classes[classIdx];
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
			Db4objects.Db4o.Internal.Btree.BTree uuidIndex = context.SourceUuidIndex();
			if (uuidIndex != null)
			{
				command.ProcessBTree(context, uuidIndex);
			}
			command.Flush(context);
			context.TargetCommit();
		}

		private static void ProcessYapClass(Db4objects.Db4o.Defragment.DefragContextImpl 
			context, Db4objects.Db4o.Internal.ClassMetadata curClass, Db4objects.Db4o.Defragment.IPassCommand
			 command)
		{
			ProcessClassIndex(context, curClass, command);
			if (!ParentHasIndex(curClass))
			{
				ProcessObjectsForYapClass(context, curClass, command);
			}
			ProcessYapClassAndFieldIndices(context, curClass, command);
		}

		private static bool ParentHasIndex(Db4objects.Db4o.Internal.ClassMetadata curClass
			)
		{
			Db4objects.Db4o.Internal.ClassMetadata parentClass = curClass.i_ancestor;
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

		private static void ProcessObjectsForYapClass(Db4objects.Db4o.Defragment.DefragContextImpl
			 context, Db4objects.Db4o.Internal.ClassMetadata curClass, Db4objects.Db4o.Defragment.IPassCommand
			 command)
		{
			bool withStringIndex = WithFieldIndex(curClass);
			context.TraverseAll(curClass, new _AnonymousInnerClass261(command, context, curClass
				, withStringIndex));
		}

		private sealed class _AnonymousInnerClass261 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass261(Db4objects.Db4o.Defragment.IPassCommand command, Db4objects.Db4o.Defragment.DefragContextImpl
				 context, Db4objects.Db4o.Internal.ClassMetadata curClass, bool withStringIndex)
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
				catch (Db4objects.Db4o.CorruptionException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}

			private readonly Db4objects.Db4o.Defragment.IPassCommand command;

			private readonly Db4objects.Db4o.Defragment.DefragContextImpl context;

			private readonly Db4objects.Db4o.Internal.ClassMetadata curClass;

			private readonly bool withStringIndex;
		}

		private static bool WithFieldIndex(Db4objects.Db4o.Internal.ClassMetadata clazz)
		{
			System.Collections.IEnumerator fieldIter = clazz.Fields();
			while (fieldIter.MoveNext())
			{
				Db4objects.Db4o.Internal.FieldMetadata curField = (Db4objects.Db4o.Internal.FieldMetadata
					)fieldIter.Current;
				if (curField.HasIndex() && (curField.GetHandler() is Db4objects.Db4o.Internal.Handlers.StringHandler
					))
				{
					return true;
				}
			}
			return false;
		}

		private static void ProcessYapClassAndFieldIndices(Db4objects.Db4o.Defragment.DefragContextImpl
			 context, Db4objects.Db4o.Internal.ClassMetadata curClass, Db4objects.Db4o.Defragment.IPassCommand
			 command)
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

		private static void ProcessClassIndex(Db4objects.Db4o.Defragment.DefragContextImpl
			 context, Db4objects.Db4o.Internal.ClassMetadata curClass, Db4objects.Db4o.Defragment.IPassCommand
			 command)
		{
			if (curClass.HasIndex())
			{
				Db4objects.Db4o.Internal.Classindex.BTreeClassIndexStrategy indexStrategy = (Db4objects.Db4o.Internal.Classindex.BTreeClassIndexStrategy
					)curClass.Index();
				Db4objects.Db4o.Internal.Btree.BTree btree = indexStrategy.Btree();
				command.ProcessBTree(context, btree);
			}
		}

		internal class NullListener : Db4objects.Db4o.Defragment.IDefragmentListener
		{
			public virtual void NotifyDefragmentInfo(Db4objects.Db4o.Defragment.DefragmentInfo
				 info)
			{
			}
		}
	}
}
