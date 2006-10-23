namespace Db4objects.Db4o
{
	/// <summary>
	/// Class metadata to be stored to the database file
	/// Don't obfuscate.
	/// </summary>
	/// <remarks>
	/// Class metadata to be stored to the database file
	/// Don't obfuscate.
	/// </remarks>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class MetaClass : Db4objects.Db4o.IInternal4
	{
		/// <summary>persistent field, don't touch</summary>
		public string name;

		/// <summary>persistent field, don't touch</summary>
		public Db4objects.Db4o.MetaField[] fields;

		public MetaClass()
		{
		}

		public MetaClass(string name_)
		{
			name = name_;
		}

		internal virtual Db4objects.Db4o.MetaField EnsureField(Db4objects.Db4o.Transaction
			 trans, string a_name)
		{
			if (fields != null)
			{
				for (int i = 0; i < fields.Length; i++)
				{
					if (fields[i].name.Equals(a_name))
					{
						return fields[i];
					}
				}
				Db4objects.Db4o.MetaField[] temp = new Db4objects.Db4o.MetaField[fields.Length + 
					1];
				System.Array.Copy(fields, 0, temp, 0, fields.Length);
				fields = temp;
			}
			else
			{
				fields = new Db4objects.Db4o.MetaField[1];
			}
			Db4objects.Db4o.MetaField newMetaField = new Db4objects.Db4o.MetaField(a_name);
			fields[fields.Length - 1] = newMetaField;
			trans.Stream().SetInternal(trans, newMetaField, Db4objects.Db4o.YapConst.UNSPECIFIED
				, false);
			trans.Stream().SetInternal(trans, this, Db4objects.Db4o.YapConst.UNSPECIFIED, false
				);
			return newMetaField;
		}
	}
}
