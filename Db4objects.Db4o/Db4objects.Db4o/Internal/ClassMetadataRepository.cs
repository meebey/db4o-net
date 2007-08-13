/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public sealed class ClassMetadataRepository : PersistentBase
	{
		private Collection4 i_classes;

		private Hashtable4 i_creating;

		private readonly Transaction _systemTransaction;

		private Hashtable4 i_yapClassByBytes;

		private Hashtable4 i_yapClassByClass;

		private Hashtable4 i_yapClassByID;

		private int i_yapClassCreationDepth;

		private IQueue4 i_initYapClassesOnUp;

		private readonly PendingClassInits _classInits;

		internal ClassMetadataRepository(Transaction systemTransaction)
		{
			_systemTransaction = systemTransaction;
			i_initYapClassesOnUp = new NonblockingQueue();
			_classInits = new PendingClassInits(_systemTransaction);
		}

		public void AddYapClass(ClassMetadata yapClass)
		{
			Stream().SetDirtyInSystemTransaction(this);
			i_classes.Add(yapClass);
			if (yapClass.StateUnread())
			{
				i_yapClassByBytes.Put(yapClass.i_nameBytes, yapClass);
			}
			else
			{
				i_yapClassByClass.Put(yapClass.ClassReflector(), yapClass);
			}
			if (yapClass.GetID() == 0)
			{
				yapClass.Write(_systemTransaction);
			}
			i_yapClassByID.Put(yapClass.GetID(), yapClass);
		}

		private byte[] AsBytes(string str)
		{
			return Stream().StringIO().Write(str);
		}

		public void AttachQueryNode(string fieldName, IVisitor4 a_visitor)
		{
			ClassMetadataIterator i = Iterator();
			while (i.MoveNext())
			{
				ClassMetadata classMetadata = i.CurrentClass();
				if (!classMetadata.IsInternal())
				{
					classMetadata.ForEachFieldMetadata(new _IVisitor4_60(this, fieldName, a_visitor, 
						classMetadata));
				}
			}
		}

		private sealed class _IVisitor4_60 : IVisitor4
		{
			public _IVisitor4_60(ClassMetadataRepository _enclosing, string fieldName, IVisitor4
				 a_visitor, ClassMetadata classMetadata)
			{
				this._enclosing = _enclosing;
				this.fieldName = fieldName;
				this.a_visitor = a_visitor;
				this.classMetadata = classMetadata;
			}

			public void Visit(object obj)
			{
				FieldMetadata yf = (FieldMetadata)obj;
				if (yf.CanAddToQuery(fieldName))
				{
					a_visitor.Visit(new object[] { classMetadata, yf });
				}
			}

			private readonly ClassMetadataRepository _enclosing;

			private readonly string fieldName;

			private readonly IVisitor4 a_visitor;

			private readonly ClassMetadata classMetadata;
		}

		public void IterateTopLevelClasses(IVisitor4 visitor)
		{
			ClassMetadataIterator i = Iterator();
			while (i.MoveNext())
			{
				ClassMetadata classMetadata = i.CurrentClass();
				if (!classMetadata.IsInternal())
				{
					if (classMetadata.GetAncestor() == null)
					{
						visitor.Visit(classMetadata);
					}
				}
			}
		}

		internal void CheckChanges()
		{
			IEnumerator i = i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				((ClassMetadata)i.Current).CheckChanges();
			}
		}

		internal bool CreateYapClass(ClassMetadata a_yapClass, IReflectClass a_class)
		{
			i_yapClassCreationDepth++;
			IReflectClass superClass = a_class.GetSuperclass();
			ClassMetadata superYapClass = null;
			if (superClass != null && !superClass.Equals(Stream()._handlers.ICLASS_OBJECT))
			{
				superYapClass = ProduceClassMetadata(superClass);
			}
			bool ret = Stream().CreateClassMetadata(a_yapClass, a_class, superYapClass);
			i_yapClassCreationDepth--;
			InitYapClassesOnUp();
			return ret;
		}

		public static void Defrag(BufferPair readers)
		{
			int numClasses = readers.ReadInt();
			for (int classIdx = 0; classIdx < numClasses; classIdx++)
			{
				readers.CopyID();
			}
		}

		private void EnsureAllClassesRead()
		{
			bool allClassesRead = false;
			while (!allClassesRead)
			{
				Collection4 unreadClasses = new Collection4();
				int numClasses = i_classes.Size();
				IEnumerator classIter = i_classes.GetEnumerator();
				while (classIter.MoveNext())
				{
					ClassMetadata yapClass = (ClassMetadata)classIter.Current;
					if (yapClass.StateUnread())
					{
						unreadClasses.Add(yapClass);
					}
				}
				IEnumerator unreadIter = unreadClasses.GetEnumerator();
				while (unreadIter.MoveNext())
				{
					ClassMetadata yapClass = (ClassMetadata)unreadIter.Current;
					yapClass = ReadClassMetadata(yapClass, null);
					if (yapClass.ClassReflector() == null)
					{
						yapClass.ForceRead();
					}
				}
				allClassesRead = (i_classes.Size() == numClasses);
			}
			ApplyReadAs();
		}

		internal bool FieldExists(string a_field)
		{
			ClassMetadataIterator i = Iterator();
			while (i.MoveNext())
			{
				if (i.CurrentClass().FieldMetadataForName(a_field) != null)
				{
					return true;
				}
			}
			return false;
		}

		public Collection4 ForInterface(IReflectClass claxx)
		{
			Collection4 col = new Collection4();
			ClassMetadataIterator i = Iterator();
			while (i.MoveNext())
			{
				ClassMetadata yc = i.CurrentClass();
				IReflectClass candidate = yc.ClassReflector();
				if (!candidate.IsInterface())
				{
					if (claxx.IsAssignableFrom(candidate))
					{
						col.Add(yc);
						IEnumerator j = new Collection4(col).GetEnumerator();
						while (j.MoveNext())
						{
							ClassMetadata existing = (ClassMetadata)j.Current;
							if (existing != yc)
							{
								ClassMetadata higher = yc.GetHigherHierarchy(existing);
								if (higher != null)
								{
									if (higher == yc)
									{
										col.Remove(existing);
									}
									else
									{
										col.Remove(yc);
									}
								}
							}
						}
					}
				}
			}
			return col;
		}

		public override byte GetIdentifier()
		{
			return Const4.YAPCLASSCOLLECTION;
		}

		internal ClassMetadata GetActiveYapClass(IReflectClass a_class)
		{
			return (ClassMetadata)i_yapClassByClass.Get(a_class);
		}

		internal ClassMetadata ClassMetadataForReflectClass(IReflectClass a_class)
		{
			ClassMetadata yapClass = (ClassMetadata)i_yapClassByClass.Get(a_class);
			if (yapClass != null)
			{
				return yapClass;
			}
			yapClass = (ClassMetadata)i_yapClassByBytes.Remove(GetNameBytes(a_class.GetName()
				));
			return ReadClassMetadata(yapClass, a_class);
		}

		internal ClassMetadata ProduceClassMetadata(IReflectClass claxx)
		{
			ClassMetadata classMetadata = ClassMetadataForReflectClass(claxx);
			if (classMetadata != null)
			{
				return classMetadata;
			}
			classMetadata = (ClassMetadata)i_creating.Get(claxx);
			if (classMetadata != null)
			{
				return classMetadata;
			}
			classMetadata = new ClassMetadata(Stream(), claxx);
			i_creating.Put(claxx, classMetadata);
			if (!CreateYapClass(classMetadata, claxx))
			{
				i_creating.Remove(claxx);
				return null;
			}
			bool addMembers = false;
			if (i_yapClassByClass.Get(claxx) == null)
			{
				AddYapClass(classMetadata);
				addMembers = true;
			}
			int id = classMetadata.GetID();
			if (id == 0)
			{
				classMetadata.Write(Stream().SystemTransaction());
				id = classMetadata.GetID();
			}
			if (i_yapClassByID.Get(id) == null)
			{
				i_yapClassByID.Put(id, classMetadata);
				addMembers = true;
			}
			if (addMembers || classMetadata.i_fields == null)
			{
				_classInits.Process(classMetadata);
			}
			i_creating.Remove(claxx);
			Stream().SetDirtyInSystemTransaction(this);
			return classMetadata;
		}

		internal ClassMetadata GetYapClass(int id)
		{
			return ReadClassMetadata((ClassMetadata)i_yapClassByID.Get(id), null);
		}

		public int ClassMetadataIdForName(string name)
		{
			ClassMetadata classMetadata = (ClassMetadata)i_yapClassByBytes.Get(GetNameBytes(name
				));
			if (classMetadata == null)
			{
				classMetadata = FindInitializedClassByName(name);
			}
			if (classMetadata != null)
			{
				return classMetadata.GetID();
			}
			return 0;
		}

		public ClassMetadata GetYapClass(string a_name)
		{
			ClassMetadata classMetadata = (ClassMetadata)i_yapClassByBytes.Remove(GetNameBytes
				(a_name));
			if (classMetadata == null)
			{
				classMetadata = FindInitializedClassByName(a_name);
			}
			if (classMetadata != null)
			{
				classMetadata = ReadClassMetadata(classMetadata, null);
			}
			return classMetadata;
		}

		private ClassMetadata FindInitializedClassByName(string name)
		{
			ClassMetadataIterator i = Iterator();
			while (i.MoveNext())
			{
				ClassMetadata classMetadata = (ClassMetadata)i.Current;
				if (name.Equals(classMetadata.GetName()))
				{
					return classMetadata;
				}
			}
			return null;
		}

		public int GetYapClassID(string name)
		{
			ClassMetadata yc = (ClassMetadata)i_yapClassByBytes.Get(GetNameBytes(name));
			if (yc != null)
			{
				return yc.GetID();
			}
			return 0;
		}

		internal byte[] GetNameBytes(string name)
		{
			return AsBytes(ResolveAliasRuntimeName(name));
		}

		private string ResolveAliasRuntimeName(string name)
		{
			return Stream().ConfigImpl().ResolveAliasRuntimeName(name);
		}

		internal void InitOnUp(Transaction systemTrans)
		{
			i_yapClassCreationDepth++;
			systemTrans.Container().ShowInternalClasses(true);
			try
			{
				IEnumerator i = i_classes.GetEnumerator();
				while (i.MoveNext())
				{
					((ClassMetadata)i.Current).InitOnUp(systemTrans);
				}
			}
			finally
			{
				systemTrans.Container().ShowInternalClasses(false);
			}
			i_yapClassCreationDepth--;
			InitYapClassesOnUp();
		}

		internal void InitTables(int a_size)
		{
			i_classes = new Collection4();
			i_yapClassByBytes = new Hashtable4(a_size);
			if (a_size < 16)
			{
				a_size = 16;
			}
			i_yapClassByClass = new Hashtable4(a_size);
			i_yapClassByID = new Hashtable4(a_size);
			i_creating = new Hashtable4(1);
		}

		private void InitYapClassesOnUp()
		{
			if (i_yapClassCreationDepth == 0)
			{
				ClassMetadata yc = (ClassMetadata)i_initYapClassesOnUp.Next();
				while (yc != null)
				{
					yc.InitOnUp(_systemTransaction);
					yc = (ClassMetadata)i_initYapClassesOnUp.Next();
				}
			}
		}

		public ClassMetadataIterator Iterator()
		{
			return new ClassMetadataIterator(this, new ArrayIterator4(i_classes.ToArray()));
		}

		private class ClassIDIterator : MappingIterator
		{
			public ClassIDIterator(Collection4 classes) : base(classes.GetEnumerator())
			{
			}

			protected override object Map(object current)
			{
				return ((ClassMetadata)current).GetID();
			}
		}

		public IEnumerator Ids()
		{
			return new ClassMetadataRepository.ClassIDIterator(i_classes);
		}

		public override int OwnLength()
		{
			return Const4.OBJECT_LENGTH + Const4.INT_LENGTH + (i_classes.Size() * Const4.ID_LENGTH
				);
		}

		internal void Purge()
		{
			IEnumerator i = i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				((ClassMetadata)i.Current).Purge();
			}
		}

		public sealed override void ReadThis(Transaction a_trans, Db4objects.Db4o.Internal.Buffer
			 a_reader)
		{
			int classCount = a_reader.ReadInt();
			InitTables(classCount);
			ObjectContainerBase stream = Stream();
			int[] ids = new int[classCount];
			for (int i = 0; i < classCount; ++i)
			{
				ids[i] = a_reader.ReadInt();
			}
			StatefulBuffer[] yapWriters = stream.ReadWritersByIDs(a_trans, ids);
			for (int i = 0; i < classCount; ++i)
			{
				ClassMetadata classMetadata = new ClassMetadata(stream, null);
				classMetadata.SetID(ids[i]);
				i_classes.Add(classMetadata);
				i_yapClassByID.Put(ids[i], classMetadata);
				byte[] name = classMetadata.ReadName1(a_trans, yapWriters[i]);
				if (name != null)
				{
					i_yapClassByBytes.Put(name, classMetadata);
				}
			}
			ApplyReadAs();
		}

		internal Hashtable4 ClassByBytes()
		{
			return i_yapClassByBytes;
		}

		private void ApplyReadAs()
		{
			Hashtable4 readAs = Stream().ConfigImpl().ReadAs();
			IEnumerator i = readAs.Iterator();
			while (i.MoveNext())
			{
				IEntry4 entry = (IEntry4)i.Current;
				string dbName = (string)entry.Key();
				string useName = (string)entry.Value();
				byte[] dbbytes = GetNameBytes(dbName);
				byte[] useBytes = GetNameBytes(useName);
				if (ClassByBytes().Get(useBytes) == null)
				{
					ClassMetadata yc = (ClassMetadata)ClassByBytes().Get(dbbytes);
					if (yc != null)
					{
						yc.i_nameBytes = useBytes;
						yc.SetConfig(ConfigClass(dbName));
						ClassByBytes().Remove(dbbytes);
						ClassByBytes().Put(useBytes, yc);
					}
				}
			}
		}

		private Config4Class ConfigClass(string name)
		{
			return Stream().ConfigImpl().ConfigClass(name);
		}

		public ClassMetadata ReadClassMetadata(ClassMetadata classMetadata, IReflectClass
			 clazz)
		{
			if (classMetadata == null)
			{
				return null;
			}
			if (!classMetadata.StateUnread())
			{
				return classMetadata;
			}
			i_yapClassCreationDepth++;
			string name = classMetadata.ResolveName(clazz);
			classMetadata.CreateConfigAndConstructor(i_yapClassByBytes, clazz, name);
			IReflectClass claxx = classMetadata.ClassReflector();
			if (claxx != null)
			{
				i_yapClassByClass.Put(claxx, classMetadata);
				classMetadata.ReadThis();
				classMetadata.CheckChanges();
				i_initYapClassesOnUp.Add(classMetadata);
			}
			i_yapClassCreationDepth--;
			InitYapClassesOnUp();
			return classMetadata;
		}

		public void RefreshClasses()
		{
			ClassMetadataRepository rereader = new ClassMetadataRepository(_systemTransaction
				);
			rereader.i_id = i_id;
			rereader.Read(Stream().SystemTransaction());
			IEnumerator i = rereader.i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				ClassMetadata yc = (ClassMetadata)i.Current;
				if (i_yapClassByID.Get(yc.GetID()) == null)
				{
					i_classes.Add(yc);
					i_yapClassByID.Put(yc.GetID(), yc);
					if (yc.StateUnread())
					{
						i_yapClassByBytes.Put(yc.ReadName(_systemTransaction), yc);
					}
					else
					{
						i_yapClassByClass.Put(yc.ClassReflector(), yc);
					}
				}
			}
			i = i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				ClassMetadata yc = (ClassMetadata)i.Current;
				yc.Refresh();
			}
		}

		internal void ReReadYapClass(ClassMetadata yapClass)
		{
			if (yapClass != null)
			{
				ReReadYapClass(yapClass.i_ancestor);
				yapClass.ReadName(_systemTransaction);
				yapClass.ForceRead();
				yapClass.SetStateClean();
				yapClass.BitFalse(Const4.CHECKED_CHANGES);
				yapClass.BitFalse(Const4.READING);
				yapClass.BitFalse(Const4.CONTINUE);
				yapClass.BitFalse(Const4.DEAD);
				yapClass.CheckChanges();
			}
		}

		public IStoredClass[] StoredClasses()
		{
			EnsureAllClassesRead();
			IStoredClass[] sclasses = new IStoredClass[i_classes.Size()];
			i_classes.ToArray(sclasses);
			return sclasses;
		}

		public void WriteAllClasses()
		{
			IStoredClass[] storedClasses = StoredClasses();
			for (int i = 0; i < storedClasses.Length; i++)
			{
				ClassMetadata yc = (ClassMetadata)storedClasses[i];
				yc.SetStateDirty();
			}
			for (int i = 0; i < storedClasses.Length; i++)
			{
				ClassMetadata yc = (ClassMetadata)storedClasses[i];
				yc.Write(_systemTransaction);
			}
		}

		public override void WriteThis(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 a_writer)
		{
			a_writer.WriteInt(i_classes.Size());
			IEnumerator i = i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				a_writer.WriteIDOf(trans, i.Current);
			}
		}

		public override string ToString()
		{
			return base.ToString();
			string str = "Active:\n";
			IEnumerator i = i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				ClassMetadata yc = (ClassMetadata)i.Current;
				str += yc.GetID() + " " + yc + "\n";
			}
			return str;
		}

		internal ObjectContainerBase Stream()
		{
			return _systemTransaction.Container();
		}

		public override void SetID(int a_id)
		{
			if (Stream().IsClient())
			{
				base.SetID(a_id);
				return;
			}
			if (i_id == 0)
			{
				SystemData().ClassCollectionID(a_id);
			}
			base.SetID(a_id);
		}

		private SystemData SystemData()
		{
			return LocalSystemTransaction().File().SystemData();
		}

		private LocalTransaction LocalSystemTransaction()
		{
			return ((LocalTransaction)_systemTransaction);
		}
	}
}
