/* Copyright (C) 2008   Versant Inc.   http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Sharpen.Lang;
#if !SILVERLIGHT
using Db4objects.Db4o.CS.Internal;
using Db4objects.Db4o.CS.Internal.Messages;
using ClassInfo=Db4objects.Db4o.Reflect.Self.ClassInfo;
using FieldInfo=Db4objects.Db4o.Reflect.Self.FieldInfo;

#endif

namespace Db4objects.Db4o.Config
{
	public class JavaSupport : IConfigurationItem
	{
		public void Prepare(IConfiguration config)
		{
			config.AddAlias(new WildcardAlias("com.db4o.ext.*", "Db4objects.Db4o.Ext.*, Db4objects.Db4o"));
			config.AddAlias(new TypeAlias("com.db4o.foundation.ChainedRuntimeException", FullyQualifiedName(typeof(Exception))));

			config.AddAlias(new TypeAlias("com.db4o.StaticField", FullyQualifiedName(typeof(StaticField))));
			config.AddAlias(new TypeAlias("com.db4o.StaticClass", FullyQualifiedName(typeof(StaticClass))));

			config.AddAlias(new TypeAlias("com.db4o.query.Evaluation", FullyQualifiedName(typeof(IEvaluation))));
			config.AddAlias(new TypeAlias("com.db4o.query.Candidate", FullyQualifiedName(typeof(ICandidate))));

			config.AddAlias(new WildcardAlias("com.db4o.internal.query.processor.*", "Db4objects.Db4o.Internal.Query.Processor.*, Db4objects.Db4o"));
			//config.AddAlias(new WildcardAlias("com.db4o.query.*", "Db4objects.Db4o.Query.*, Db4objects.Db4o"));

			config.AddAlias(new TypeAlias("com.db4o.foundation.Collection4", FullyQualifiedName(typeof(Collection4))));
			config.AddAlias(new TypeAlias("com.db4o.foundation.List4", FullyQualifiedName(typeof(List4))));
			config.AddAlias(new TypeAlias("com.db4o.User", FullyQualifiedName(typeof(User))));

#if !SILVERLIGHT
			config.AddAlias(new TypeAlias("com.db4o.cs.internal.ClassInfo", FullyQualifiedName(typeof(ClassInfo))));
			config.AddAlias(new TypeAlias("com.db4o.cs.internal.FieldInfo", FullyQualifiedName(typeof(FieldInfo))));

			config.AddAlias(new TypeAlias("com.db4o.cs.internal.messages.MUserMessage$UserMessagePayload", FullyQualifiedName(typeof(MUserMessage.UserMessagePayload))));
			config.AddAlias(new WildcardAlias("com.db4o.cs.internal.messages.*", "Db4objects.Db4o.Internal.CS.Messages.*, Db4objects.Db4o.CS"));
#endif

			config.AddAlias(new TypeAlias("java.lang.Throwable", FullyQualifiedName(typeof(Exception))));
			config.AddAlias(new TypeAlias("java.lang.RuntimeException", FullyQualifiedName(typeof(Exception))));
			config.AddAlias(new TypeAlias("java.lang.Exception", FullyQualifiedName(typeof(Exception))));
			

			config.ObjectClass("java.lang.Class").Translate(new TType());
		}

		private static string FullyQualifiedName(Type type)
		{
			return ReflectPlatform.FullyQualifiedName(type);
		}

		public void Apply(IInternalObjectContainer container)
		{
		}
	}
}
