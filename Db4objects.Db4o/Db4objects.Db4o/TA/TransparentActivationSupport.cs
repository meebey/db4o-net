/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.TA
{
	public class TransparentActivationSupport : IConfigurationItem
	{
		// TODO: unbindOnClose should be configurable
		public virtual void Prepare(IConfiguration configuration)
		{
		}

		// Nothing to do...
		public virtual void Apply(IInternalObjectContainer container)
		{
			if (ActivationProvider(container) is ITransparentActivationDepthProvider)
			{
				return;
			}
			TransparentActivationDepthProviderImpl provider = new TransparentActivationDepthProviderImpl
				();
			SetActivationDepthProvider(container, provider);
			IEventRegistry registry = EventRegistryFor(container);
			registry.Instantiated += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_30
				(this).OnEvent);
			registry.Created += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_35
				(this).OnEvent);
			registry.Closing += new Db4objects.Db4o.Events.ObjectContainerEventHandler(new _IEventListener4_41
				(this).OnEvent);
			TransparentActivationSupport.TADiagnosticProcessor processor = new TransparentActivationSupport.TADiagnosticProcessor
				(this, container);
			registry.ClassRegistered += new Db4objects.Db4o.Events.ClassEventHandler(new _IEventListener4_52
				(processor).OnEvent);
		}

		private sealed class _IEventListener4_30
		{
			public _IEventListener4_30(TransparentActivationSupport _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				this._enclosing.BindActivatableToActivator((ObjectEventArgs)args);
			}

			private readonly TransparentActivationSupport _enclosing;
		}

		private sealed class _IEventListener4_35
		{
			public _IEventListener4_35(TransparentActivationSupport _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				this._enclosing.BindActivatableToActivator((ObjectEventArgs)args);
			}

			private readonly TransparentActivationSupport _enclosing;
		}

		private sealed class _IEventListener4_41
		{
			public _IEventListener4_41(TransparentActivationSupport _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectContainerEventArgs
				 args)
			{
				IInternalObjectContainer objectContainer = (IInternalObjectContainer)((ObjectContainerEventArgs
					)args).ObjectContainer;
				this._enclosing.UnbindAll(objectContainer);
				if (!this._enclosing.IsEmbeddedClient(objectContainer))
				{
					this._enclosing.SetActivationDepthProvider(objectContainer, null);
				}
			}

			private readonly TransparentActivationSupport _enclosing;
		}

		private sealed class _IEventListener4_52
		{
			public _IEventListener4_52(TransparentActivationSupport.TADiagnosticProcessor processor
				)
			{
				this.processor = processor;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ClassEventArgs args)
			{
				ClassEventArgs cea = (ClassEventArgs)args;
				processor.OnClassRegistered(cea.ClassMetadata());
			}

			private readonly TransparentActivationSupport.TADiagnosticProcessor processor;
		}

		private void SetActivationDepthProvider(IInternalObjectContainer container, IActivationDepthProvider
			 provider)
		{
			container.ConfigImpl().ActivationDepthProvider(provider);
		}

		private IEventRegistry EventRegistryFor(IObjectContainer container)
		{
			return EventRegistryFactory.ForObjectContainer(container);
		}

		private void UnbindAll(IInternalObjectContainer container)
		{
			Db4objects.Db4o.Internal.Transaction transaction = container.Transaction();
			// FIXME should that ever happen?
			if (transaction == null)
			{
				return;
			}
			IReferenceSystem referenceSystem = transaction.ReferenceSystem();
			referenceSystem.TraverseReferences(new _IVisitor4_76(this));
		}

		private sealed class _IVisitor4_76 : IVisitor4
		{
			public _IVisitor4_76(TransparentActivationSupport _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				this._enclosing.Unbind((ObjectReference)obj);
			}

			private readonly TransparentActivationSupport _enclosing;
		}

		private void Unbind(ObjectReference objectReference)
		{
			object obj = objectReference.GetObject();
			if (obj == null || !(obj is IActivatable))
			{
				return;
			}
			Bind(obj, null);
		}

		private void BindActivatableToActivator(ObjectEventArgs oea)
		{
			object obj = oea.Object;
			if (obj is IActivatable)
			{
				Db4objects.Db4o.Internal.Transaction transaction = (Db4objects.Db4o.Internal.Transaction
					)oea.Transaction();
				ObjectReference objectReference = transaction.ReferenceForObject(obj);
				Bind(obj, ActivatorForObject(transaction, objectReference));
			}
		}

		private void Bind(object activatable, IActivator activator)
		{
			((IActivatable)activatable).Bind(activator);
		}

		private IActivator ActivatorForObject(Db4objects.Db4o.Internal.Transaction transaction
			, ObjectReference objectReference)
		{
			if (IsEmbeddedClient(transaction))
			{
				return new TransactionalActivator(transaction, objectReference);
			}
			return objectReference;
		}

		private bool IsEmbeddedClient(Db4objects.Db4o.Internal.Transaction transaction)
		{
			return IsEmbeddedClient(transaction.ObjectContainer());
		}

		internal virtual Db4objects.Db4o.Internal.Transaction Transaction(EventArgs args)
		{
			return (Db4objects.Db4o.Internal.Transaction)((TransactionalEventArgs)args).Transaction
				();
		}

		protected virtual IActivationDepthProvider ActivationProvider(IInternalObjectContainer
			 container)
		{
			return container.ConfigImpl().ActivationDepthProvider();
		}

		private bool IsEmbeddedClient(IObjectContainer objectContainer)
		{
			return objectContainer is EmbeddedClientObjectContainer;
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
				// if(Platform4.isDb4oClass(clazz.getName())) {
				// return;
				// }
				IReflectClass reflectClass = clazz.ClassReflector();
				if (this.ActivatableClass().IsAssignableFrom(reflectClass))
				{
					return;
				}
				if (this.HasNoActivatingFields(reflectClass))
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

			private bool HasNoActivatingFields(IReflectClass clazz)
			{
				IReflectClass curClass = clazz;
				while (curClass != null)
				{
					IReflectField[] fields = curClass.GetDeclaredFields();
					if (!this.HasNoActivatingFields(fields))
					{
						return false;
					}
					curClass = curClass.GetSuperclass();
				}
				return true;
			}

			private bool HasNoActivatingFields(IReflectField[] fields)
			{
				for (int i = 0; i < fields.Length; i++)
				{
					if (this.IsActivating(fields[i]))
					{
						return false;
					}
				}
				return true;
			}

			private bool IsActivating(IReflectField field)
			{
				IReflectClass fieldType = field.GetFieldType();
				return fieldType != null && !fieldType.IsPrimitive();
			}

			private readonly TransparentActivationSupport _enclosing;
		}
	}
}
