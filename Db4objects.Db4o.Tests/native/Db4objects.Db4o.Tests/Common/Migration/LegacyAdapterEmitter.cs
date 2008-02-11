using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Migration
{
#if !CF
	internal class LegacyAdapterEmitter
	{
		private string _legacyAssembly;
		private string _version;

		public LegacyAdapterEmitter(string legacyAssembly, string version)
		{
			_legacyAssembly = legacyAssembly;
			_version = version;
		}

		public void Emit(string fname)
		{	
			CompilationServices.EmitAssembly(fname, new string[] {_legacyAssembly}, GetCode());
		}	

		public string GetCode()
		{
			if (_version.StartsWith("5")) return PascalCaseAdapter;
			return CamelCaseAdapter;
		}

		#region PascalCaseAdapter
		string PascalCaseAdapter
		{
			get
			{
				return CommonCode + @"
namespace Db4objects.Db4o
{
	using Db4objects.Db4o.Ext;
	
	public class Db4oFactory
	{
		public static IObjectContainer OpenFile(string fname)
		{
			return new ObjectContainerAdapter(com.db4o.Db4o.OpenFile(fname));
		}
	}

	class ObjectContainerAdapter : IExtObjectContainer
	{
		private readonly ObjectContainer _container;

		public ObjectContainerAdapter(ObjectContainer container)
		{
			_container = container;
		}

		public void Set(object o)
		{
			_container.Set(o);
		}

		public bool Close()
		{
			return _container.Close();
		}

		public IExtObjectContainer Ext()
		{
			return this;
		}
	}
}
";
			}
		}
		#endregion

		#region CamelCaseAdapter
		string CamelCaseAdapter
		{
			get
			{
				return CommonCode + @"
namespace Db4objects.Db4o
{
	using Db4objects.Db4o.Ext;

	public class Db4oFactory
	{
		public static IObjectContainer OpenFile(string fname)
		{
			return new ObjectContainerAdapter(com.db4o.Db4o.openFile(fname));
		}
	}

	class ObjectContainerAdapter : IExtObjectContainer
	{
		private readonly ObjectContainer _container;

		public ObjectContainerAdapter(ObjectContainer container)
		{
			_container = container;
		}

		public void Set(object o)
		{
			_container.set(o);
		}

		public bool Close()
		{
			return _container.close();
		}

		public IExtObjectContainer Ext()
		{
			return this;
		}
	}
}
";
			}
		}
		#endregion

		#region CommonCode
		string CommonCode
		{
			get
			{
				return @"
using System;
using com.db4o;

namespace Db4objects.Db4o.Ext
{
	public interface IExtObjectContainer : IObjectContainer
	{
	}
}

namespace Db4objects.Db4o
{
	using Db4objects.Db4o.Ext;
	
	public interface IObjectContainer
	{
		void Set(object o);
		IExtObjectContainer Ext();
		bool Close();
	}
}

namespace Db4objects.Db4o.Foundation.IO
{
	using System.IO;

	public class File4
	{
		public static void Delete(string file)
		{
			if (File.Exists(file))
			{
				File.Delete(file);
			}
		}
	}
}

namespace Sharpen
{
	public class Runtime 
    {
		public static string Substring(string s, int startIndex, int endIndex)
		{
			return s.Substring(startIndex, endIndex - startIndex);
		}
    }
}

";
			}
		}
		#endregion
	}
#endif
}