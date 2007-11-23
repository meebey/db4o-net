/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.TA
{
	public class TransparentActivationSupport : IConfigurationItem
	{
		public virtual void Prepare(IConfiguration configuration)
		{
		}

		public virtual void Apply(IInternalObjectContainer container)
		{
			container.ConfigImpl().ActivationDepthProvider(new TransparentActivationDepthProvider
				());
			IEventRegistry registry = EventRegistryFactory.ForObjectContainer(container);
			registry.Instantiated += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_23
				(this).OnEvent);
			TransparentActivationSupport.TADiagnosticProcessor processor = new TransparentActivationSupport.TADiagnosticProcessor
				(this, container);
			registry.ClassRegistered += new Db4objects.Db4o.Events.ClassEventHandler(new _IEventListener4_50
				(this, processor).OnEvent);
		}

		private sealed class _IEventListener4_23
		{
			public _IEventListener4_23(TransparentActivationSupport _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				this.BindActivatableToActivator((ObjectEventArgs)args);
			}

			private void BindActivatableToActivator(ObjectEventArgs oea)
			{
				object obj = oea.Object;
				if (obj is IActivatable)
				{
					Transaction transaction = (Transaction)oea.Transaction();
					((IActivatable)obj).Bind(this.ActivatorForObject(transaction, obj));
				}
			}

			private IActivator ActivatorForObject(Transaction transaction, object obj)
			{
				ObjectReference objectReference = transaction.ReferenceForObject(obj);
				if (this.IsEmbeddedClient(transaction))
				{
					return new TransactionalActivator(transaction, objectReference);
				}
				return objectReference;
			}

			private bool IsEmbeddedClient(Transaction transaction)
			{
				return transaction.ObjectContainer() is EmbeddedClientObjectContainer;
			}

			private readonly TransparentActivationSupport _enclosing;
		}

		private sealed class _IEventListener4_50
		{
			public _IEventListener4_50(TransparentActivationSupport _enclosing, TransparentActivationSupport.TADiagnosticProcessor
				 processor)
			{
				this._enclosing = _enclosing;
				this.processor = processor;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ClassEventArgs args)
			{
				ClassEventArgs cea = (ClassEventArgs)args;
				processor.OnClassRegistered(cea.ClassMetadata());
			}

			private readonly TransparentActivationSupport _enclosing;

			private readonly TransparentActivationSupport.TADiagnosticProcessor processor;
		}

		private sealed class TADiagnosticProcessor
		{
			private readonly IInternalObjectContainer _container;

			public TADiagnosticProcessor(TransparentActivationSupport _enclosing, IInternalObjectContainer
				 container)
			{
				this._enclosing = _enclosing;
				this._container = container;
			}

			public void OnClassRegistered(ClassMetadata clazz)
			{
				IReflectClass reflectClass = clazz.ClassReflector();
				if (this.ActivatableClass().IsAssignableFrom(reflectClass))
				{
					return;
				}
				if (this.HasOnlyPrimitiveFields(reflectClass))
				{
					return;
				}
				NotTransparentActivationEnabled diagnostic = new NotTransparentActivationEnabled(
					clazz);
				DiagnosticProcessor processor = this._container.Handlers()._diagnosticProcessor;
				processor.OnDiagnostic(diagnostic);
			}

			private IReflectClass ActivatableClass()
			{
				return this._container.Reflector().ForClass(typeof(IActivatable));
			}

			private bool HasOnlyPrimitiveFields(IReflectClass clazz)
			{
				IReflectClass curClass = clazz;
				while (curClass != null)
				{
					IReflectField[] fields = curClass.GetDeclaredFields();
					if (!this.HasOnlyPrimitiveFields(fields))
					{
						return false;
					}
					curClass = curClass.GetSuperclass();
				}
				return true;
			}

			private bool HasOnlyPrimitiveFields(IReflectField[] fields)
			{
				for (int i = 0; i < fields.Length; i++)
				{
					if (!this.IsPrimitive(fields[i]))
					{
						return false;
					}
				}
				return true;
			}

			private bool IsPrimitive(IReflectField field)
			{
				return field.GetFieldType().IsPrimitive();
			}

			private readonly TransparentActivationSupport _enclosing;
		}
	}
}
