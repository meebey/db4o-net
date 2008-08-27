/* Copyright (C) 2008   db4objects Inc.   http://www.db4o.com */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Db4objects.Db4o.Foundation.Collections
{
	public interface ICollectionInitializer
	{
		void Clear();
		void Add(object o);
		void FinishAdding();
	}

	public sealed class CollectionInitializer
	{
		private static Dictionary<Type, Type> initializerByType = new Dictionary<Type, Type>();

		static CollectionInitializer()
		{
			initializerByType[typeof (ICollection<>)] = typeof (CollectionInitializerImpl<>);
			initializerByType[typeof(Stack<>)] = typeof(StackInitializer<>);
			initializerByType[typeof(Queue<>)] = typeof(QueueInitializer<>);
		}

		public static ICollectionInitializer For(object destination)
		{
			if (IsNonGenericList(destination))
			{
			    return new ListInitializer((IList)destination);
			}

			return InitializerFor(destination);
		}

		private static ICollectionInitializer InitializerFor(object destination)
		{
			Type destinationType = destination.GetType();
			if (!destinationType.IsGenericType)
			{
				throw new ArgumentException("Unknown collection: " + destination);
			}

			Type containerType = GenericContainerTypeFor(destination);
			if (containerType != null)
			{
				return GetInitializer(destination, initializerByType[containerType]);
			}

			throw new ArgumentException("Unknown collection: " + destination);
		}

		private static Type GenericContainerTypeFor(object destination)
		{
			Type containerType = destination.GetType().GetGenericTypeDefinition();
			while (containerType != null && !initializerByType.ContainsKey(containerType))
			{
				foreach (Type interfaceType in containerType.GetInterfaces())
				{
					if (!interfaceType.IsGenericType)
					{
						continue;
					}

					Type genericInterfaceType = interfaceType.GetGenericTypeDefinition();
					if (initializerByType.ContainsKey(genericInterfaceType))
					{
						return genericInterfaceType;
					}
				}

				containerType = containerType.BaseType;
			}

			return containerType;
		}

		private static ICollectionInitializer GetInitializer(object destination, Type initializerType)
		{
			ICollectionInitializer initializer = null;
			Type containedElementType = ContainerElementTypeFor(destination);
			if (containedElementType != null)
			{
				Type genericProtocolType = initializerType.MakeGenericType(containedElementType);
				initializer = InstantiateInitializer(destination, genericProtocolType);
				
			}
			return initializer;
		}

		private static bool IsNonGenericList(object destination)
		{
			return !destination.GetType().IsGenericType && destination is IList;
		}

		private static ICollectionInitializer InstantiateInitializer(object destination, Type genericProtocolType)
	    {
#if !CF
            return (ICollectionInitializer) Activator.CreateInstance(genericProtocolType, destination);
#else
	        ConstructorInfo constructor = genericProtocolType.GetConstructors()[0];
	        return (ICollectionInitializer) constructor.Invoke(new object[] {destination});
#endif
	    }

	    private static Type ContainerElementTypeFor(object destination)
		{
	    	Type containerType = destination.GetType();
	    	return containerType.GetGenericArguments()[0];
		}

		private sealed class ListInitializer : ICollectionInitializer
		{
			private readonly IList _list;

			public ListInitializer(IList list)
			{
				_list = list;
			}

			public void Clear()
			{
				_list.Clear();
			}

			public void Add(object o)
			{
				_list.Add(o);
			}

			public void FinishAdding()
			{
			}
		}

		private sealed class CollectionInitializerImpl<T> : ICollectionInitializer
		{
			private readonly ICollection<T> _collection;

			public CollectionInitializerImpl(ICollection<T> collection)
			{
				_collection = collection;
			}

			public void Clear()
			{
				_collection.Clear();
			}

			public void Add(object o)
			{
				_collection.Add((T)o);
			}

			public void FinishAdding()
			{
			}
		}

		private sealed class StackInitializer<T> : ICollectionInitializer
		{
			private readonly Stack<T> _stack;
			private readonly Stack<T> _tempStack;

			public StackInitializer(Stack<T> stack)
			{
				_stack= stack;
				_tempStack = new Stack<T>();
			}

			public void Clear()
			{
				_tempStack.Clear();
				_stack.Clear();
			}

			public void Add(object o)
			{
				_tempStack.Push((T) o);
			}

			public void FinishAdding()
			{
				foreach(T item in _tempStack)
				{
					_stack.Push(item);
				}

				_tempStack.Clear();
			}
		}

		private sealed class QueueInitializer<T> : ICollectionInitializer
		{
			private readonly Queue<T> _queue;

			public QueueInitializer(Queue<T> queue)
			{
				_queue = queue;
			}

			public void Clear()
			{
				_queue.Clear();
			}

			public void Add(object o)
			{
				_queue.Enqueue((T) o);
			}

			public void FinishAdding()
			{
			}
		}
	}
}
