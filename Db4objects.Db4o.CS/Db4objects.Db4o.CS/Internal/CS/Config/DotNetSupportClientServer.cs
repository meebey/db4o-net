/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Internal.CS.Config
{
	/// <exclude></exclude>
	public class DotNetSupportClientServer : IConfigurationItem
	{
		public virtual void Apply(IInternalObjectContainer container)
		{
		}

		// do nothing.
		public virtual void Prepare(IConfiguration config)
		{
			config.AddAlias(new TypeAlias("System.Exception, mscorlib", typeof(Exception).FullName
				));
			//		config.addAlias(new TypeAlias("java.lang.Throwable", FullTypeNameFor(typeof(Exception))));
			//		config.addAlias(new TypeAlias("java.lang.RuntimeException", FullTypeNameFor(typeof(Exception))));
			//		config.addAlias(new TypeAlias("java.lang.Exception", FullTypeNameFor(typeof(Exception))));
			config.AddAlias(new TypeAlias("Db4objects.Db4o.Query.IEvaluation, Db4objects.Db4o"
				, typeof(IEvaluation).FullName));
			config.AddAlias(new TypeAlias("Db4objects.Db4o.Query.ICandidate, Db4objects.Db4o"
				, typeof(ICandidate).FullName));
			config.AddAlias(new WildcardAlias("Db4objects.Db4o.Internal.Query.Processor.*, Db4objects.Db4o"
				, "com.db4o.internal.query.processor.*"));
			config.AddAlias(new TypeAlias("Db4objects.Db4o.Foundation.Collection4, Db4objects.Db4o"
				, typeof(Collection4).FullName));
			config.AddAlias(new TypeAlias("Db4objects.Db4o.Foundation.List4, Db4objects.Db4o"
				, typeof(List4).FullName));
			config.AddAlias(new TypeAlias("Db4objects.Db4o.User, Db4objects.Db4o", typeof(User
				).FullName));
			config.AddAlias(new TypeAlias("Db4objects.Db4o.Internal.CS.ClassInfo, Db4objects.Db4o"
				, typeof(ClassInfo).FullName));
			config.AddAlias(new TypeAlias("Db4objects.Db4o.Internal.CS.FieldInfo, Db4objects.Db4o"
				, typeof(FieldInfo).FullName));
			config.AddAlias(new TypeAlias("Db4objects.Db4o.Internal.CS.Messages.MUserMessage+UserMessagePayload, Db4objects.Db4o"
				, typeof(MUserMessage.UserMessagePayload).FullName));
			config.AddAlias(new WildcardAlias("Db4objects.Db4o.Internal.CS.Messages.*, Db4objects.Db4o"
				, "com.db4o.internal.cs.messages.*"));
		}
	}
}
