/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	internal class Config4Field : Config4Abstract, IObjectField, IDeepClone
	{
		private readonly Config4Class _configClass;

		private static readonly KeySpec QUERY_EVALUATION = new KeySpec(true);

		private static readonly KeySpec INDEXED = new KeySpec(TernaryBool.UNSPECIFIED);

		protected Config4Field(Config4Class a_class, KeySpecHashtable4 config) : base(config
			)
		{
			_configClass = a_class;
		}

		internal Config4Field(Config4Class a_class, string a_name)
		{
			_configClass = a_class;
			SetName(a_name);
		}

		private Config4Class ClassConfig()
		{
			return _configClass;
		}

		internal override string ClassName()
		{
			return ClassConfig().GetName();
		}

		public virtual object DeepClone(object param)
		{
			return new Db4objects.Db4o.Internal.Config4Field((Config4Class)param, _config);
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

		public virtual void InitOnUp(Transaction systemTrans, FieldMetadata yapField)
		{
			ObjectContainerBase anyStream = systemTrans.Stream();
			if (!anyStream.MaintainsIndices())
			{
				return;
			}
			if (!yapField.SupportsIndex())
			{
				Indexed(false);
			}
			TernaryBool indexedFlag = _config.GetAsTernaryBool(INDEXED);
			if (indexedFlag.DefiniteNo())
			{
				yapField.DropIndex(systemTrans);
				return;
			}
			if (UseExistingIndex(systemTrans, yapField))
			{
				return;
			}
			if (!indexedFlag.DefiniteYes())
			{
				return;
			}
			yapField.CreateIndex();
		}

		private bool UseExistingIndex(Transaction systemTrans, FieldMetadata yapField)
		{
			return yapField.GetIndex(systemTrans) != null;
		}

		internal virtual bool QueryEvaluation()
		{
			return _config.GetAsBoolean(QUERY_EVALUATION);
		}
	}
}
