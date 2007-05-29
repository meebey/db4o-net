/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class ActivationExceptionBubblesUpTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ActivationExceptionBubblesUpTestCase().RunAll();
		}

		public sealed class ItemTranslator : IObjectTranslator
		{
			public void OnActivate(IObjectContainer container, object applicationObject, object
				 storedObject)
			{
				throw new ItemException();
			}

			public object OnStore(IObjectContainer container, object applicationObject)
			{
				return applicationObject;
			}

			public Type StoredClass()
			{
				return typeof(Item);
			}
		}

		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(Item)).Translate(new ActivationExceptionBubblesUpTestCase.ItemTranslator
				());
		}

		protected override void Store()
		{
			Store(new Item());
		}

		public virtual void Test()
		{
			Assert.Expect(typeof(ReflectException), typeof(ItemException), new _ICodeBlock_49
				(this));
		}

		private sealed class _ICodeBlock_49 : ICodeBlock
		{
			public _ICodeBlock_49(ActivationExceptionBubblesUpTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				IQuery q = this._enclosing.Db().Query();
				q.Constrain(typeof(Item));
				q.Constrain(new _IEvaluation_53(this));
				q.Execute().Next();
			}

			private sealed class _IEvaluation_53 : IEvaluation
			{
				public _IEvaluation_53(_ICodeBlock_49 _enclosing)
				{
					this._enclosing = _enclosing;
				}

				public void Evaluate(ICandidate candidate)
				{
					candidate.Include(true);
				}

				private readonly _ICodeBlock_49 _enclosing;
			}

			private readonly ActivationExceptionBubblesUpTestCase _enclosing;
		}
	}
}
