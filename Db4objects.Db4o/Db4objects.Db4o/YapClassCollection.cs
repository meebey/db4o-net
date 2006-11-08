namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public sealed class YapClassCollection : Db4objects.Db4o.YapMeta
	{
		private Db4objects.Db4o.Foundation.Collection4 i_classes;

		private Db4objects.Db4o.Foundation.Hashtable4 i_creating;

		private readonly Db4objects.Db4o.Transaction _systemTransaction;

		private Db4objects.Db4o.Foundation.Hashtable4 i_yapClassByBytes;

		private Db4objects.Db4o.Foundation.Hashtable4 i_yapClassByClass;

		private Db4objects.Db4o.Foundation.Hashtable4 i_yapClassByID;

		private int i_yapClassCreationDepth;

		private Db4objects.Db4o.Foundation.Queue4 i_initYapClassesOnUp;

		private readonly Db4objects.Db4o.PendingClassInits _classInits;

		internal YapClassCollection(Db4objects.Db4o.Transaction systemTransaction)
		{
			_systemTransaction = systemTransaction;
			i_initYapClassesOnUp = new Db4objects.Db4o.Foundation.Queue4();
			_classInits = new Db4objects.Db4o.PendingClassInits(_systemTransaction);
		}

		public void AddYapClass(Db4objects.Db4o.YapClass yapClass)
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

		internal void AttachQueryNode(string fieldName, Db4objects.Db4o.Foundation.IVisitor4
			 a_visitor)
		{
			Db4objects.Db4o.YapClassCollectionIterator i = Iterator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.YapClass yc = i.CurrentClass();
				if (!yc.IsInternal())
				{
					yc.ForEachYapField(new _AnonymousInnerClass59(this, fieldName, a_visitor, yc));
				}
			}
		}

		private sealed class _AnonymousInnerClass59 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass59(YapClassCollection _enclosing, string fieldName, Db4objects.Db4o.Foundation.IVisitor4
				 a_visitor, Db4objects.Db4o.YapClass yc)
			{
				this._enclosing = _enclosing;
				this.fieldName = fieldName;
				this.a_visitor = a_visitor;
				this.yc = yc;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.YapField yf = (Db4objects.Db4o.YapField)obj;
				if (yf.CanAddToQuery(fieldName))
				{
					a_visitor.Visit(new object[] { yc, yf });
				}
			}

			private readonly YapClassCollection _enclosing;

			private readonly string fieldName;

			private readonly Db4objects.Db4o.Foundation.IVisitor4 a_visitor;

			private readonly Db4objects.Db4o.YapClass yc;
		}

		internal void CheckChanges()
		{
			System.Collections.IEnumerator i = i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.YapClass)i.Current).CheckChanges();
			}
		}

		internal bool CreateYapClass(Db4objects.Db4o.YapClass a_yapClass, Db4objects.Db4o.Reflect.IReflectClass
			 a_class)
		{
			i_yapClassCreationDepth++;
			Db4objects.Db4o.Reflect.IReflectClass superClass = a_class.GetSuperclass();
			Db4objects.Db4o.YapClass superYapClass = null;
			if (superClass != null && !superClass.Equals(Stream().i_handlers.ICLASS_OBJECT))
			{
				superYapClass = GetYapClass(superClass, true);
			}
			bool ret = Stream().CreateYapClass(a_yapClass, a_class, superYapClass);
			i_yapClassCreationDepth--;
			InitYapClassesOnUp();
			return ret;
		}

		internal bool FieldExists(string a_field)
		{
			Db4objects.Db4o.YapClassCollectionIterator i = Iterator();
			while (i.MoveNext())
			{
				if (i.CurrentClass().GetYapField(a_field) != null)
				{
					return true;
				}
			}
			return false;
		}

		internal Db4objects.Db4o.Foundation.Collection4 ForInterface(Db4objects.Db4o.Reflect.IReflectClass
			 claxx)
		{
			Db4objects.Db4o.Foundation.Collection4 col = new Db4objects.Db4o.Foundation.Collection4
				();
			Db4objects.Db4o.YapClassCollectionIterator i = Iterator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.YapClass yc = i.CurrentClass();
				Db4objects.Db4o.Reflect.IReflectClass candidate = yc.ClassReflector();
				if (!candidate.IsInterface())
				{
					if (claxx.IsAssignableFrom(candidate))
					{
						col.Add(yc);
						System.Collections.IEnumerator j = new Db4objects.Db4o.Foundation.Collection4(col
							).GetEnumerator();
						while (j.MoveNext())
						{
							Db4objects.Db4o.YapClass existing = (Db4objects.Db4o.YapClass)j.Current;
							if (existing != yc)
							{
								Db4objects.Db4o.YapClass higher = yc.GetHigherHierarchy(existing);
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
			return Db4objects.Db4o.YapConst.YAPCLASSCOLLECTION;
		}

		internal Db4objects.Db4o.YapClass GetActiveYapClass(Db4objects.Db4o.Reflect.IReflectClass
			 a_class)
		{
			return (Db4objects.Db4o.YapClass)i_yapClassByClass.Get(a_class);
		}

		internal Db4objects.Db4o.YapClass GetYapClass(Db4objects.Db4o.Reflect.IReflectClass
			 a_class, bool a_create)
		{
			Db4objects.Db4o.YapClass yapClass = (Db4objects.Db4o.YapClass)i_yapClassByClass.Get
				(a_class);
			if (yapClass == null)
			{
				yapClass = (Db4objects.Db4o.YapClass)i_yapClassByBytes.Remove(GetNameBytes(a_class
					.GetName()));
				ReadYapClass(yapClass, a_class);
			}
			if (yapClass != null || (!a_create))
			{
				return yapClass;
			}
			yapClass = (Db4objects.Db4o.YapClass)i_creating.Get(a_class);
			if (yapClass != null)
			{
				return yapClass;
			}
			yapClass = new Db4objects.Db4o.YapClass(Stream(), a_class);
			i_creating.Put(a_class, yapClass);
			if (!CreateYapClass(yapClass, a_class))
			{
				i_creating.Remove(a_class);
				return null;
			}
			bool addMembers = false;
			if (i_yapClassByClass.Get(a_class) == null)
			{
				AddYapClass(yapClass);
				addMembers = true;
			}
			int id = yapClass.GetID();
			if (id == 0)
			{
				yapClass.Write(Stream().GetSystemTransaction());
				id = yapClass.GetID();
			}
			if (i_yapClassByID.Get(id) == null)
			{
				i_yapClassByID.Put(id, yapClass);
				addMembers = true;
			}
			if (addMembers || yapClass.i_fields == null)
			{
				_classInits.Process(yapClass);
			}
			i_creating.Remove(a_class);
			Stream().SetDirtyInSystemTransaction(this);
			return yapClass;
		}

		internal Db4objects.Db4o.YapClass GetYapClass(int a_id)
		{
			return ReadYapClass((Db4objects.Db4o.YapClass)i_yapClassByID.Get(a_id), null);
		}

		public Db4objects.Db4o.YapClass GetYapClass(string a_name)
		{
			Db4objects.Db4o.YapClass yapClass = (Db4objects.Db4o.YapClass)i_yapClassByBytes.Remove
				(GetNameBytes(a_name));
			ReadYapClass(yapClass, null);
			if (yapClass == null)
			{
				Db4objects.Db4o.YapClassCollectionIterator i = Iterator();
				while (i.MoveNext())
				{
					yapClass = (Db4objects.Db4o.YapClass)i.Current;
					if (a_name.Equals(yapClass.GetName()))
					{
						ReadYapClass(yapClass, null);
						return yapClass;
					}
				}
				return null;
			}
			return yapClass;
		}

		public int GetYapClassID(string name)
		{
			Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)i_yapClassByBytes.Get(GetNameBytes
				(name));
			if (yc != null)
			{
				return yc.GetID();
			}
			return 0;
		}

		private byte[] GetNameBytes(string name)
		{
			return AsBytes(ResolveAlias(name));
		}

		private string ResolveAlias(string name)
		{
			return Stream().ConfigImpl().ResolveAlias(name);
		}

		internal void InitOnUp(Db4objects.Db4o.Transaction systemTrans)
		{
			i_yapClassCreationDepth++;
			systemTrans.Stream().ShowInternalClasses(true);
			System.Collections.IEnumerator i = i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.YapClass)i.Current).InitOnUp(systemTrans);
			}
			systemTrans.Stream().ShowInternalClasses(false);
			i_yapClassCreationDepth--;
			InitYapClassesOnUp();
		}

		internal void InitTables(int a_size)
		{
			i_classes = new Db4objects.Db4o.Foundation.Collection4();
			i_yapClassByBytes = new Db4objects.Db4o.Foundation.Hashtable4(a_size);
			if (a_size < 16)
			{
				a_size = 16;
			}
			i_yapClassByClass = new Db4objects.Db4o.Foundation.Hashtable4(a_size);
			i_yapClassByID = new Db4objects.Db4o.Foundation.Hashtable4(a_size);
			i_creating = new Db4objects.Db4o.Foundation.Hashtable4(1);
		}

		private void InitYapClassesOnUp()
		{
			if (i_yapClassCreationDepth == 0)
			{
				Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)i_initYapClassesOnUp.Next
					();
				while (yc != null)
				{
					yc.InitOnUp(_systemTransaction);
					yc = (Db4objects.Db4o.YapClass)i_initYapClassesOnUp.Next();
				}
			}
		}

		public Db4objects.Db4o.YapClassCollectionIterator Iterator()
		{
			return new Db4objects.Db4o.YapClassCollectionIterator(this, new Db4objects.Db4o.Foundation.ArrayIterator4
				(i_classes.ToArray()));
		}

		private class ClassIDIterator : Db4objects.Db4o.Foundation.MappingIterator
		{
			public ClassIDIterator(Db4objects.Db4o.Foundation.Collection4 classes) : base(classes
				.GetEnumerator())
			{
			}

			protected override object Map(object current)
			{
				return ((Db4objects.Db4o.YapClass)current).GetID();
			}
		}

		public System.Collections.IEnumerator Ids()
		{
			return new Db4objects.Db4o.YapClassCollection.ClassIDIterator(i_classes);
		}

		public override int OwnLength()
		{
			return Db4objects.Db4o.YapConst.OBJECT_LENGTH + Db4objects.Db4o.YapConst.INT_LENGTH
				 + (i_classes.Size() * Db4objects.Db4o.YapConst.ID_LENGTH);
		}

		internal void Purge()
		{
			System.Collections.IEnumerator i = i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.YapClass)i.Current).Purge();
			}
		}

		public sealed override void ReadThis(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.YapReader
			 a_reader)
		{
			int classCount = a_reader.ReadInt();
			InitTables(classCount);
			for (int i = classCount; i > 0; i--)
			{
				Db4objects.Db4o.YapClass yapClass = new Db4objects.Db4o.YapClass(Stream(), null);
				int id = a_reader.ReadInt();
				yapClass.SetID(id);
				i_classes.Add(yapClass);
				i_yapClassByID.Put(id, yapClass);
				i_yapClassByBytes.Put(yapClass.ReadName(a_trans), yapClass);
			}
			ApplyReadAs();
		}

		private void ApplyReadAs()
		{
			Db4objects.Db4o.Foundation.Hashtable4 readAs = Stream().ConfigImpl().ReadAs();
			readAs.ForEachKey(new _AnonymousInnerClass321(this, readAs));
		}

		private sealed class _AnonymousInnerClass321 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass321(YapClassCollection _enclosing, Db4objects.Db4o.Foundation.Hashtable4
				 readAs)
			{
				this._enclosing = _enclosing;
				this.readAs = readAs;
			}

			public void Visit(object a_object)
			{
				string dbName = (string)a_object;
				byte[] dbbytes = this._enclosing.GetNameBytes(dbName);
				string useName = (string)readAs.Get(dbName);
				byte[] useBytes = this._enclosing.GetNameBytes(useName);
				if (this._enclosing.i_yapClassByBytes.Get(useBytes) == null)
				{
					Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)this._enclosing.i_yapClassByBytes
						.Get(dbbytes);
					if (yc != null)
					{
						yc.i_nameBytes = useBytes;
						yc.SetConfig(this._enclosing.Stream().ConfigImpl().ConfigClass(dbName));
						this._enclosing.i_yapClassByBytes.Put(dbbytes, null);
						this._enclosing.i_yapClassByBytes.Put(useBytes, yc);
					}
				}
			}

			private readonly YapClassCollection _enclosing;

			private readonly Db4objects.Db4o.Foundation.Hashtable4 readAs;
		}

		public Db4objects.Db4o.YapClass ReadYapClass(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.Reflect.IReflectClass
			 a_class)
		{
			if (yapClass != null && !yapClass.StateUnread())
			{
				return yapClass;
			}
			i_yapClassCreationDepth++;
			if (yapClass != null && yapClass.StateUnread())
			{
				yapClass.CreateConfigAndConstructor(i_yapClassByBytes, Stream(), a_class);
				Db4objects.Db4o.Reflect.IReflectClass claxx = yapClass.ClassReflector();
				if (claxx != null)
				{
					i_yapClassByClass.Put(claxx, yapClass);
					yapClass.ReadThis();
					yapClass.CheckChanges();
					i_initYapClassesOnUp.Add(yapClass);
				}
			}
			i_yapClassCreationDepth--;
			InitYapClassesOnUp();
			return yapClass;
		}

		public void RefreshClasses()
		{
			Db4objects.Db4o.YapClassCollection rereader = new Db4objects.Db4o.YapClassCollection
				(_systemTransaction);
			rereader.i_id = i_id;
			rereader.Read(Stream().GetSystemTransaction());
			System.Collections.IEnumerator i = rereader.i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)i.Current;
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
				Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)i.Current;
				yc.Refresh();
			}
		}

		internal void ReReadYapClass(Db4objects.Db4o.YapClass yapClass)
		{
			if (yapClass != null)
			{
				ReReadYapClass(yapClass.i_ancestor);
				yapClass.ReadName(_systemTransaction);
				yapClass.ForceRead();
				yapClass.SetStateClean();
				yapClass.BitFalse(Db4objects.Db4o.YapConst.CHECKED_CHANGES);
				yapClass.BitFalse(Db4objects.Db4o.YapConst.READING);
				yapClass.BitFalse(Db4objects.Db4o.YapConst.CONTINUE);
				yapClass.BitFalse(Db4objects.Db4o.YapConst.DEAD);
				yapClass.CheckChanges();
			}
		}

		public Db4objects.Db4o.Ext.IStoredClass[] StoredClasses()
		{
			Db4objects.Db4o.Foundation.Collection4 classes = new Db4objects.Db4o.Foundation.Collection4
				();
			System.Collections.IEnumerator i = i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)i.Current;
				ReadYapClass(yc, null);
				if (yc.ClassReflector() == null)
				{
					yc.ForceRead();
				}
				classes.Add(yc);
			}
			ApplyReadAs();
			Db4objects.Db4o.Ext.IStoredClass[] sclasses = new Db4objects.Db4o.Ext.IStoredClass
				[classes.Size()];
			classes.ToArray(sclasses);
			return sclasses;
		}

		public void WriteAllClasses()
		{
			Db4objects.Db4o.Ext.IStoredClass[] storedClasses = StoredClasses();
			for (int i = 0; i < storedClasses.Length; i++)
			{
				Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)storedClasses[i];
				yc.SetStateDirty();
			}
			for (int i = 0; i < storedClasses.Length; i++)
			{
				Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)storedClasses[i];
				yc.Write(_systemTransaction);
			}
		}

		public override void WriteThis(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 a_writer)
		{
			a_writer.WriteInt(i_classes.Size());
			System.Collections.IEnumerator i = i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				a_writer.WriteIDOf(trans, i.Current);
			}
		}

		public override string ToString()
		{
			return base.ToString();
			string str = string.Empty;
			System.Collections.IEnumerator i = i_classes.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)i.Current;
				str += yc.GetID() + " " + yc + "\r\n";
			}
			return str;
		}

		public static void Defrag(Db4objects.Db4o.ReaderPair readers)
		{
			int numClasses = readers.ReadInt();
			for (int classIdx = 0; classIdx < numClasses; classIdx++)
			{
				readers.CopyID();
			}
		}

		private Db4objects.Db4o.YapStream Stream()
		{
			return _systemTransaction.Stream();
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

		private Db4objects.Db4o.Inside.SystemData SystemData()
		{
			return _systemTransaction.i_file.SystemData();
		}
	}
}
