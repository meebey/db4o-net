namespace Db4objects.Db4o.Internal
{
	internal class Config4Field : Db4objects.Db4o.Internal.Config4Abstract, Db4objects.Db4o.Config.IObjectField
		, Db4objects.Db4o.Foundation.IDeepClone
	{
		private readonly Db4objects.Db4o.Internal.Config4Class _configClass;

		private static readonly Db4objects.Db4o.Foundation.KeySpec QUERY_EVALUATION = new 
			Db4objects.Db4o.Foundation.KeySpec(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec INDEXED = new Db4objects.Db4o.Foundation.KeySpec
			(Db4objects.Db4o.Internal.Const4.DEFAULT);

		protected Config4Field(Db4objects.Db4o.Internal.Config4Class a_class, Db4objects.Db4o.Foundation.KeySpecHashtable4
			 config) : base(config)
		{
			_configClass = a_class;
		}

		internal Config4Field(Db4objects.Db4o.Internal.Config4Class a_class, string a_name
			)
		{
			_configClass = a_class;
			SetName(a_name);
		}

		private Db4objects.Db4o.Internal.Config4Class ClassConfig()
		{
			return _configClass;
		}

		internal override string ClassName()
		{
			return ClassConfig().GetName();
		}

		public virtual object DeepClone(object param)
		{
			return new Db4objects.Db4o.Internal.Config4Field((Db4objects.Db4o.Internal.Config4Class
				)param, _config);
		}

		public virtual void QueryEvaluation(bool flag)
		{
			_config.Put(QUERY_EVALUATION, flag);
		}

		public virtual void Rename(string newName)
		{
			ClassConfig().Config().Rename(new Db4objects.Db4o.Rename(ClassName(), GetName(), 
				newName));
			SetName(newName);
		}

		public virtual void Indexed(bool flag)
		{
			PutThreeValued(INDEXED, flag);
		}

		public virtual void InitOnUp(Db4objects.Db4o.Internal.Transaction systemTrans, Db4objects.Db4o.Internal.FieldMetadata
			 yapField)
		{
			Db4objects.Db4o.Internal.ObjectContainerBase anyStream = systemTrans.Stream();
			if (!anyStream.MaintainsIndices())
			{
				return;
			}
			if (!yapField.SupportsIndex())
			{
				Indexed(false);
			}
			Db4objects.Db4o.Internal.LocalObjectContainer stream = (Db4objects.Db4o.Internal.LocalObjectContainer
				)anyStream;
			int indexedFlag = _config.GetAsInt(INDEXED);
			if (indexedFlag == Db4objects.Db4o.Internal.Const4.NO)
			{
				yapField.DropIndex(systemTrans);
				return;
			}
			if (UseExistingIndex(systemTrans, yapField))
			{
				return;
			}
			if (indexedFlag != Db4objects.Db4o.Internal.Const4.YES)
			{
				return;
			}
			CreateIndex(systemTrans, yapField, stream);
		}

		private bool UseExistingIndex(Db4objects.Db4o.Internal.Transaction systemTrans, Db4objects.Db4o.Internal.FieldMetadata
			 yapField)
		{
			return yapField.GetIndex(systemTrans) != null;
		}

		private void CreateIndex(Db4objects.Db4o.Internal.Transaction systemTrans, Db4objects.Db4o.Internal.FieldMetadata
			 yapField, Db4objects.Db4o.Internal.LocalObjectContainer stream)
		{
			if (stream.ConfigImpl().MessageLevel() > Db4objects.Db4o.Internal.Const4.NONE)
			{
				stream.Message("creating index " + yapField.ToString());
			}
			yapField.InitIndex(systemTrans);
			stream.SetDirtyInSystemTransaction(yapField.GetParentYapClass());
			Reindex(systemTrans, yapField, stream);
		}

		private void Reindex(Db4objects.Db4o.Internal.Transaction systemTrans, Db4objects.Db4o.Internal.FieldMetadata
			 yapField, Db4objects.Db4o.Internal.LocalObjectContainer stream)
		{
			Db4objects.Db4o.Internal.ClassMetadata yapClass = yapField.GetParentYapClass();
			if (yapField.RebuildIndexForClass(stream, yapClass))
			{
				systemTrans.Commit();
			}
		}

		internal virtual bool QueryEvaluation()
		{
			return _config.GetAsBoolean(QUERY_EVALUATION);
		}
	}
}
