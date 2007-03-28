namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Serializer
	{
		public static Db4objects.Db4o.Internal.StatefulBuffer Marshall(Db4objects.Db4o.Internal.Transaction
			 ta, object obj)
		{
			Db4objects.Db4o.Internal.SerializedGraph serialized = Marshall(ta.Stream(), obj);
			Db4objects.Db4o.Internal.StatefulBuffer buffer = new Db4objects.Db4o.Internal.StatefulBuffer
				(ta, serialized.Length());
			buffer.Append(serialized._bytes);
			buffer.UseSlot(serialized._id, 0, serialized.Length());
			return buffer;
		}

		public static Db4objects.Db4o.Internal.SerializedGraph Marshall(Db4objects.Db4o.Internal.ObjectContainerBase
			 serviceProvider, object obj)
		{
			Db4objects.Db4o.Ext.MemoryFile memoryFile = new Db4objects.Db4o.Ext.MemoryFile();
			memoryFile.SetInitialSize(223);
			memoryFile.SetIncrementSizeBy(300);
			serviceProvider.ProduceClassMetadata(serviceProvider.Reflector().ForObject(obj));
			try
			{
				Db4objects.Db4o.Internal.TransportObjectContainer carrier = new Db4objects.Db4o.Internal.TransportObjectContainer
					(serviceProvider, memoryFile);
				carrier.Set(obj);
				int id = (int)carrier.GetID(obj);
				carrier.Close();
				return new Db4objects.Db4o.Internal.SerializedGraph(id, memoryFile.GetBytes());
			}
			catch (System.IO.IOException)
			{
				Db4objects.Db4o.Internal.Exceptions4.ShouldNeverHappen();
				return null;
			}
		}

		public static object Unmarshall(Db4objects.Db4o.Internal.ObjectContainerBase serviceProvider
			, Db4objects.Db4o.Internal.StatefulBuffer yapBytes)
		{
			return Unmarshall(serviceProvider, yapBytes._buffer, yapBytes.GetID());
		}

		public static object Unmarshall(Db4objects.Db4o.Internal.ObjectContainerBase serviceProvider
			, Db4objects.Db4o.Internal.SerializedGraph serialized)
		{
			return Unmarshall(serviceProvider, serialized._bytes, serialized._id);
		}

		public static object Unmarshall(Db4objects.Db4o.Internal.ObjectContainerBase serviceProvider
			, byte[] bytes, int id)
		{
			if (id <= 0)
			{
				return null;
			}
			Db4objects.Db4o.Ext.MemoryFile memoryFile = new Db4objects.Db4o.Ext.MemoryFile(bytes
				);
			try
			{
				Db4objects.Db4o.Internal.TransportObjectContainer carrier = new Db4objects.Db4o.Internal.TransportObjectContainer
					(serviceProvider, memoryFile);
				object obj = carrier.GetByID(id);
				carrier.Activate(obj, int.MaxValue);
				carrier.Close();
				return obj;
			}
			catch (System.IO.IOException)
			{
				Db4objects.Db4o.Internal.Exceptions4.ShouldNeverHappen();
				return null;
			}
		}
	}
}
