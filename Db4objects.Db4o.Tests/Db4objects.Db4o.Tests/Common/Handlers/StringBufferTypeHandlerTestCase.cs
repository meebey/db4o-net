/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Text;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Handlers;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class StringBufferTypeHandlerTestCase : AbstractDb4oTestCase, IOptOutDefragSolo
	{
		private sealed class ClassPredicate : ITypeHandlerPredicate
		{
			private readonly Type _klass;

			public ClassPredicate(StringBufferTypeHandlerTestCase _enclosing, Type klass)
			{
				this._enclosing = _enclosing;
				//TODO: quick for failure in "defrag-solo" mode
				this._klass = klass;
			}

			public bool Match(IReflectClass classReflector, int version)
			{
				IReflectClass reflectClass = classReflector.Reflector().ForClass(this._klass);
				return classReflector == reflectClass;
			}

			private readonly StringBufferTypeHandlerTestCase _enclosing;
		}

		internal sealed class StringBufferTypeHandler : ITypeHandler4, ISecondClassTypeHandler
			, IVariableLengthTypeHandler
		{
			public void Defragment(IDefragmentContext context)
			{
				throw new NotImplementedException();
			}

			/// <exception cref="Db4oIOException"></exception>
			public void Delete(IDeleteContext context)
			{
				throw new NotImplementedException();
			}

			public object Read(IReadContext context)
			{
				object read = StringHandler(context).Read(context);
				if (null == read)
				{
					return null;
				}
				return new StringBuilder((string)read);
			}

			public void Write(IWriteContext context, object obj)
			{
				StringHandler(context).Write(context, obj.ToString());
			}

			private ITypeHandler4 StringHandler(IContext context)
			{
				return Handlers(context)._stringHandler;
			}

			private HandlerRegistry Handlers(IContext context)
			{
				return ((IInternalObjectContainer)context.ObjectContainer()).Handlers();
			}

			public IPreparedComparison PrepareComparison(object obj)
			{
				throw new NotImplementedException();
			}
		}

		public class Item
		{
			public StringBuilder buffer;

			public Item()
			{
			}

			public Item(StringBuilder contents)
			{
				buffer = contents;
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.ExceptionsOnNotStorable(true);
			config.RegisterTypeHandler(new StringBufferTypeHandlerTestCase.ClassPredicate(this
				, typeof(StringBuilder)), new StringBufferTypeHandlerTestCase.StringBufferTypeHandler
				());
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new StringBufferTypeHandlerTestCase.Item(new StringBuilder("42")));
		}

		public virtual void _testRetrieve()
		{
			StringBufferTypeHandlerTestCase.Item item = RetrieveItem();
			Assert.AreEqual("42", item.buffer.ToString());
		}

		public virtual void _testTopLevelStore()
		{
			Assert.Expect(typeof(ObjectNotStorableException), new _ICodeBlock_98(this));
		}

		private sealed class _ICodeBlock_98 : ICodeBlock
		{
			public _ICodeBlock_98(StringBufferTypeHandlerTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Store(new StringBuilder("a"));
			}

			private readonly StringBufferTypeHandlerTestCase _enclosing;
		}

		private StringBufferTypeHandlerTestCase.Item RetrieveItem()
		{
			return (StringBufferTypeHandlerTestCase.Item)RetrieveOnlyInstance(typeof(StringBufferTypeHandlerTestCase.Item
				));
		}
	}
}
