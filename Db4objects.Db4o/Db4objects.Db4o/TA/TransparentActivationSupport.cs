/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
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
			IEventRegistry registry = EventRegistryFor(container);
			registry.Instantiated += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_24
				(this).OnEvent);
			registry.Created += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_29
				(this).OnEvent);
			TransparentActivationSupport.TADiagnosticProcessor processor = new TransparentActivationSupport.TADiagnosticProcessor
				(this, container);
			registry.ClassRegistered += new Db4objects.Db4o.Events.ClassEventHandler(new _IEventListener4_36
				(this, processor).OnEvent);
		}

		private sealed class _IEventListener4_24
		{
			public _IEventListener4_24(TransparentActivationSupport _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				this._enclosing.BindActivatableToActivator((ObjectEventArgs)args);
			}

			private readonly TransparentActivationSupport _enclosing;
		}

		private sealed class _IEventListener4_29
		{
			public _IEventListener4_29(TransparentActivationSupport _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				this._enclosing.BindActivatableToActivator((ObjectEventArgs)args);
			}

			private readonly TransparentActivationSupport _enclosing;
		}

		private sealed class _IEventListener4_36
		{
			public _IEventListener4_36(TransparentActivationSupport _enclosing, TransparentActivationSupport.TADiagnosticProcessor
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

		private IEventRegistry EventRegistryFor(IObjectContainer container)
		{
			return EventRegistryFactory.ForObjectContainer(container);
		}

		private void BindActivatableToActivator(ObjectEventArgs oea)
		{
			object obj = oea.Object;
			if (obj is IActivatable)
			{
				Transaction transaction = (Transaction)oea.Transaction();
				ObjectReference objectReference = transaction.ReferenceForObject(obj);
				Bind(obj, ActivatorForObject(transaction, objectReference));
				UnbindActivatableUponClosing(transaction, objectReference);
			}
		}

		private void Bind(object activatable, IActivator activator)
		{
			((IActivatable)activatable).Bind(activator);
		}

		private void UnbindActivatableUponClosing(Transaction transaction, ObjectReference
			 objectReference)
		{
			IEventRegistry eventRegistry = EventRegistryFor(transaction.ObjectContainer());
			eventRegistry.Closing += new Db4objects.Db4o.Events.ObjectContainerEventHandler(new 
				_IEventListener4_64(this, objectReference).OnEvent);
		}

		private sealed class _IEventListener4_64
		{
			public _IEventListener4_64(TransparentActivationSupport _enclosing, ObjectReference
				 objectReference)
			{
				this._enclosing = _enclosing;
				this.objectReference = objectReference;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectContainerEventArgs
				 args)
			{
				object obj = objectReference.GetObject();
				if (obj == null)
				{
					return;
				}
				this._enclosing.Bind(obj, null);
			}

			private readonly TransparentActivationSupport _enclosing;

			private readonly ObjectReference objectReference;
		}

		private IActivator ActivatorForObject(Transaction transaction, ObjectReference objectReference
			)
		{
			if (IsEmbeddedClient(transaction))
			{
				return new TransactionalActivator(transaction, objectReference);
			}
			return objectReference;
		}

		private bool IsEmbeddedClient(Transaction transaction)
		{
			return transaction.ObjectContainer() is EmbeddedClientObjectContainer;
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
