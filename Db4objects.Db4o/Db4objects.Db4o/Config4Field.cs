namespace Db4objects.Db4o
{
	internal class Config4Field : Db4objects.Db4o.Config4Abstract, Db4objects.Db4o.Config.IObjectField
		, Db4objects.Db4o.Foundation.IDeepClone
	{
		private readonly Db4objects.Db4o.Config4Class _configClass;

		private static readonly Db4objects.Db4o.Foundation.KeySpec QUERY_EVALUATION = new 
			Db4objects.Db4o.Foundation.KeySpec(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec INDEXED = new Db4objects.Db4o.Foundation.KeySpec
			(Db4objects.Db4o.YapConst.DEFAULT);

		protected Config4Field(Db4objects.Db4o.Config4Class a_class, Db4objects.Db4o.Foundation.KeySpecHashtable4
			 config) : base(config)
		{
			_configClass = a_class;
		}

		internal Config4Field(Db4objects.Db4o.Config4Class a_class, string a_name)
		{
			_configClass = a_class;
			SetName(a_name);
		}

		private Db4objects.Db4o.Config4Class ClassConfig()
		{
			return _configClass;
		}

		internal override string ClassName()
		{
			return ClassConfig().GetName();
		}

		public virtual object DeepClone(object param)
		{
			return new Db4objects.Db4o.Config4Field((Db4objects.Db4o.Config4Class)param, _config
				);
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

		public virtual void InitOnUp(Db4objects.Db4o.Transaction systemTrans, Db4objects.Db4o.YapField
			 yapField)
		{
			Db4objects.Db4o.YapStream anyStream = systemTrans.Stream();
			if (!anyStream.MaintainsIndices())
			{
				return;
			}
			if (!yapField.SupportsIndex())
			{
				Indexed(false);
			}
			Db4objects.Db4o.YapFile stream = (Db4objects.Db4o.YapFile)anyStream;
			int indexedFlag = _config.GetAsInt(INDEXED);
			if (indexedFlag == Db4objects.Db4o.YapConst.NO)
			{
				yapField.DropIndex(systemTrans);
				return;
			}
			if (UseExistingIndex(systemTrans, yapField))
			{
				return;
			}
			if (indexedFlag != Db4objects.Db4o.YapConst.YES)
			{
				return;
			}
			CreateIndex(systemTrans, yapField, stream);
		}

		private bool UseExistingIndex(Db4objects.Db4o.Transaction systemTrans, Db4objects.Db4o.YapField
			 yapField)
		{
			return yapField.GetIndex(systemTrans) != null;
		}

		private void CreateIndex(Db4objects.Db4o.Transaction systemTrans, Db4objects.Db4o.YapField
			 yapField, Db4objects.Db4o.YapFile stream)
		{
			if (stream.ConfigImpl().MessageLevel() > Db4objects.Db4o.YapConst.NONE)
			{
				stream.Message("creating index " + yapField.ToString());
			}
			yapField.InitIndex(systemTrans);
			stream.SetDirtyInSystemTransaction(yapField.GetParentYapClass());
			Reindex(systemTrans, yapField, stream);
		}

		private void Reindex(Db4objects.Db4o.Transaction systemTrans, Db4objects.Db4o.YapField
			 yapField, Db4objects.Db4o.YapFile stream)
		{
			Db4objects.Db4o.YapClass yapClass = yapField.GetParentYapClass();
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
