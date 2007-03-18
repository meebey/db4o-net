namespace Db4objects.Db4o.Constraints
{
	/// <summary>configures a field of a class to allow unique values only.</summary>
	/// <remarks>configures a field of a class to allow unique values only.</remarks>
	public class UniqueFieldValueConstraint : Db4objects.Db4o.Config.IConfigurationItem
	{
		private readonly object _clazz;

		private readonly string _fieldName;

		public UniqueFieldValueConstraint(object clazz, string fieldName)
		{
			_clazz = clazz;
			_fieldName = fieldName;
		}

		/// <summary>internal method, public for implementation reasons.</summary>
		/// <remarks>internal method, public for implementation reasons.</remarks>
		public virtual void Apply(Db4objects.Db4o.Internal.ObjectContainerBase objectContainer
			)
		{
			Db4objects.Db4o.Events.EventRegistryFactory.ForObjectContainer(objectContainer).Committing
				 += new Db4objects.Db4o.Events.CommitEventHandler(new _AnonymousInnerClass37(this
				, objectContainer).OnEvent);
		}

		private sealed class _AnonymousInnerClass37
		{
			public _AnonymousInnerClass37(UniqueFieldValueConstraint _enclosing, Db4objects.Db4o.Internal.ObjectContainerBase
				 objectContainer)
			{
				this._enclosing = _enclosing;
				this.objectContainer = objectContainer;
			}

			private Db4objects.Db4o.Internal.FieldMetadata _fieldMetaData;

			private void EnsureSingleOccurence(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Ext.IObjectInfoCollection
				 col)
			{
				System.Collections.IEnumerator i = col.GetEnumerator();
				while (i.MoveNext())
				{
					Db4objects.Db4o.Ext.IObjectInfo info = (Db4objects.Db4o.Ext.IObjectInfo)i.Current;
					int id = (int)info.GetInternalID();
					Db4objects.Db4o.Internal.HardObjectReference @ref = Db4objects.Db4o.Internal.HardObjectReference
						.PeekPersisted(trans, id, 1);
					object fieldValue = this.FieldMetadata().GetOn(trans, @ref._object);
					if (fieldValue == null)
					{
						continue;
					}
					Db4objects.Db4o.Internal.Btree.IBTreeRange range = this.FieldMetadata().Search(trans
						, fieldValue);
					if (range.Size() > 1)
					{
						throw new Db4objects.Db4o.Constraints.UniqueFieldValueConstraintViolationException
							(this.ClassMetadata().GetName(), this.FieldMetadata().GetName());
					}
				}
			}

			private Db4objects.Db4o.Internal.FieldMetadata FieldMetadata()
			{
				if (this._fieldMetaData != null)
				{
					return this._fieldMetaData;
				}
				this._fieldMetaData = this.ClassMetadata().FieldMetadataForName(this._enclosing._fieldName
					);
				return this._fieldMetaData;
			}

			private Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
			{
				Db4objects.Db4o.Reflect.IReflectClass reflectClass = Db4objects.Db4o.Reflect.ReflectorUtils
					.ReflectClassFor(objectContainer.Reflector(), this._enclosing._clazz);
				return objectContainer.ClassMetadataForReflectClass(reflectClass);
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CommitEventArgs args)
			{
				Db4objects.Db4o.Events.CommitEventArgs commitEventArgs = (Db4objects.Db4o.Events.CommitEventArgs
					)args;
				Db4objects.Db4o.Internal.Transaction trans = (Db4objects.Db4o.Internal.Transaction
					)commitEventArgs.Transaction;
				this.EnsureSingleOccurence(trans, commitEventArgs.Added);
				this.EnsureSingleOccurence(trans, commitEventArgs.Updated);
			}

			private readonly UniqueFieldValueConstraint _enclosing;

			private readonly Db4objects.Db4o.Internal.ObjectContainerBase objectContainer;
		}
	}
}
