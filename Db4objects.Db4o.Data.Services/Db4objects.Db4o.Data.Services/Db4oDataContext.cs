/* Copyright (C) 2009  db4objects Inc.  http://www.db4o.com */

#if !CF && !MONO

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Services;
using System.Linq;
using System.Reflection;

using Db4objects.Db4o;
using Db4objects.Db4o.Linq.Internals;

namespace Db4objects.Db4o.Data.Services
{
	public abstract class Db4oDataContext : IUpdatable
	{
		enum Change
		{
			Store = 1,
			Delete = 2,
		}

		IObjectContainer _container;
		IDictionary<object, Change> _changes = new Dictionary<object, Change>();

		protected IObjectContainer Container
		{
			get
			{
				if (_container == null)
				{
					_container = GetContainer();
				}
				return _container;
			}
		}

		public Db4oDataContext()
		{
		}

		public Db4oDataContext(IObjectContainer container)
		{
			_container = container;
		}

		protected virtual IObjectContainer GetContainer()
		{
			throw new NotImplementedException();
		}

		public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
		{
			ProcessCollection(
				targetResource, propertyName, resourceToBeAdded,
				TryAddToList,
				TryReflectionAdd);
		}

		private void ProcessCollection(object target, string propertyName, object @object, params Func<object, object, bool>[] processors)
		{
			var property = GetValue(target, propertyName);
			if (property == null) return;

			if (!TryApplyProcessors(property, @object, processors))
			{
				throw DataServiceException("Can not interact with collection {0} on {1}", propertyName, target);
			}

			Store (target);
		}

		private static bool TryApplyProcessors(object target, object @object, IEnumerable<Func<object, object, bool>> processors)
		{
			foreach (var processor in processors)
			{
				if (processor(target, @object)) return true;
			}

			return false;
		}

		private static bool TryAddToList(object target, object @object)
		{
			return TryAsList(target, list => list.Add(@object));
		}

		private static bool TryRemoveFromList(object target, object @object)
		{
			return TryAsList(target, list => list.Remove(@object));
		}

		private static bool TryAsList(object target, Action<IList> action)
		{
			var list = target as IList;
			if (list == null) return false;

			action(list);
			return true;
		}

		private static bool TryReflectionAdd(object target, object @object)
		{
			return TryReflectionMethod(target, "Add", @object);
		}

		private static bool TryReflectionRemove(object target, object @object)
		{
			return TryReflectionMethod(target, "Remove", @object);
		}

		private static bool TryReflectionMethod(object target, string method_name, object @object)
		{
			const BindingFlags public_instance = BindingFlags.Public | BindingFlags.Instance;

			var target_method = (from method in target.GetType().GetMethods(public_instance)
								 where method.Name == method_name
								 let parameters = method.GetParameters()
								 where parameters.Length == 1
								 let first_parameter = parameters[0]
								 where first_parameter.ParameterType.IsAssignableFrom(@object.GetType())
								 select method).FirstOrDefault();

			if (target_method == null) return false;

			target_method.Invoke(target, new []{ @object });
			return true;
		}

		public void ClearChanges()
		{
			_changes.Clear();
		}

		private static Type GetType(string fullname)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				var type = assembly.GetType(fullname);
				if (type != null) return type;
			}
			return null;
		}

		private void SetChange(object @object, Change change)
		{
			_changes[@object] = change;
		}

		private void Store(object @object)
		{
			SetChange(@object, Change.Store);
		}

		private void Delete(object @object)
		{
			SetChange(@object, Change.Delete);
		}

		public object CreateResource(string containerName, string fullTypeName)
		{
			object @object = CreateInstance(fullTypeName);
			Store (@object);
			return @object;
		}

		private static object CreateInstance(string fullname)
		{
			var type = GetType(fullname);
			if (type == null) throw DataServiceException("Failed to get type: {0}", fullname);

			return CreateInstance(type);
		}

		private static object CreateInstance(Type type)
		{
			try
			{
				return Activator.CreateInstance(type);
			}
			catch (Exception e)
			{
				throw DataServiceException(e, "Failed to create resource: {0}", type);
			}
		}

		public void DeleteResource(object targetResource)
		{
			Delete(targetResource);
		}

		public object GetResource (IQueryable query, string fullTypeName)
		{
			return query.Cast<object>().First();
		}

		public object GetValue(object targetResource, string propertyName)
		{
			return GetProperty(targetResource, propertyName, property => property.GetValue (targetResource, null));
		}

		private static object GetProperty(object target, string propertyName, Func<PropertyInfo, object> func)
		{
			var property = target.GetType().GetProperty(propertyName);
			if (property == null) throw DataServiceException("Can't get property {0} on {1}", propertyName, target);

			return func(property);
		}

		private static DataServiceException DataServiceException(string pattern, params object[] objs)
		{
			return new DataServiceException(string.Format(pattern, objs));
		}

		private static DataServiceException DataServiceException(Exception cause, string pattern, params object[] objs)
		{
			return new DataServiceException(string.Format(pattern, objs), cause);
		}

		private static void GetProperty(object target, string propertyName, Action<PropertyInfo> action)
		{
			GetProperty (target, propertyName, property => { action(property); return null; });
		}

		public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
		{
			ProcessCollection(
				targetResource, propertyName, resourceToBeRemoved,
				TryRemoveFromList,
				TryReflectionRemove);
		}

		public object ResetResource(object resource)
		{
			throw new NotImplementedException();
			//return CreateInstance(resource.GetType());
		}

		public object ResolveResource(object resource)
		{
			return resource;
		}

		public void SaveChanges()
		{
			ProcessChanges(_changes);
			ClearChanges();
			Container.Commit();
		}

		private void ProcessChanges(IEnumerable<KeyValuePair<object, Change>> changes)
		{
			foreach (var pair in changes)
			{
				ProcessChange(pair.Value, pair.Key);
			}
		}

		private void ProcessChange(Change change, object @object)
		{
			switch (change)
			{
				case Change.Delete:
					Container.Delete(@object);
					break;
				case Change.Store:
					Container.Store(@object);
					break;
				default:
					throw DataServiceException("Unknown change: {0}", change);
			}
		}

		public void SetReference(object targetResource, string propertyName, object propertyValue)
		{
			SetValue(targetResource, propertyName, propertyValue);
		}

		public void SetValue(object targetResource, string propertyName, object propertyValue)
		{
			GetProperty(targetResource, propertyName, property => {
				property.SetValue(targetResource, propertyValue, null);
				Store(targetResource);
			});
		}
	}
}

#endif
