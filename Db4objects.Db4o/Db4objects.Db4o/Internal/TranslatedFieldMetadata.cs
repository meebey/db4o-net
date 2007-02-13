namespace Db4objects.Db4o.Internal
{
	internal sealed class TranslatedFieldMetadata : Db4objects.Db4o.Internal.FieldMetadata
	{
		private readonly Db4objects.Db4o.Config.IObjectTranslator i_translator;

		internal TranslatedFieldMetadata(Db4objects.Db4o.Internal.ClassMetadata containingClass
			, Db4objects.Db4o.Config.IObjectTranslator translator) : base(containingClass, translator
			)
		{
			i_translator = translator;
			Db4objects.Db4o.Internal.ObjectContainerBase stream = containingClass.GetStream();
			Configure(stream.Reflector().ForClass(translator.StoredClass()), false);
		}

		public override bool CanUseNullBitmap()
		{
			return false;
		}

		internal override void Deactivate(Db4objects.Db4o.Internal.Transaction a_trans, object
			 a_onObject, int a_depth)
		{
			if (a_depth > 0)
			{
				CascadeActivation(a_trans, a_onObject, a_depth, false);
			}
			SetOn(a_trans.Stream(), a_onObject, null);
		}

		public override object GetOn(Db4objects.Db4o.Internal.Transaction a_trans, object
			 a_OnObject)
		{
			try
			{
				return i_translator.OnStore(a_trans.Stream(), a_OnObject);
			}
			catch
			{
				return null;
			}
		}

		public override object GetOrCreate(Db4objects.Db4o.Internal.Transaction a_trans, 
			object a_OnObject)
		{
			return GetOn(a_trans, a_OnObject);
		}

		public override void Instantiate(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.ObjectReference a_yapObject, object a_onObject, Db4objects.Db4o.Internal.StatefulBuffer
			 a_bytes)
		{
			object toSet = Read(mf, a_bytes);
			a_bytes.GetStream().Activate1(a_bytes.GetTransaction(), toSet, a_bytes.GetInstantiationDepth
				());
			SetOn(a_bytes.GetStream(), a_onObject, toSet);
		}

		internal override void Refresh()
		{
		}

		private void SetOn(Db4objects.Db4o.Internal.ObjectContainerBase a_stream, object 
			a_onObject, object toSet)
		{
			try
			{
				i_translator.OnActivate(a_stream, a_onObject, toSet);
			}
			catch
			{
			}
		}

		protected override object IndexEntryFor(object indexEntry)
		{
			return indexEntry;
		}

		protected override Db4objects.Db4o.Internal.IX.IIndexable4 IndexHandler(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream)
		{
			return i_handler;
		}
	}
}
