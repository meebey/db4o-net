/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Config4Class : Config4Abstract, IObjectClass, IDeepClone
	{
		private readonly Config4Impl _configImpl;

		private static readonly KeySpec CallConstructorKey = new KeySpec(TernaryBool.Unspecified
			);

		private static readonly KeySpec ClassIndexedKey = new KeySpec(true);

		private static readonly KeySpec ExceptionalFieldsKey = new KeySpec(null);

		private static readonly KeySpec GenerateUuidsKey = new KeySpec(false);

		private static readonly KeySpec GenerateVersionNumbersKey = new KeySpec(false);

		/// <summary>
		/// We are running into cyclic dependancies on reading the PBootRecord
		/// object, if we maintain MetaClass information there
		/// </summary>
		private static readonly KeySpec MaintainMetaclassKey = new KeySpec(true);

		private static readonly KeySpec MaximumActivationDepthKey = new KeySpec(0);

		private static readonly KeySpec MinimumActivationDepthKey = new KeySpec(0);

		private static readonly KeySpec PersistStaticFieldValuesKey = new KeySpec(false);

		private static readonly KeySpec QueryAttributeProviderKey = new KeySpec(null);

		private static readonly KeySpec StoreTransientFieldsKey = new KeySpec(false);

		private static readonly KeySpec TranslatorKey = new KeySpec(null);

		private static readonly KeySpec TranslatorNameKey = new KeySpec((string)null);

		private static readonly KeySpec UpdateDepthKey = new KeySpec(0);

		private static readonly KeySpec WriteAsKey = new KeySpec((string)null);

		protected Config4Class(Config4Impl configuration, KeySpecHashtable4 config) : base
			(config)
		{
			_configImpl = configuration;
		}

		internal Config4Class(Config4Impl a_configuration, string a_name)
		{
			_configImpl = a_configuration;
			SetName(a_name);
		}

		public virtual int AdjustActivationDepth(int depth)
		{
			TernaryBool cascadeOnActivate = CascadeOnActivate();
			if (cascadeOnActivate.DefiniteYes() && depth < 2)
			{
				depth = 2;
			}
			if (cascadeOnActivate.DefiniteNo() && depth > 1)
			{
				depth = 1;
			}
			if (Config().ClassActivationDepthConfigurable())
			{
				int minimumActivationDepth = MinimumActivationDepth();
				if (minimumActivationDepth != 0 && depth < minimumActivationDepth)
				{
					depth = minimumActivationDepth;
				}
				int maximumActivationDepth = MaximumActivationDepth();
				if (maximumActivationDepth != 0 && depth > maximumActivationDepth)
				{
					depth = maximumActivationDepth;
				}
			}
			return depth;
		}

		public virtual void CallConstructor(bool flag)
		{
			PutThreeValued(CallConstructorKey, flag);
		}

		internal override string ClassName()
		{
			return GetName();
		}

		internal virtual IReflectClass ClassReflector()
		{
			return Config().Reflector().ForName(GetName());
		}

		[System.ObsoleteAttribute]
		public virtual void Compare(IObjectAttribute comparator)
		{
			_config.Put(QueryAttributeProviderKey, comparator);
		}

		internal virtual Config4Field ConfigField(string fieldName)
		{
			Hashtable4 exceptionalFields = ExceptionalFieldsOrNull();
			if (exceptionalFields == null)
			{
				return null;
			}
			return (Config4Field)exceptionalFields.Get(fieldName);
		}

		public virtual object DeepClone(object param)
		{
			return new Db4objects.Db4o.Internal.Config4Class((Config4Impl)param, _config);
		}

		public virtual void EnableReplication(bool setting)
		{
			GenerateUUIDs(setting);
			GenerateVersionNumbers(setting);
		}

		public virtual void GenerateUUIDs(bool setting)
		{
			_config.Put(GenerateUuidsKey, setting);
		}

		public virtual void GenerateVersionNumbers(bool setting)
		{
			_config.Put(GenerateVersionNumbersKey, setting);
		}

		public virtual IObjectTranslator GetTranslator()
		{
			IObjectTranslator translator = (IObjectTranslator)_config.Get(TranslatorKey);
			if (translator != null)
			{
				return translator;
			}
			string translatorName = _config.GetAsString(TranslatorNameKey);
			if (translatorName == null)
			{
				return null;
			}
			try
			{
				translator = NewTranslatorFromReflector(translatorName);
			}
			catch (Exception)
			{
				try
				{
					translator = NewTranslatorFromPlatform(translatorName);
				}
				catch (Exception e)
				{
					throw new Db4oException(e);
				}
			}
			Translate(translator);
			return translator;
		}

		/// <exception cref="InstantiationException"></exception>
		/// <exception cref="MemberAccessException"></exception>
		private IObjectTranslator NewTranslatorFromPlatform(string translatorName)
		{
			return (IObjectTranslator)System.Activator.CreateInstance(ReflectPlatform.ForName
				(translatorName));
		}

		private IObjectTranslator NewTranslatorFromReflector(string translatorName)
		{
			return (IObjectTranslator)Config().Reflector().ForName(translatorName).NewInstance
				();
		}

		public virtual void Indexed(bool flag)
		{
			_config.Put(ClassIndexedKey, flag);
		}

		public virtual bool Indexed()
		{
			return _config.GetAsBoolean(ClassIndexedKey);
		}

		internal virtual object Instantiate(ObjectContainerBase a_stream, object a_toTranslate
			)
		{
			return ((IObjectConstructor)_config.Get(TranslatorKey)).OnInstantiate((IInternalObjectContainer
				)a_stream, a_toTranslate);
		}

		internal virtual bool Instantiates()
		{
			return GetTranslator() is IObjectConstructor;
		}

		public virtual void MaximumActivationDepth(int depth)
		{
			_config.Put(MaximumActivationDepthKey, depth);
		}

		internal virtual int MaximumActivationDepth()
		{
			return _config.GetAsInt(MaximumActivationDepthKey);
		}

		public virtual void MinimumActivationDepth(int depth)
		{
			_config.Put(MinimumActivationDepthKey, depth);
		}

		public virtual int MinimumActivationDepth()
		{
			return _config.GetAsInt(MinimumActivationDepthKey);
		}

		public virtual TernaryBool CallConstructor()
		{
			if (_config.Get(TranslatorKey) != null)
			{
				return TernaryBool.Yes;
			}
			return _config.GetAsTernaryBool(CallConstructorKey);
		}

		private Hashtable4 ExceptionalFieldsOrNull()
		{
			return (Hashtable4)_config.Get(ExceptionalFieldsKey);
		}

		private Hashtable4 ExceptionalFields()
		{
			Hashtable4 exceptionalFieldsCollection = ExceptionalFieldsOrNull();
			if (exceptionalFieldsCollection == null)
			{
				exceptionalFieldsCollection = new Hashtable4(16);
				_config.Put(ExceptionalFieldsKey, exceptionalFieldsCollection);
			}
			return exceptionalFieldsCollection;
		}

		public virtual IObjectField ObjectField(string fieldName)
		{
			Hashtable4 exceptionalFieldsCollection = ExceptionalFields();
			Config4Field c4f = (Config4Field)exceptionalFieldsCollection.Get(fieldName);
			if (c4f == null)
			{
				c4f = new Config4Field(this, fieldName);
				exceptionalFieldsCollection.Put(fieldName, c4f);
			}
			return c4f;
		}

		public virtual void PersistStaticFieldValues()
		{
			_config.Put(PersistStaticFieldValuesKey, true);
		}

		internal virtual bool QueryEvaluation(string fieldName)
		{
			Hashtable4 exceptionalFields = ExceptionalFieldsOrNull();
			if (exceptionalFields != null)
			{
				Config4Field field = (Config4Field)exceptionalFields.Get(fieldName);
				if (field != null)
				{
					return field.QueryEvaluation();
				}
			}
			return true;
		}

		[System.ObsoleteAttribute]
		public virtual void ReadAs(object clazz)
		{
			Config4Impl configRef = Config();
			IReflectClass claxx = configRef.ReflectorFor(clazz);
			if (claxx == null)
			{
				return;
			}
			_config.Put(WriteAsKey, GetName());
			configRef.ReadAs().Put(GetName(), claxx.GetName());
		}

		public virtual void Rename(string newName)
		{
			Config().Rename(new Db4objects.Db4o.Rename(string.Empty, GetName(), newName));
			SetName(newName);
		}

		public virtual void StoreTransientFields(bool flag)
		{
			_config.Put(StoreTransientFieldsKey, flag);
		}

		public virtual void Translate(IObjectTranslator translator)
		{
			if (translator == null)
			{
				_config.Put(TranslatorNameKey, null);
			}
			_config.Put(TranslatorKey, translator);
		}

		internal virtual void TranslateOnDemand(string a_translatorName)
		{
			_config.Put(TranslatorNameKey, a_translatorName);
		}

		public virtual void UpdateDepth(int depth)
		{
			_config.Put(UpdateDepthKey, depth);
		}

		internal virtual Config4Impl Config()
		{
			return _configImpl;
		}

		internal virtual bool GenerateUUIDs()
		{
			return _config.GetAsBoolean(GenerateUuidsKey);
		}

		internal virtual bool GenerateVersionNumbers()
		{
			return _config.GetAsBoolean(GenerateVersionNumbersKey);
		}

		internal virtual void MaintainMetaClass(bool flag)
		{
			_config.Put(MaintainMetaclassKey, flag);
		}

		internal virtual bool StaticFieldValuesArePersisted()
		{
			return _config.GetAsBoolean(PersistStaticFieldValuesKey);
		}

		public virtual IObjectAttribute QueryAttributeProvider()
		{
			return (IObjectAttribute)_config.Get(QueryAttributeProviderKey);
		}

		internal virtual bool StoreTransientFields()
		{
			return _config.GetAsBoolean(StoreTransientFieldsKey);
		}

		internal virtual int UpdateDepth()
		{
			return _config.GetAsInt(UpdateDepthKey);
		}

		internal virtual string WriteAs()
		{
			return _config.GetAsString(WriteAsKey);
		}
	}
}
