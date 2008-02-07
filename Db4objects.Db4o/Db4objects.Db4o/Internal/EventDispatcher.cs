/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public sealed class EventDispatcher
	{
		private static readonly string[] events = new string[] { "objectCanDelete", "objectOnDelete"
			, "objectOnActivate", "objectOnDeactivate", "objectOnNew", "objectOnUpdate", "objectCanActivate"
			, "objectCanDeactivate", "objectCanNew", "objectCanUpdate" };

		internal const int CanDelete = 0;

		internal const int Delete = 1;

		internal const int ServerCount = 2;

		internal const int Activate = 2;

		internal const int Deactivate = 3;

		internal const int New = 4;

		public const int Update = 5;

		internal const int CanActivate = 6;

		internal const int CanDeactivate = 7;

		internal const int CanNew = 8;

		internal const int CanUpdate = 9;

		internal const int Count = 10;

		private readonly IReflectMethod[] methods;

		private EventDispatcher(IReflectMethod[] methods_)
		{
			methods = methods_;
		}

		internal bool Dispatch(Transaction trans, object obj, int eventID)
		{
			if (methods[eventID] == null)
			{
				return true;
			}
			object[] parameters = new object[] { trans.ObjectContainer() };
			ObjectContainerBase container = trans.Container();
			int stackDepth = container.StackDepth();
			int topLevelCallId = container.TopLevelCallId();
			container.StackDepth(0);
			try
			{
				object res = methods[eventID].Invoke(obj, parameters);
				if (res is bool)
				{
					return ((bool)res);
				}
			}
			finally
			{
				container.StackDepth(stackDepth);
				container.TopLevelCallId(topLevelCallId);
			}
			return true;
		}

		internal static Db4objects.Db4o.Internal.EventDispatcher ForClass(ObjectContainerBase
			 a_stream, IReflectClass classReflector)
		{
			if (a_stream == null || classReflector == null)
			{
				return null;
			}
			Db4objects.Db4o.Internal.EventDispatcher dispatcher = null;
			int count = 0;
			if (a_stream.ConfigImpl().Callbacks())
			{
				count = Count;
			}
			else
			{
				if (a_stream.ConfigImpl().IsServer())
				{
					count = ServerCount;
				}
			}
			if (count > 0)
			{
				IReflectClass[] parameterClasses = new IReflectClass[] { a_stream._handlers.IclassObjectcontainer
					 };
				IReflectMethod[] methods = new IReflectMethod[Count];
				for (int i = Count - 1; i >= 0; i--)
				{
					IReflectMethod method = classReflector.GetMethod(events[i], parameterClasses);
					if (null == method)
					{
						method = classReflector.GetMethod(ToPascalCase(events[i]), parameterClasses);
					}
					if (method != null)
					{
						methods[i] = method;
						if (dispatcher == null)
						{
							dispatcher = new Db4objects.Db4o.Internal.EventDispatcher(methods);
						}
					}
				}
			}
			return dispatcher;
		}

		private static string ToPascalCase(string name)
		{
			return Sharpen.Runtime.Substring(name, 0, 1).ToUpper() + Sharpen.Runtime.Substring
				(name, 1);
		}

		public bool HasEventRegistered(int eventID)
		{
			return methods[eventID] != null;
		}
	}
}
