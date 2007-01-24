namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public sealed class EventDispatcher
	{
		private static readonly string[] events = { "objectCanDelete", "objectOnDelete", 
			"objectOnActivate", "objectOnDeactivate", "objectOnNew", "objectOnUpdate", "objectCanActivate"
			, "objectCanDeactivate", "objectCanNew", "objectCanUpdate" };

		internal const int CAN_DELETE = 0;

		internal const int DELETE = 1;

		internal const int SERVER_COUNT = 2;

		internal const int ACTIVATE = 2;

		internal const int DEACTIVATE = 3;

		internal const int NEW = 4;

		public const int UPDATE = 5;

		internal const int CAN_ACTIVATE = 6;

		internal const int CAN_DEACTIVATE = 7;

		internal const int CAN_NEW = 8;

		internal const int CAN_UPDATE = 9;

		internal const int COUNT = 10;

		private readonly Db4objects.Db4o.Reflect.IReflectMethod[] methods;

		private EventDispatcher(Db4objects.Db4o.Reflect.IReflectMethod[] methods_)
		{
			methods = methods_;
		}

		internal bool Dispatch(Db4objects.Db4o.YapStream stream, object obj, int eventID)
		{
			if (methods[eventID] != null)
			{
				object[] parameters = new object[] { stream };
				int stackDepth = stream.StackDepth();
				int topLevelCallId = stream.TopLevelCallId();
				stream.StackDepth(0);
				try
				{
					object res = methods[eventID].Invoke(obj, parameters);
					if (res != null && res is bool)
					{
						return ((bool)res);
					}
				}
				catch
				{
				}
				finally
				{
					stream.StackDepth(stackDepth);
					stream.TopLevelCallId(topLevelCallId);
				}
			}
			return true;
		}

		internal static Db4objects.Db4o.EventDispatcher ForClass(Db4objects.Db4o.YapStream
			 a_stream, Db4objects.Db4o.Reflect.IReflectClass classReflector)
		{
			if (a_stream == null || classReflector == null)
			{
				return null;
			}
			Db4objects.Db4o.EventDispatcher dispatcher = null;
			int count = 0;
			if (a_stream.ConfigImpl().Callbacks())
			{
				count = COUNT;
			}
			else
			{
				if (a_stream.ConfigImpl().IsServer())
				{
					count = SERVER_COUNT;
				}
			}
			if (count > 0)
			{
				Db4objects.Db4o.Reflect.IReflectClass[] parameterClasses = { a_stream.i_handlers.
					ICLASS_OBJECTCONTAINER };
				Db4objects.Db4o.Reflect.IReflectMethod[] methods = new Db4objects.Db4o.Reflect.IReflectMethod
					[COUNT];
				for (int i = COUNT - 1; i >= 0; i--)
				{
					try
					{
						Db4objects.Db4o.Reflect.IReflectMethod method = classReflector.GetMethod(events[i
							], parameterClasses);
						if (null == method)
						{
							method = classReflector.GetMethod(ToPascalCase(events[i]), parameterClasses);
						}
						if (method != null)
						{
							methods[i] = method;
							if (dispatcher == null)
							{
								dispatcher = new Db4objects.Db4o.EventDispatcher(methods);
							}
						}
					}
					catch
					{
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
	}
}
