/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Util;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.TA.Tests;

namespace Db4objects.Db4o.TA.Tests
{
	public class TransparentActivationDiagnosticsTestCase : AbstractDb4oTestCase
	{
		public class SomeTAAwareData
		{
			public int _id;

			public SomeTAAwareData(int id)
			{
				_id = id;
			}
		}

		public class SomeOtherTAAwareData : IActivatable
		{
			public TransparentActivationDiagnosticsTestCase.SomeTAAwareData _data;

			public virtual void Bind(IActivator activator)
			{
			}

			public virtual void Activate()
			{
			}

			public SomeOtherTAAwareData(TransparentActivationDiagnosticsTestCase.SomeTAAwareData
				 data)
			{
				_data = data;
			}
		}

		public class NotTAAwareData
		{
			public TransparentActivationDiagnosticsTestCase.SomeTAAwareData _data;

			public NotTAAwareData(TransparentActivationDiagnosticsTestCase.SomeTAAwareData data
				)
			{
				_data = data;
			}
		}

		private class DiagnosticsRegistered
		{
			public int _registeredCount = 0;
		}

		private readonly TransparentActivationDiagnosticsTestCase.DiagnosticsRegistered _registered
			 = new TransparentActivationDiagnosticsTestCase.DiagnosticsRegistered();

		private readonly IDiagnosticListener _checker;

		public TransparentActivationDiagnosticsTestCase()
		{
			_checker = new _IDiagnosticListener_57(this);
		}

		private sealed class _IDiagnosticListener_57 : IDiagnosticListener
		{
			public _IDiagnosticListener_57(TransparentActivationDiagnosticsTestCase _enclosing
				)
			{
				this._enclosing = _enclosing;
			}

			public void OnDiagnostic(IDiagnostic diagnostic)
			{
				if (!(diagnostic is NotTransparentActivationEnabled))
				{
					return;
				}
				NotTransparentActivationEnabled taDiagnostic = (NotTransparentActivationEnabled)diagnostic;
				Assert.AreEqual(CrossPlatformServices.FullyQualifiedName(typeof(TransparentActivationDiagnosticsTestCase.NotTAAwareData)
					), ((ClassMetadata)taDiagnostic.Reason()).GetName());
				this._enclosing._registered._registeredCount++;
			}

			private readonly TransparentActivationDiagnosticsTestCase _enclosing;
		}

		protected override void Configure(IConfiguration config)
		{
			config.Add(new TransparentActivationSupport());
			config.Diagnostic().AddListener(_checker);
		}

		/// <exception cref="Exception"></exception>
		protected override void Db4oTearDownBeforeClean()
		{
			Db().Ext().Configure().Diagnostic().RemoveAllListeners();
			base.Db4oTearDownBeforeClean();
		}

		public virtual void TestTADiagnostics()
		{
			Store(new TransparentActivationDiagnosticsTestCase.SomeTAAwareData(1));
			Assert.AreEqual(0, _registered._registeredCount);
			Store(new TransparentActivationDiagnosticsTestCase.SomeOtherTAAwareData(new TransparentActivationDiagnosticsTestCase.SomeTAAwareData
				(2)));
			Assert.AreEqual(0, _registered._registeredCount);
			Store(new TransparentActivationDiagnosticsTestCase.NotTAAwareData(new TransparentActivationDiagnosticsTestCase.SomeTAAwareData
				(3)));
			Assert.AreEqual(1, _registered._registeredCount);
		}

		public static void Main(string[] args)
		{
			new TransparentActivationDiagnosticsTestCase().RunSolo();
		}
	}
}
