/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.IO;
using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.CS;
using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Config.Encoding;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Config;
using Db4objects.Db4o.Tests.Common.Api;

namespace Db4objects.Db4o.Tests.Common.Api
{
	public class BaseAndLocalConfigurationTestSuite : FixtureBasedTestSuite
	{
		public class BaseConfigurationProviderTestUnit : ITestCase
		{
			public virtual void Test()
			{
				IBaseConfigurationProvider config = ((IBaseConfigurationProvider)Subject());
				Config4Impl legacy = LegacyFrom(config);
				IBaseConfiguration @base = config.Base;
				@base.ActivationDepth = 42;
				Assert.AreEqual(42, legacy.ActivationDepth());
				Assert.AreEqual(42, @base.ActivationDepth);
				// TODO: assert
				@base.Add(new _IConfigurationItem_33());
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
				DiagnosticCollector collector = new DiagnosticCollector();
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
				// TODO: test reflectWith()
				// TODO: test refreshClasses()
				// TODO: this probably won't sharpen :/
				TextWriter outStream = Sharpen.Runtime.Out;
				@base.OutStream = outStream;
				Assert.AreEqual(outStream, legacy.OutStream());
				IStringEncoding stringEncoding = new _IStringEncoding_98();
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

			private sealed class _IConfigurationItem_33 : IConfigurationItem
			{
				public _IConfigurationItem_33()
				{
				}

				public void Apply(IInternalObjectContainer container)
				{
				}

				public void Prepare(IConfiguration configuration)
				{
				}
			}

			private sealed class _IStringEncoding_98 : IStringEncoding
			{
				public _IStringEncoding_98()
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
				return new _DiagnosticBase_128();
			}

			private sealed class _DiagnosticBase_128 : DiagnosticBase
			{
				public _DiagnosticBase_128()
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
		}

		public class LocalConfigurationProviderTestUnit : ITestCase
		{
			/// <exception cref="Exception"></exception>
			public virtual void Test()
			{
				if (Subject() is IClientConfiguration)
				{
					return;
				}
				ILocalConfigurationProvider config = ((ILocalConfigurationProvider)Subject());
				ILocalConfiguration local = config.Local;
				Config4Impl legacy = LegacyFrom(config);
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
				local.ReadOnly = true;
				Assert.IsTrue(legacy.IsReadOnly());
			}
		}

		public override IFixtureProvider[] FixtureProviders()
		{
			return new IFixtureProvider[] { Subjects(new object[] { Db4oEmbedded.NewConfiguration
				(), Db4oClientServer.NewClientConfiguration(), Db4oClientServer.NewServerConfiguration
				() }) };
		}

		private IFixtureProvider Subjects(object[] subjects)
		{
			return new SubjectFixtureProvider(subjects);
		}

		public override Type[] TestUnits()
		{
			return new Type[] { typeof(BaseAndLocalConfigurationTestSuite.BaseConfigurationProviderTestUnit
				), typeof(BaseAndLocalConfigurationTestSuite.LocalConfigurationProviderTestUnit)
				 };
		}

		private static Config4Impl LegacyFrom(object config)
		{
			return ((ILegacyConfigurationProvider)config).Legacy();
		}

		public static object Subject()
		{
			return (object)SubjectFixtureProvider.Value();
		}
	}
}
