/* Copyright (C) 2008   db4objects Inc.   http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.Lang;

namespace Db4objects.Db4o.native.Db4objects.Db4o.Config
{
	public class JavaSupport : IConfigurationItem
	{
		public void Prepare(IConfiguration config)
		{
			config.AddAlias(new TypeAlias("com.db4o.ext.Db4oDatabase", FullTypeNameFor(typeof(Db4oDatabase))));

			config.AddAlias(new TypeAlias("com.db4o.StaticField", FullTypeNameFor(typeof(StaticField))));
			config.AddAlias(new TypeAlias("com.db4o.StaticClass", FullTypeNameFor(typeof(StaticClass))));
			//config.AddAlias(new TypeAlias("com.db4o.query.Evaluation", FullTypeNameFor(typeof(IEvaluation))));
			//config.AddAlias(new TypeAlias("com.db4o.query.Candidate", FullTypeNameFor(typeof(ICandidate))));

			config.AddAlias(new WildcardAlias("com.db4o.internal.query.processor.*", "Db4objects.Db4o.Internal.Query.Processor.*, Db4objects.Db4o"));

			config.AddAlias(new TypeAlias("com.db4o.foundation.Collection4", FullTypeNameFor(typeof(Collection4))));
			config.AddAlias(new TypeAlias("com.db4o.foundation.List4", FullTypeNameFor(typeof(List4))));
			config.AddAlias(new TypeAlias("com.db4o.User", FullTypeNameFor(typeof(User))));

			config.AddAlias(new TypeAlias("com.db4o.internal.cs.ClassInfo", FullTypeNameFor(typeof(ClassInfo))));
			config.AddAlias(new TypeAlias("com.db4o.internal.cs.FieldInfo", FullTypeNameFor(typeof(FieldInfo))));

			config.AddAlias(new TypeAlias("com.db4o.internal.cs.messages.MUserMessage$UserMessagePayload", FullTypeNameFor(typeof(MUserMessage.UserMessagePayload))));
		}

		private static string FullTypeNameFor(Type type)
		{
			return TypeReference.FromType(type).GetUnversionedName();
		}

		public void Apply(IInternalObjectContainer container)
		{
		}
	}
}
