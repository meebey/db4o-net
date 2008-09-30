/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.IO;
using Db4oUnit;
using Db4oUnit.Mocking;
using Db4objects.Db4o;
using Db4objects.Db4o.CS;
using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Config.Encoding;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Config;
using Db4objects.Db4o.Tests.Common.Api;

namespace Db4objects.Db4o.Tests.Common.Api
{
	public class Db4oClientServerTestCase : TestWithTempFile
	{
		private sealed class DiagnosticCollector : IDiagnosticListener
		{
			internal ArrayList _diagnostics = new ArrayList();

			public void OnDiagnostic(IDiagnostic d)
			{
				_diagnostics.Add(d);
			}

			public void Verify(object[] expected)
			{
				ArrayAssert.AreEqual(expected, _diagnostics.ToArray());
			}
		}

		private sealed class ClientServerFactoryStub : MethodCallRecorder, IClientServerFactory
		{
			/// <exception cref="Db4oIOException"></exception>
			/// <exception cref="OldFormatException"></exception>
			/// <exception cref="InvalidPasswordException"></exception>
			public IObjectContainer OpenClient(IConfiguration config, string hostName, int port
				, string user, string password, INativeSocketFactory socketFactory)
			{
				this.Record(new MethodCall("openClient", new object[] { config, hostName, port, user
					, password, socketFactory }));
				return null;
			}

			/// <exception cref="Db4oIOException"></exception>
			/// <exception cref="IncompatibleFileFormatException"></exception>
			/// <exception cref="OldFormatException"></exception>
			/// <exception cref="DatabaseFileLockedException"></exception>
			/// <exception cref="DatabaseReadOnlyException"></exception>
			public IObjectServer OpenServer(IConfiguration config, string databaseFileName, int
				 port, INativeSocketFactory socketFactory)
			{
				this.Record(new MethodCall("openServer", new object[] { config, databaseFileName, 
					port, socketFactory }));
				return null;
			}

			internal ClientServerFactoryStub(Db4oClientServerTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly Db4oClientServerTestCase _enclosing;
		}

		public virtual void TestClientServerApi()
		{
			IServerConfiguration config = Db4oClientServer.NewServerConfiguration();
			IObjectServer server = Db4oClientServer.OpenServer(config, _tempFile, unchecked((
				int)(0xdb40)));
			try
			{
				server.GrantAccess("user", "password");
				IClientConfiguration clientConfig = Db4oClientServer.NewClientConfiguration();
				IObjectContainer client1 = Db4oClientServer.OpenClient(clientConfig, "localhost", 
					unchecked((int)(0xdb40)), "user", "password");
				try
				{
				}
				finally
				{
					Assert.IsTrue(client1.Close());
				}
			}
			finally
			{
				Assert.IsTrue(server.Close());
			}
		}

		public virtual void TestOpenServer()
		{
			Db4oClientServerTestCase.ClientServerFactoryStub factoryStub = new Db4oClientServerTestCase.ClientServerFactoryStub
				(this);
			IServerConfiguration config = Db4oClientServer.NewServerConfiguration();
			config.Networking.Factory = factoryStub;
			Assert.IsNull(Db4oClientServer.OpenServer(config, _tempFile, unchecked((int)(0xdb40
				))));
			factoryStub.Verify(new MethodCall[] { new MethodCall("openServer", new object[] { 
				MethodCall.IgnoredArgument, _tempFile, unchecked((int)(0xdb40)), MethodCall.IgnoredArgument
				 }) });
		}

		public virtual void TestOpenClient()
		{
			Db4oClientServerTestCase.ClientServerFactoryStub factoryStub = new Db4oClientServerTestCase.ClientServerFactoryStub
				(this);
			IClientConfiguration config = Db4oClientServer.NewClientConfiguration();
			config.Networking.Factory = factoryStub;
			Assert.IsNull(Db4oClientServer.OpenClient(config, "foo", 42, "u", "p"));
			factoryStub.Verify(new MethodCall[] { new MethodCall("openClient", new object[] { 
				MethodCall.IgnoredArgument, "foo", 42, "u", "p", MethodCall.IgnoredArgument }) }
				);
		}

		public virtual void TestConfigurationHierarchy()
		{
			Assert.IsInstanceOf(typeof(INetworkingConfigurationProvider), Db4oClientServer.NewClientConfiguration
				());
			Assert.IsInstanceOf(typeof(INetworkingConfigurationProvider), Db4oClientServer.NewServerConfiguration
				());
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestServerConfigurationLocal()
		{
			IServerConfiguration config = Db4oClientServer.NewServerConfiguration();
			Config4Impl legacy = LegacyFrom(config);
			ILocalConfiguration local = config.Local;
			TypeAlias alias = new TypeAlias("foo", "bar");
			local.AddAlias(alias);
			Assert.AreEqual("bar", legacy.ResolveAliasStoredName("foo"));
			Assert.AreEqual("foo", legacy.ResolveAliasRuntimeName("bar"));
			local.RemoveAlias(alias);
			Assert.AreEqual("foo", legacy.ResolveAliasStoredName("foo"));
			local.BlockSize = 42;
			Assert.AreEqual(42, legacy.BlockSize());
			local.DatabaseGrowthSize = 42;
			Assert.AreEqual(42, legacy.DatabaseGrowthSize());
			local.DisableCommitRecovery();
			Assert.IsTrue(legacy.CommitRecoveryDisabled());
			local.Freespace.DiscardSmallerThan(8);
			Assert.AreEqual(8, legacy.DiscardFreeSpace());
			local.GenerateUUIDs = ConfigScope.Globally;
			Assert.AreEqual(ConfigScope.Globally, legacy.GenerateUUIDs());
			local.GenerateVersionNumbers = ConfigScope.Globally;
			Assert.AreEqual(ConfigScope.Globally, legacy.GenerateVersionNumbers());
			MemoryIoAdapter ioAdapter = new MemoryIoAdapter();
			local.Io = ioAdapter;
			Assert.AreEqual(ioAdapter, legacy.Io());
			Assert.AreEqual(ioAdapter, local.Io);
			local.LockDatabaseFile = true;
			Assert.IsTrue(legacy.LockFile());
			local.ReserveStorageSpace = 1024;
			Assert.AreEqual(1024, legacy.ReservedStorageSpace());
			local.BlobPath = Path.GetTempPath();
			Assert.AreEqual(Path.GetTempPath(), legacy.BlobPath());
		}

		public virtual void TestServerConfigurationBase()
		{
			IServerConfiguration config = Db4oClientServer.NewServerConfiguration();
			Config4Impl legacy = LegacyFrom(config);
			IBaseConfiguration @base = config.Base;
			@base.ActivationDepth = 42;
			Assert.AreEqual(42, legacy.ActivationDepth());
			Assert.AreEqual(42, @base.ActivationDepth);
			// TODO: assert
			@base.Add(new _IConfigurationItem_164());
			@base.AllowVersionUpdates = false;
			Assert.IsFalse(legacy.AllowVersionUpdates());
			@base.AutomaticShutDown = false;
			Assert.IsFalse(legacy.AutomaticShutDown());
			@base.BTreeNodeSize = 42;
			Assert.AreEqual(42, legacy.BTreeNodeSize());
			@base.Callbacks = false;
			Assert.IsFalse(legacy.Callbacks());
			@base.CallConstructors = false;
			Assert.IsTrue(legacy.CallConstructors().DefiniteNo());
			@base.DetectSchemaChanges = false;
			Assert.IsFalse(legacy.DetectSchemaChanges());
			Db4oClientServerTestCase.DiagnosticCollector collector = new Db4oClientServerTestCase.DiagnosticCollector
				();
			@base.Diagnostic.AddListener(collector);
			IDiagnostic diagnostic = DummyDiagnostic();
			legacy.DiagnosticProcessor().OnDiagnostic(diagnostic);
			collector.Verify(new object[] { diagnostic });
			@base.ExceptionsOnNotStorable = true;
			Assert.IsTrue(legacy.ExceptionsOnNotStorable());
			@base.InternStrings = true;
			Assert.IsTrue(legacy.InternStrings());
			// TODO: assert
			@base.MarkTransient("Foo");
			@base.MessageLevel = 3;
			Assert.AreEqual(3, legacy.MessageLevel());
			IObjectClass objectClass = @base.ObjectClass(typeof(ICollection));
			objectClass.CascadeOnDelete(true);
			Assert.IsTrue(((Config4Class)legacy.ObjectClass(typeof(ICollection))).CascadeOnDelete
				().DefiniteYes());
			Assert.IsTrue(((Config4Class)@base.ObjectClass(typeof(ICollection))).CascadeOnDelete
				().DefiniteYes());
			@base.OptimizeNativeQueries = false;
			Assert.IsFalse(legacy.OptimizeNativeQueries());
			Assert.IsFalse(@base.OptimizeNativeQueries);
			@base.Queries.EvaluationMode(QueryEvaluationMode.Lazy);
			Assert.AreEqual(QueryEvaluationMode.Lazy, legacy.QueryEvaluationMode());
			@base.ReadOnly = true;
			Assert.IsTrue(legacy.IsReadOnly());
			// TODO: test reflectWith()
			// TODO: test refreshClasses()
			// TODO: this probably won't sharpen :/
			TextWriter outStream = Sharpen.Runtime.Out;
			@base.OutStream(outStream);
			Assert.AreEqual(outStream, legacy.OutStream());
			IStringEncoding stringEncoding = new _IStringEncoding_232();
			@base.StringEncoding = stringEncoding;
			Assert.AreEqual(stringEncoding, legacy.StringEncoding());
			@base.TestConstructors = false;
			Assert.IsFalse(legacy.TestConstructors());
			@base.TestConstructors = true;
			Assert.IsTrue(legacy.TestConstructors());
			@base.UpdateDepth = 1024;
			Assert.AreEqual(1024, legacy.UpdateDepth());
			@base.WeakReferences = false;
			Assert.IsFalse(legacy.WeakReferences());
			@base.WeakReferenceCollectionInterval = 1024;
			Assert.AreEqual(1024, legacy.WeakReferenceCollectionInterval());
		}

		private sealed class _IConfigurationItem_164 : IConfigurationItem
		{
			public _IConfigurationItem_164()
			{
			}

			public void Apply(IInternalObjectContainer container)
			{
			}

			public void Prepare(IConfiguration configuration)
			{
			}
		}

		private sealed class _IStringEncoding_232 : IStringEncoding
		{
			public _IStringEncoding_232()
			{
			}

			public string Decode(byte[] bytes, int start, int length)
			{
				return null;
			}

			public byte[] Encode(string str)
			{
				return null;
			}
		}

		// TODO: test registerTypeHandler()
		private DiagnosticBase DummyDiagnostic()
		{
			return new _DiagnosticBase_262();
		}

		private sealed class _DiagnosticBase_262 : DiagnosticBase
		{
			public _DiagnosticBase_262()
			{
			}

			public override string Problem()
			{
				return null;
			}

			public override object Reason()
			{
				return null;
			}

			public override string Solution()
			{
				return null;
			}
		}

		private Config4Impl LegacyFrom(IServerConfiguration config)
		{
			return ((NetworkingConfigurationImpl)config.Networking).Config();
		}
	}
}
