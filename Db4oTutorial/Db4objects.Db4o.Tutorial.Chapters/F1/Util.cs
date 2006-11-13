using System;
using Db4objects.Db4o;

namespace Db4objects.Db4o.Tutorial.F1
{
	public class Util
	{
		public readonly static string YapFileName = "formula1.yap";
		
		public readonly static int ServerPort = 0xdb40;
		
		public readonly static string ServerUser = "user";
		
		public readonly static string ServerPassword = "password";

		public static void ListResult(IObjectSet result)
		{
			Console.WriteLine(result.Count);
			foreach (object item in result)
			{
				Console.WriteLine(item);
			}
		}

		public static void ListRefreshedResult(IObjectContainer container, IObjectSet items, int depth)
		{
			Console.WriteLine(items.Count);
			foreach (object item in items)
			{	
				container.Ext().Refresh(item, depth);
				Console.WriteLine(item);
			}
		}
		
		public static void RetrieveAll(IObjectContainer db) 
		{
			IObjectSet result = db.Get(typeof(Object));
			ListResult(result);
		}
		
		public static void DeleteAll(IObjectContainer db) 
		{
			IObjectSet result = db.Get(typeof(Object));
			foreach (object item in result)
			{
				db.Delete(item);
			}
		}		
	}
}
