/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
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
			ObjectContainerBase stream = containingClass.Container();
			Configure(stream.Reflector().ForClass(TranslatorStoredClass(translator)), false);
		}

		public override bool CanUseNullBitmap()
		{
			return false;
		}

		internal override void Deactivate(Transaction trans, object onObject, int depth)
		{
			if (depth > 0)
			{
				CascadeActivation(trans, onObject, depth, false);
			}
			SetOn(trans, onObject, null);
		}

		public override object GetOn(Transaction a_trans, object a_OnObject)
		{
			try
			{
				return i_translator.OnStore(a_trans.ObjectContainer(), a_OnObject);
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
			a_bytes.GetStream().Activate(a_bytes.GetTransaction(), toSet, a_bytes.GetInstantiationDepth
				());
			SetOn(a_bytes.GetTransaction(), a_onObject, toSet);
		}

		internal override void Refresh()
		{
		}

		private void SetOn(Transaction trans, object a_onObject, object toSet)
		{
			try
			{
				i_translator.OnActivate(trans.ObjectContainer(), a_onObject, toSet);
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
			return (IIndexable4)i_handler;
		}
	}
}
