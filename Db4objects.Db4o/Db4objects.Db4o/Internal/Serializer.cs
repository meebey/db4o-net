/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Serializer
	{
		public static StatefulBuffer Marshall(Transaction ta, object obj)
		{
			SerializedGraph serialized = Marshall(ta.Container(), obj);
			StatefulBuffer buffer = new StatefulBuffer(ta, serialized.Length());
			buffer.Append(serialized._bytes);
			buffer.UseSlot(serialized._id, 0, serialized.Length());
			return buffer;
		}

		public static SerializedGraph Marshall(ObjectContainerBase serviceProvider, object
			 obj)
		{
			MemoryFile memoryFile = new MemoryFile();
			memoryFile.SetInitialSize(223);
			memoryFile.SetIncrementSizeBy(300);
			TransportObjectContainer carrier = new TransportObjectContainer(serviceProvider, 
				memoryFile);
			carrier.ProduceClassMetadata(carrier.Reflector().ForObject(obj));
			carrier.Store(obj);
			int id = (int)carrier.GetID(obj);
			carrier.Close();
			return new SerializedGraph(id, memoryFile.GetBytes());
		}

		public static object Unmarshall(ObjectContainerBase serviceProvider, StatefulBuffer
			 yapBytes)
		{
			return Unmarshall(serviceProvider, yapBytes._buffer, yapBytes.GetID());
		}

		public static object Unmarshall(ObjectContainerBase serviceProvider, SerializedGraph
			 serialized)
		{
			return Unmarshall(serviceProvider, serialized._bytes, serialized._id);
		}

		public static object Unmarshall(ObjectContainerBase serviceProvider, byte[] bytes
			, int id)
		{
			if (id <= 0)
			{
				return null;
			}
			MemoryFile memoryFile = new MemoryFile(bytes);
			TransportObjectContainer carrier = new TransportObjectContainer(serviceProvider, 
				memoryFile);
			object obj = carrier.GetByID(id);
			carrier.Activate(carrier.Transaction(), obj, new FullActivationDepth());
			carrier.Close();
			return obj;
		}
	}
}
