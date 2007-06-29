/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Reflect.Custom;

namespace Db4objects.Db4o.Tests.Common.Reflect.Custom
{
	/// <summary>
	/// Custom class information is stored to db4o itself as
	/// a CustomClassRepository singleton.
	/// </summary>
	/// <remarks>
	/// Custom class information is stored to db4o itself as
	/// a CustomClassRepository singleton.
	/// </remarks>
	public class Db4oPersistenceProvider : IPersistenceProvider
	{
		internal class MyContext
		{
			public readonly CustomClassRepository repository;

			public readonly IObjectContainer metadata;

			public readonly IObjectContainer data;

			public MyContext(CustomClassRepository repository, IObjectContainer metadata, IObjectContainer
				 data)
			{
				this.repository = repository;
				this.metadata = metadata;
				this.data = data;
			}
		}

		public virtual void CloseContext(PersistenceContext context, bool purge)
		{
			LogMethodCall("closeContext", context, purge);
			CloseContext(context);
			if (purge)
			{
				Purge(context.Url());
			}
		}

		public virtual void CreateEntryClass(PersistenceContext context, string className
			, string[] fieldNames, string[] fieldTypes)
		{
			LogMethodCall("createEntryClass", context, className);
			Repository(context).DefineClass(className, fieldNames, fieldTypes);
			UpdateRepository(context);
		}

		public virtual void CreateIndex(PersistenceContext context, string className, string
			 fieldName)
		{
		}

		public virtual int Delete(PersistenceContext context, string className, object uid
			)
		{
			return 0;
		}

		public virtual void DropEntryClass(PersistenceContext context, string className)
		{
		}

		public virtual void DropIndex(PersistenceContext context, string className, string
			 fieldName)
		{
		}

		public virtual void InitContext(PersistenceContext context)
		{
			LogMethodCall("initContext", context);
			IObjectContainer metadata = OpenMetadata(context.Url());
			CustomClassRepository repository = InitializeClassRepository(metadata);
			CustomReflector reflector = new CustomReflector(repository);
			IObjectContainer data = OpenData(reflector, context.Url());
			context.SetProviderContext(new Db4oPersistenceProvider.MyContext(repository, metadata
				, data));
		}

		public virtual void Insert(PersistenceContext context, PersistentEntry entry)
		{
			LogMethodCall("insert", context, entry);
			DataContainer(context).Set(Clone(entry));
		}

		public virtual IEnumerator Select(PersistenceContext context, PersistentEntryTemplate
			 template)
		{
			LogMethodCall("select", context, template);
			IQuery query = QueryFromTemplate(context, template);
			return new ObjectSetIterator(query.Execute());
		}

		public virtual void Update(PersistenceContext context, PersistentEntry entry)
		{
		}

		private void AddClassConstraint(PersistenceContext context, IQuery query, PersistentEntryTemplate
			 template)
		{
			query.Constrain(Repository(context).ForName(template.className));
		}

		private IConstraint AddFieldConstraint(IQuery query, PersistentEntryTemplate template
			, int index)
		{
			return query.Descend(template.fieldNames[index]).Constrain(template.fieldValues[index
				]);
		}

		private void AddFieldConstraints(IQuery query, PersistentEntryTemplate template)
		{
			if (template.fieldNames.Length == 0)
			{
				return;
			}
			AddFieldConstraint(query, template, 0);
		}

		private PersistentEntry Clone(PersistentEntry entry)
		{
			return new PersistentEntry(entry.className, entry.uid, entry.fieldValues);
		}

		private void CloseContext(PersistenceContext context)
		{
			LogMethodCall("closeContext", context);
			Db4oPersistenceProvider.MyContext customContext = My(context);
			if (null != customContext)
			{
				customContext.metadata.Close();
				customContext.data.Close();
				context.SetProviderContext(null);
			}
		}

		private Db4oPersistenceProvider.MyContext My(PersistenceContext context)
		{
			return ((Db4oPersistenceProvider.MyContext)context.GetProviderContext());
		}

		private IConfiguration DataConfiguration(IReflector reflector)
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			config.ReflectWith(reflector);
			return config;
		}

		private IObjectContainer DataContainer(PersistenceContext context)
		{
			return My(context).data;
		}

		private CustomClassRepository InitializeClassRepository(IObjectContainer container
			)
		{
			CustomClassRepository repository = QueryClassRepository(container);
			if (repository == null)
			{
				Log("Initializing new class repository.");
				repository = new CustomClassRepository();
				Store(container, repository);
			}
			else
			{
				Log("Found existing class repository: " + repository);
			}
			return repository;
		}

		private IConfiguration MetaConfiguration()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			config.ObjectClass(typeof(CustomClassRepository)).CascadeOnUpdate(true);
			config.ObjectClass(typeof(CustomClassRepository)).CascadeOnActivate(true);
			return config;
		}

		private IObjectContainer MetadataContainer(PersistenceContext context)
		{
			return My(context).metadata;
		}

		private string MetadataFile(string fname)
		{
			return fname + ".metadata";
		}

		private IObjectContainer OpenData(IReflector reflector, string fname)
		{
			return Db4oFactory.OpenFile(DataConfiguration(reflector), fname);
		}

		private IObjectContainer OpenMetadata(string fname)
		{
			return Db4oFactory.OpenFile(MetaConfiguration(), MetadataFile(fname));
		}

		private void Purge(string url)
		{
			File4.Delete(url);
			File4.Delete(MetadataFile(url));
		}

		private CustomClassRepository QueryClassRepository(IObjectContainer container)
		{
			IObjectSet found = container.Query(typeof(CustomClassRepository));
			if (!found.HasNext())
			{
				return null;
			}
			return (CustomClassRepository)found.Next();
		}

		private IQuery QueryFromTemplate(PersistenceContext context, PersistentEntryTemplate
			 template)
		{
			IQuery query = DataContainer(context).Query();
			AddClassConstraint(context, query, template);
			AddFieldConstraints(query, template);
			return query;
		}

		private CustomClassRepository Repository(PersistenceContext context)
		{
			return My(context).repository;
		}

		private void Store(IObjectContainer container, object obj)
		{
			container.Set(obj);
			container.Commit();
		}

		private void UpdateRepository(PersistenceContext context)
		{
			Store(MetadataContainer(context), Repository(context));
		}

		private void Log(string message)
		{
			Logger.Log("Db4oPersistenceProvider: " + message);
		}

		private void LogMethodCall(string methodName, object arg)
		{
			Logger.LogMethodCall("Db4oPersistenceProvider", methodName, arg);
		}

		private void LogMethodCall(string methodName, object arg1, object arg2)
		{
			Logger.LogMethodCall("Db4oPersistenceProvider", methodName, arg1, arg2);
		}
	}
}
