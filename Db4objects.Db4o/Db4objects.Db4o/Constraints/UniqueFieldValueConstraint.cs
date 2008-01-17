/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Constraints;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Constraints
{
	/// <summary>configures a field of a class to allow unique values only.</summary>
	/// <remarks>configures a field of a class to allow unique values only.</remarks>
	public class UniqueFieldValueConstraint : IConfigurationItem
	{
		protected readonly object _clazz;

		protected readonly string _fieldName;

		/// <summary>constructor to create a UniqueFieldValueConstraint.</summary>
		/// <remarks>constructor to create a UniqueFieldValueConstraint.</remarks>
		/// <param name="clazz">can be a class (Java) / Type (.NET) / instance of the class / fully qualified class name
		/// 	</param>
		/// <param name="fieldName">the name of the field that is to be unique.</param>
		public UniqueFieldValueConstraint(object clazz, string fieldName)
		{
			_clazz = clazz;
			_fieldName = fieldName;
		}

		public virtual void Prepare(IConfiguration configuration)
		{
		}

		/// <summary>internal method, public for implementation reasons.</summary>
		/// <remarks>internal method, public for implementation reasons.</remarks>
		public virtual void Apply(IInternalObjectContainer objectContainer)
		{
			// Nothing to do...
			EventRegistryFactory.ForObjectContainer(objectContainer).Committing += new Db4objects.Db4o.Events.CommitEventHandler
				(new _IEventListener4_41(this, objectContainer).OnEvent);
		}

		private sealed class _IEventListener4_41
		{
			public _IEventListener4_41(UniqueFieldValueConstraint _enclosing, IInternalObjectContainer
				 objectContainer)
			{
				this._enclosing = _enclosing;
				this.objectContainer = objectContainer;
			}

			private FieldMetadata _fieldMetaData;

			private void EnsureSingleOccurence(Transaction trans, IObjectInfoCollection col)
			{
				IEnumerator i = col.GetEnumerator();
				while (i.MoveNext())
				{
					IObjectInfo info = (IObjectInfo)i.Current;
					int id = (int)info.GetInternalID();
					// TODO: check if the object is of the appropriate
					// type before going further?
					HardObjectReference @ref = HardObjectReference.PeekPersisted(trans, id, 1);
					object fieldValue = this.FieldMetadata().GetOn(trans, @ref._object);
					if (fieldValue == null)
					{
						continue;
					}
					IBTreeRange range = this.FieldMetadata().Search(trans, fieldValue);
					if (range.Size() > 1)
					{
						throw new UniqueFieldValueConstraintViolationException(this.ClassMetadata().GetName
							(), this.FieldMetadata().GetName());
					}
				}
			}

			private bool IsClassMetadataAvailable()
			{
				return null != this.ClassMetadata();
			}

			private FieldMetadata FieldMetadata()
			{
				if (this._fieldMetaData != null)
				{
					return this._fieldMetaData;
				}
				this._fieldMetaData = this.ClassMetadata().FieldMetadataForName(this._enclosing._fieldName
					);
				return this._fieldMetaData;
			}

			private ClassMetadata ClassMetadata()
			{
				return objectContainer.ClassMetadataForReflectClass(this.ReflectClass());
			}

			private IReflectClass ReflectClass()
			{
				return ReflectorUtils.ReflectClassFor(objectContainer.Reflector(), this._enclosing
					._clazz);
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CommitEventArgs args)
			{
				if (!this.IsClassMetadataAvailable())
				{
					return;
				}
				CommitEventArgs commitEventArgs = (CommitEventArgs)args;
				Transaction trans = (Transaction)commitEventArgs.Transaction();
				this.EnsureSingleOccurence(trans, commitEventArgs.Added);
				this.EnsureSingleOccurence(trans, commitEventArgs.Updated);
			}

			private readonly UniqueFieldValueConstraint _enclosing;

			private readonly IInternalObjectContainer objectContainer;
		}
	}
}
