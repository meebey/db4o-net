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
			container.ConfigImpl().ActivationDepthProvider(new TransparentActivationDepthProvider
				());
			IEventRegistry registry = EventRegistryFor(container);
			registry.Instantiated += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_25
				(this).OnEvent);
			registry.Created += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_30
				(this).OnEvent);
			registry.Closing += new Db4objects.Db4o.Events.ObjectContainerEventHandler(new _IEventListener4_36
				(this).OnEvent);
			TransparentActivationSupport.TADiagnosticProcessor processor = new TransparentActivationSupport.TADiagnosticProcessor
				(this, container);
			registry.ClassRegistered += new Db4objects.Db4o.Events.ClassEventHandler(new _IEventListener4_43
				(this, processor).OnEvent);
		}

		private sealed class _IEventListener4_25
		{
			public _IEventListener4_25(TransparentActivationSupport _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				this._enclosing.BindActivatableToActivator((ObjectEventArgs)args);
			}

			private readonly TransparentActivationSupport _enclosing;
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

		private sealed class _IEventListener4_36
		{
			public _IEventListener4_36(TransparentActivationSupport _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectContainerEventArgs
				 args)
			{
				this._enclosing.UnbindAll((IInternalObjectContainer)((ObjectContainerEventArgs)args
					).ObjectContainer);
			}

			private readonly TransparentActivationSupport _enclosing;
		}

		private sealed class _IEventListener4_43
		{
			public _IEventListener4_43(TransparentActivationSupport _enclosing, TransparentActivationSupport.TADiagnosticProcessor
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

		private void UnbindAll(IInternalObjectContainer container)
		{
			Transaction transaction = container.Transaction();
			// FIXME should that ever happen?
			if (transaction == null)
			{
				return;
			}
			IReferenceSystem referenceSystem = transaction.ReferenceSystem();
			referenceSystem.TraverseReferences(new _IVisitor4_62(this));
		}

		private sealed class _IVisitor4_62 : IVisitor4
		{
			public _IVisitor4_62(TransparentActivationSupport _enclosing)
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
				Transaction transaction = (Transaction)oea.Transaction();
				ObjectReference objectReference = transaction.ReferenceForObject(obj);
				Bind(obj, ActivatorForObject(transaction, objectReference));
			}
		}

		private void Bind(object activatable, IActivator activator)
		{
			((IActivatable)activatable).Bind(activator);
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
				// if(Platform4.isDb4oClass(clazz.getName())) {
				// return;
				// }
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
