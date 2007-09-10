/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Config4Class : Config4Abstract, IObjectClass, IDeepClone
	{
		private readonly Config4Impl _configImpl;

		private static readonly KeySpec CALL_CONSTRUCTOR = new KeySpec(TernaryBool.UNSPECIFIED
			);

		private static readonly KeySpec CLASS_INDEXED = new KeySpec(true);

		private static readonly KeySpec EXCEPTIONAL_FIELDS = new KeySpec(null);

		private static readonly KeySpec GENERATE_UUIDS = new KeySpec(false);

		private static readonly KeySpec GENERATE_VERSION_NUMBERS = new KeySpec(false);

		/// <summary>
		/// We are running into cyclic dependancies on reading the PBootRecord
		/// object, if we maintain MetaClass information there
		/// </summary>
		private static readonly KeySpec MAINTAIN_METACLASS = new KeySpec(true);

		private static readonly KeySpec MARSHALLER = new KeySpec(null);

		private static readonly KeySpec MAXIMUM_ACTIVATION_DEPTH = new KeySpec(0);

		private static readonly KeySpec MINIMUM_ACTIVATION_DEPTH = new KeySpec(0);

		private static readonly KeySpec PERSIST_STATIC_FIELD_VALUES = new KeySpec(false);

		private static readonly KeySpec QUERY_ATTRIBUTE_PROVIDER = new KeySpec(null);

		private static readonly KeySpec STORE_TRANSIENT_FIELDS = new KeySpec(false);

		private static readonly KeySpec TRANSLATOR = new KeySpec(null);

		private static readonly KeySpec TRANSLATOR_NAME = new KeySpec((string)null);

		private static readonly KeySpec UPDATE_DEPTH = new KeySpec(0);

		private static readonly KeySpec WRITE_AS = new KeySpec((string)null);

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
			PutThreeValued(CALL_CONSTRUCTOR, flag);
		}

		internal override string ClassName()
		{
			return GetName();
		}

		internal virtual IReflectClass ClassReflector()
		{
			return Config().Reflector().ForName(GetName());
		}

		public virtual void Compare(IObjectAttribute comparator)
		{
			_config.Put(QUERY_ATTRIBUTE_PROVIDER, comparator);
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
			_config.Put(GENERATE_UUIDS, setting);
		}

		public virtual void GenerateVersionNumbers(bool setting)
		{
			_config.Put(GENERATE_VERSION_NUMBERS, setting);
		}

		public virtual IObjectTranslator GetTranslator()
		{
			IObjectTranslator translator = (IObjectTranslator)_config.Get(TRANSLATOR);
			if (translator != null)
			{
				return translator;
			}
			string translatorName = _config.GetAsString(TRANSLATOR_NAME);
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
			_config.Put(CLASS_INDEXED, flag);
		}

		public virtual bool Indexed()
		{
			return _config.GetAsBoolean(CLASS_INDEXED);
		}

		internal virtual object Instantiate(ObjectContainerBase a_stream, object a_toTranslate
			)
		{
			return ((IObjectConstructor)_config.Get(TRANSLATOR)).OnInstantiate((IInternalObjectContainer
				)a_stream, a_toTranslate);
		}

		internal virtual bool Instantiates()
		{
			return GetTranslator() is IObjectConstructor;
		}

		public virtual void MarshallWith(IObjectMarshaller marshaller)
		{
			_config.Put(MARSHALLER, marshaller);
		}

		internal virtual IObjectMarshaller GetMarshaller()
		{
			return (IObjectMarshaller)_config.Get(MARSHALLER);
		}

		public virtual void MaximumActivationDepth(int depth)
		{
			_config.Put(MAXIMUM_ACTIVATION_DEPTH, depth);
		}

		internal virtual int MaximumActivationDepth()
		{
			return _config.GetAsInt(MAXIMUM_ACTIVATION_DEPTH);
		}

		public virtual void MinimumActivationDepth(int depth)
		{
			_config.Put(MINIMUM_ACTIVATION_DEPTH, depth);
		}

		public virtual int MinimumActivationDepth()
		{
			return _config.GetAsInt(MINIMUM_ACTIVATION_DEPTH);
		}

		public virtual TernaryBool CallConstructor()
		{
			if (_config.Get(TRANSLATOR) != null)
			{
				return TernaryBool.YES;
			}
			return _config.GetAsTernaryBool(CALL_CONSTRUCTOR);
		}

		private Hashtable4 ExceptionalFieldsOrNull()
		{
			return (Hashtable4)_config.Get(EXCEPTIONAL_FIELDS);
		}

		private Hashtable4 ExceptionalFields()
		{
			Hashtable4 exceptionalFieldsCollection = ExceptionalFieldsOrNull();
			if (exceptionalFieldsCollection == null)
			{
				exceptionalFieldsCollection = new Hashtable4(16);
				_config.Put(EXCEPTIONAL_FIELDS, exceptionalFieldsCollection);
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
			_config.Put(PERSIST_STATIC_FIELD_VALUES, true);
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

		public virtual void ReadAs(object clazz)
		{
			Config4Impl configRef = Config();
			IReflectClass claxx = configRef.ReflectorFor(clazz);
			if (claxx == null)
			{
				return;
			}
			_config.Put(WRITE_AS, GetName());
			configRef.ReadAs().Put(GetName(), claxx.GetName());
		}

		public virtual void Rename(string newName)
		{
			Config().Rename(new Db4objects.Db4o.Rename(string.Empty, GetName(), newName));
			SetName(newName);
		}

		public virtual void StoreTransientFields(bool flag)
		{
			_config.Put(STORE_TRANSIENT_FIELDS, flag);
		}

		public virtual void Translate(IObjectTranslator translator)
		{
			if (translator == null)
			{
				_config.Put(TRANSLATOR_NAME, null);
			}
			_config.Put(TRANSLATOR, translator);
		}

		internal virtual void TranslateOnDemand(string a_translatorName)
		{
			_config.Put(TRANSLATOR_NAME, a_translatorName);
		}

		public virtual void UpdateDepth(int depth)
		{
			_config.Put(UPDATE_DEPTH, depth);
		}

		internal virtual Config4Impl Config()
		{
			return _configImpl;
		}

		internal virtual bool GenerateUUIDs()
		{
			return _config.GetAsBoolean(GENERATE_UUIDS);
		}

		internal virtual bool GenerateVersionNumbers()
		{
			return _config.GetAsBoolean(GENERATE_VERSION_NUMBERS);
		}

		internal virtual void MaintainMetaClass(bool flag)
		{
			_config.Put(MAINTAIN_METACLASS, flag);
		}

		internal virtual bool StaticFieldValuesArePersisted()
		{
			return _config.GetAsBoolean(PERSIST_STATIC_FIELD_VALUES);
		}

		public virtual IObjectAttribute QueryAttributeProvider()
		{
			return (IObjectAttribute)_config.Get(QUERY_ATTRIBUTE_PROVIDER);
		}

		internal virtual bool StoreTransientFields()
		{
			return _config.GetAsBoolean(STORE_TRANSIENT_FIELDS);
		}

		internal virtual int UpdateDepth()
		{
			return _config.GetAsInt(UPDATE_DEPTH);
		}

		internal virtual string WriteAs()
		{
			return _config.GetAsString(WRITE_AS);
		}
	}
}
