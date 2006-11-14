namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class Config4Class : Db4objects.Db4o.Config4Abstract, Db4objects.Db4o.Config.IObjectClass
		, Db4objects.Db4o.Foundation.IDeepClone
	{
		private readonly Db4objects.Db4o.Config4Impl _configImpl;

		private static readonly Db4objects.Db4o.Foundation.KeySpec CALL_CONSTRUCTOR = new 
			Db4objects.Db4o.Foundation.KeySpec(0);

		private static readonly Db4objects.Db4o.Foundation.KeySpec EXCEPTIONAL_FIELDS = new 
			Db4objects.Db4o.Foundation.KeySpec(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec GENERATE_UUIDS = new Db4objects.Db4o.Foundation.KeySpec
			(0);

		private static readonly Db4objects.Db4o.Foundation.KeySpec GENERATE_VERSION_NUMBERS
			 = new Db4objects.Db4o.Foundation.KeySpec(0);

		/// <summary>
		/// We are running into cyclic dependancies on reading the PBootRecord
		/// object, if we maintain MetaClass information there
		/// </summary>
		private static readonly Db4objects.Db4o.Foundation.KeySpec MAINTAIN_METACLASS = new 
			Db4objects.Db4o.Foundation.KeySpec(true);

		private static readonly Db4objects.Db4o.Foundation.KeySpec MAXIMUM_ACTIVATION_DEPTH
			 = new Db4objects.Db4o.Foundation.KeySpec(0);

		private static readonly Db4objects.Db4o.Foundation.KeySpec METACLASS = new Db4objects.Db4o.Foundation.KeySpec
			(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec MINIMUM_ACTIVATION_DEPTH
			 = new Db4objects.Db4o.Foundation.KeySpec(0);

		private static readonly Db4objects.Db4o.Foundation.KeySpec PERSIST_STATIC_FIELD_VALUES
			 = new Db4objects.Db4o.Foundation.KeySpec(false);

		private static readonly Db4objects.Db4o.Foundation.KeySpec QUERY_ATTRIBUTE_PROVIDER
			 = new Db4objects.Db4o.Foundation.KeySpec(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec STORE_TRANSIENT_FIELDS
			 = new Db4objects.Db4o.Foundation.KeySpec(false);

		private static readonly Db4objects.Db4o.Foundation.KeySpec TRANSLATOR = new Db4objects.Db4o.Foundation.KeySpec
			(null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec TRANSLATOR_NAME = new 
			Db4objects.Db4o.Foundation.KeySpec((string)null);

		private static readonly Db4objects.Db4o.Foundation.KeySpec UPDATE_DEPTH = new Db4objects.Db4o.Foundation.KeySpec
			(0);

		private static readonly Db4objects.Db4o.Foundation.KeySpec WRITE_AS = new Db4objects.Db4o.Foundation.KeySpec
			((string)null);

		protected Config4Class(Db4objects.Db4o.Config4Impl configuration, Db4objects.Db4o.Foundation.KeySpecHashtable4
			 config) : base(config)
		{
			_configImpl = configuration;
		}

		internal Config4Class(Db4objects.Db4o.Config4Impl a_configuration, string a_name)
		{
			_configImpl = a_configuration;
			SetName(a_name);
		}

		internal virtual int AdjustActivationDepth(int a_depth)
		{
			if ((CascadeOnActivate() == Db4objects.Db4o.YapConst.YES) && a_depth < 2)
			{
				a_depth = 2;
			}
			if ((CascadeOnActivate() == Db4objects.Db4o.YapConst.NO) && a_depth > 1)
			{
				a_depth = 1;
			}
			if (Config().ClassActivationDepthConfigurable())
			{
				int minimumActivationDepth = _config.GetAsInt(MINIMUM_ACTIVATION_DEPTH);
				if (minimumActivationDepth != 0)
				{
					if (a_depth < minimumActivationDepth)
					{
						a_depth = minimumActivationDepth;
					}
				}
				int maximumActivationDepth = _config.GetAsInt(MAXIMUM_ACTIVATION_DEPTH);
				if (maximumActivationDepth != 0)
				{
					if (a_depth > maximumActivationDepth)
					{
						a_depth = maximumActivationDepth;
					}
				}
			}
			return a_depth;
		}

		public virtual void CallConstructor(bool flag)
		{
			PutThreeValued(CALL_CONSTRUCTOR, flag);
		}

		internal override string ClassName()
		{
			return GetName();
		}

		internal virtual Db4objects.Db4o.Reflect.IReflectClass ClassReflector()
		{
			return Config().Reflector().ForName(GetName());
		}

		public virtual void Compare(Db4objects.Db4o.Config.IObjectAttribute comparator)
		{
			_config.Put(QUERY_ATTRIBUTE_PROVIDER, comparator);
		}

		internal virtual Db4objects.Db4o.Config4Field ConfigField(string fieldName)
		{
			Db4objects.Db4o.Foundation.Hashtable4 exceptionalFields = ExceptionalFieldsOrNull
				();
			if (exceptionalFields == null)
			{
				return null;
			}
			return (Db4objects.Db4o.Config4Field)exceptionalFields.Get(fieldName);
		}

		public virtual object DeepClone(object param)
		{
			return new Db4objects.Db4o.Config4Class((Db4objects.Db4o.Config4Impl)param, _config
				);
		}

		public virtual void EnableReplication(bool setting)
		{
			GenerateUUIDs(setting);
			GenerateVersionNumbers(setting);
		}

		public virtual void GenerateUUIDs(bool setting)
		{
			PutThreeValued(GENERATE_UUIDS, setting);
		}

		public virtual void GenerateVersionNumbers(bool setting)
		{
			PutThreeValued(GENERATE_VERSION_NUMBERS, setting);
		}

		public virtual Db4objects.Db4o.Config.IObjectTranslator GetTranslator()
		{
			Db4objects.Db4o.Config.IObjectTranslator translator = (Db4objects.Db4o.Config.IObjectTranslator
				)_config.Get(TRANSLATOR);
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
				translator = (Db4objects.Db4o.Config.IObjectTranslator)Config().Reflector().ForName
					(translatorName).NewInstance();
			}
			catch
			{
				Db4objects.Db4o.Messages.LogErr(Config(), 48, translatorName, null);
				TranslateOnDemand(null);
			}
			Translate(translator);
			return translator;
		}

		internal virtual object Instantiate(Db4objects.Db4o.YapStream a_stream, object a_toTranslate
			)
		{
			return ((Db4objects.Db4o.Config.IObjectConstructor)_config.Get(TRANSLATOR)).OnInstantiate
				(a_stream, a_toTranslate);
		}

		internal virtual bool Instantiates()
		{
			return GetTranslator() is Db4objects.Db4o.Config.IObjectConstructor;
		}

		public virtual void MaximumActivationDepth(int depth)
		{
			_config.Put(MAXIMUM_ACTIVATION_DEPTH, depth);
		}

		public virtual void MinimumActivationDepth(int depth)
		{
			_config.Put(MINIMUM_ACTIVATION_DEPTH, depth);
		}

		public virtual int CallConstructor()
		{
			if (_config.Get(TRANSLATOR) != null)
			{
				return Db4objects.Db4o.YapConst.YES;
			}
			return _config.GetAsInt(CALL_CONSTRUCTOR);
		}

		private Db4objects.Db4o.Foundation.Hashtable4 ExceptionalFieldsOrNull()
		{
			return (Db4objects.Db4o.Foundation.Hashtable4)_config.Get(EXCEPTIONAL_FIELDS);
		}

		private Db4objects.Db4o.Foundation.Hashtable4 ExceptionalFields()
		{
			Db4objects.Db4o.Foundation.Hashtable4 exceptionalFieldsCollection = ExceptionalFieldsOrNull
				();
			if (exceptionalFieldsCollection == null)
			{
				exceptionalFieldsCollection = new Db4objects.Db4o.Foundation.Hashtable4(16);
				_config.Put(EXCEPTIONAL_FIELDS, exceptionalFieldsCollection);
			}
			return exceptionalFieldsCollection;
		}

		public virtual Db4objects.Db4o.Config.IObjectField ObjectField(string fieldName)
		{
			Db4objects.Db4o.Foundation.Hashtable4 exceptionalFieldsCollection = ExceptionalFields
				();
			Db4objects.Db4o.Config4Field c4f = (Db4objects.Db4o.Config4Field)exceptionalFieldsCollection
				.Get(fieldName);
			if (c4f == null)
			{
				c4f = new Db4objects.Db4o.Config4Field(this, fieldName);
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
			Db4objects.Db4o.Foundation.Hashtable4 exceptionalFields = ExceptionalFieldsOrNull
				();
			if (exceptionalFields != null)
			{
				Db4objects.Db4o.Config4Field field = (Db4objects.Db4o.Config4Field)exceptionalFields
					.Get(fieldName);
				if (field != null)
				{
					return field.QueryEvaluation();
				}
			}
			return true;
		}

		public virtual void ReadAs(object clazz)
		{
			Db4objects.Db4o.Config4Impl configRef = Config();
			Db4objects.Db4o.Reflect.IReflectClass claxx = configRef.ReflectorFor(clazz);
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

		public virtual void Translate(Db4objects.Db4o.Config.IObjectTranslator translator
			)
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

		internal virtual Db4objects.Db4o.Config4Impl Config()
		{
			return _configImpl;
		}

		internal virtual int GenerateUUIDs()
		{
			return _config.GetAsInt(GENERATE_UUIDS);
		}

		internal virtual int GenerateVersionNumbers()
		{
			return _config.GetAsInt(GENERATE_VERSION_NUMBERS);
		}

		internal virtual void MaintainMetaClass(bool flag)
		{
			_config.Put(MAINTAIN_METACLASS, flag);
		}

		internal virtual Db4objects.Db4o.MetaClass MetaClass()
		{
			return (Db4objects.Db4o.MetaClass)_config.Get(METACLASS);
		}

		internal virtual bool StaticFieldValuesArePersisted()
		{
			return _config.GetAsBoolean(PERSIST_STATIC_FIELD_VALUES);
		}

		internal virtual Db4objects.Db4o.Config.IObjectAttribute QueryAttributeProvider()
		{
			return (Db4objects.Db4o.Config.IObjectAttribute)_config.Get(QUERY_ATTRIBUTE_PROVIDER
				);
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
