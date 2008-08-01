/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal
{
	internal sealed class TranslatedAspect : Db4objects.Db4o.Internal.FieldMetadata
	{
		private readonly IObjectTranslator _translator;

		internal TranslatedAspect(ClassMetadata containingClass, IObjectTranslator translator
			) : base(containingClass, translator)
		{
			_translator = translator;
			ObjectContainerBase stream = containingClass.Container();
			Configure(stream.Reflector().ForClass(TranslatorStoredClass(translator)), false);
		}

		public override bool CanUseNullBitmap()
		{
			return false;
		}

		public override void Deactivate(Transaction trans, object onObject, IActivationDepth
			 depth)
		{
			if (depth.RequiresActivation())
			{
				CascadeActivation(trans, onObject, depth);
			}
			SetOn(trans, onObject, null);
		}

		public override object GetOn(Transaction a_trans, object a_OnObject)
		{
			try
			{
				return _translator.OnStore(a_trans.ObjectContainer(), a_OnObject);
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

		public override void Instantiate(UnmarshallingContext context)
		{
			object obj = Read(context);
			// Activation of members is necessary on purpose here.
			// Classes like Hashtable need fully activated members
			// to be able to calculate hashCode()
			context.Container().Activate(context.Transaction(), obj, context.ActivationDepth(
				));
			SetOn(context.Transaction(), context.PersistentObject(), obj);
		}

		internal override void Refresh()
		{
		}

		// do nothing
		private void SetOn(Transaction trans, object a_onObject, object toSet)
		{
			try
			{
				_translator.OnActivate(trans.ObjectContainer(), a_onObject, toSet);
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
			return (IIndexable4)_handler;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			TranslatedAspect other = (TranslatedAspect)obj;
			return _translator.Equals(other._translator);
		}

		public override int GetHashCode()
		{
			return _translator.GetHashCode();
		}

		public override Db4objects.Db4o.Internal.Marshall.AspectType AspectType()
		{
			return Db4objects.Db4o.Internal.Marshall.AspectType.Translator;
		}
	}
}
