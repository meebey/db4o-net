using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.IX;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal
{
	internal sealed class TranslatedFieldMetadata : FieldMetadata
	{
		private readonly IObjectTranslator i_translator;

		internal TranslatedFieldMetadata(ClassMetadata containingClass, IObjectTranslator
			 translator) : base(containingClass, translator)
		{
			i_translator = translator;
			ObjectContainerBase stream = containingClass.GetStream();
			Configure(stream.Reflector().ForClass(TranslatorStoredClass(translator)), false);
		}

		public override bool CanUseNullBitmap()
		{
			return false;
		}

		internal override void Deactivate(Transaction a_trans, object a_onObject, int a_depth
			)
		{
			if (a_depth > 0)
			{
				CascadeActivation(a_trans, a_onObject, a_depth, false);
			}
			SetOn(a_trans.Stream(), a_onObject, null);
		}

		public override object GetOn(Transaction a_trans, object a_OnObject)
		{
			try
			{
				return i_translator.OnStore(a_trans.Stream(), a_OnObject);
			}
			catch (ReflectException e)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new ReflectException(e);
			}
		}

		public override object GetOrCreate(Transaction a_trans, object a_OnObject)
		{
			return GetOn(a_trans, a_OnObject);
		}

		public override void Instantiate(MarshallerFamily mf, ObjectReference a_yapObject
			, object a_onObject, StatefulBuffer a_bytes)
		{
			object toSet = Read(mf, a_bytes);
			a_bytes.GetStream().Activate1(a_bytes.GetTransaction(), toSet, a_bytes.GetInstantiationDepth
				());
			SetOn(a_bytes.GetStream(), a_onObject, toSet);
		}

		internal override void Refresh()
		{
		}

		private void SetOn(ObjectContainerBase a_stream, object a_onObject, object toSet)
		{
			try
			{
				i_translator.OnActivate(a_stream, a_onObject, toSet);
			}
			catch (Exception e)
			{
				throw new ReflectException(e);
			}
		}

		protected override object IndexEntryFor(object indexEntry)
		{
			return indexEntry;
		}

		protected override IIndexable4 IndexHandler(ObjectContainerBase stream)
		{
			return i_handler;
		}
	}
}
