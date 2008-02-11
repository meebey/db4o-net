/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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
		private Collection4 _classes;

		private Hashtable4 _creating;

		private readonly Transaction _systemTransaction;

		private Hashtable4 _classMetadataByBytes;

		private Hashtable4 _classMetadataByClass;

		private Hashtable4 _classMetadataByID;

		private int _classMetadataCreationDepth;

		private IQueue4 _initClassMetadataOnUp;

		private readonly PendingClassInits _classInits;

		internal ClassMetadataRepository(Transaction systemTransaction)
		{
			_systemTransaction = systemTransaction;
			_initClassMetadataOnUp = new NonblockingQueue();
			_classInits = new PendingClassInits(_systemTransaction);
		}

		public void AddClassMetadata(ClassMetadata clazz)
		{
			Container().SetDirtyInSystemTransaction(this);
			_classes.Add(clazz);
			if (clazz.StateUnread())
			{
				_classMetadataByBytes.Put(clazz.i_nameBytes, clazz);
			}
			else
			{
				_classMetadataByClass.Put(clazz.ClassReflector(), clazz);
			}
			if (clazz.GetID() == 0)
			{
				clazz.Write(_systemTransaction);
			}
			_classMetadataByID.Put(clazz.GetID(), clazz);
		}

		private byte[] AsBytes(string str)
		{
			return Container().StringIO().Write(str);
		}

		public void AttachQueryNode(string fieldName, IVisitor4 visitor)
		{
			ClassMetadataIterator i = Iterator();
			while (i.MoveNext())
			{
				ClassMetadata classMetadata = i.CurrentClass();
				if (!classMetadata.IsInternal())
				{
					classMetadata.ForEachFieldMetadata(new _IVisitor4_59(this, fieldName, visitor, classMetadata
						));
				}
			}
		}

		private sealed class _IVisitor4_59 : IVisitor4
		{
			public _IVisitor4_59(ClassMetadataRepository _enclosing, string fieldName, IVisitor4
				 visitor, ClassMetadata classMetadata)
			{
				this._enclosing = _enclosing;
				this.fieldName = fieldName;
				this.visitor = visitor;
				this.classMetadata = classMetadata;
			}

			public void Visit(object obj)
			{
				FieldMetadata field = (FieldMetadata)obj;
				if (field.CanAddToQuery(fieldName))
				{
					visitor.Visit(new object[] { classMetadata, field });
				}
			}

			private readonly ClassMetadataRepository _enclosing;

			private readonly string fieldName;

			private readonly IVisitor4 visitor;

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
			IEnumerator i = _classes.GetEnumerator();
			while (i.MoveNext())
			{
				((ClassMetadata)i.Current).CheckChanges();
			}
		}

		internal bool CreateClassMetadata(ClassMetadata clazz, IReflectClass reflectClazz
			)
		{
			_classMetadataCreationDepth++;
			IReflectClass parentReflectClazz = reflectClazz.GetSuperclass();
			ClassMetadata parentClazz = null;
			if (parentReflectClazz != null && !parentReflectClazz.Equals(Container()._handlers
				.IclassObject))
			{
				parentClazz = ProduceClassMetadata(parentReflectClazz);
			}
			bool ret = Container().CreateClassMetadata(clazz, reflectClazz, parentClazz);
			_classMetadataCreationDepth--;
			InitClassMetadataOnUp();
			return ret;
		}

		private void EnsureAllClassesRead()
		{
			bool allClassesRead = false;
			while (!allClassesRead)
			{
				Collection4 unreadClasses = new Collection4();
				int numClasses = _classes.Size();
				IEnumerator classIter = _classes.GetEnumerator();
				while (classIter.MoveNext())
				{
					ClassMetadata clazz = (ClassMetadata)classIter.Current;
					if (clazz.StateUnread())
					{
						unreadClasses.Add(clazz);
					}
				}
				IEnumerator unreadIter = unreadClasses.GetEnumerator();
				while (unreadIter.MoveNext())
				{
					ClassMetadata clazz = (ClassMetadata)unreadIter.Current;
					clazz = ReadClassMetadata(clazz, null);
					if (clazz.ClassReflector() == null)
					{
						clazz.ForceRead();
					}
				}
				allClassesRead = (_classes.Size() == numClasses);
			}
			ApplyReadAs();
		}

		internal bool FieldExists(string field)
		{
			ClassMetadataIterator i = Iterator();
			while (i.MoveNext())
			{
				if (i.CurrentClass().FieldMetadataForName(field) != null)
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
				ClassMetadata clazz = i.CurrentClass();
				IReflectClass candidate = clazz.ClassReflector();
				if (!candidate.IsInterface())
				{
					if (claxx.IsAssignableFrom(candidate))
					{
						col.Add(clazz);
						IEnumerator j = new Collection4(col).GetEnumerator();
						while (j.MoveNext())
						{
							ClassMetadata existing = (ClassMetadata)j.Current;
							if (existing != clazz)
							{
								ClassMetadata higher = clazz.GetHigherHierarchy(existing);
								if (higher != null)
								{
									if (higher == clazz)
									{
										col.Remove(existing);
									}
									else
									{
										col.Remove(clazz);
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
			return Const4.Yapclasscollection;
		}

		internal ClassMetadata GetActiveClassMetadata(IReflectClass reflectClazz)
		{
			return (ClassMetadata)_classMetadataByClass.Get(reflectClazz);
		}

		internal ClassMetadata ClassMetadataForReflectClass(IReflectClass reflectClazz)
		{
			ClassMetadata clazz = (ClassMetadata)_classMetadataByClass.Get(reflectClazz);
			if (clazz != null)
			{
				return clazz;
			}
			clazz = (ClassMetadata)_classMetadataByBytes.Remove(GetNameBytes(reflectClazz.GetName
				()));
			return ReadClassMetadata(clazz, reflectClazz);
		}

		internal ClassMetadata ProduceClassMetadata(IReflectClass reflectClazz)
		{
			ClassMetadata classMetadata = ClassMetadataForReflectClass(reflectClazz);
			if (classMetadata != null)
			{
				return classMetadata;
			}
			classMetadata = (ClassMetadata)_creating.Get(reflectClazz);
			if (classMetadata != null)
			{
				return classMetadata;
			}
			classMetadata = new ClassMetadata(Container(), reflectClazz);
			_creating.Put(reflectClazz, classMetadata);
			if (!CreateClassMetadata(classMetadata, reflectClazz))
			{
				_creating.Remove(reflectClazz);
				return null;
			}
			// ObjectContainerBase#createClassMetadata may add the ClassMetadata already,
			// so we have to check again
			bool addMembers = false;
			if (_classMetadataByClass.Get(reflectClazz) == null)
			{
				AddClassMetadata(classMetadata);
				addMembers = true;
			}
			int id = classMetadata.GetID();
			if (id == 0)
			{
				classMetadata.Write(Container().SystemTransaction());
				id = classMetadata.GetID();
			}
			if (_classMetadataByID.Get(id) == null)
			{
				_classMetadataByID.Put(id, classMetadata);
				addMembers = true;
			}
			if (addMembers || classMetadata.i_fields == null)
			{
				_classInits.Process(classMetadata);
			}
			_creating.Remove(reflectClazz);
			Container().SetDirtyInSystemTransaction(this);
			return classMetadata;
		}

		internal ClassMetadata GetClassMetadata(int id)
		{
			return ReadClassMetadata((ClassMetadata)_classMetadataByID.Get(id), null);
		}

		public int ClassMetadataIdForName(string name)
		{
			ClassMetadata classMetadata = (ClassMetadata)_classMetadataByBytes.Get(GetNameBytes
				(name));
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

		public ClassMetadata GetClassMetadata(string name)
		{
			ClassMetadata classMetadata = (ClassMetadata)_classMetadataByBytes.Remove(GetNameBytes
				(name));
			if (classMetadata == null)
			{
				classMetadata = FindInitializedClassByName(name);
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

		public int GetClassMetadataID(string name)
		{
			ClassMetadata clazz = (ClassMetadata)_classMetadataByBytes.Get(GetNameBytes(name)
				);
			if (clazz != null)
			{
				return clazz.GetID();
			}
			return 0;
		}

		internal byte[] GetNameBytes(string name)
		{
			return AsBytes(ResolveAliasRuntimeName(name));
		}

		private string ResolveAliasRuntimeName(string name)
		{
			return Container().ConfigImpl().ResolveAliasRuntimeName(name);
		}

		internal void InitOnUp(Transaction systemTrans)
		{
			_classMetadataCreationDepth++;
			systemTrans.Container().ShowInternalClasses(true);
			try
			{
				IEnumerator i = _classes.GetEnumerator();
				while (i.MoveNext())
				{
					((ClassMetadata)i.Current).InitOnUp(systemTrans);
				}
			}
			finally
			{
				systemTrans.Container().ShowInternalClasses(false);
			}
			_classMetadataCreationDepth--;
			InitClassMetadataOnUp();
		}

		internal void InitTables(int size)
		{
			_classes = new Collection4();
			_classMetadataByBytes = new Hashtable4(size);
			if (size < 16)
			{
				size = 16;
			}
			_classMetadataByClass = new Hashtable4(size);
			_classMetadataByID = new Hashtable4(size);
			_creating = new Hashtable4(1);
		}

		private void InitClassMetadataOnUp()
		{
			if (_classMetadataCreationDepth == 0)
			{
				ClassMetadata clazz = (ClassMetadata)_initClassMetadataOnUp.Next();
				while (clazz != null)
				{
					clazz.InitOnUp(_systemTransaction);
					clazz = (ClassMetadata)_initClassMetadataOnUp.Next();
				}
			}
		}

		public ClassMetadataIterator Iterator()
		{
			return new ClassMetadataIterator(this, new ArrayIterator4(_classes.ToArray()));
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
			return new ClassMetadataRepository.ClassIDIterator(_classes);
		}

		public override int OwnLength()
		{
			return Const4.ObjectLength + Const4.IntLength + (_classes.Size() * Const4.IdLength
				);
		}

		internal void Purge()
		{
			IEnumerator i = _classes.GetEnumerator();
			while (i.MoveNext())
			{
				((ClassMetadata)i.Current).Purge();
			}
		}

		public sealed override void ReadThis(Transaction trans, ByteArrayBuffer buffer)
		{
			int classCount = buffer.ReadInt();
			InitTables(classCount);
			ObjectContainerBase container = Container();
			int[] ids = new int[classCount];
			for (int i = 0; i < classCount; ++i)
			{
				ids[i] = buffer.ReadInt();
			}
			StatefulBuffer[] clazzWriters = container.ReadWritersByIDs(trans, ids);
			for (int i = 0; i < classCount; ++i)
			{
				ClassMetadata classMetadata = new ClassMetadata(container, null);
				classMetadata.SetID(ids[i]);
				_classes.Add(classMetadata);
				_classMetadataByID.Put(ids[i], classMetadata);
				byte[] name = classMetadata.ReadName1(trans, clazzWriters[i]);
				if (name != null)
				{
					_classMetadataByBytes.Put(name, classMetadata);
				}
			}
			ApplyReadAs();
		}

		internal Hashtable4 ClassByBytes()
		{
			return _classMetadataByBytes;
		}

		private void ApplyReadAs()
		{
			Hashtable4 readAs = Container().ConfigImpl().ReadAs();
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
					ClassMetadata clazz = (ClassMetadata)ClassByBytes().Get(dbbytes);
					if (clazz != null)
					{
						clazz.i_nameBytes = useBytes;
						clazz.SetConfig(ConfigClass(dbName));
						ClassByBytes().Remove(dbbytes);
						ClassByBytes().Put(useBytes, clazz);
					}
				}
			}
		}

		private Config4Class ConfigClass(string name)
		{
			return Container().ConfigImpl().ConfigClass(name);
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
			_classMetadataCreationDepth++;
			string name = classMetadata.ResolveName(clazz);
			classMetadata.CreateConfigAndConstructor(_classMetadataByBytes, clazz, name);
			IReflectClass claxx = classMetadata.ClassReflector();
			if (claxx != null)
			{
				_classMetadataByClass.Put(claxx, classMetadata);
				classMetadata.ReadThis();
				classMetadata.CheckChanges();
				_initClassMetadataOnUp.Add(classMetadata);
			}
			_classMetadataCreationDepth--;
			InitClassMetadataOnUp();
			return classMetadata;
		}

		public void RefreshClasses()
		{
			ClassMetadataRepository rereader = new ClassMetadataRepository(_systemTransaction
				);
			rereader._id = _id;
			rereader.Read(Container().SystemTransaction());
			IEnumerator i = rereader._classes.GetEnumerator();
			while (i.MoveNext())
			{
				ClassMetadata clazz = (ClassMetadata)i.Current;
				RefreshClass(clazz);
			}
			i = _classes.GetEnumerator();
			while (i.MoveNext())
			{
				ClassMetadata clazz = (ClassMetadata)i.Current;
				clazz.Refresh();
			}
		}

		public void RefreshClass(ClassMetadata clazz)
		{
			if (_classMetadataByID.Get(clazz.GetID()) == null)
			{
				_classes.Add(clazz);
				_classMetadataByID.Put(clazz.GetID(), clazz);
				RefreshClassCache(clazz, null);
			}
		}

		public void RefreshClassCache(ClassMetadata clazz, IReflectClass oldReflector)
		{
			if (clazz.StateUnread())
			{
				_classMetadataByBytes.Put(clazz.ReadName(_systemTransaction), clazz);
			}
			else
			{
				if (oldReflector != null)
				{
					_classMetadataByClass.Remove(oldReflector);
				}
				_classMetadataByClass.Put(clazz.ClassReflector(), clazz);
			}
		}

		internal void ReReadClassMetadata(ClassMetadata clazz)
		{
			if (clazz != null)
			{
				ReReadClassMetadata(clazz.i_ancestor);
				clazz.ReadName(_systemTransaction);
				clazz.ForceRead();
				clazz.SetStateClean();
				clazz.BitFalse(Const4.CheckedChanges);
				clazz.BitFalse(Const4.Reading);
				clazz.BitFalse(Const4.Continue);
				clazz.BitFalse(Const4.Dead);
				clazz.CheckChanges();
			}
		}

		public IStoredClass[] StoredClasses()
		{
			EnsureAllClassesRead();
			IStoredClass[] sclasses = new IStoredClass[_classes.Size()];
			_classes.ToArray(sclasses);
			return sclasses;
		}

		public void WriteAllClasses()
		{
			IStoredClass[] storedClasses = StoredClasses();
			for (int i = 0; i < storedClasses.Length; i++)
			{
				ClassMetadata clazz = (ClassMetadata)storedClasses[i];
				clazz.SetStateDirty();
			}
			for (int i = 0; i < storedClasses.Length; i++)
			{
				ClassMetadata clazz = (ClassMetadata)storedClasses[i];
				clazz.Write(_systemTransaction);
			}
		}

		public override void WriteThis(Transaction trans, ByteArrayBuffer buffer)
		{
			buffer.WriteInt(_classes.Size());
			IEnumerator i = _classes.GetEnumerator();
			while (i.MoveNext())
			{
				buffer.WriteIDOf(trans, i.Current);
			}
		}

		public override string ToString()
		{
			string str = "Active:\n";
			IEnumerator i = _classes.GetEnumerator();
			while (i.MoveNext())
			{
				ClassMetadata clazz = (ClassMetadata)i.Current;
				str += clazz.GetID() + " " + clazz + "\n";
			}
			return str;
		}

		internal ObjectContainerBase Container()
		{
			return _systemTransaction.Container();
		}

		public override void SetID(int id)
		{
			if (Container().IsClient())
			{
				base.SetID(id);
				return;
			}
			if (_id == 0)
			{
				SystemData().ClassCollectionID(id);
			}
			base.SetID(id);
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
